using System;
using System.Linq;
using Corno.Web.Areas.Kitchen.Services.Interfaces;
using Corno.Web.Globals;
using Corno.Web.Reports;
using Corno.Web.Services.Interfaces;
using Corno.Web.Windsor;

namespace Corno.Web.Areas.Kitchen.Reports;

public partial class ScanningDateWiseRpt : BaseReport
{
    #region -- Constructors --
    public ScanningDateWiseRpt(/*ICartonService cartonService, IPlanService planService,
        IMiscMasterService miscMasterService*/)
    {
        // Required for telerik Reporting designer support
        InitializeComponent();

        /*_cartonService = cartonService;
        _planService = planService;
        _miscMasterService = miscMasterService;*/

    }
    #endregion

    /*#region -- Data Members --

    private readonly ICartonService _cartonService;
    private readonly IPlanService _planService;
    private readonly IMiscMasterService _miscMasterService;
    #endregion*/

    private void ScanningDateWiseRpt_NeedDataSource(object sender, EventArgs e)
    {
        var report = (Telerik.Reporting.Processing.Report)sender;
        var warehouseOrderNo = report.Parameters[FieldConstants.WarehouseOrderNo].Value.ToString();

        if (string.IsNullOrEmpty(warehouseOrderNo))
            return;

        // Use stored procedure for better performance
        var cartonService = Bootstrapper.Get<ICartonService>();
        // Use RunAsync helper method to avoid deadlocks by executing async code on thread pool thread
        var dataSource = RunAsync(() => cartonService.ExecuteStoredProcedureAsync<ScanningDateWiseReportDto>(
            "GetScanningDateWiseReport", warehouseOrderNo));

        report.DataSource = dataSource is { Count: > 0 } ? dataSource : null;
    }
}

public class ScanningDateWiseReportDto
{
    public DateTime? PackingDate { get; set; }
    public string WarehouseOrderNo { get; set; }
    public string WarehousePosition { get; set; }
    public string WarehouseCode { get; set; }
    public int? CartonNo { get; set; }
    public string CartonBarcode { get; set; }
    public string ItemCode { get; set; }
    public string ItemName { get; set; }
    public double? OrderQuantity { get; set; }
    public double? ScanQuantity { get; set; }
}