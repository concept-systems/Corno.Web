using System;
using System.Data.Entity;
using System.Linq;
using Corno.Web.Areas.Kitchen.Services.Interfaces;
using Corno.Web.Extensions;
using Corno.Web.Globals;
using Corno.Web.Reports;
using Corno.Web.Windsor;

namespace Corno.Web.Areas.Kitchen.Reports;

public partial class PlanSummaryRpt : BaseReport
{
    #region -- Constructrs --
    public PlanSummaryRpt()
    {
        InitializeComponent();
    }
    #endregion

    #region -- Methods --

    private void HandleNeedDataSource(object sender)
    {
        var report = (Telerik.Reporting.Processing.Report)sender;

        var fromDate = report.Parameters[FieldConstants.FromDate].Value.ToDateTime();
        var toDate = report.Parameters[FieldConstants.ToDate].Value.ToDateTime();

        var planService = Bootstrapper.Get<IPlanService>();
        var data = RunAsync(() => planService.GetAsync(
                p => DbFunctions.TruncateTime(p.DueDate) >= DbFunctions.TruncateTime(fromDate) &&
                     DbFunctions.TruncateTime(p.DueDate) <= DbFunctions.TruncateTime(toDate),
                p => new
                {
                    p.PlanDate,
                    p.DueDate,
                    p.SoNo,
                    p.WarehouseOrderNo,
                    OneLineItemCode = p.System,
                }));

        table2.DataSource = data?.ToList();
    }
    #endregion

    #region -- Events --
    private void PanelDetailRpt_NeedDataSource(object sender, EventArgs e)
    {
        HandleNeedDataSource(sender);
    }
    #endregion
}