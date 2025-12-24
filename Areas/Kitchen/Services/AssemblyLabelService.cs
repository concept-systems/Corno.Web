using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Corno.Web.Areas.Kitchen.Dto.Label;
using Corno.Web.Areas.Kitchen.Labels;
using Corno.Web.Extensions;
using Corno.Web.Globals;
using Corno.Web.Globals.Enums;
using Corno.Web.Models.Packing;
using Corno.Web.Models.Plan;
using Corno.Web.Reports;
using Corno.Web.Services.Masters.Interfaces;
using Mapster;
using Corno.Web.Areas.Admin.Services.Interfaces;
using Corno.Web.Areas.Kitchen.Services.Interfaces;
using Corno.Web.Repository.Interfaces;
using Corno.Web.Services.File.Interfaces;
using Corno.Web.Windsor;
using MoreLinq;

namespace Corno.Web.Areas.Kitchen.Services;

public class AssemblyLabelService : LabelService, IAssemblyLabelService
{
    #region -- Constructors --
    public AssemblyLabelService(IGenericRepository<Label> genericRepository,
        IBaseItemService itemService, IExcelFileService<SalvaginiExcelDto> excelFileService,
        IUserService userService)
    : base(genericRepository, excelFileService, userService)
    {
        _itemService = itemService;
        _userService = userService;
    }
    #endregion

    #region -- Data Members --

    private readonly IBaseItemService _itemService;
    private readonly IUserService _userService;

    private const string NewStatus = StatusConstants.Printed;

    #endregion

    #region -- Protected Methods --
    protected bool ValidateDto(SubAssemblyCrudDto dto)
    {
        return string.IsNullOrEmpty(dto.Barcode1) ? throw new Exception("Invalid Barcode 1.") : true;
    }

    private static void ValidateLabels(IReadOnlyList<Label> labels, Plan plan)
    {
        var expectedStatus = new[] { StatusConstants.Sorted };
        var families2223 = new[] { "FGWN22", "FGWN23" };

        for (var index = 0; index < labels.Count; index++)
        {
            var label = labels[index];
            if (!expectedStatus.Contains(label.Status))
            {
                // Check whether family is 22 / 23
                var planItemDetail = plan.PlanItemDetails.FirstOrDefault(d => d.Position == label.Position);
                if (families2223.Contains(planItemDetail?.Group))
                {
                    if (labels.All(d => d.Status != StatusConstants.Sorted))
                        throw new Exception($"At least single label should be sorted.");
                }
                else
                    throw new Exception($"Label {label.Barcode}' has '{label.Status}' status. Expected status is '{string.Join(",", expectedStatus)}'.");
            }

            var assemblyCode = label.AssemblyCode;
            if (string.IsNullOrEmpty(assemblyCode))
                throw new Exception($"Invalid assembly code {assemblyCode} of label {index}.");
        }

        var label1 = labels.First();
        var assemblyLabelCount = plan.PlanItemDetails
            .Where(d => d.AssemblyCode == label1.AssemblyCode)
            .DistinctBy(d => d.Position)
            .Select(d => d.OrderQuantity).Sum().ToInt();
        if (assemblyLabelCount <= 1)
            throw new Exception($"There are only {assemblyLabelCount} items against assembly code {label1.AssemblyCode} 1");
        if (!assemblyLabelCount.Equals(labels.Count))
            throw new Exception($"Assembly code total order quantity is {assemblyLabelCount}, but label printed count is {labels.Count}");
    }
    #endregion

    #region  -- Public Methods --

