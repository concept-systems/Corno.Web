using System;
using System.Linq;
using Corno.Web.Areas.Kitchen.Services.Interfaces;
using Corno.Web.Globals;
using Corno.Web.Models.Plan;
using Corno.Web.Reports;
using Corno.Web.Services.Masters.Interfaces;
using Corno.Web.Windsor;

namespace Corno.Web.Areas.Kitchen.Reports;

public partial class CartonItemDetailRpt : BaseReport
{
    #region -- Constructrs --
    public CartonItemDetailRpt(/*ILabelService labelService, ICartonService cartonService,
        IPlanService planService, IBaseItemService itemService*/)
    {
        InitializeComponent();

        /*_labelService = labelService;
        _cartonService = cartonService;
        _planService = planService;
        _itemService = itemService;*/

    }
    #endregion

    /*#region -- Data Members --

    private readonly ILabelService _labelService;
    private readonly ICartonService _cartonService;
    private readonly IPlanService _planService;
    private readonly IBaseItemService _itemService;

    #endregion*/

    #region -- Methods --

    private void HandleNeedDataSource(object sender)
    {
        var report = (Telerik.Reporting.Processing.Report)sender;
        var warehouseOrderNo = report.Parameters[FieldConstants.WarehouseOrderNo]
            .Value.ToString();

        if (string.IsNullOrEmpty(warehouseOrderNo))
            return;

        var planService = Bootstrapper.Get<IPlanService>();
        var itemService = Bootstrapper.Get<IBaseItemService>();
        var cartonService = Bootstrapper.Get<ICartonService>();
        var labelService = Bootstrapper.Get<ILabelService>();

        // Optimized: Load plan (needs PlanItemDetails for the report)
        var plan = RunAsync(() => planService.GetByWarehouseOrderNoAsync(warehouseOrderNo));
        
        // Optimized: Enable includes for CartonDetails since we need them for the report
        //cartonService.SetIncludes($"{nameof(Corno.Web.Models.Packing.Carton.CartonDetails)}");
        var cartons = RunAsync(() => cartonService.GetAsync(c => c.WarehouseOrderNo == warehouseOrderNo,
            c => c));
        
        // Extract barcodes for label query - filter out null/empty barcodes
        var barcodes = cartons.SelectMany(c => c.CartonDetails, (c, d) => d.Barcode)
            .Where(b => !string.IsNullOrEmpty(b))
            .Distinct()
            .ToList();
        
        // Optimized: Use projection to only select needed fields from Labels, ignore includes
        var labels = RunAsync(() => labelService
                .GetAsync(b => b.WarehouseOrderNo == warehouseOrderNo && barcodes.Contains(b.Barcode), b => new
                {
                    b.SoNo,
                    b.WarehouseOrderNo,
                    b.Position,
                    b.SerialNo,
                    b.LabelDate,
                    b.ItemId,
                    b.Barcode,
                    b.Quantity,
                    b.Status
                }, null, ignoreInclude: true));
        
        // Optimized: Create lookup dictionaries for faster in-memory joins (O(1) instead of O(n))
        var labelLookup = labels.ToDictionary(l => l.Barcode ?? string.Empty, StringComparer.OrdinalIgnoreCase);
        var planItemDetailLookup = plan?.PlanItemDetails?.ToDictionary(p => p.Position ?? string.Empty, StringComparer.OrdinalIgnoreCase);
        
        // Optimized: Use dictionary lookups instead of FirstOrDefault in loop (much faster for large datasets)
        var dataSource = cartons.SelectMany(c => c.CartonDetails, (c, d) =>
            {
                labelLookup.TryGetValue(d.Barcode ?? string.Empty, out var label);
                PlanItemDetail planItemDetail = null;
                planItemDetailLookup?.TryGetValue(d.Position ?? string.Empty, out planItemDetail);

                return new
                {
                    plan?.SoNo,
                    plan?.WarehouseOrderNo,
                    planItemDetail?.Position,
                    ScanDate = d.ModifiedDate,
                    CartonNo = c.GetCartonNoString(),
                    label?.SerialNo,
                    label?.LabelDate,
                    label?.Barcode,
                    OneLineItemCode = plan?.System,
                    ItemCode = planItemDetail?.DrawingNo,
                    ItemName = planItemDetail?.Description,
                    d.Quantity,
                    label?.Status
                };
            }).ToList();

        report.DataSource = dataSource is { Count: > 0 } ? dataSource : null;
    }
    #endregion

    #region -- Events --
    private void CartonDetailRpt_NeedDataSource(object sender, EventArgs e)
    {
        HandleNeedDataSource(sender);
    }
    #endregion
}