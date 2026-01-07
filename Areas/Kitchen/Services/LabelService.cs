using Corno.Web.Areas.Admin.Models;
using Corno.Web.Areas.Admin.Services.Interfaces;
using Corno.Web.Areas.Kitchen.Dto.Kitting;
using Corno.Web.Areas.Kitchen.Dto.Label;
using Corno.Web.Areas.Kitchen.Dto.Sorting;
using Corno.Web.Areas.Kitchen.Labels;
using Corno.Web.Areas.Kitchen.Services.Interfaces;
using Corno.Web.Extensions;
using Corno.Web.Globals;
using Corno.Web.Globals.Enums;
using Corno.Web.Logger;
using Corno.Web.Models.Packing;
using Corno.Web.Models.Plan;
using Corno.Web.Reports;
using Corno.Web.Repository.Interfaces;
using Corno.Web.Services;
using Corno.Web.Services.File.Interfaces;
using Corno.Web.Windsor;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Mapster;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


namespace Corno.Web.Areas.Kitchen.Services;

public class LabelService : BaseService<Label>, ILabelService
{
    #region -- Constructors --
    public LabelService(IGenericRepository<Label> genericRepository, IExcelFileService<SalvaginiExcelDto> excelFileService, IUserService userService) : base(genericRepository)
    {
        _excelFileService = excelFileService;
        _userService = userService;

        SetIncludes(nameof(Label.LabelDetails));
    }
    #endregion

    #region -- Data Members --

    private readonly IExcelFileService<SalvaginiExcelDto> _excelFileService;
    private readonly IUserService _userService;

    #endregion

    #region -- Protected Methods --

