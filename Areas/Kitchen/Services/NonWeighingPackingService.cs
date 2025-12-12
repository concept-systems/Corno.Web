using Corno.Web.Areas.Kitchen.Dto.Carton;
using Corno.Web.Areas.Kitchen.Services.Interfaces;
using Corno.Web.Extensions;
using Corno.Web.Globals;
using Corno.Web.Logger;
using Corno.Web.Models.Packing;
using Corno.Web.Repository.Interfaces;
using Corno.Web.Services.Interfaces;
using Corno.Web.Windsor;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telerik.Reporting;

namespace Corno.Web.Areas.Kitchen.Services;

public class NonWeighingPackingService : CartonService, INonWeighingPackingService
{
    #region -- Constructors --
    public NonWeighingPackingService(IGenericRepository<Carton> genericRepository, 
        ILabelService labelService, IMiscMasterService miscMasterService)
     : base(genericRepository, labelService, miscMasterService)
    {
        _labelService = labelService;

        TypeAdapterConfig<CartonCrudDto, Carton>
            .NewConfig()
            .Map(dest => dest.CartonDetails, src => src.CartonDetailsDtos.Adapt<List<CartonDetail>>())
            .AfterMapping((src, dest) =>
            {
                dest.Id = src.Id;
                // CartonNo is computed during database update (async). Do not call async here.
                for (var index = 0; index < src.CartonDetailsDtos.Count; index++)
                    dest.CartonDetails[index].Id = src.CartonDetailsDtos[index].Id;
            });

        TypeAdapterConfig<Carton, CartonCrudDto>
            .NewConfig()
            .Map(dest => dest.CartonDetailsDtos, src => src.CartonDetails.Adapt<List<CartonCrudDto>>())
            .AfterMapping((src, dest) =>
            {
                dest.Id = src.Id;
                for (var index = 0; index < src.CartonDetails.Count; index++)
                    dest.CartonDetailsDtos[index].Id = src.CartonDetails[index].Id;
            });
    }
    #endregion

    #region -- Data Members --
    private readonly ILabelService _labelService;

    #endregion

    #region -- Private Methods --
    private static void ValidateDto(CartonCrudDto dto)
    {
        if (string.IsNullOrEmpty(dto.WarehouseOrderNo))
            throw new Exception($"Invalid warehouse order {dto.WarehouseOrderNo}.");
        if (dto.CartonDetailsDtos.Count <= 0)
            throw new Exception("At least single item should be scanned.");
    }

