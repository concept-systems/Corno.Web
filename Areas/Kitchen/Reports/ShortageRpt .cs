using System;
using System.Collections;
using System.ComponentModel;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Corno.Web.Areas.Kitchen.Services.Interfaces;
using Corno.Web.Extensions;
using Corno.Web.Globals;
using Corno.Web.Reports;
using Corno.Web.Services.Interfaces;
using Corno.Web.Windsor;
using Telerik.Reporting.Processing;

namespace Corno.Web.Areas.Kitchen.Reports;

public partial class ShortageRpt : BaseReport
{
    #region -- Constructors --
    public ShortageRpt()
    {
        // Required for telerik Reporting designer support
        InitializeComponent();

        var reportService = Bootstrapper.Get<IReportService>();

        //reportService.AddIsPreviewParameter(ReportParameters);
        reportService.FillTwoDateFilterParameter(ReportParameters, "UpdateLotNos",
            typeof(ShortageRpt), FieldConstants.LotNo);
        reportService.FillLocationParameter(ReportParameters[FieldConstants.Location]);
        ReportParameters["isPreview"].Visible = true;

        // Ensure LotNo parameter is set up as a child parameter of FromDate and ToDate
        // This ensures it refreshes when dates change
        var lotNoParam = ReportParameters[FieldConstants.LotNo];
        if (lotNoParam != null)
        {
            // Set up parameter dependencies so LotNo refreshes when dates change
            var fromDateParam = ReportParameters[FieldConstants.FromDate];
            var toDateParam = ReportParameters[FieldConstants.ToDate];

            if (fromDateParam != null && toDateParam != null)
            {
                // The FillTwoDateFilterParameter already sets up the data source dependency
                // But we need to ensure the parameter value is cleared/refreshed on first load
                // This is typically handled by Telerik automatically, but we can force it
                lotNoParam.Value = null; // Clear initial value to force refresh
            }
        }
    }
    #endregion

    #region -- Events --
    [DataObjectMethod(DataObjectMethodType.Select)]
    public IEnumerable UpdateLotNos(DateTime fromDate, DateTime toDate)
    {
        var planService = Bootstrapper.Get<IPlanService>();
        var plans = RunAsync(() => planService.GetAsync(p => DbFunctions.TruncateTime(p.DueDate) >= DbFunctions.TruncateTime(fromDate) &&
                                       DbFunctions.TruncateTime(p.DueDate) <= DbFunctions.TruncateTime(toDate),
            p => new { p.LotNo }));
        return plans.Select(p => new { p.LotNo }).Distinct().ToList();
    }

    private void ShortageRpt_NeedDataSource(object sender, EventArgs e)
    {
        var report = (Telerik.Reporting.Processing.Report)sender;

        var fromDate = Convert.ToDateTime(report.Parameters[FieldConstants.FromDate].Value);
        var toDate = Convert.ToDateTime(report.Parameters[FieldConstants.ToDate].Value);
        var lotNos = ((object[])report.Parameters[FieldConstants.LotNo].Value).OfType<string>().ToList();
        var isPreviewParam = report.Parameters["isPreview"];
        var isPreview = isPreviewParam?.Value?.ToBoolean() ?? true; // Default to true (preview) if not set

        // Additional check: if isPreview is false, definitely don't update quantities (export scenario)
        // If isPreview is true but this might be the first of two renders (preview then export),
        // we could add additional logic here, but for now we trust the client-side parameter

        var planService = Bootstrapper.Get<IPlanService>();
        var miscMasterService = Bootstrapper.Get<IMiscMasterService>();

        var plans = RunAsync(() => planService.GetAsync(p => lotNos.Contains(p.LotNo) &&
                                                             DbFunctions.TruncateTime(p.DueDate) >= DbFunctions.TruncateTime(fromDate) &&
                                                             DbFunctions.TruncateTime(p.DueDate) <= DbFunctions.TruncateTime(toDate),
                p => p));

        // Only update quantities when viewing the report, not during export
        // isPreview will be false during export, so quantities won't be updated
        if (isPreview)
        {
            foreach (var plan in plans)
                planService.UpdateQuantitiesAsync(plan).ConfigureAwait(false);
        }

        var dataSource = from plan in plans
                         from planItemDetail in plan.PlanItemDetails
                         where (planItemDetail.OrderQuantity ?? 0) > (planItemDetail.PackQuantity ?? 0)
                         join warehouse in miscMasterService.GetQuery()
                             on plan.WarehouseId equals warehouse.Id into defaultWarehouse
                         from warehouse in defaultWarehouse.DefaultIfEmpty()
                         select new
                         {
                             plan.DueDate,
                             OneLineItemCode = plan.System,
                             plan.SoNo,
                             plan.WarehouseOrderNo,
                             Family = planItemDetail.Group,
                             BranchName = warehouse.Name,

                             planItemDetail.SoPosition,
                             planItemDetail.Position,
                             planItemDetail.ItemCode,
                             planItemDetail.Description,

                             Color = planItemDetail.Reserved1,
                             OrderQuantity = planItemDetail.OrderQuantity ?? 0,
                             planItemDetail.DrawingNo,
                             ScanQuantity = planItemDetail.PackQuantity ?? 0
                         };

        var data = dataSource.ToList();

        var tableItem = report.ItemDefinition.Items.Find("table1", true)
            .OfType<Telerik.Reporting.Table>()
            .FirstOrDefault();

        tableItem.DataSource = data;
    }
    #endregion
}