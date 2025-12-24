using System;
using Corno.Web.Areas.Kitchen.Services.Interfaces;
using Corno.Web.Globals;
using Corno.Web.Reports;
using Corno.Web.Services.Interfaces;
using Corno.Web.Windsor;

namespace Corno.Web.Areas.Kitchen.Reports;

public partial class WipRpt : BaseReport
{
    #region -- Constructors --
    public WipRpt()
    {
        // Required for telerik Reporting designer support
        InitializeComponent();
    }
    #endregion

    #region -- Events --
    private void WipRpt_NeedDataSource(object sender, EventArgs e)
    {
        var report = (Telerik.Reporting.Processing.Report)sender;

        var fromDate = Convert.ToDateTime(report.Parameters[FieldConstants.FromDate].Value);
        var toDate = Convert.ToDateTime(report.Parameters[FieldConstants.ToDate].Value);

        // Use stored procedure for better performance, especially for large date ranges
        // Use RunAsync helper method to avoid deadlocks by executing async code on thread pool thread
        var planService = Bootstrapper.Get<IPlanService>();
        var dataSource = RunAsync(() => planService.ExecuteStoredProcedureAsync<WipReportDto>(
            "GetWipReport", fromDate, toDate));

        table1.DataSource = dataSource is { Count: > 0 } ? dataSource : null;
    }

    //private void WipRpt_NeedDataSource(object sender, EventArgs e)
    //{
    //    var report = (Telerik.Reporting.Processing.Report)sender;

    //    var fromDate = Convert.ToDateTime(report.Parameters[FieldConstants.FromDate].Value);
    //    var toDate = Convert.ToDateTime(report.Parameters[FieldConstants.ToDate].Value);

    //    var plans = _planService.Get(p =>
    //            DbFunctions.TruncateTime(p.DueDate) >= DbFunctions.TruncateTime(fromDate) &&
    //            DbFunctions.TruncateTime(p.DueDate) <= DbFunctions.TruncateTime(toDate),
    //        p => new { p.DueDate, p.System, p.WarehouseOrderNo, p.PlanItemDetails });
    //    var warehouseOrderNos = plans.Select(p => p.WarehouseOrderNo).Distinct();
    //    var cartons = _cartonService.Get(c => warehouseOrderNos.Contains(c.WarehouseOrderNo),
    //                c => c).OrderBy(c => c.ModifiedDate);
    //    var dataSource = plans.Select(p =>
    //    {
    //        var warehouseCartons = cartons.Where(c => c.WarehouseOrderNo == p.WarehouseOrderNo).ToList();
    //        var cartonInfo = GetCartonInfo(warehouseCartons, p.PlanItemDetails);
    //        return new
    //        {
    //            p.DueDate,
    //            OneLineItemCode = p.System,
    //            p.WarehouseOrderNo,
    //            BranchName = string.Empty,

    //            cartonInfo.CartonCount,
    //            cartonInfo.RackInDate,
    //            cartonInfo.RackOutDate,
    //            cartonInfo.DispatchDate,

    //            cartonInfo.LoadNo,
    //            cartonInfo.PrintStatus,
    //            cartonInfo.RackInStatus,

    //            cartonInfo.LastDate,
    //            cartonInfo.Families
    //        };
    //    });


    //    table1.DataSource = dataSource;
    //}

    /*public override void OnActionExecuting(object sender, InteractiveActionCancelEventArgs args)
    {
        var navigateAction = args.Action as Telerik.Reporting.Processing.NavigateToReportAction;
        if (navigateAction?.ReportSource == null) return;

        var warehouseOrderNo = navigateAction.ReportSource.Parameters?[FieldConstants.WarehouseOrderNo]?.Value.ToString();
        var plan = _planService.GetByWarehouseOrderNoAsync(warehouseOrderNo).GetAwaiter().GetResult();
        if (null == plan) return;

        plan.Status = StatusConstants.Printed;
        plan.PlanItemDetails.ForEach(b => b.Status = StatusConstants.Printed);

        _planService.UpdateAndSaveAsync(plan).GetAwaiter().GetResult();
    }*/

    #endregion
}

public class WipReportDto
{
    public DateTime? DueDate { get; set; }
    public string OneLineItemCode { get; set; }
    public string WarehouseOrderNo { get; set; }
    public string BranchName { get; set; }
    public int CartonCount { get; set; }
    public DateTime? RackInDate { get; set; }
    public DateTime? RackOutDate { get; set; }
    public DateTime? DispatchDate { get; set; }
    public string LoadNo { get; set; }
    public string PrintStatus { get; set; }
    public string RackInStatus { get; set; }
    public string LastDate { get; set; }
    public string Families { get; set; }
}
