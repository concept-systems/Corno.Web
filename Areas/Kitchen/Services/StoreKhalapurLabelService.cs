using Corno.Web.Areas.Admin.Services.Interfaces;
using Corno.Web.Areas.Kitchen.Dto.Label;
using Corno.Web.Areas.Kitchen.Dto.StoreLabel;
using Corno.Web.Areas.Kitchen.Labels;
using Corno.Web.Areas.Kitchen.Services.Interfaces;
using Corno.Web.Globals;
using Corno.Web.Models.Packing;
using Corno.Web.Models.Plan;
using Corno.Web.Reports;
using Corno.Web.Repository.Interfaces;
using Corno.Web.Services.File.Interfaces;
using Corno.Web.Windsor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Corno.Web.Areas.Kitchen.Services;

public class StoreKhalapurLabelService : LabelService, IStoreKhalapurLabelService
{
    #region -- Constructors --
    public StoreKhalapurLabelService(IGenericRepository<Label> genericRepository, 
        IExcelFileService<SalvaginiExcelDto> excelFileService,
        IUserService userService)
        : base(genericRepository, excelFileService, userService)
    {
        
    }
    #endregion

    #region -- Data Members --

    private const string NewStatus = StatusConstants.Sorted;

    #endregion

    #region -- Protected Methods --
    protected bool ValidateDto(StoreLabelCrudDto dto)
    {
        if (dto.DueDate == null || dto.DueDate == DateTime.MinValue)
            throw new Exception("Invalid Due Date.");
        if (string.IsNullOrEmpty(dto.LotNo))
            throw new Exception("Invalid lotNo Order.");
        if (string.IsNullOrEmpty(dto.WarehouseOrderNo))
            throw new Exception("Invalid warehouse Order.");

        if (string.IsNullOrEmpty(dto.Family))
            throw new Exception("Invalid family Order.");

        if (dto.StoreLabelCrudDetailDtos.All(d => d.IsSelected == false))
            throw new Exception("You have not selected any position");

        return true;
    }

    #endregion

    #region  -- Public Methods --
    public async Task<List<StoreLabelCrudDetailDto>> GetPendingItemsAsync(Plan plan, string family)
    {
        var pendingItems = plan.PlanItemDetails
            .Where(d => d.Group.Equals(family) && d.ItemType.Equals(FieldConstants.Bo) && 
                        (d.OrderQuantity ?? 0) > (d.PrintQuantity ?? 0))
            .Select(d => new StoreLabelCrudDetailDto
            {
                ItemCode = d.ItemCode,
                ItemName = d.ItemType,
                Position = d.Position,
                IsSelected = false,
            }).ToList();

        return await Task.FromResult(pendingItems).ConfigureAwait(false);
    }
    
    public async Task<List<Label>> CreateLabelsAsync(StoreLabelCrudDto dto, Plan plan)
    {
        ValidateDto(dto);

        var labels = new List<Label>();
        var selectedDetailDtos = dto.StoreLabelCrudDetailDtos.Where(d => d.IsSelected)
            .ToList();
        foreach(var selectedDetailDto in selectedDetailDtos)
        {
            var planItemDetail = plan.PlanItemDetails.FirstOrDefault(d =>
                d.Position == selectedDetailDto.Position);
            if(null == planItemDetail) continue;

            for (int index = 0; index < planItemDetail.OrderQuantity; index++)
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
                    OrderQuantity = planItemDetail.OrderQuantity,
                    Quantity = 1,

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

        var planService = Bootstrapper.Get<IPlanService>();
        await planService.UpdateAndSaveAsync(plan).ConfigureAwait(false);
        await AddRangeAndSaveAsync(labels).ConfigureAwait(false);
    }
    #endregion
}