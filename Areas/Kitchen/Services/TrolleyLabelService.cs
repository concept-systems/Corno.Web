 using System;
using System.Collections.Generic;
using System.Linq;
using Corno.Web.Areas.Kitchen.Services.Interfaces;
using System.Threading.Tasks;
using Corno.Web.Globals;
using Corno.Web.Models.Packing;
using Corno.Web.Models.Plan;
using Corno.Web.Globals.Enums;
using Corno.Web.Areas.Kitchen.Dto.TrolleyLabel;
using Corno.Web.Areas.Kitchen.Labels;
using Corno.Web.Reports;
using Microsoft.Ajax.Utilities;
using Corno.Web.Areas.Kitchen.Dto.Label;
using Mapster;
using Corno.Web.Services.Masters.Interfaces;
using Corno.Web.Areas.Admin.Services.Interfaces;
using Corno.Web.Repository.Interfaces;
 using Corno.Web.Services.File.Interfaces;
 using Corno.Web.Windsor;

 namespace Corno.Web.Areas.Kitchen.Services;
public class TrolleyLabelService : LabelService, ITrolleyLabelService
{
    #region -- Constructors --
    public TrolleyLabelService(IGenericRepository<Label> genericRepository, IBaseItemService itemService, IExcelFileService<SalvaginiExcelDto> excelFileService,
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

    private const string NewStatus = StatusConstants.Active;

    #endregion

    #region -- Protected Methods --
    protected bool ValidateDto(TrolleyLabelCrudDto dto)
    {
        if (dto.DueDate == null || dto.DueDate == DateTime.MinValue)
            throw new Exception("Invalid Due Date.");
        if (string.IsNullOrEmpty(dto.LotNo))
            throw new Exception("Invalid lot No.");
        if (string.IsNullOrEmpty(dto.WarehouseOrderNo))
            throw new Exception("Invalid warehouse Order.");
        if (string.IsNullOrEmpty(dto.Family))
            throw new Exception("Invalid family.");
        return true;
    }

    #endregion

    #region  -- Public Methods --
    public async Task<List<Label>> CreateLabelsAsync(TrolleyLabelCrudDto dto, Plan plan)
    {
        // 1. Validate Dto
        ValidateDto(dto);

        var planItemDetails = plan.PlanItemDetails.Where(d =>
            d.Group == dto.Family).DistinctBy(d => d.CarcassCode);

        var existing = await GetAsync(l => l.WarehouseOrderNo == dto.WarehouseOrderNo &&
                                l.LabelType == LabelType.Trolley.ToString(),
            l => new { l.Id, l.CarcassCode }).ConfigureAwait(false);
        
        // Filter out existing labels first
        var newDetails = planItemDetails.ToList()
            .Where(detail => existing.FirstOrDefault(l => l.CarcassCode == detail.CarcassCode) == null)
            .ToList();
        
        if (!newDetails.Any())
            throw new Exception("All trolley labels are already printed. Please, print duplicate");
        
        // Pre-compute starting serial number to avoid deadlock
        var startSerialNo = await GetNextSequenceAsync(FieldConstants.BarcodeLabelSerialNo).ConfigureAwait(false);
        
        var labels = newDetails.Select((detail, index) => new Label
        {
            SerialNo = startSerialNo + index,
            SoNo = plan.SoNo,
            Code = (startSerialNo + index).ToString().PadLeft(7, '0'),
            LabelDate = DateTime.Now,
            LabelType = LabelType.Trolley.ToString(),
            ProductId = plan.ProductId,
            ItemId = detail.ItemId,
            OrderQuantity = detail.OrderQuantity ?? 0,
            Quantity = 1,
            Barcode = (startSerialNo + index).ToString(),
            WarehouseOrderNo = dto.WarehouseOrderNo,
            WarehousePosition = detail.WarehousePosition,
            Position = detail.Position,
            CarcassCode = detail.CarcassCode,
            LotNo = plan.LotNo,
            Operation = OperationConstants.None,
            Status = NewStatus
        });

        return await Task.FromResult(labels.ToList()).ConfigureAwait(false);
    }
    public async Task<BaseReport> CreateLabelReportAsync(List<Label> labels, bool bDuplicate)
    {
        var labelRpt = new TrolleyLabelRpt(labels);
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
        await planService.UpdateAsync(plan).ConfigureAwait(false);
        await AddRangeAndSaveAsync(labels).ConfigureAwait(false);
    }

    public async Task<LabelViewDto> CreateViewDtoAsync(int? id)
    {
        var label = await FirstOrDefaultAsync(p => p.Id == id, p => p).ConfigureAwait(false);
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