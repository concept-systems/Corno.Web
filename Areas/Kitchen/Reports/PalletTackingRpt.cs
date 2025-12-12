using Corno.Web.Areas.Kitchen.Services.Interfaces;
using Corno.Web.Globals;
using Corno.Web.Reports;
using Corno.Web.Services.Interfaces;
using Corno.Web.Windsor;
using System.Linq;

namespace Corno.Web.Areas.Kitchen.Reports;

public partial class PalletTrackingRpt : BaseReport
{
    #region -- Constructors --
    public PalletTrackingRpt(/*IPlanService planService,
        ICartonService cartonService, IMiscMasterService miscMasterService*/)
    {
        InitializeComponent();

        /*_planService = planService;
        _cartonService = cartonService;
        _miscMasterService = miscMasterService;*/
    }
    #endregion

    /*#region -- Data Members --

    private readonly IPlanService _planService;
    private readonly ICartonService _cartonService;
    private readonly IMiscMasterService _miscMasterService;
    #endregion*/

    #region -- Events --
    private void PalletTrackingRpt_NeedDataSource(object sender, System.EventArgs e)
    {
        var report = (Telerik.Reporting.Processing.Report)sender;
        var warehouseOrderNo = report.Parameters[FieldConstants.WarehouseOrderNo].Value.ToString();

        var planService = Bootstrapper.Get<IPlanService>();
        var miscMasterService = Bootstrapper.Get<IMiscMasterService>();
        var cartonService = Bootstrapper.Get<ICartonService>();

        var plan = RunAsync(() => planService.FirstOrDefaultAsync(p => p.WarehouseOrderNo == warehouseOrderNo,
            p => new { p.Id, p.DueDate, p.WarehouseOrderNo, p.WarehouseId, OneLineItemCode = p.System }));
        var cartons = RunAsync(() => cartonService.GetAsync(c => c.WarehouseOrderNo == warehouseOrderNo, c =>
                new
                {
                    c.Id, c.WarehouseOrderNo, c.CartonNo, c.Status, NetWeight = c.CartonDetails.Sum(d => d.NetWeight),
                    c.CartonRackingDetails
                }))
            .ToList();
        var warehouse = RunAsync(() => miscMasterService.GetViewModelAsync(plan?.WarehouseId ?? 0));
        var dataSource = cartons.Select(carton => new 
            {
                plan?.OneLineItemCode,
                plan?.WarehouseOrderNo,
                BranchName = warehouse?.Name,
            //CartonNo = _cartonService.GetCartonNo(carton.CartonNo),
            CartonNo = carton.CartonNo?.ToString(),
            PalletNo = carton.Status != StatusConstants.Packed ? 
                    carton.CartonRackingDetails.LastOrDefault(d => d.Status == StatusConstants.PalletIn)?.PalletNo : string.Empty,
                RackNo = carton.Status != StatusConstants.Packed ? 
                    carton.CartonRackingDetails.LastOrDefault(d => d.Status == StatusConstants.RackIn)?.RackNo : string.Empty,
                carton.Status,
                carton.NetWeight
            });

        var dataList = dataSource.ToList();
        report.DataSource = dataList is { Count: > 0 } ? dataList : null;
    }

    /*private void PalletTrackingRpt_NeedDataSource(object sender, System.EventArgs e)
    {
        var report = (Telerik.Reporting.Processing.Report)sender;

        var warehouseOrderNo = report.Parameters[FieldConstants.WarehouseOrderNo].Value.ToString();

        var dataSource = from plan in _planService.Get(p =>
                    p.WarehouseOrderNo == warehouseOrderNo,
                p => new { p.Id, p.DueDate, p.WarehouseOrderNo, p.WarehouseId, OneLineItemCode = p.System })
            join carton in _cartonService.Get(null,
                    c => new
                    {
                        c.Id,
                        c.WarehouseOrderNo,
                        c.CartonNo,
                        c.Status,
                        NetWeight = c.CartonDetails.Sum(d => d.NetWeight),
                        c.CartonRackingDetails
                    })
                on plan.WarehouseOrderNo equals carton.WarehouseOrderNo into defaultCarton
            from carton in defaultCarton.DefaultIfEmpty()
            join warehouse in _miscMasterService.Get(null, w =>
                new { w.Id, w.Name }) on plan?.WarehouseId equals warehouse.Id into defaultWarehouse
            from warehouse in defaultWarehouse.DefaultIfEmpty()
            select new
            {
                plan?.OneLineItemCode,
                plan?.WarehouseOrderNo,
                BranchName = warehouse?.Name,
                CartonNo = _cartonService.GetCartonNo(carton.CartonNo),

                carton?.CartonRackingDetails
                    .LastOrDefault(d =>
                        d.Status == StatusConstants.PalletIn)?.PalletNo,
                carton?.CartonRackingDetails
                    .LastOrDefault(d =>
                        d.Status == StatusConstants.RackIn)?.RackNo,
                carton?.Status,
                carton.NetWeight
            };

        report.DataSource = dataSource;
    }*/
    #endregion
}