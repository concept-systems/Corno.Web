using Corno.Web.Areas.Admin.Models;
using Corno.Web.Areas.Admin.Services.Interfaces;
using Corno.Web.Areas.Kitchen.Dto.Kitting;
using Corno.Web.Areas.Kitchen.Dto.Label;
using Corno.Web.Areas.Kitchen.Dto.Sorting;
using Corno.Web.Areas.Kitchen.Services.Interfaces;
using Corno.Web.Extensions;
using Corno.Web.Globals;
using Corno.Web.Logger;
using Corno.Web.Models.Packing;
using Corno.Web.Models.Plan;
using Corno.Web.Reports;
using Corno.Web.Repository.Interfaces;
using Corno.Web.Services;
using Corno.Web.Services.File.Interfaces;
using Corno.Web.Services.Progress.Interfaces;
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

    protected async Task<LabelViewDto> GetLabelViewDto(Label label)
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
            var cartonService = Bootstrapper.Get<ICartonService>();
            var carton = await cartonService.FirstOrDefaultAsync(
                c => c.WarehouseOrderNo == label.WarehouseOrderNo && 
                     c.CartonDetails.Any(d => d.Barcode == label.Barcode), 
                c => c).ConfigureAwait(false);

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

    public async Task PerformKitting(KittingCrudDto dto, string userId)
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

    public async Task PerformSorting(SortingCrudDto dto, string userId)
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

    public async Task<List<SalvaginiExcelDto>> Import(Stream fileStream, string filePath,
        string newStatus, IBaseProgressService progressService, string userId)
    {
        try
        {
            progressService.ResetProgressModel();

            progressService.Report("Reading excel file", MessageType.Progress);
            var records = _excelFileService.Read(fileStream)
                .ToList().Trim();
            if (!records.Any())
                throw new Exception("No records found to import.");

            // Create progress model

            var unImportedBarcodes = new List<string>();

            progressService.Report("Loading labels", MessageType.Progress);
            var barcodes = records.Select(r => r.Barcode).Where(d => null != d).Distinct().ToList();
            if (!barcodes.Any())
                throw new Exception("All barcodes are null. Check excel file.");
            var labels = await GetAsync(l => barcodes.Contains(l.Barcode), l => l).ConfigureAwait(false);
            // Get only distinct with last entry
            labels = labels.GroupBy(l => l.Barcode)
                .Select(g => g.OrderBy(x => x.Id).LastOrDefault())
                .ToList();
            var warehouseOrderNos = labels.Select(l => l.WarehouseOrderNo).Distinct().ToList();

            progressService.Report("Loading plans", MessageType.Progress);
            var planService = Bootstrapper.Get<IPlanService>();
            var plans = (await planService.GetAsync(p => warehouseOrderNos.Contains(p.WarehouseOrderNo), p => p).ConfigureAwait(false)).ToList();
            var updatedLabels = new List<Label>();
            var updatedPlans = new List<Plan>();
            foreach (var record in records)
            {
                try
                {
                    var label = labels.LastOrDefault(l => l.Barcode == record.Barcode);
                    if (null == label)
                    {
                        unImportedBarcodes.Add(record.Barcode);
                        progressService.Report(0, 0, 1);
                        record.Status = StatusConstants.Exists;
                        record.Remark = $"Label not found for barcode {record.Barcode}.";
                        continue;
                    }
                    var context = new MapContext
                    {
                        Parameters = { ["UserId"] = userId, ["IsUpdate"] = false }
                    };
                    ConfigureMapping(context);
                    // Execute plan operation
                    var plan = plans.FirstOrDefault(p => p.WarehouseOrderNo == label.WarehouseOrderNo);
                    if (null == plan)
                    {
                        unImportedBarcodes.Add(record.Barcode);
                        progressService.Report(0, 0, 1);
                        record.Status = StatusConstants.Ignore;
                        record.Remark = $"Plan with warehouse order {label.WarehouseOrderNo} not found.";
                        continue;
                    }

                    if (label.Status == newStatus)
                    {
                        progressService.Report(0, 1, 0);
                        record.Status = StatusConstants.Exists;
                        continue;
                    }

                    var planDetail = plan.PlanItemDetails.FirstOrDefault(d =>
                        d.Position == label.Position);
                    if (null == planDetail)
                        throw new Exception($"No plan detail available warehouse order no {plan?.WarehouseOrderNo} & " +
                                            $"Item Id {label.ItemId} & Position {label.Position}");
                    await planService.IncreasePlanQuantityAsync(plan, planDetail, label.Quantity ?? 0, newStatus).ConfigureAwait(false);
                    
                    //_planService.IncreasePlanQuantity(plan, label, newStatus);
                    updatedPlans.Add(plan);

                    // Execute barcode label operation
                    //label.Operation = operation;
                    label.Status = newStatus;
                    label.AddDetail(null, null, null, null, newStatus, null);
                    //Update(label);
                    updatedLabels.Add(label);

                    //UpdateAndSave(label);
                    progressService.Report(1, 0, 0);
                    record.Status = StatusConstants.Imported;
                }
                catch (Exception exception)
                {
                    unImportedBarcodes.Add(record.Barcode);
                    progressService.Report(0, 0, 1);
                    record.Status = StatusConstants.Ignore;
                    record.Remark = LogHandler.GetDetailException(exception)?.Message;
                    LogHandler.LogError(exception);
                }
            }

            progressService.Report("Saving Records");
            await planService.UpdateRangeAndSaveAsync(updatedPlans).ConfigureAwait(false);
            await UpdateRangeAndSaveAsync(updatedLabels).ConfigureAwait(false);
            progressService.Report("Saved Records");

            progressService.Report("Updated Successfully", MessageType.Info);

            return records;
        }
        catch (Exception exception)
        {
            LogHandler.LogError(exception);
            progressService.Report(LogHandler.GetDetailException(exception).Message, MessageType.Error);
            throw;
        }
    }

    #endregion

    #region -- Public Methods --
    public async Task<DataSourceResult> GetIndexDataSource(DataSourceRequest request)
    {
        // Project on server using Kendo ToDataSourceResultAsync for paging
        return await GetQuery().ProjectToType<LabelIndexDto>().ToDataSourceResultAsync(request).ConfigureAwait(false);
    }
    #endregion
}