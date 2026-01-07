using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Corno.Web.Areas.Admin.Models;
using Corno.Web.Areas.Admin.Services.Interfaces;
using Corno.Web.Areas.Kitchen.Services;
using Corno.Web.Areas.Kitchen.Services.Interfaces;
using Corno.Web.Areas.Nilkamal.Dto.PartLabel;
using Corno.Web.Areas.Nilkamal.Labels;
using Corno.Web.Extensions;
using Corno.Web.Globals;
using Corno.Web.Logger;
using Corno.Web.Models.Packing;
using Corno.Web.Models.Plan;
using Corno.Web.Reports;
using Corno.Web.Repository.Interfaces;
using Corno.Web.Services.Masters.Interfaces;
using Corno.Web.Windsor;
using Mapster;
using Microsoft.AspNet.Identity;
using Volo.Abp.Data;
using IPartLabelService = Corno.Web.Areas.Nilkamal.Services.Interfaces.IPartLabelService;
using IPlanService = Corno.Web.Areas.Nilkamal.Services.Interfaces.IPlanService;

namespace Corno.Web.Areas.Nilkamal.Services;

public class PartLabelService : LabelService, IPartLabelService
{
    #region -- Constructors --
    public PartLabelService(IGenericRepository<Label> genericRepository, IProductService productService, IUserService userService)
        : base(genericRepository, null, userService)
    {
        _productService = productService;
        _userService = userService;
    }
    #endregion

    #region -- Data Members --
    private readonly IProductService _productService;
    private readonly IUserService _userService;

    #endregion

    #region -- Protected Methods --
    protected bool ValidateDto(PartLabelCrudDto dto)
    {
        if (string.IsNullOrEmpty(dto.ProductionOrderNo))
            throw new Exception("Invalid production Order.");

        if (string.IsNullOrEmpty(dto.Position))
            throw new Exception("Invalid position.");

        if (dto.Quantity <= 0)
            throw new Exception("Invalid quantity.");

        return true;
    }

    #endregion

    #region  -- Public Methods --

    [HttpPost]
    public async Task<BaseReport> GenerateBomLabels(int? productId, int packingTypeId, int itemId, int quantity, bool bSave = true)
    {
        if (quantity <= 0)
            throw new Exception("Quantity must be greater than zero.");

        var product = await _productService.GetByIdAsync(productId);
        if (product == null)
            throw new Exception("Product not found.");

        var productItemDetails = product.ProductItemDetails.Where(x => x.ProductId == productId).ToList();
        if (packingTypeId > 0)
            productItemDetails = productItemDetails.Where(x => x.PackingTypeId == packingTypeId).ToList();
        if (itemId > 0)
            productItemDetails = productItemDetails.Where(x => x.ItemId == itemId).ToList();

        var labels = new List<Label>();

        const string newStatus = StatusConstants.Packed;
        var serialNo = await GetNextSerialNoAsync();
        var userId = HttpContext.Current.User.Identity.GetUserId();
        foreach (var plaProductItemDetail in productItemDetails)
        {
            var labelQuantity = plaProductItemDetail.Quantity.ToInt() * quantity;
            for (var index = 0; index < labelQuantity; index++)
            {
                var label = new Label
                {
                    SerialNo = serialNo,
                    LabelDate = DateTime.Now,
                    ProductId = product.Id,
                    ItemId = plaProductItemDetail.ItemId,
                    Quantity = 1,
                    Barcode = $"{plaProductItemDetail.Code},{serialNo}",

                    Status = newStatus,

                    CreatedBy = userId,
                    ModifiedBy = userId,
                };

                label.LabelDetails.Add(new LabelDetail
                {
                    ScanDate = DateTime.Now,
                    Quantity = 1,
                    Status = newStatus,

                    CreatedBy = userId,
                    ModifiedBy = userId,
                });

                labels.Add(label);
                serialNo++;
            }
        }

        if (bSave)
        {
            //var labelService = Bootstrapper.Get<IPartLabelService>();
            await AddRangeAndSaveAsync(labels);
        }


        var report = new PartLabelRpt(labels);
        return report;
    }

