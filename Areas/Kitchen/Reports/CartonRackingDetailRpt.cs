using Corno.Web.Areas.Kitchen.Services.Interfaces;
using Corno.Web.Globals;
using Corno.Web.Reports;
using Corno.Web.Windsor;
using System;
using System.Linq;

namespace Corno.Web.Areas.Kitchen.Reports;

public partial class CartonRackingDetailRpt : BaseReport
{
    #region -- Constructrs --
    public CartonRackingDetailRpt(/*ICartonService cartonService, IPlanService planService*/)
    {
        InitializeComponent();

        /*_cartonService = cartonService;
        _planService = planService;*/

    }
    #endregion

    /*#region -- Data Members --

    private readonly ICartonService _cartonService;
    private readonly IPlanService _planService;

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
        var cartonService = Bootstrapper.Get<ICartonService>();

        var plan = RunAsync(() => planService.GetByWarehouseOrderNoAsync(warehouseOrderNo));
        var cartons = RunAsync(() => cartonService
            .GetAsync(c => c.WarehouseOrderNo == warehouseOrderNo,
                c => new { c.PackingDate, c.CartonNo, c.CartonRackingDetails }))
            .ToList(); // Materialize the query here

        var dataSource = cartons
            .SelectMany(c => c.CartonRackingDetails, (c, d) => new
            {
                OneLineItemCode = plan.System,
                plan.SoNo,
                plan.WarehouseOrderNo,
                c.PackingDate,
                CartonNo = c.CartonNo?.ToString(),
                //CartonNo = _cartonService.GetCartonNo(c.CartonNo), // Now valid after ToList
                d.ScanDate,
                d.PalletNo,
                d.RackNo,
                d.Status
            }).ToList();

        report.DataSource = dataSource is { Count: > 0 } ? dataSource : null;
    }
    #endregion

    #region -- Events --
    private void CartonRackingDetailRpt_NeedDataSource(object sender, EventArgs e)
    {
        HandleNeedDataSource(sender);
    }
    #endregion
}