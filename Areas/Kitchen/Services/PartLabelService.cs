using Corno.Web.Areas.Admin.Services.Interfaces;
using Corno.Web.Areas.Kitchen.Dto.Label;
using Corno.Web.Areas.Kitchen.Labels;
using Corno.Web.Areas.Kitchen.Services.Interfaces;
using Corno.Web.Extensions;
using Corno.Web.Globals;
using Corno.Web.Globals.Enums;
using Corno.Web.Models.Packing;
using Corno.Web.Models.Plan;
using Corno.Web.Reports;
using Corno.Web.Repository.Interfaces;
using Corno.Web.Services.File.Interfaces;
using Corno.Web.Windsor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Corno.Web.Areas.Kitchen.Services;

public class PartLabelService : LabelService, IPartLabelService
{
    #region -- Constructors --
    public PartLabelService(IGenericRepository<Label> genericRepository, IExcelFileService<SalvaginiExcelDto> excelFileService,
        IUserService userService)
        : base(genericRepository, excelFileService, userService)
    {
    }
    #endregion

    #region -- Data Members --

    private const string NewStatus = StatusConstants.Printed;

    #endregion

    #region -- Protected Methods --
    protected bool ValidateDto(PartLabelCrudDto dto)
    {
        if (string.IsNullOrEmpty(dto.WarehouseOrderNo))
            throw new Exception("Invalid warehouse Order.");

        if (string.IsNullOrEmpty(dto.Position))
            throw new Exception("Invalid position.");

        if (dto.Quantity <= 0)
            throw new Exception("Invalid quantity.");

        return true;
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

    public async Task<List<Label>> CreateLabelsAsync(PartLabelCrudDto dto, Plan plan)
    {
        ValidateDto(dto);

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

                WarehouseOrderNo = plan.WarehouseOrderNo,
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

        return labels;
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
        foreach(var label in entities)
        {
            var planItemDetail = plan.PlanItemDetails.FirstOrDefault(d =>
                d.Position == label.Position);
            if(null == planItemDetail) continue;
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

        var report = await CreateLabelReportAsync([label], true).ConfigureAwait(false);
        dto.Base64 = Convert.ToBase64String(report.ToDocumentBytes());
        System.IO.File.WriteAllBytes("D://debug.pdf", report.ToDocumentBytes());

        return dto;
    }

    #endregion
}