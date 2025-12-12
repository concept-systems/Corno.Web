using Corno.Web.Areas.Admin.Services.Interfaces;
using Corno.Web.Areas.Kitchen.Dto.Label;
using Corno.Web.Areas.Kitchen.Dto.StiffenerLabel;
using Corno.Web.Areas.Kitchen.Labels;
using Corno.Web.Areas.Kitchen.Services.Interfaces;
using Corno.Web.Globals;
using Corno.Web.Models.Packing;
using Corno.Web.Models.Plan;
using Corno.Web.Reports;
using Corno.Web.Repository.Interfaces;
using Corno.Web.Services.File.Interfaces;
using Corno.Web.Services.Masters.Interfaces;
using Corno.Web.Windsor;
using Mapster;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Corno.Web.Areas.Kitchen.Services;

public class StiffenerLabelService : LabelService, IStiffenerLabelService
{
    #region -- Constructors --
    public StiffenerLabelService(IGenericRepository<Label> genericRepository, 
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
    protected bool ValidateDto(StiffenerLabelCrudDto dto)
    {
        if (dto.DueDate == null || dto.DueDate == DateTime.MinValue)
            throw new Exception("Invalid Due Date.");
        if (string.IsNullOrEmpty(dto.LotNo))
            throw new Exception("Invalid lotNo Order.");
        if (string.IsNullOrEmpty(dto.DrawingNo))
            throw new Exception("Invalid drawing No.");

        return true;
    }

    #endregion

    #region -- Private Methods --

    private async Task<List<Label>> GenerateLabelsAsync(StiffenerLabelCrudDto dto, Plan plan)
    {
        // 1. Create Label
        var planItemDetail = plan.PlanItemDetails.FirstOrDefault(d => d.DrawingNo == dto.DrawingNo);
        if (null == planItemDetail)
            throw new Exception($"No item found for drawingNo '{dto.DrawingNo}' in plan");

        var labels = new List<Label>();
        for (var index = 0; index < planItemDetail.OrderQuantity; index++)
        {
            await Task.Delay(1).ConfigureAwait(false);
            var barcode = $"{DateTime.Now:ddMMyyyyhhmmssffff}";
            var label = new Label
            {
                Code = barcode,

                LabelDate = DateTime.Now,
                LabelType = "Store",

                LotNo = plan.LotNo,
                WarehouseOrderNo = plan.WarehouseOrderNo,
                WarehousePosition = planItemDetail.WarehousePosition,

                SoNo = plan.SoNo,
                SoPosition = planItemDetail.SoPosition,

                CarcassCode = planItemDetail.CarcassCode,
                AssemblyCode = planItemDetail.AssemblyCode,
                AssemblyQuantity = planItemDetail.SubAssemblyQuantity,

                Position = planItemDetail.Position,
                ItemId = planItemDetail.ItemId,
                Description = planItemDetail.Description,
                OrderQuantity = planItemDetail.OrderQuantity,
                Quantity = 1,

                Barcode = barcode,
                SerialNo = plan.SerialNo,
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

        return labels;
    }
    #endregion

    #region  -- Public Methods --

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

    public async Task<List<Label>> CreateLabelsAsync(StiffenerLabelCrudDto dto, List<Plan> plans)
    {
        // 1. Validate Dto
        ValidateDto(dto);

        var labels = new List<Label>();

        foreach (var plan in plans)
        {
            var generatedLabels = await GenerateLabelsAsync(dto, plan).ConfigureAwait(false);
            labels.AddRange(generatedLabels);
        }

        return labels;
    }

    public async Task<BaseReport> CreateLabelReportAsync(List<Label> labels, bool bDuplicate)
    {
        var labelRpt = new PartLabelRpt(labels, bDuplicate);
        return await Task.FromResult<BaseReport>(labelRpt).ConfigureAwait(false);
    }

    public async Task UpdateDatabaseAsync(List<Label> labels, Plan plan)
    {
        if (null == plan)
            throw new Exception("Invalid Plan");
        foreach (var label in labels)
        {
            var planItemDetail = plan.PlanItemDetails.FirstOrDefault(d =>
                d.Position == label.Position);
            if (null == planItemDetail) continue;
            planItemDetail.PrintQuantity ??= 0;
            planItemDetail.PrintQuantity += label.Quantity;
        }

        // 2. Update Database
        var planService = Bootstrapper.Get<IPlanService>();
        await planService.UpdateAndSaveAsync(plan).ConfigureAwait(false);
        await AddRangeAndSaveAsync(labels).ConfigureAwait(false);
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
            ItemName = item?.Description
        };
        var userIds = dto.LabelViewDetailDto.Select(d => d.CreatedBy).ToList();
        var users = await _userService.GetAsync(p => userIds.Contains(p.Id), p => p).ConfigureAwait(false);

        dto.LabelViewDetailDto.ForEach(d =>
            d.UserName = users.FirstOrDefault(x => x.Id == d.CreatedBy)?.UserName);

        dto.LabelReport = await CreateLabelReportAsync(new List<Label> { label }, true).ConfigureAwait(false);
        return dto;
    }


    #endregion
}