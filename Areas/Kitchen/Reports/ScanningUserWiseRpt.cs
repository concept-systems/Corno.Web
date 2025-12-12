using System;
using System.Data.Entity;
using System.Linq;
using Corno.Web.Areas.Kitchen.Services;
using Corno.Web.Areas.Kitchen.Services.Interfaces;
using Corno.Web.Globals;
using Corno.Web.Reports;
using Corno.Web.Windsor;

namespace Corno.Web.Areas.Kitchen.Reports;

public partial class ScanningUserWiseRpt : BaseReport
{
    #region -- Constructors --
    public ScanningUserWiseRpt(/*ICartonService cartonService, IPlanService planService, IReportService reportService*/)
    {
        // Required for telerik Reporting designer support
        InitializeComponent();

        /*_cartonService = cartonService;
        _planService = planService;*/


        var reportService = Bootstrapper.Get<IReportService>();
        reportService.FillUsersParameter(ReportParameters, "UpdateUsers", typeof(ReportService));
    }
    #endregion

    //#region -- Data Members --

    //private readonly ICartonService _cartonService;
    //private readonly IPlanService _planService;

    //#endregion

    #region -- Events --
    private void ScanningUserWiseRpt_NeedDataSource(object sender, EventArgs e)
    {
        var report = (Telerik.Reporting.Processing.Report)sender;

        var fromDate = Convert.ToDateTime(report.Parameters[FieldConstants.FromDate].Value);
        var toDate = Convert.ToDateTime(report.Parameters[FieldConstants.ToDate].Value);
        var userName = report.Parameters[FieldConstants.User].Label;
        var userId = report.Parameters[FieldConstants.User].Value.ToString();

        // Use stored procedure for better performance
        var cartonService = Bootstrapper.Get<ICartonService>();
        // Use RunAsync helper method to avoid deadlocks by executing async code on thread pool thread
        var dataSource = RunAsync(() => cartonService.ExecuteStoredProcedureAsync<ScanningUserWiseReportDto>(
            "GetScanningUserWiseReport", fromDate, toDate, userId, userName));

        report.DataSource = dataSource is { Count: > 0 } ? dataSource : null;
    }
    #endregion
}

public class ScanningUserWiseReportDto
{
    public string UserName { get; set; }
    public DateTime? PackingDate { get; set; }
    public string Family { get; set; }
    public string WarehouseOrderNo { get; set; }
    public string WarehousePosition { get; set; }
    public string ItemCode { get; set; }
    public string ItemName { get; set; }
    public string CartonNo { get; set; }
}