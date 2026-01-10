using Corno.Web.Areas.Kitchen.Dto.Carcass;
using Corno.Web.Areas.Kitchen.Helper;
using Corno.Web.Areas.Kitchen.Services.Interfaces;
using Corno.Web.Extensions;
using Corno.Web.Globals;
using Corno.Web.Models.Packing;
using Corno.Web.Repository.Interfaces;
using Corno.Web.Services.Interfaces;
using Corno.Web.Windsor;
using Mapster;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Corno.Web.Logger;
using System.Transactions;
using Telerik.Reporting;


namespace Corno.Web.Areas.Kitchen.Services;

public class CarcassPackingService : CartonService, ICarcassPackingService
{
    #region -- Constructors --
    public CarcassPackingService(IGenericRepository<Carton> genericRepository,
        ICartonService cartonService, ILabelService labelService, IMiscMasterService miscMasterService)
    : base(genericRepository, labelService, miscMasterService)
    {
        _cartonService = cartonService;
        _labelService = labelService;

        TypeAdapterConfig<CarcassCrudDto, Carton>
            .NewConfig()
            .Map(dest => dest.CartonDetails, src => src.CarcassDetailsDtos.Adapt<List<CartonDetail>>())
            .AfterMapping((src, dest) =>
            {
                dest.Id = src.Id;
                // Use RunAsync helper to avoid deadlock in Mapster callback
                dest.CartonNo = RunAsync(() => MaxAsync(p => p.WarehouseOrderNo == src.WarehouseOrderNo, p => p.CartonNo)) + 1;
                for (var index = 0; index < src.CarcassDetailsDtos.Count; index++)
                    dest.CartonDetails[index].Id = src.CarcassDetailsDtos[index].Id;
            });

        TypeAdapterConfig<Carton, CarcassCrudDto>
            .NewConfig()
            .Map(dest => dest.CarcassDetailsDtos, src => src.CartonDetails.Adapt<List<CarcassCrudDto>>())
            .AfterMapping((src, dest) =>
            {
                dest.Id = src.Id;
                for (var index = 0; index < src.CartonDetails.Count; index++)
                    dest.CarcassDetailsDtos[index].Id = src.CartonDetails[index].Id;
            });
    }
    #endregion

    #region -- Data Members --

    private readonly ILabelService _labelService;
    private readonly ICartonService _cartonService;

    #endregion

    #region -- Private Methods --
    private void ValidateDto(CarcassCrudDto dto)
    {
        if (string.IsNullOrEmpty(dto.WarehouseOrderNo))
            throw new Exception($"Invalid warehouse order {dto.WarehouseOrderNo}.");
        if (dto.CarcassDetailsDtos.Count <= 0)
            throw new Exception("At least single item should be scanned.");

        if (dto.CarcassDetailsDtos.Any(d => string.IsNullOrEmpty(d.Barcode)))
            throw new Exception("All items should be scanned with barcode.");
    }

    private async Task UpdateDatabase(CarcassCrudDto dto, Carton carton, string userId)
    {
        const string newStatus = StatusConstants.Packed;

        using var scope = new TransactionScope(TransactionScopeOption.Required,
            new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
            TransactionScopeAsyncFlowOption.Enabled);

        // Update plan
        var plan = dto.Plan;
        var barcodes = carton.CartonDetails.Select(d => d.Barcode).ToList();
        var labels = dto.Labels.Where(d => barcodes.Contains(d.Barcode))
            .DistinctBy(d => d.Barcode).ToList();
        if (!labels.Count.Equals(carton.CartonDetails.Count))
            throw new Exception(
                "Something went wrong with session labels. Carton labels doesn't match with labels count.");

        // Update carton fields
        carton.CartonNo = await MaxAsync(p => p.WarehouseOrderNo == carton.WarehouseOrderNo, p => p.CartonNo)
                          + 1;
        carton.CartonNo = await GetNextCartonNoAsync(carton.WarehouseOrderNo).ConfigureAwait(false);
        carton.CartonBarcode = await GetCartonBarcodeAsync(carton.WarehouseOrderNo, carton.CartonNo ?? 0).ConfigureAwait(false);

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

        // Add and save the new carton
        await _labelService.UpdateRangeAsync(labels).ConfigureAwait(false);
        var planService = Bootstrapper.Get<IPlanService>();
        await planService.UpdateAndSaveAsync(plan).ConfigureAwait(false);                // Keep it for database update sequence

        await AddAsync(carton).ConfigureAwait(false);
        await SaveAsync().ConfigureAwait(false);

        scope.Complete();
    }
    #endregion

    #region  -- Public Methods --

