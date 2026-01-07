using Corno.Web.Areas.Admin.Services.Interfaces;
using Corno.Web.Areas.Kitchen.Dto.Label;
using Corno.Web.Areas.Kitchen.Dto.StoreShirwalLabel;
using Corno.Web.Areas.Kitchen.Services.Interfaces;
using Corno.Web.Globals;
using Corno.Web.Globals.Enums;
using Corno.Web.Models.Packing;
using Corno.Web.Models.Plan;
using Corno.Web.Repository.Interfaces;
using Corno.Web.Services.File.Interfaces;
using Corno.Web.Services.Masters.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace Corno.Web.Areas.Kitchen.Services;

public class StoreShirwalLabelService : LabelService, IStoreShirwalLabelService
{
    #region -- Constructors --
    public StoreShirwalLabelService(IGenericRepository<Label> genericRepository, 
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
    private const string NewStatus = StatusConstants.Sorted;

    #endregion

    #region -- Protected Methods --
    protected bool ValidateDto(StoreShirwalCrudDto dto)
    {
        if (string.IsNullOrEmpty(dto.WarehouseOrderNo))
            throw new Exception("Invalid warehouse Order.");

        if (string.IsNullOrEmpty(dto.Family))
            throw new Exception("Invalid family Order.");

        return true;
    }

    #endregion

    #region  -- Public Methods --
    public async Task<List<Label>> CreateLabelsAsync(StoreShirwalCrudDto dto, Plan plan)
    {
        // 1. Validate Dto
        ValidateDto(dto);

        // 1. Create Label
        var planItemDetail = plan.PlanItemDetails.FirstOrDefault(d => d.Group == dto.Family);
        if (null == planItemDetail)
            throw new Exception($"No item found for position '{dto.Family}' in plan");
        // Validate quantities

        var planItemDetails = plan.PlanItemDetails.Where(d =>
          (d.OrderQuantity ?? 0) > (d.PrintQuantity ?? 0) &&
          d.Group == dto.Family).ToList();

        var labels = new List<Label>();

        foreach (var planDetail in planItemDetails)
        {
            //var quantity = planDetail.OrderQuantity ?? 0 - planDetail.PrintQuantity ?? 0;
            for (var index = 0; index < planItemDetail.OrderQuantity; index++)
            {
                Thread.Sleep(1);
                var barcode = $"{DateTime.Now:ddMMyyyyhhmmssffff}";

                var articleNo = (FieldConstants.ArticleNo);
                var label = new Label
                {
                    Code = barcode,
                    LabelDate = DateTime.Now,
                    LabelType = nameof(LabelType.Part),

                    LotNo = plan.LotNo,
                    SoNo = plan.SoNo,
                    ArticleNo = articleNo,
                    SoPosition = planDetail.SoPosition,
                    WarehouseOrderNo = plan.WarehouseOrderNo,
                    WarehousePosition = planDetail.WarehousePosition,
                    Position = planDetail.Position,

                    CarcassCode = planDetail.CarcassCode,
                    AssemblyCode = planDetail.AssemblyCode,
                    AssemblyQuantity = planDetail.SubAssemblyQuantity ?? 0,

                    ProductId = plan.ProductId,
                    ItemId = planDetail.ItemId,
                    OrderQuantity = planDetail.OrderQuantity ?? 0,
                    Quantity =1,
                    Barcode = barcode,

                    Operation = OperationConstants.None,
                    NextOperation = OperationConstants.None,

                    Status = NewStatus,

                    NotMapped = new NotMapped
                    {
                        ItemCode = planDetail.Description,
                        ItemName = planDetail.DrawingNo
                    }
                };
                label.LabelDetails.Add(new LabelDetail
                {
                    ScanDate = DateTime.Now,
                    Status = StatusConstants.Printed
                });

                labels.Add(label);
            }
        }

        return await Task.FromResult(labels).ConfigureAwait(false);
    }

    #endregion
}