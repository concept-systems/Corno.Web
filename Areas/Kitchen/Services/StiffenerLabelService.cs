using Corno.Web.Areas.Admin.Services.Interfaces;
using Corno.Web.Areas.Kitchen.Dto.Label;
using Corno.Web.Areas.Kitchen.Dto.StiffenerLabel;
using Corno.Web.Areas.Kitchen.Services.Interfaces;
using Corno.Web.Extensions;
using Corno.Web.Globals;
using Corno.Web.Models.Packing;
using Corno.Web.Models.Plan;
using Corno.Web.Repository.Interfaces;
using Corno.Web.Services.File.Interfaces;
using Corno.Web.Services.Masters.Interfaces;
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

    /// <summary>
    /// Gets the next serial number for a given warehouse order number
    /// SerialNo should be unique for every WarehouseOrderNo
    /// </summary>
    private async Task<int> GetNextSerialNoAsync(string warehouseOrderNo)
    {
        var maxSerialNo = await MaxAsync(l => l.WarehouseOrderNo == warehouseOrderNo, l => l.SerialNo ?? 0).ConfigureAwait(false);
        return maxSerialNo + 1;
    }

    private async Task<List<Label>> GenerateLabelsAsync(StiffenerLabelCrudDto dto, Plan plan)
    {
        // 1. Create Label
        var planItemDetail = plan.PlanItemDetails.FirstOrDefault(d => d.DrawingNo == dto.DrawingNo);
        if (null == planItemDetail)
            throw new Exception($"No item found for drawingNo '{dto.DrawingNo}' in plan");

        var labels = new List<Label>();
        var serialNo = await GetNextSerialNoAsync(plan.WarehouseOrderNo);
        
        for (var index = 0; index < planItemDetail.OrderQuantity; index++)
        {
            // Generate barcode as "CRN" + serialNo padded to 6 digits
            var barcode = $"CRN{serialNo.ToString().PadLeft(6, '0')}";
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
                SerialNo = serialNo,
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
            serialNo++; // Increment serial number for next label
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
        
        // Group plans by WarehouseOrderNo to get unique serial numbers per warehouse order
        var plansByWarehouseOrder = plans.GroupBy(p => p.WarehouseOrderNo);

        foreach (var warehouseOrderGroup in plansByWarehouseOrder)
        {
            var warehouseOrderNo = warehouseOrderGroup.Key;
            // Get the next serial number for this warehouse order number
            var currentSerialNo = await GetNextSerialNoAsync(warehouseOrderNo).ConfigureAwait(false);

            foreach (var plan in warehouseOrderGroup)
            {
                var planItemDetail = plan.PlanItemDetails.FirstOrDefault(d => d.DrawingNo == dto.DrawingNo);
                if (planItemDetail != null)
                {
                    var generatedLabels = await GenerateLabelsAsync(dto, plan).ConfigureAwait(false);
                    labels.AddRange(generatedLabels);
                }
            }
        }

        return labels;
    }

    #endregion
}