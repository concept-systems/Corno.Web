using Corno.Web.Areas.Kitchen.Services.Interfaces;
using Corno.Web.Globals;
using Corno.Web.Reports;
using Corno.Web.Windsor;
using System;
using System.Linq;

namespace Corno.Web.Areas.Kitchen.Reports;

public partial class CartonSummaryRpt : BaseReport
{
    #region -- Constructrs --
    public CartonSummaryRpt(/*ICartonService cartonService, IPlanService planService*/)
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
        var warehouseOrderNo = report.Parameters[FieldConstants.WarehouseOrderNo].Value.ToString();

        if (string.IsNullOrEmpty(warehouseOrderNo))
            return;

        var planService = Bootstrapper.Get<IPlanService>();
        var cartonService = Bootstrapper.Get<ICartonService>();

        var plan = RunAsync(() => planService.GetByWarehouseOrderNoAsync(warehouseOrderNo));

        // Step 1: Fetch raw data from DB
        var cartons = RunAsync(() => cartonService.GetAsync(c => c.WarehouseOrderNo == warehouseOrderNo,
            c => new { c.CartonNo, c.ModifiedDate, c.CartonDetails })).ToList();

        // Step 2: Apply custom logic in memory
        var dataSource = cartons.Select(c => new
        {
            plan.SoNo,
            plan.WarehouseOrderNo,
            OneLineItemCode = plan.System,
            CartonNo = c.CartonNo?.ToString(),
            //CartonNo = _cartonService.GetCartonNo(c.CartonNo)?.ToString(), // Now safe
            ScanDate = c.ModifiedDate,
            ItemCount = c.CartonDetails.Count
        }).ToList();

        report.DataSource = dataSource is { Count: > 0 } ? dataSource : null;
    }
    #endregion

    #region -- Events --
    private void CartonSummaryRpt_NeedDataSource(object sender, EventArgs e)
    {
        HandleNeedDataSource(sender);
    }
    #endregion
}