    protected async Task<LabelViewDto> GetLabelViewDtoAsync(Label label)
    {
        try
        {
            // Perform all async operations first while DbContext is still alive
            var planService = Bootstrapper.Get<IPlanService>();
            var plan = await planService.GetByWarehouseOrderNoAsync(label.WarehouseOrderNo).ConfigureAwait(false);
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
                     c.CartonDetails.Any(d => d.Barcode == label.Barcode && d.Position == label.Position),
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

    protected virtual bool ValidateDto(LabelViewDto dto)
    {
        return false;
    }

    protected byte[] GetPdfReport(BaseReport report)
    {
        var reportProcessor = new Telerik.Reporting.Processing.ReportProcessor();
        var result = reportProcessor.RenderReport("PDF", report, null);

        return result.DocumentBytes;
    }

    /// <summary>
    /// Creates a label report for view. Override this method in derived services to provide service-specific report creation.
    /// </summary>
    public async Task<BaseReport> CreateLabelReportAsync(List<Label> labels, bool bDuplicate, LabelType? labelType = null)
    {
        // If labelType is provided, create the appropriate report
        if (!labelType.HasValue) 
            return await Task.FromResult<BaseReport>(null).ConfigureAwait(false);

        return labelType.Value switch
        {
            LabelType.StoreShirwal => await Task.FromResult<BaseReport>(new ShirwalPartLabelRpt(labels, bDuplicate))
                .ConfigureAwait(false),
            LabelType.SubAssembly => await Task.FromResult<BaseReport>(new SubAssemblyLabelRpt(labels))
                .ConfigureAwait(false),
            LabelType.Trolley => await Task.FromResult<BaseReport>(new TrolleyLabelRpt(labels))
                .ConfigureAwait(false),
            _ => await Task.FromResult<BaseReport>(new PartLabelRpt(labels, bDuplicate)).ConfigureAwait(false)
        };
    }

    #endregion

    #region -- Private Methods --
    public void ConfigureMapping(MapContext context)
    {
        var userId = context.Parameters["UserId"] as string ?? "System";
        var isUpdate = context.Parameters.ContainsKey("IsUpdate") && (bool)context.Parameters["IsUpdate"];
        TypeAdapterConfig<SalvaginiExcelDto, Label>
            .NewConfig()
            .AfterMapping((src, dest) =>
            {
                dest.Status = StatusConstants.Sorted;
                dest.ModifiedBy = userId;
                dest.ModifiedDate = DateTime.Now;
                dest.CreatedDate = DateTime.Now;
            });

        TypeAdapterConfig<SalvaginiExcelDto, LabelDetail>
            .NewConfig()
            .AfterMapping((src, dest) =>
            {
                dest.Status = StatusConstants.Sorted;
                dest.ModifiedDate = DateTime.Now;
                if (isUpdate) return;
                dest.CreatedBy = userId;
                dest.CreatedDate = DateTime.Now;
            });
    }

    public async Task PerformKittingAsync(KittingCrudDto dto, string userId)
    {
        var dueDate = dto.DueDate;
        var label = await FirstOrDefaultAsync(d => d.Barcode == dto.Barcode && dueDate == dto.DueDate, d => d,
                d => d.OrderByDescending(x => x.Id)).ConfigureAwait(false);

        if (null == label)
            throw new Exception($"No label found for barcode {dto.Barcode} ");

        if (null == dueDate)
            throw new Exception($" barcode due date dose not match  {dto.DueDate} ");
        var oldStatus = new List<string> { "Active", "Printed" };
        if (label.Status != StatusConstants.Active && label.Status != StatusConstants.Printed)
            throw new Exception($"Expected label status is '{string.Join(",", oldStatus)}', but current label status is '{label.Status}'");
        var planService = Bootstrapper.Get<IPlanService>();
        var plan = await planService.GetByWarehouseOrderNoAsync(label.WarehouseOrderNo).ConfigureAwait(false);
        var planItemDetail = plan?.PlanItemDetails?.FirstOrDefault(d =>
            d.Position == label.Position);
        if (planItemDetail != null)
        {
            planItemDetail.BendQuantity ??= 0;
            planItemDetail.BendQuantity += label.Quantity;
        }
        // Save updates
        await planService.UpdateAndSaveAsync(plan).ConfigureAwait(false);

        var newStatus = StatusConstants.Bent;
        label.Status = newStatus;
        label.LabelDetails.Add(new LabelDetail
        {
            ScanDate = DateTime.Now,
            Quantity = label.Quantity,
            Status = newStatus,
            CreatedBy = userId,
            CreatedDate = DateTime.Now
        });

        await UpdateAndSaveAsync(label).ConfigureAwait(false);

        dto.ItemCode = planItemDetail?.ItemCode;
        dto.ItemName = planItemDetail?.Description;
        dto.SoNo = plan?.SoNo;
        dto.WarehousePosition = planItemDetail?.CarcassCode;
    }

    public async Task PerformSortingAsync(SortingCrudDto dto, string userId)
    {
        var label = await FirstOrDefaultAsync(d => d.Barcode == dto.Barcode, d => d,
            d => d.OrderByDescending(x => x.Id)).ConfigureAwait(false);
        if (null == label)
            throw new Exception($"No label found for barcode {dto.Barcode}");
        if (label.Status != StatusConstants.Bent)
            throw new Exception($"Expected label status is 'Bent', but current label status is '{label.Status}'");
        var planService = Bootstrapper.Get<IPlanService>();
        var plan = await planService.GetByWarehouseOrderNoAsync(label.WarehouseOrderNo).ConfigureAwait(false);
        var planItemDetail = plan?.PlanItemDetails?.FirstOrDefault(d =>
            d.Position == label.Position);
        if (planItemDetail != null)
        {
            planItemDetail.SortQuantity ??= 0;
            planItemDetail.SortQuantity += label.Quantity;
        }
        // Save updates
        await planService.UpdateAndSaveAsync(plan).ConfigureAwait(false);

        var newStatus = StatusConstants.Sorted;
        label.Status = newStatus;
        label.LabelDetails.Add(new LabelDetail
        {
            ScanDate = DateTime.Now,
            Quantity = label.Quantity,
            Status = newStatus,
            CreatedBy = userId,
            CreatedDate = DateTime.Now
        });

        await UpdateAndSaveAsync(label).ConfigureAwait(false);

        dto.ItemCode = planItemDetail?.ItemCode;
        dto.ItemName = planItemDetail?.Description;
        dto.SoNo = plan?.SoNo;
        dto.WarehousePosition = planItemDetail?.CarcassCode;
    }

    // OLD METHOD - REMOVED: ImportAsync with progress service
    // This functionality is now in LabelBusinessImportService
    // The import is now handled through FileImportService<SalvaginiExcelDto> which delegates to LabelBusinessImportService

    #endregion

    #region -- Public Methods --
    public async Task<DataSourceResult> GetIndexDataSourceAsync(DataSourceRequest request)
    {
        // Project on server using Kendo ToDataSourceResultAsync for paging
        return await GetQuery().ProjectToType<LabelIndexDto>().ToDataSourceResultAsync(request).ConfigureAwait(false);
    }

    public async Task<LabelViewDto> CreateViewDtoAsync(int? id, LabelType? labelType = null)
    {
        var label = await FirstOrDefaultAsync(p => p.Id == id, p => p).ConfigureAwait(false);
        if (label == null)
            throw new Exception($"Label with Id '{id}' not found.");

        // If labelType is not provided, try to infer it from the label's LabelType property
        if (!labelType.HasValue && !string.IsNullOrEmpty(label.LabelType))
        {
            if (Enum.TryParse<LabelType>(label.LabelType, true, out var inferredType))
            {
                labelType = inferredType;
            }
        }

        var dto = await GetLabelViewDtoAsync(label).ConfigureAwait(false);

        var report = await CreateLabelReportAsync([label], true, labelType).ConfigureAwait(false);
        if (report != null)
            dto.Base64 = Convert.ToBase64String(report.ToDocumentBytes());

        return dto;
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
    #endregion
}
