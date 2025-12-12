using Corno.Web.Areas.Kitchen.Services.Interfaces;
using Corno.Web.Extensions;
using Corno.Web.Globals;
using Corno.Web.Reports;
using Corno.Web.Windsor;
using System;

namespace Corno.Web.Areas.Kitchen.Reports;

public partial class ScanningRplWiseRpt : BaseReport
{
    #region -- Constructors --
    public ScanningRplWiseRpt()
    {
        // Required for telerik Reporting designer support
        InitializeComponent();
    }
    #endregion

    private void ScanningRplWiseRpt_NeedDataSource(object sender, EventArgs e)
    {
        var report = (Telerik.Reporting.Processing.Report)sender;

        var fromDate = report.Parameters[FieldConstants.FromDate].Value.ToDateTime().Date;
        var toDate = report.Parameters[FieldConstants.ToDate].Value.ToDateTime().Date;

        var cartonService = Bootstrapper.Get<ICartonService>();
        // Use RunAsync helper method to avoid deadlocks by executing async code on thread pool thread
        var dataSource = RunAsync(() => cartonService.ExecuteStoredProcedureAsync<ScanningRplWiseReportDto>(
            "GetScanningRplWiseReport", fromDate, toDate));
        
        table1.DataSource = dataSource is { Count: > 0 } ? dataSource : null;

        /*var planService = Bootstrapper.Get<IPlanService>();
        var cartonService = Bootstrapper.Get<ICartonService>();
        var userService = Bootstrapper.Get<IUserService>();

        // Get all necessary data as IQueryable
        var cartonsQuery = cartonService.Get(c =>
            DbFunctions.TruncateTime(c.PackingDate) >= fromDate &&
            DbFunctions.TruncateTime(c.PackingDate) <= toDate, c => c);

        var plansQuery = planService.GetQuery();
        var usersQuery = userService.GetQuery();

        // Compose the query using joins
        var query = from carton in cartonsQuery
                    from racking in carton.CartonRackingDetails
                    where racking.Status == StatusConstants.Packed
                    join plan in plansQuery on carton.WarehouseOrderNo equals plan.WarehouseOrderNo into planGroup
                    from plan in planGroup.DefaultIfEmpty()
                    join user in usersQuery on racking.ModifiedBy equals user.Id into userGroup
                    from user in userGroup.DefaultIfEmpty()
                    let planItemDetail = plan.PlanItemDetails.FirstOrDefault(x => x.Position == carton.CartonDetails.FirstOrDefault().Position)
                    select new
                    {
                        carton.WarehouseOrderNo,
                        carton.PackingDate,
                        Family = planItemDetail != null ? planItemDetail.Group : null,
                        CartonNo = carton.CartonNo ?? 0,
                        UserName = user != null ? user.UserName : null
                    };

        // Execute the query and remove duplicates
        var result = query
            .AsEnumerable() // switch to in-memory to use DistinctBy
            .DistinctBy(o => new { o.WarehouseOrderNo, o.CartonNo })
            .ToList();

        if (result.Count == 0) return;

        table1.DataSource = result;*/

    }

    /*private void ScanningRplWiseRpt_NeedDataSource(object sender, EventArgs e)
    {
        var report = (Telerik.Reporting.Processing.Report)sender;

        var fromDate = report.Parameters[FieldConstants.FromDate].Value.ToDateTime();
        var toDate = report.Parameters[FieldConstants.ToDate].Value.ToDateTime();

        var planService = Bootstrapper.Get<IPlanService>();
        var cartonService = Bootstrapper.Get<ICartonService>();
        var userService = Bootstrapper.Get<IUserService>();

        var cartons = cartonService.Get(c => DbFunctions.TruncateTime(c.PackingDate) >= DbFunctions.TruncateTime(fromDate) &&
                                              DbFunctions.TruncateTime(c.PackingDate) <= DbFunctions.TruncateTime(toDate),
            c => new { c.WarehouseOrderNo, c.PackingDate, c.CartonNo, UserId = c.ModifiedBy, c.CartonDetails.FirstOrDefault().Position, c.CartonRackingDetails }).ToList();

        if (cartons.Count <= 0) return;

        var warehouseOrderNos = cartons.Select(c => c.WarehouseOrderNo).Distinct().ToList();
        var plans = planService.Get(p => warehouseOrderNos.Contains(p.WarehouseOrderNo),
            p => new { p.WarehouseOrderNo, p.PlanItemDetails }).ToList();

        var users = userService.Get(null, p => p).ToList();

        table1.DataSource = cartons.SelectMany(c => c.CartonRackingDetails.Where(d => d.Status == StatusConstants.Packed),
            (c, d) =>
        {
            var plan = plans.FirstOrDefault(p => p.WarehouseOrderNo == c.WarehouseOrderNo);
            var planItemDetail = plan?.PlanItemDetails.FirstOrDefault(x => x.Position == c.Position);
            var user = users.FirstOrDefault(p => p.Id == d.ModifiedBy);
            return new
            {
                c.WarehouseOrderNo,
                c.PackingDate,
                Family = planItemDetail?.Group,
                CartonNo = c.CartonNo ?? 0,
                //CartonNo = _cartonService.GetCartonNo(c.CartonNo ?? 0),
                user?.UserName
            };
        }).DistinctBy(o => new { o.WarehouseOrderNo, o.CartonNo }).ToList();
    }*/
}

public class ScanningRplWiseReportDto
{
    public string WarehouseOrderNo { get; set; }
    public DateTime PackingDate { get; set; }
    public string Family { get; set; }
    public int CartonNo { get; set; }
    public string UserName { get; set; }
}