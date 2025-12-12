using System;
using System.Linq;
using Corno.Web.Areas.Kitchen.Services.Interfaces;
using Corno.Web.Globals;
using Corno.Web.Reports;
using Corno.Web.Services.Masters.Interfaces;
using Corno.Web.Windsor;

namespace Corno.Web.Areas.Kitchen.Reports;

public partial class CartonItemDetailRpt : BaseReport
{
    #region -- Constructrs --
    public CartonItemDetailRpt(/*ILabelService labelService, ICartonService cartonService,
        IPlanService planService, IBaseItemService itemService*/)
    {
        InitializeComponent();

        /*_labelService = labelService;
        _cartonService = cartonService;
        _planService = planService;
        _itemService = itemService;*/

    }
    #endregion

    /*#region -- Data Members --

    private readonly ILabelService _labelService;
    private readonly ICartonService _cartonService;
    private readonly IPlanService _planService;
    private readonly IBaseItemService _itemService;

    #endregion*/

    #region -- Methods --

    private void HandleNeedDataSource(object sender)
    {
        var report = (Telerik.Reporting.Processing.Report)sender;
        var warehouseOrderNo = report.Parameters[FieldConstants.WarehouseOrderNo]
            .Value.ToString();

        if (string.IsNullOrEmpty(warehouseOrderNo))
            return;

        var planService = Bootstrapper.Get<IPlanService>();
        var itemService = Bootstrapper.Get<IBaseItemService>();
        var cartonService = Bootstrapper.Get<ICartonService>();
        var labelService = Bootstrapper.Get<ILabelService>();

        var plan = RunAsync(() => planService.GetByWarehouseOrderNoAsync(warehouseOrderNo));
        var cartons = RunAsync(() => cartonService.GetAsync(c => c.WarehouseOrderNo == warehouseOrderNo,
            c => c));
        var barcodes = cartons.SelectMany(c => c.CartonDetails, (c, d) => d.Barcode).ToList();
        var labels = RunAsync(() => labelService
                .GetAsync(b => b.WarehouseOrderNo == warehouseOrderNo && barcodes.Contains(b.Barcode), b => new
                {
                    b.SoNo,
                    b.WarehouseOrderNo,
                    b.Position,
                    b.SerialNo,
                    b.LabelDate,

                    b.ItemId,
                    b.Barcode,
                    b.Quantity,
                    b.Status
                }));
        var dataSource = cartons.SelectMany(d => d.CartonDetails, (c, d) =>
            {
                var label = labels.FirstOrDefault(l =>
                    l.Barcode == d.Barcode);
                /*var item = items.FirstOrDefault(i =>
                    i.Id == label.ItemId );*/
                var planItemDetail = plan?.PlanItemDetails.FirstOrDefault(x =>
                    x.Position == d.Position);
                return new
                {
                    plan?.SoNo,
                    plan?.WarehouseOrderNo,
                    planItemDetail?.Position,
                    ScanDate = d.ModifiedDate,
                    CartonNo = c.GetCartonNoString(),
                    label?.SerialNo,
                    label?.LabelDate,
                    label?.Barcode,

                    OneLineItemCode = plan?.System,
                    ItemCode = planItemDetail?.DrawingNo,
                    ItemName = planItemDetail?.Description,
                        
                    d.Quantity,
                    label?.Status
                };
            }).ToList();

        report.DataSource = dataSource is { Count: > 0 } ? dataSource : null;
    }
    #endregion

    #region -- Events --
    private void CartonDetailRpt_NeedDataSource(object sender, EventArgs e)
    {
        HandleNeedDataSource(sender);
    }
    #endregion
}