    /*public void ValidateBarcode(CarcassCrudDto dto)
    {
        // Check in dto
        if (dto.CarcassDetailsDtos.Any(d => d.Barcode == dto.Barcode))
            throw new Exception("Barcode already scanned in same carton.");
        // Validate barcode and get label
        var label = GetLabel(dto.WarehouseOrderNo, dto.Barcode);

        // Validate label whether it is already packed.
        ValidateLabelForAlreadyPacked(label.WarehouseOrderNo, dto.Barcode, label.Position);

        if (string.IsNullOrEmpty(label.CarcassCode) || label.CarcassCode.Equals(FieldConstants.NA))
            throw new Exception($"Carcass code is not available for barcode '{dto.Barcode}'.");

        if (string.IsNullOrEmpty(dto.WarehouseOrderNo) && dto.CarcassDetailsDtos.Count <= 0)
        {
            dto.WarehouseOrderNo = label.WarehouseOrderNo;
            dto.PackingDate = DateTime.Now;

            // Update details
            //var plan = _planService.FirstOrDefault(p => p.WarehouseOrderNo == label.WarehouseOrderNo,
            //    p => new { p.WarehouseOrderNo, p.PlanItemDetails });
            var planService = Bootstrapper.Get<IPlanService>();
            dto.Plan ??= planService.GetByWarehouseOrderNo(label.WarehouseOrderNo);
            if (null == dto.Plan)
                throw new Exception($"Plan not found for warehouse order {label.WarehouseOrderNo}");
         
            var carcassItems = dto.Plan.PlanItemDetails.Where(d =>
                d.CarcassCode == label.CarcassCode).ToList();
            if (carcassItems.Count <= 1)
                throw new Exception($"Carcass code '{label.CarcassCode}' not found in plan for warehouse order {label.WarehouseOrderNo}");

            dto.CarcassDetailsDtos = carcassItems
                .SelectMany(detail => Enumerable.Range(0, detail.OrderQuantity.ToInt())
                    .Select(_ => new CarcassDetailsDto
                    {
                        WarehousePosition = detail.WarehousePosition,
                        Position = detail.Position,
                        CarcassCode = detail.CarcassCode,
                        AssemblyCode = detail.AssemblyCode,
                        ItemCode = detail.ItemCode,
                        Description = detail.Description,
                    })
                ).ToList();
            dto.CarcassCode = label.CarcassCode;
        }

        // Check for SubAssembly.
        var labels = label.Status != StatusConstants.SubAssembled ? [label] :
            GetSubAssemblyLabels(label.WarehouseOrderNo, label.AssemblyCode);

        foreach (var record in labels)
        {
            var detail = dto.CarcassDetailsDtos.FirstOrDefault(d => d.Position == record.Position &&
                                                                    string.IsNullOrEmpty(d.Barcode));
            if (detail == null)
                throw new Exception("Invalid position for barcode");

            if (!detail.CarcassCode.Equals(record.CarcassCode))
                throw new Exception($"Barcode '{dto.Barcode}' does not belongs to carcass '{detail.CarcassCode}'. " +
                                    $"It belongs to carcass '{record.CarcassCode}'");

            //// Check whether pack quantity exceeds order quantity
            var planItemDetail = dto.Plan?.PlanItemDetails.FirstOrDefault(p => p.Position == record.Position);
            if (record.Quantity.ToInt() + planItemDetail?.PackQuantity.ToInt() > planItemDetail?.OrderQuantity.ToInt())
                throw new Exception($"Pack quantity for barcode '{record.Barcode}' exceeds order quantity for position '{record.Position}' in warehouse order '{dto.WarehouseOrderNo}'.");

            detail.Barcode = record.Barcode;
            detail.Quantity = record.Quantity;

        }

        // Add scanned labels in dto
        dto.Labels ??= new List<Label>();
        dto.Labels.AddRange(labels);
    }*/