    public void UpdateDatabase(List<Label> labels)
    {
        foreach (var label in labels)
        {
            label.GetProperty(FieldConstants.ProductCode, string.Empty);
        }

        AddRangeAndSaveAsync(labels);
    }

    public async Task<IEnumerable> GetPendingItemsAsync(Plan plan)
    {
        var pendingItems = plan.PlanItemDetails
            .Where(d => (d.OrderQuantity ?? 0) > (d.PrintQuantity ?? 0))
            .Select(d => new
            {
                d.Position,
                d.ItemCode,
                Name = d.Description,
                OrderQuantity = d.OrderQuantity ?? 0,
                PrintQuantity = d.PrintQuantity ?? 0,
                Family = d.Group,
                d.DrawingNo
            });

        return await Task.FromResult(pendingItems).ConfigureAwait(false);
    }

    public async Task<List<Label>> CreateLabelsAsync(PartLabelCrudDto dto, Plan plan)
    {
        /* ValidateDto(dto);

         var planItemDetail = plan.PlanItemDetails.FirstOrDefault(d => d.Position == dto.Position);
         if (null == planItemDetail)
             throw new Exception($"No item found for position '{dto.Position}' in plan");
         var pendingQuantity = planItemDetail.OrderQuantity.ToInt() - planItemDetail.PrintQuantity.ToInt();
         if (dto.Quantity.ToInt() > pendingQuantity)
             throw new Exception($"You can print only '{pendingQuantity}' quantity");

         var labels = new List<Label>();
         for (var index = 0; index < dto.Quantity; index++)
         {
             await Task.Delay(1).ConfigureAwait(false);
             var barcode = $"{DateTime.Now:ddMMyyyyhhmmssffff}";
             var label = new Label
             {
                 Code = barcode,

                 LabelDate = DateTime.Now,
                 LabelType = LabelType.Part.ToString(),

                 LotNo = plan.LotNo,

                 ProductionOrderNo = plan.ProductionOrderNo,
                 WarehousePosition = planItemDetail.WarehousePosition,

                 SoNo = plan.SoNo,
                 SoPosition = planItemDetail.SoPosition,

                 CarcassCode = planItemDetail.CarcassCode,
                 AssemblyCode = planItemDetail.AssemblyCode,
                 AssemblyQuantity = planItemDetail.SubAssemblyQuantity,

                 Position = planItemDetail.Position,
                 ItemId = planItemDetail.ItemId,
                 OrderQuantity = planItemDetail.OrderQuantity,
                 Quantity = dto.Quantity,

                 Barcode = barcode,

                 Reserved1 = planItemDetail.Reserved1,

                 Status = NewStatus,

                 NotMapped = new NotMapped
                 {
                     ItemCode = planItemDetail.ItemCode,
                     ItemName = planItemDetail.Description,
                 }
             };

             label.LabelDetails.Add(new LabelDetail
             {
                 ScanDate = DateTime.Now,
                 Status = NewStatus,
             });

             labels.Add(label);
         }

         return labels;*/

        return null;
    }

    public async Task<BaseReport> CreateLabelReportAsync(List<Label> labels, bool bDuplicate)
    {
        var labelRpt = new PartLabelRpt(labels, bDuplicate);
        return await Task.FromResult<BaseReport>(labelRpt).ConfigureAwait(false);
    }

    public async Task UpdateDatabaseAsync(List<Label> entities, Plan plan)
    {
        if (null == plan)
            throw new Exception("Invalid Plan");
        foreach (var label in entities)
        {
            var planItemDetail = plan.PlanItemDetails.FirstOrDefault(d =>
                d.Position == label.Position);
            if (null == planItemDetail) continue;
            planItemDetail.PrintQuantity ??= 0;
            planItemDetail.PrintQuantity += label.Quantity;
        }

        var planService = Bootstrapper.Get<IPlanService>();
        await planService.UpdateAndSaveAsync(plan).ConfigureAwait(false);
        await AddRangeAndSaveAsync(entities).ConfigureAwait(false);
    }

