using System;
using System.Collections;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using Corno.Web.Areas.Kitchen.Services.Interfaces;
using Corno.Web.Extensions;
using Corno.Web.Globals;
using Corno.Web.Models.Plan;
using Corno.Web.Reports;
using Corno.Web.Services.Interfaces;
using Corno.Web.Services.Masters.Interfaces;
using Corno.Web.Windsor;

namespace Corno.Web.Areas.Kitchen.Reports;

public partial class ShortageRpt : BaseReport
{
    #region -- Constructors --
    public ShortageRpt()
    {
        // Required for telerik Reporting designer support
        InitializeComponent();

        var reportService = Bootstrapper.Get<IReportService>();

        reportService.FillTwoDateFilterParameter(ReportParameters, "UpdateLotNos",
            typeof(ShortageRpt), FieldConstants.LotNo);
        reportService.FillLocationParameter(ReportParameters[FieldConstants.Location]);
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
        return plans.Select(p => new { p.LotNo }).ToList();
    }

    private void ShortageRpt_NeedDataSource(object sender, EventArgs e)
    {
        var report = (Telerik.Reporting.Processing.Report)sender;

        var lotNos = ((object[])report.Parameters[FieldConstants.LotNo].Value).OfType<string>().ToList();
        var locationId = report.Parameters[FieldConstants.Location].Value.ToString().ToInt();
        var bUpdateQuantity = report.Parameters["UpdateQuantities"].Value.ToBoolean();

        var planService = Bootstrapper.Get<IPlanService>();
        var itemService = Bootstrapper.Get<IBaseItemService>();
        var miscMasterService = Bootstrapper.Get<IMiscMasterService>();

        var plans = RunAsync(() => planService.GetAsync<Plan>(p => lotNos.Contains(p.LotNo) && (p.Status == StatusConstants.InProcess || p.Status == StatusConstants.Active),
                p => p));
        var families = RunAsync(() => planService.GetFamiliesAsync(0));
        /*if (bUpdateQuantity)
        {
            foreach (var plan in plans.Where(plan => plan.Status != StatusConstants.Packed))
                planService.UpdatePackQuantities(plan);
        }*/

        var dataSource = from plan in plans
                         from planItemDetail in plan.PlanItemDetails
                         where (planItemDetail.OrderQuantity ?? 0) > (planItemDetail.PackQuantity ?? 0)
                         join warehouse in miscMasterService.GetQuery()
                             on plan.WarehouseId equals warehouse.Id into defaultWarehouse
                         from warehouse in defaultWarehouse.DefaultIfEmpty()
                         join item in itemService.GetQuery()
                             on planItemDetail.ItemId equals item.Id into defaultItem
                         from item in defaultItem.DefaultIfEmpty()
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
                             //ScanQuantity = (planItemDetail.PackQuantity ?? 0) > 0 ? planItemDetail.PackQuantity.ToString() : string.Empty,
                             ScanQuantity = planItemDetail.PackQuantity ?? 0
                         };

         var data = dataSource.ToList();

        var tableItem = report.ItemDefinition.Items.Find("table1", true)
            .OfType<Telerik.Reporting.Table>()
            .FirstOrDefault();

        if (tableItem == null) return;

        tableItem.DataSource = data; 
    }
    #endregion
}