    private async Task UpdateDatabase(CartonCrudDto dto, Carton carton, string userId)
    {
        const string newStatus = StatusConstants.Packed;

        // Update plan
        var plan = dto.Plan;
        //var labels = dto.Labels;
        var labelBarcodes = carton.CartonDetails.Select(d => d.Barcode).ToList();
        var labels = await _labelService.GetAsync(p => labelBarcodes.Contains(p.Barcode), p => p).ConfigureAwait(false);

        if (null == labels || labels.Count <= 0)
            throw new Exception("No labels found to pack.");
        if (null == plan)
            throw new Exception($"Plan for Warehouse order '{dto.WarehouseOrderNo}' not found.");

        // Update carton fields
        carton.CartonNo = await MaxAsync(p => p.WarehouseOrderNo == carton.WarehouseOrderNo, p =>
                              p.CartonNo).ConfigureAwait(false) + 1;
        carton.CartonNo = await GetNextCartonNoAsync(carton.WarehouseOrderNo).ConfigureAwait(false);
        carton.CartonBarcode = GetCartonBarcode(carton.WarehouseOrderNo, carton.CartonNo ?? 0);
        carton.SoNo = plan.SoNo;
        carton.Code = "Web";
        carton.Status = newStatus;
        carton.PackingDate = DateTime.Now;
        carton.UpdateCreated(userId);
        carton.UpdateModified(userId);
        carton.CartonDetails.ForEach(d =>
        {
            d.CartonId = carton.Id;
            d.UpdateCreated(userId);
            d.UpdateModified(userId);
        });

        LogHandler.LogInfo($"Carton Barcode : {carton.CartonBarcode}, Carton No : {carton.CartonNo}, Carton Barcodes : {string.Join(", ", carton.CartonDetails.Select(d => d.Barcode))}");

        // Update all labels to Sold Status

        var currentTime = DateTime.Now;
        labels.ForEach(p =>
        {
            p.Status = newStatus;
            p.UpdateModified(userId);

            var labelDetail = new LabelDetail
            {
                ScanDate = currentTime,
                Status = newStatus
            };
            labelDetail.UpdateCreated(userId);
            labelDetail.UpdateModified(userId);
            p.LabelDetails.Add(labelDetail);

            // Update plan items
            var planItemDetail = plan.PlanItemDetails.FirstOrDefault(d => d.Position == p.Position);
            if (null == planItemDetail) return;
            planItemDetail.PackQuantity ??= 0;
            planItemDetail.PackQuantity += p.Quantity;
        });

        //var labelIds = labels.Select(p => p.Id);
        //var info = $"Updating Labels (2) : {labels.Count}, Label Ids : {string.Join("-", labelIds)} UserId : {userId}";
        //LogHandler.LogInfo(info);

        await _labelService.UpdateRangeAsync(labels).ConfigureAwait(false);
        var planService = Bootstrapper.Get<IPlanService>();
        await planService.UpdateAsync(plan).ConfigureAwait(false);
        await planService.SaveAsync().ConfigureAwait(false);            // Keep it for database udate sequence

        await AddAsync(carton).ConfigureAwait(false);
        await SaveAsync().ConfigureAwait(false);
    }

    #endregion

    #region  -- Public Methods --

    public async Task ValidateBarcodeAsync(CartonCrudDto dto)
    {
        // Check in dto
        if (dto.CartonDetailsDtos.Any(d => d.Barcode == dto.Barcode))
            throw new Exception("Barcode already scanned in same carton.");

        // Validate barcode and get label asynchronously
        var label = await GetLabelAsync(dto.Barcode, dto.Plan).ConfigureAwait(false);

        // Validate label whether it is already packed (make this async if service supports)
        await ValidateLabelForAlreadyPacked(label.WarehouseOrderNo, dto.Barcode, label.Position).ConfigureAwait(false);

        // Check if warehouse order number is already present in dto
        if (string.IsNullOrEmpty(dto.WarehouseOrderNo) && dto.CartonDetailsDtos.Count <= 0)
            dto.WarehouseOrderNo = label.WarehouseOrderNo;

        // Check whether barcode belongs to the warehouse order
        if (!label.WarehouseOrderNo.Equals(dto.WarehouseOrderNo))
            throw new Exception($"Barcode '{dto.Barcode}' does not belong to warehouse order '{dto.WarehouseOrderNo}'. " +
                                $"It belongs to warehouse order '{label.WarehouseOrderNo}'");

        // Check for SubAssembly
        var labels = label.Status != StatusConstants.SubAssembled
            ? [label] : await GetSubAssemblyLabelsAsync(label.WarehouseOrderNo, label.AssemblyCode).ConfigureAwait(false);

        // Get product details by product id
        if (dto.Plan == null || dto.Plan.WarehouseOrderNo != label.WarehouseOrderNo)
        {
            var planService = Bootstrapper.Get<IPlanService>();
            dto.Plan = await planService.GetByWarehouseOrderNoAsync(label.WarehouseOrderNo).ConfigureAwait(false);
        }

        foreach (var record in labels)
        {
            var planItemDetail = dto.Plan?.PlanItemDetails.FirstOrDefault(p => p.Position == record.Position);

            // Check in dto
            if (dto.CartonDetailsDtos.Any(d => d.Barcode == record.Barcode))
                throw new Exception("SubAssembly Barcode already scanned in same carton.");

            // Check whether pack quantity exceeds order quantity
            if (record.Quantity.ToInt() + planItemDetail?.PackQuantity.ToInt() > planItemDetail?.OrderQuantity.ToInt())
                throw new Exception($"Pack quantity for barcode '{record.Barcode}' exceeds order quantity for position '{record.Position}' in warehouse order '{dto.WarehouseOrderNo}'.");

            dto.CartonDetailsDtos.Add(new CartonDetailsDto
            {
                Barcode = record.Barcode,
                WarehousePosition = record.WarehousePosition,
                Position = record.Position,
                AssemblyCode = record.AssemblyCode,
                CarcassCode = record.CarcassCode,
                ItemCode = planItemDetail?.ItemCode,
                Description = planItemDetail?.Description,
                OrderQuantity = record.OrderQuantity ?? 0,
                Quantity = record.Quantity,
            });
        }

        // Add scanned labels in dto
        dto.Labels ??= [];
        dto.Labels.AddRange(labels);
    }



