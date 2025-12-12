using System;
using System.Linq;
using Corno.Web.Areas.Kitchen.Services;
using Corno.Web.Areas.Kitchen.Services.Interfaces;
using Corno.Web.Extensions;
using Corno.Web.Globals;
using Corno.Web.Models.Plan;
using Corno.Web.Reports;
using Corno.Web.Services.Interfaces;
using Corno.Web.Windsor;

namespace Corno.Web.Areas.Kitchen.Reports
{
    public partial class ShortageRpt_New : BaseReport
    {
        #region -- Constructors --
        public ShortageRpt_New()
        {
            // Required for telerik Reporting designer support
            InitializeComponent();

            /*_planService = Bootstrapper.Get<IPlanService>();
            _miscMasterService = Bootstrapper.Get<IMiscMasterService>();*/
            var reportService = Bootstrapper.Get<IReportService>();

            reportService.FillOneFilterParameter(ReportParameters, "UpdateLotNos",
                typeof(ReportService), FieldConstants.DueDate, typeof(DateTime), FieldConstants.LotNo);
            reportService.FillLocationParameter(ReportParameters[FieldConstants.Location]);
        }
        #endregion

        #region -- Events --

        private void ShortageRpt_NeedDataSource(object sender, EventArgs e)
        {
            var report = (Telerik.Reporting.Processing.Report)sender;

            var lotNo = report.Parameters[FieldConstants.LotNo]
                .Value.ToString();
            var locationId = report.Parameters[FieldConstants.Location].Value.ToString().ToInt();

            var planService = Bootstrapper.Get<IPlanService>();
            var miscMasterService = Bootstrapper.Get<IMiscMasterService>();
            
            var families = RunAsync(() => planService.GetFamiliesAsync(locationId)).ToList();
            var plans = RunAsync(() => planService.GetAsync<Plan>(p => p.LotNo == lotNo && p.Status == StatusConstants.InProcess, p => p)).ToList();
            var planItemDetails = plans.SelectMany(p => p.PlanItemDetails
                .Where(d => (d.OrderQuantity ?? 0) > (d.PackQuantity ?? 0)), (plan, planItemDetail) => planItemDetail);
            planItemDetails = planItemDetails.Where(d => families.Contains(d.Group));

            /*foreach (var plan in plans.Where(plan => plan.Status != StatusConstants.Packed))
                RunAsync(() => planService.UpdatePackQuantitiesAsync(plan));*/

            var distinctWarehouseId = plans.Select(p => p.WarehouseId).Distinct();
            var warehouses = RunAsync(() => miscMasterService.GetViewModelListAsync(i =>
                distinctWarehouseId.Contains(i.Id))).ToList();

            var dataSource = from planItemDetail in planItemDetails
                             join plan in plans on planItemDetail.PlanId equals plan?.Id
                             join warehouse in warehouses on plan.WarehouseId equals warehouse?.Id into defaultWarehouse
                             from warehouse in defaultWarehouse.DefaultIfEmpty()
                             select new
                             {
                                 plan.DueDate,
                                 OneLineItemCode = plan.System,
                                 plan.SoNo,
                                 plan.WarehouseOrderNo,
                                 Family = planItemDetail.Group,
                                 BranchName = warehouse?.Name,

                                 planItemDetail.SoPosition,

                                 planItemDetail.Position,
                                 planItemDetail.ItemCode,
                                 planItemDetail.Description,
                                 planItemDetail.OrderQuantity,
                                 ScanQuantity = planItemDetail.PackQuantity,
                             };


            var dataList = dataSource.ToList();
            report.DataSource = dataList is { Count: > 0 } ? dataList : null;
        }
        #endregion
    }
}