    public async Task<LabelViewDto> CreateViewDtoAsync(int? id)
    {
        var label = await FirstOrDefaultAsync(p => p.Id == id, p => p).ConfigureAwait(false);
        if (label == null)
            throw new Exception($"Label with Id '{id}' not found.");
        
        var dto = await GetLabelViewDto(label).ConfigureAwait(false);

        var report = await CreateLabelReportAsync(null, true).ConfigureAwait(false);
        dto.Base64 = Convert.ToBase64String(report.ToDocumentBytes());

        return dto;
    }

    public async Task<LabelViewDto> GetLabelViewDto(Label label)
    {
        try
        {
            // Perform all async operations first while DbContext is still alive
            var planService = Bootstrapper.Get<IPlanService>();
            var plan = await planService.GetByProductionOrderNoAsync(label.ProductionOrderNo).ConfigureAwait(false);
            var planItemDetail = plan?.PlanItemDetails?.FirstOrDefault(x => x.Position == label.Position);

            // Get users while DbContext is still alive
            var labelViewDetailDtos = label.LabelDetails.Adapt<List<LabelViewDetailDto>>();
            var userIds = labelViewDetailDtos.Select(d => d.CreatedBy).Where(id => id != null).Distinct().ToList();
            var users = userIds.Any()
                ? await _userService.GetAsync(p => userIds.Contains(p.Id), p => p).ConfigureAwait(false)
                : new List<AspNetUser>();

            // Get carton information using label barcode and warehouseOrderNo
            // Optimized: Use ignoreInclude to avoid loading all CartonDetails
            // The .Any() will be translated to SQL EXISTS subquery which is efficient with proper indexes
            var cartonService = Bootstrapper.Get<ICartonService>();
            var cartons = await cartonService.GetAsync(
                c => c.WarehouseOrderNo == label.WarehouseOrderNo &&
                     c.CartonDetails.Any(d => d.Barcode == label.Barcode),
                c => c,
                null,
                ignoreInclude: true).ConfigureAwait(false);
            var carton = cartons.FirstOrDefault();

            // Perform the mapping using existing configuration
            var dto = label.Adapt<LabelViewDto>();

            // Set additional properties directly after mapping
            dto.Family = planItemDetail?.Group;
            dto.Color = planItemDetail?.Reserved1;
            dto.DrawingNo = planItemDetail?.DrawingNo;
            dto.DueDate = plan?.DueDate;
            dto.ItemCode = planItemDetail?.ItemCode;
            dto.Description = planItemDetail?.Description;

            // Add status and quantities from PlanItemDetail
            dto.LabelStatus = label.Status;
            dto.PrintQuantity = planItemDetail?.PrintQuantity;
            dto.BendQuantity = planItemDetail?.BendQuantity;
            dto.SortQuantity = planItemDetail?.SortQuantity;
            dto.PackQuantity = planItemDetail?.PackQuantity;

            // Set carton information
            if (carton != null)
            {
                dto.CartonNo = carton.CartonNo;
                dto.CartonBarcode = carton.CartonBarcode;
                var cartonDetail = carton.CartonDetails?.FirstOrDefault(d => d.Barcode == label.Barcode);
                dto.CartonQuantity = cartonDetail?.Quantity;
            }

            // Set label details with user names
            dto.LabelViewDetailDto = labelViewDetailDtos;
            dto.LabelViewDetailDto.ForEach(d =>
            {
                d.UserName = users.FirstOrDefault(x => x.Id == d.CreatedBy)?.UserName;
            });

            label.NotMapped = new NotMapped
            {
                ItemCode = planItemDetail?.ItemCode,
                ItemName = planItemDetail?.Description
            };

            return dto;
        }
        catch (Exception exception)
        {
            LogHandler.LogInfo(exception);
            // Return a basic mapped DTO even if additional data loading fails
            var dto = label.Adapt<LabelViewDto>();
            dto.LabelStatus = label.Status;
            return dto;
        }
    }

    #endregion
}