using Corno.Web.Areas.Kitchen.Services.Interfaces;
using Corno.Web.Globals;
using Corno.Web.Models.Packing;
using Corno.Web.Reports;
using Corno.Web.Services.Interfaces;
using Corno.Web.Windsor;
using System;
using System.Data.Entity;
using System.Linq;

namespace Corno.Web.Areas.Kitchen.Reports;

public partial class DeliveryLoadingRpt : BaseReport
{
    #region -- Constructors --
    public DeliveryLoadingRpt(/*IPlanService planService, ICartonService cartonService,
        IMiscMasterService miscMasterService, IReportService reportService*/)
    {
        // Required for telerik Reporting designer support
        InitializeComponent();

        /*_planService = planService;
        _cartonService = cartonService;
        _miscMasterService = miscMasterService;*/


        var reportService = Bootstrapper.Get<IReportService>();
        reportService.FillLocationParameter(ReportParameters[FieldConstants.Location]);
    }
    #endregion

    /*#region -- Data Members --
    private readonly IPlanService _planService;
    private readonly ICartonService _cartonService;
    private readonly IMiscMasterService _miscMasterService;

    #endregion*/

    #region -- Events --
    private void DeliveryLoadingRpt_NeedDataSource(object sender, EventArgs e)
    {
        var report = (Telerik.Reporting.Processing.Report)sender;

        var fromDate = Convert.ToDateTime(report.Parameters[FieldConstants.FromDate].Value);
        var toDate = Convert.ToDateTime(report.Parameters[FieldConstants.ToDate].Value);

        var planService = Bootstrapper.Get<IPlanService>();
        var miscMasterService = Bootstrapper.Get<IMiscMasterService>();
        var cartonService = Bootstrapper.Get<ICartonService>();

        var cartons = RunAsync(() => cartonService.GetAsync<Carton>(c => c.CartonRackingDetails.Any(d =>
                c.Status == StatusConstants.Loaded && DbFunctions.TruncateTime(d.ScanDate) >= DbFunctions.TruncateTime(fromDate) &&
                DbFunctions.TruncateTime(d.ScanDate) <= DbFunctions.TruncateTime(toDate)), c => c))
            .ToList();

        var warehouseOrderNos = cartons.Select(c => c.WarehouseOrderNo).Distinct();
        var plans = RunAsync(() => planService.GetAsync(p => warehouseOrderNos.Contains(p.WarehouseOrderNo), p => new
            { p.WarehouseOrderNo, p.SoNo, p.WarehouseId, OneLineItemCode = p.System })).ToList();
        var warehouseIds = plans.Select(p => p.WarehouseId).Distinct();
        var warehouses = RunAsync(() => miscMasterService.GetViewModelListAsync(w => warehouseIds.Contains(w.Id))).ToList();

        var dataSource = cartons.SelectMany(c => c.CartonRackingDetails, (c, d) =>
        {
            var plan = plans.FirstOrDefault(p => p.WarehouseOrderNo == c?.WarehouseOrderNo);
            var warehouse = warehouses.FirstOrDefault(w => w.Id == plan?.WarehouseId);
            return new
            {
                plan?.SoNo,
                c.WarehouseOrderNo,
                plan?.OneLineItemCode,
                BranchName = warehouse?.Name,
                CartonNo = c.CartonNo?.ToString(),
                //CartonNo = _cartonService.GetCartonNo(c.CartonNo),
                c.CartonBarcode,
                c.LoadNo,
                LoadingDate = d?.ScanDate
            };
        });

        var enumerable = dataSource.ToList();
        if (!enumerable.ToList().Any())
            return;

        report.DataSource = enumerable;
    }

    /*private void DeliveryLoadingRpt_NeedDataSource(object sender, EventArgs e)
    {
        var report = (Telerik.Reporting.Processing.Report)sender;

        var fromDate = Convert.ToDateTime(report.Parameters[FieldConstants.FromDate].Value);
        var toDate = Convert.ToDateTime(report.Parameters[FieldConstants.ToDate].Value);
        var locationId = report.Parameters[FieldConstants.Location].Value.ToString().ToInt();

        //var families = _planService.GetFamilies(locationId);

        var dataSource = from cartonRackingDetail in _cartonRackingDetailService
                .Get(d => d.Status == StatusConstants.Loaded &&
                          DbFunctions.TruncateTime(d.ScanDate) >= DbFunctions.TruncateTime(fromDate) &&
                          DbFunctions.TruncateTime(d.ScanDate) <= DbFunctions.TruncateTime(toDate),
                    d => new { d.CartonId, d.ScanDate })
            join carton in _cartonService.Get(null,
                    c => new { c.Id, c.WarehouseOrderNo, c.CartonNo, c.CartonBarcode, c.LoadNo })
                on cartonRackingDetail?.CartonId equals carton?.Id
            join plan in _planService.Get(null, p =>
                    new { p.WarehouseOrderNo, p.SoNo, p.WarehouseId, OneLineItemCode = p.System })
                on carton?.WarehouseOrderNo equals plan?.WarehouseOrderNo
            join warehouse in _miscMasterService.Get(null, m => new { m.Id, m.Name })
                on plan?.WarehouseId equals warehouse?.Id
            select new
            {
                plan.SoNo,
                carton.WarehouseOrderNo,
                plan.OneLineItemCode,
                BranchName = warehouse.Name,
                CartonNo = _cartonService.GetCartonNo(carton.CartonNo),
                carton.CartonBarcode,
                carton.LoadNo,
                LoadingDate = cartonRackingDetail?.ScanDate
            };

        var dataList = dataSource.ToList();
        report.DataSource = dataList is { Count: > 0 } ? dataList : null;
    }*/
    #endregion
}