    public async Task ValidateBarcodeAsync(CarcassCrudDto dto)
    {
        // Check in dto
        if (dto.CarcassDetailsDtos.Any(d => d.Barcode == dto.Barcode))
            throw new Exception("Barcode already scanned in same carton.");
        // Validate barcode and get label
        var label = await GetLabelAsync(dto.Barcode, dto.Plan).ConfigureAwait(false);

        // Validate label whether it is already packed.
        await ValidateLabelForAlreadyPacked(label.WarehouseOrderNo, dto.Barcode, label.Position).ConfigureAwait(false);

        if (string.IsNullOrEmpty(label.CarcassCode) || label.CarcassCode.Equals(FieldConstants.NA))
            throw new Exception($"Carcass code is not available for barcode '{dto.Barcode}'.");

        if (string.IsNullOrEmpty(dto.WarehouseOrderNo) && dto.CarcassDetailsDtos.Count <= 0)
        {
            dto.WarehouseOrderNo = label.WarehouseOrderNo;
            dto.PackingDate = DateTime.Now;

            // Update details
            //var plan = _planService.FirstOrDefault(p => p.WarehouseOrderNo == label.WarehouseOrderNo,
            //    p => new { p.WarehouseOrderNo, p.PlanItemDetails });
            LogHandler.LogInfo($"Fetching plan for warehouse order no: {label.WarehouseOrderNo}, dto.Plan : {dto.Plan}");
            var planService = Bootstrapper.Get<IPlanService>();
            dto.Plan ??= await planService.GetByWarehouseOrderNoAsync(label.WarehouseOrderNo).ConfigureAwait(false);
            if (null == dto.Plan)
                throw new Exception($"Plan not found for warehouse order {dto.Plan?.WarehouseOrderNo}");
            LogHandler.LogInfo($"Plan fetched for warehouse order no: {label.WarehouseOrderNo}, Total Items : {dto.Plan?.PlanItemDetails.Count}");
            var carcassItems = dto.Plan?.PlanItemDetails.Where(d =>
                d.CarcassCode == label.CarcassCode).ToList();
            if (carcassItems?.Count <= 1)
                throw new Exception($"Carcass code '{label.CarcassCode}' not found in plan for warehouse order {label.WarehouseOrderNo}");

            dto.CarcassDetailsDtos = carcassItems?
                .SelectMany(detail => Enumerable.Range(0, detail.OrderQuantity.ToInt())
                    .Select(_ => new CarcassDetailsDto
                    {
                        WarehousePosition = detail.WarehousePosition,
                        Position = detail.Position,
                        CarcassCode = detail.CarcassCode,
                        AssemblyCode = detail.AssemblyCode,
                        ItemCode = detail.ItemCode,
                        Description = detail.Description,
                    })
                ).ToList();
            dto.CarcassCode = label.CarcassCode;
        }

        // Check for SubAssembly.
        var labels = label.Status != StatusConstants.SubAssembled ? [label] :
            await GetSubAssemblyLabelsAsync(label.WarehouseOrderNo, label.AssemblyCode).ConfigureAwait(false);

        // Validate subassembled labels for families 22 & 23
        if (label.Status == StatusConstants.SubAssembled)
            LabelFamilyValidationHelper.ValidateSubAssembledLabelsForFamilies2223(labels, dto.Plan);

        foreach (var record in labels)
        {
            var detail = dto.CarcassDetailsDtos.FirstOrDefault(d => d.Position == record.Position &&
                                                                    string.IsNullOrEmpty(d.Barcode));
            if (detail == null)
                throw new Exception("Invalid position for barcode");

            // Check in dto
            if (dto.CarcassDetailsDtos.Any(d => d.Barcode == record.Barcode))
                throw new Exception("SubAssembly Barcode already scanned in same carton.");

            if (!detail.CarcassCode.Equals(record.CarcassCode))
                throw new Exception($"Barcode '{dto.Barcode}' does not belongs to carcass '{detail.CarcassCode}'. " +
                                    $"It belongs to carcass '{record.CarcassCode}'");

            //// Check whether pack quantity exceeds order quantity
            var planItemDetail = dto.Plan?.PlanItemDetails.FirstOrDefault(p => p.Position == record.Position);
            if (record.Quantity.ToInt() + planItemDetail?.PackQuantity.ToInt() > planItemDetail?.OrderQuantity.ToInt())
                throw new Exception($"Pack quantity for barcode '{record.Barcode}' exceeds order quantity for position '{record.Position}' in warehouse order '{dto.WarehouseOrderNo}'.");

            detail.Barcode = record.Barcode;
            detail.Quantity = record.Quantity;

        }

        // Add scanned labels in dto
        dto.Labels ??= new List<Label>();
        dto.Labels.AddRange(labels);
    }

    public async Task<ReportBook> Print(CarcassCrudDto dto, string userId)
    {
        // Validate the input DTO
        ValidateDto(dto);

        // Add and save the new c
        // Map DTO to carton entity
        var carton = dto.Adapt<Carton>();
        // store / update database
        await UpdateDatabase(dto, carton, userId).ConfigureAwait(false);

        // Create Label
        var reportBook = await CreateLabelReport(carton, dto.Plan, false).ConfigureAwait(false);
        return reportBook;
    }

    public async Task<ReportBook> Preview(CarcassCrudDto dto)
    {
        // Validate the input DTO
        ValidateDto(dto);

        // Add and save the new c
        // Map DTO to carton entity
        var carton = dto.Adapt<Carton>();
        var planService = Bootstrapper.Get<IPlanService>();
        var plan = await planService.FirstOrDefaultAsync(p => p.WarehouseOrderNo == dto.WarehouseOrderNo, p => p).ConfigureAwait(false);

        // Create Label

        var reportBook = await CreateLabelReport(carton, plan, false).ConfigureAwait(false);
        return reportBook;
    }


    #endregion
}