    public async Task<ReportBook> Preview(CartonCrudDto dto)
    {
        // Validate the input DTO
        ValidateDto(dto);

        // Add and save the new c
        // Map DTO to carton entity
        var carton = dto.Adapt<Carton>();

        // Create Label
        var labelRpt = await CreateLabelReport(carton, dto.Plan, false).ConfigureAwait(false);
        return labelRpt;
    }

    public async Task<ReportBook> Print(CartonCrudDto dto, string userId)
    {
        // Validate the input DTO
        ValidateDto(dto);

        // Add and save the new c
        // Map DTO to carton entity
        var carton = dto.Adapt<Carton>();

        // store / update database
        await UpdateDatabase(dto, carton, userId).ConfigureAwait(false);

        LogHandler.LogInfo($"Creating Label, UserId : {userId}");

        // Create Label
        var labelRpt = await CreateLabelReport(carton, dto.Plan, false).ConfigureAwait(false);

        LogHandler.LogInfo($"Label Created, UserId : {userId}");
        return labelRpt;
    }


    public async Task<CartonViewDto> View(int? id)
    {
        var carton = await GetByIdAsync(id).ConfigureAwait(false);
        if (null == carton)
            throw new Exception($"Label with Id '{id}' not found.");

        var planService = Bootstrapper.Get<IPlanService>();
        var plan = await planService.GetByWarehouseOrderNoAsync(carton.WarehouseOrderNo).ConfigureAwait(false);

        var dto = new CartonViewDto
        {
            Id = carton.Id,
            WarehouseOrderNo = carton.WarehouseOrderNo,
            SoNo = carton.SoNo,
            LotNo = plan?.LotNo,
            DueDate = plan?.DueDate,
            OrderQuantity = plan?.OrderQuantity,
            PrintQuantity = plan?.PrintQuantity,

            CartonDetailsDtos = carton.CartonDetails.Select(d =>
            {
                var planItemDetail = plan?.PlanItemDetails.FirstOrDefault(x => x.Position == d.Position);
                return new CartonDetailsDto
                {
                    Position = d.Position,
                    WarehousePosition = d.WarehousePosition,
                    AssemblyCode = planItemDetail?.AssemblyCode,
                    CarcassCode = planItemDetail?.CarcassCode,
                    NetWeight = d.NetWeight,
                    Tolerance = d.Tolerance,
                    Quantity = d.Quantity,
                    OrderQuantity = d.OrderQuantity ?? 0
                };
            }).ToList(),

            CartonRackingDetailDtos = carton.CartonRackingDetails.Select(d => new CartonRackingDetailDto
            {
                ScanDate = d.ScanDate,
                PalletNo = d.PalletNo,
                RackNo = d.RackNo,
                Status = d.Status
            }).ToList(),

            ReportBook = await CreateLabelReport(carton, plan, false).ConfigureAwait(false) // Make sure this method is accessible
        };

        return dto;
    }

    #endregion
}