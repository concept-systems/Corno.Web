using System;
using System.Data.Entity;
using System.Linq;
using Corno.Web.Areas.Kitchen.Services.Interfaces;
using Corno.Web.Extensions;
using Corno.Web.Globals;
using Corno.Web.Models.Packing;
using Corno.Web.Reports;
using Corno.Web.Services.Masters.Interfaces;
using Corno.Web.Windsor;

namespace Corno.Web.Areas.Kitchen.Reports.Racking;

public partial class CurrentRackInStatusRpt : BaseReport
{
    #region -- Constructors --
    public CurrentRackInStatusRpt()
    {
        InitializeComponent();
    }
    #endregion

    #region -- Events --
    private void CurrentRackInStatusRpt_NeedDataSource(object sender, EventArgs e)
    {
        var report = (Telerik.Reporting.Processing.Report)sender;

        var fromDate = report.Parameters[FieldConstants.FromDate]?.Value?.ToDateTime();
        var toDate = report.Parameters[FieldConstants.ToDate]?.Value?.ToDateTime();

        var cartonService = Bootstrapper.Get<ICartonService>();
        var itemService = Bootstrapper.Get<IBaseItemService>();

        var cartons = RunAsync(() => cartonService.GetAsync<Carton>(c =>
                c.Status == StatusConstants.RackIn && c.CartonRackingDetails.Any(d =>
                    DbFunctions.TruncateTime(d.ScanDate) >= DbFunctions.TruncateTime(fromDate) &&
                    DbFunctions.TruncateTime(d.ScanDate) <= DbFunctions.TruncateTime(toDate)), c => c))
            .ToList();
        var distinctItemIds = cartons.SelectMany(c =>
            c.CartonDetails, (_, d) => d.ItemId).Distinct();
        var items = RunAsync(() => itemService.GetViewModelListAsync(m => 
            distinctItemIds.Contains(m.Id)));

        var datSource = cartons.SelectMany(c => 
                c.CartonDetails.GroupBy(d => d.Position),
            (p, d) =>
            {
                var cartonRackingDetail = p.CartonRackingDetails.Find(x => x.Status == StatusConstants.RackIn);
                var cartonDetail = d.FirstOrDefault();
                var item = items.FirstOrDefault(x => x.Id == cartonDetail.ItemId);
                return new
                {
                    p.WarehouseOrderNo,
                    cartonDetail?.WarehousePosition,
                    ItemCode = item?.Code,
                    Description = item?.Name,
                    Quantity = d.Sum(x => x.Quantity),
                    CartonNo = p.CartonNo?.ToString(),
                    //CartonNo = _cartonService.GetCartonNo(p.CartonNo),
                    cartonRackingDetail?.RackNo,
                    cartonRackingDetail?.PalletNo
                };
            });

        if (!datSource.Any())
            return;

        report.DataSource = datSource;
    }
    #endregion
}