using System;
using System.Linq;
using Corno.Web.Areas.Kitchen.Services.Interfaces;
using Corno.Web.Globals;
using Corno.Web.Reports;
using Corno.Web.Windsor;

namespace Corno.Web.Areas.Kitchen.Reports;

public partial class PlanDetailRpt : BaseReport
{
    #region -- Constructrs --
    public PlanDetailRpt()
    {
        InitializeComponent();
    }
    #endregion

    #region -- Methods --

    private void HandleNeedDataSource(object sender)
    {
        var report = (Telerik.Reporting.Processing.Report)sender;

        var warehouseOrderNo = report.Parameters[FieldConstants.WarehouseOrderNo]
            .Value.ToString();

        if (string.IsNullOrEmpty(warehouseOrderNo))
            return;

        var planService = Bootstrapper.Get<IPlanService>();
        var plan = RunAsync(() => planService.GetByWarehouseOrderNoAsync(warehouseOrderNo));
        var dataSource = plan.PlanItemDetails.Select(detail => new
        {
            plan.DueDate,
            plan.PlanDate,
            detail.WarehousePosition,
            plan.SoNo,
            plan.WarehouseOrderNo,
            detail.Position,
            OneLineItemCode = plan.System,
            detail.ItemCode,
            ItemName = detail.Description,
            detail.DrawingNo,
            detail.CarcassCode,
            detail.AssemblyCode,
            detail.Group,
            detail.OrderQuantity,
            detail.PrintQuantity,
            detail.BendQuantity,
            detail.SortQuantity,
            detail.PackQuantity
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