    public async Task<List<Label>> CreateLabelsAsync(SubAssemblyCrudDto dto, Plan plan, Label label1,
        bool bUpdateDatabase)
    {
        // 1. Validate Dto
        ValidateDto(dto);

        var planItemDetail = plan.PlanItemDetails.FirstOrDefault(d => d.Position == label1.Position);
        var labels = await GetAsync(p => p.WarehouseOrderNo == label1.WarehouseOrderNo && 
                                         p.AssemblyCode == label1.AssemblyCode, p => p,
            null, // Explicitly specify null for the orderBy parameter
            false // Explicitly specify false for the last bool parameter to resolve ambiguity
        ).ConfigureAwait(false);
        // Remove duplicate barcodes.
        labels = labels.DistinctBy(l => l.Barcode).ToList();

        // Validate labels
        ValidateLabels(labels, plan);

        var subAssemblyItem = await _itemService.FirstOrDefaultAsync(i => i.Code == planItemDetail.AssemblyCode,
            i => new { i.Id }).ConfigureAwait(false);
        var assemblyLabel = new Label
        {
            LabelDate = DateTime.Now,
            LabelType = nameof(LabelType.SubAssembly),

            SoNo = plan.SoNo,
            SoPosition = planItemDetail?.SoPosition,
            WarehouseOrderNo = label1.WarehouseOrderNo,
            WarehousePosition = label1.WarehousePosition,
            Position = planItemDetail?.Position,

            CarcassCode = planItemDetail?.CarcassCode,
            AssemblyCode = planItemDetail?.AssemblyCode,
            AssemblyQuantity = planItemDetail?.SubAssemblyQuantity ?? 0,

            ProductId = label1.ProductId,
            ItemId = subAssemblyItem?.Id,
            OrderQuantity = label1.OrderQuantity,
            Quantity = 1,

            Barcode = $"SUB{DateTime.Now:ddMMyyhhmmssffff}",

            Links = $"{label1.Id}",

            Operation = OperationConstants.SubAssembly,
            NextOperation = OperationConstants.Packing,

            Reserved1 = planItemDetail?.Reserved1,

            Status = StatusConstants.SubAssembled,

            NotMapped = new NotMapped
            {
                ItemCode = planItemDetail?.ItemCode,
                ItemName = planItemDetail?.DrawingNo
            }
        };

        var links = string.Join(",", labels.Select(p => p.Id.ToString()));
        labels.ForEach(p => p.Links = links);

        //// Update Database
        if (bUpdateDatabase)
            await UpdateDatabaseAsync(assemblyLabel, labels, plan).ConfigureAwait(false);

        return [assemblyLabel];
    }

    public async Task<BaseReport> CreateLabelReportAsync(IEnumerable<Label> labels, bool bDuplicate)
    {
        var labelRpt = new SubAssemblyLabelRpt(labels);
        return await Task.FromResult<BaseReport>(labelRpt).ConfigureAwait(false);
    }

    public async Task UpdateDatabaseAsync(Label assemblyLabel, List<Label> labels, Plan plan)
    {
        if (null == plan)
            throw new Exception("Invalid Plan");
        foreach (var label in labels)
        {
            var planItemDetail = plan.PlanItemDetails.FirstOrDefault(d =>
                d.Position == label.Position);
            if (null == planItemDetail) continue;
            planItemDetail.SubAssemblyQuantity ??= 0;
            planItemDetail.SubAssemblyQuantity += label.Quantity;
        }

        // 2. Update Database
        var planService = Bootstrapper.Get<IPlanService>();
        await planService.UpdateAndSaveAsync(plan).ConfigureAwait(false);
        // 3. Update labels
        await UpdateRangeAndSaveAsync(labels).ConfigureAwait(false);
        // 4. Add Assembly label
        await AddAndSaveAsync(assemblyLabel).ConfigureAwait(false);
    }

    public async Task<LabelViewDto> CreateViewDtoAsync(int? id)
    {
        var label = await GetByIdAsync(id).ConfigureAwait(false);
        if (null == label)
            throw new Exception($"Label with Id '{id}' not found.");

        // Create dto
        var dto = label.Adapt<LabelViewDto>();

        // Create label report
        var item = await _itemService.GetViewModelAsync(label.ItemId ?? 0).ConfigureAwait(false);
        label.NotMapped = new NotMapped
        {
            ItemCode = item?.Code,
            DrawingNo = item?.Description,

        };
        var userIds = dto.LabelViewDetailDto.Select(d => d.CreatedBy).ToList();
        var users = await _userService.GetAsync(u => userIds.Contains(u.Id), u => u).ConfigureAwait(false);

        dto.LabelViewDetailDto.ForEach(d =>
            d.UserName = users.FirstOrDefault(x => x.Id == d.CreatedBy)?.UserName);

        dto.LabelReport = await CreateLabelReportAsync(new List<Label> { label }, true).ConfigureAwait(false);
        return dto;
    }
    #endregion
}