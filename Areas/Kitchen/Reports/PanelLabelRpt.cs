using System;
using System.Collections;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using Corno.Web.Areas.Kitchen.Services.Interfaces;
using Corno.Web.Globals;
using Corno.Web.Models.Masters;
using Corno.Web.Models.Packing;
using Corno.Web.Models.Plan;
using Corno.Web.Reports;
using Corno.Web.Services.Masters.Interfaces;
using Corno.Web.Windsor;

namespace Corno.Web.Areas.Kitchen.Reports;

public partial class PanelLabelRpt : BaseReport
{
    #region -- Constructrs --
    public PanelLabelRpt()
    {
        InitializeComponent();

        /*_labelService = Bootstrapper.Get<ILabelService>();
        _planService = Bootstrapper.Get<IPlanService>();
        _itemService = Bootstrapper.Get<IBaseItemService>();*/
        var reportService = Bootstrapper.Get<IBaseReportService>();


        //var objectDataSource = new Telerik.Reporting.ObjectDataSource
        //{
        //    DataMember = "UpdateLotNos",
        //    DataSource = typeof(PanelLabelRpt)
        //};
        //objectDataSource.Parameters.Add(new Telerik.Reporting.ObjectDataSourceParameter("dueDate", typeof(DateTime), "=Parameters.DueDate"));
        //ReportParameters[FieldConstants.LotNo].AvailableValues.DataSource = objectDataSource;

        reportService.FillOneFilterParameter(ReportParameters, "UpdateLotNos",
            typeof(PanelLabelRpt), FieldConstants.DueDate, typeof(DateTime), FieldConstants.LotNo);
    }
    #endregion

    /*#region -- Data Members --

    private readonly ILabelService _labelService;
    private readonly IPlanService _planService;
    private readonly IBaseItemService _itemService;

    #endregion*/

    #region -- Methods --
    [DataObjectMethod(DataObjectMethodType.Select)]
    public IEnumerable UpdateLotNos(DateTime dueDate)
    {
        var planService = Bootstrapper.Get<IPlanService>();
        return RunAsync(() => planService.GetAsync<object>(p => DbFunctions.TruncateTime(p.DueDate) ==
                                     DbFunctions.TruncateTime(dueDate), p => new { p.LotNo }))
            .ToList();
    }

    private void HandleNeedDataSource(object sender)
    {
        var report = (Telerik.Reporting.Processing.Report)sender;
        var lotNo = report.Parameters[FieldConstants.LotNo]
            .Value.ToString();

        if (string.IsNullOrEmpty(lotNo))
            return;

        // Use stored procedure for better performance
        var planService = Bootstrapper.Get<IPlanService>();
        // Use RunAsync helper method to avoid deadlocks by executing async code on thread pool thread
        var dataSource = RunAsync(() => planService.ExecuteStoredProcedureAsync<PanelLabelReportDto>(
            "GetPanelLabelReport", lotNo));

        report.DataSource = dataSource is { Count: > 0 } ? dataSource : null;

        /*var plans = _planService.Get(p => p.LotNo == lotNo, p => 
            new {p.WarehouseOrderNo, p.System, p.PlanItemDetails});
        var distinctWarehouseOrderNos = plans.Select(p => p.WarehouseOrderNo)
            .Distinct();

        var labels = _labelService.Get(b => 
                distinctWarehouseOrderNos.Contains(b.WarehouseOrderNo), b => new
            {
                b.SoNo,
                b.WarehouseOrderNo,
                b.Position,

                b.SerialNo,
                b.ItemId,
                b.Quantity,
                b.Status
            }).AsEnumerable()
            .OrderBy(b => b.Status).ToList();

        var distinctItemIds = labels.Select(l => l.ItemId).Distinct();
        var items = _itemService.Get(m => distinctItemIds.Contains(m.Id),
            m => new { m.Id, m.Code, m.Name, m.Reserved1 });

        var dataSource = labels.Select(l =>
        {
            var item = items.FirstOrDefault(i => i.Id == l.ItemId);
            var plan = plans.FirstOrDefault(p => p.WarehouseOrderNo == l.WarehouseOrderNo);
            var planItemDetail = plan?.PlanItemDetails.FirstOrDefault(d =>
                d.Position == l.Position);
            return new
            {
                OneLineItemCode = plan?.System,
                l.SoNo,
                l.WarehouseOrderNo,
                l.Position,
                l.SerialNo,
                ItemCode = planItemDetail?.DrawingNo,
                ItemName = item?.Name,
                Color = planItemDetail?.Reserved1,
                l.Quantity,
                ScanQuantity = l.Status == StatusConstants.Packed ? l.Quantity : 0,
            };
        }).ToList();*/

        //report.DataSource = dataSource.Count <= 0 ? null : dataSource;
    }
    #endregion

    #region -- Events --
    private void PanelLabelRpt_NeedDataSource(object sender, EventArgs e)
    {
        HandleNeedDataSource(sender);
    }
    #endregion
}

public class PanelLabelReportDto
{
    public string OneLineItemCode { get; set; }
    public string SoNo { get; set; }
    public string WarehouseOrderNo { get; set; }
    public string Position { get; set; }
    public int? SerialNo { get; set; }
    public string ItemCode { get; set; }
    public string ItemName { get; set; }
    public string Color { get; set; }
    public double? Quantity { get; set; }
    public double? ScanQuantity { get; set; }
}