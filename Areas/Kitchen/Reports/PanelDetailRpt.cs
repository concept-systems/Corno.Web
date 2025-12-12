using System;
using System.Collections;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using Corno.Web.Areas.Kitchen.Services.Interfaces;
using Corno.Web.Globals;
using Corno.Web.Reports;
using Corno.Web.Windsor;

namespace Corno.Web.Areas.Kitchen.Reports;

public partial class PanelDetailRpt : BaseReport
{
    #region -- Constructrs --
    public PanelDetailRpt()
    {
        InitializeComponent();

        var reportService = Bootstrapper.Get<IBaseReportService>();
        reportService.FillTwoDateFilterParameter(ReportParameters, "UpdateLotNos",
            typeof(PanelDetailRpt), FieldConstants.LotNo);
        reportService.FillOneFilterParameter(ReportParameters, "UpdateFamilies",
            typeof(PanelDetailRpt), FieldConstants.LotNo, typeof(object[]), FieldConstants.Family);
    }
    #endregion

    #region -- Methods --
    [DataObjectMethod(DataObjectMethodType.Select)]
    public IEnumerable UpdateLotNos(DateTime fromDate, DateTime toDate)
    {
        var planService = Bootstrapper.Get<IPlanService>();
        var plans = planService.GetQuery()!
            .Where(p => DbFunctions.TruncateTime(p.DueDate) >= DbFunctions.TruncateTime(fromDate) &&
                        DbFunctions.TruncateTime(p.DueDate) <= DbFunctions.TruncateTime(toDate))
            .Select(p => new { p.LotNo })
            .Distinct();
        return plans.ToList();
    }

    [DataObjectMethod(DataObjectMethodType.Select)]
    public IEnumerable UpdateWarehouseOrderNos(string lotNo)
    {
        var planService = Bootstrapper.Get<IPlanService>();
        var plans = planService.GetQuery()!
            .Where(p => p.LotNo == lotNo)
            //.Select(p => new { p.WarehouseOrderNo })
            .Distinct();
        return plans.ToList();
    }

    [DataObjectMethod(DataObjectMethodType.Select)]
    public IEnumerable UpdateFamilies(object[] lotNo)
    {
        var planService = Bootstrapper.Get<IPlanService>();
        var lotNos = lotNo.OfType<string>().ToList();
        var plans = planService.GetQuery()!
            .Where(p => lotNos.Contains(p.LotNo))
            .SelectMany(d => d.PlanItemDetails,
                (_, d) => new
                {
                    Family = d.Group,
                }).Distinct();
        return plans.ToList();
    }

    private void HandleNeedDataSource(object sender)
    {
        var report = (Telerik.Reporting.Processing.Report)sender;
        var lotNos = ((object[])report.Parameters[FieldConstants.LotNo].Value).OfType<string>().ToList();
        var families = ((object[])report.Parameters[FieldConstants.Family].Value).OfType<string>().ToList();

        // Convert lists to comma-separated strings for stored procedure
        var lotNosString = string.Join(",", lotNos);
        var familiesString = string.Join(",", families);

        // Use stored procedure for better performance, especially for large date ranges
        var planService = Bootstrapper.Get<IPlanService>();
        // Use RunAsync helper method to avoid deadlocks by executing async code on thread pool thread
        var dataSource = RunAsync(() => planService.ExecuteStoredProcedureAsync<PanelDetailReportDto>(
            "GetPanelDetailReport", lotNosString, familiesString));

        report.DataSource = dataSource is { Count: > 0 } ? dataSource : null;
    }

    #endregion

    #region -- Events --
    private void PanelDetailRpt_NeedDataSource(object sender, EventArgs e)
    {
        HandleNeedDataSource(sender);
    }
    #endregion
}

public class PanelDetailReportDto
{
    public string OneLineItemCode { get; set; }
    public string SoNo { get; set; }
    public string WarehouseOrderNo { get; set; }
    public string Family { get; set; }
    public string Position { get; set; }
    public string ItemCode { get; set; }
    public string ItemName { get; set; }
    public double? OrderQuantity { get; set; }
    public string Color { get; set; }
    public double? KitQuantity { get; set; }
    public DateTime? KitDate { get; set; }
    public double? SortQuantity { get; set; }
    public DateTime? SortDate { get; set; }
    public double? PackQuantity { get; set; }
    public DateTime? PackDate { get; set; }
}