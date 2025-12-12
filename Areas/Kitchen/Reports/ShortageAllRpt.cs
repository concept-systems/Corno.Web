using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using Corno.Web.Areas.Kitchen.Services.Interfaces;
using Corno.Web.Dtos;
using Corno.Web.Extensions;
using Corno.Web.Globals;
using Corno.Web.Models.Plan;
using Corno.Web.Reports;
using Corno.Web.Services.Interfaces;
using Corno.Web.Windsor;
using Telerik.Reporting;

namespace Corno.Web.Areas.Kitchen.Reports;

public partial class ShortageAllRpt : BaseReport
{
    #region -- Constructors --
    public ShortageAllRpt()
    {
        // Required for telerik Reporting designer support
        InitializeComponent();

        var reportService = Bootstrapper.Get<IReportService>();

        reportService.FillTwoDateFilterParameter(ReportParameters, "UpdateFamiliesNos",
            typeof(ShortageAllRpt), FieldConstants.Family);
        FillReportType(ReportParameters[FieldConstants.ReportType]);
    }
    #endregion

    #region -- Data Members --
    private static List<Plan> _plans = new();
    #endregion

    #region -- Private Methods --
    private static void FillReportType(ReportParameter parameter)
    {
        if (parameter == null)
            return;
        parameter.AvailableValues.ValueMember = "=Fields.Id";
        parameter.AvailableValues.DisplayMember = "=Fields.Name";
        parameter.AvailableValues.DataSource = new List<MasterDto>
        {
            new() {Id = 0, Name = "Kitting Shortage Report"},
            new() {Id = 1, Name = "Sorting Shortage Report"},
            new() {Id = 2, Name = "Third Label Shortage Report"},
            new() {Id = 3, Name = "Packing Shortage Report"},
        };
    }

    private static int GetScanQuantity(PlanItemDetail planItemDetail, int reportType)
    {
        return reportType switch
        {
            0 => planItemDetail.BendQuantity.ToInt(),
            1 => planItemDetail.SortQuantity.ToInt(),
            2 => planItemDetail.SubAssemblyQuantity.ToInt(),
            3 => planItemDetail.PackQuantity.ToInt(),
            _ => 0
        };
    }
    #endregion

    #region -- Events --
    [DataObjectMethod(DataObjectMethodType.Select)]
    public IEnumerable UpdateFamiliesNos(DateTime fromDate, DateTime toDate)
    {
        var planService = Bootstrapper.Get<IPlanService>();
        _plans = RunAsync(() => planService.GetAsync<Plan>(p => DbFunctions.TruncateTime(p.DueDate) >= DbFunctions.TruncateTime(fromDate) &&
                                       DbFunctions.TruncateTime(p.DueDate) <= DbFunctions.TruncateTime(toDate), p => p)).ToList();
        var families = _plans.SelectMany(p => p.PlanItemDetails, (_, d) => new { Family = d.Group })
            .Distinct().ToList();
        return families;
    }


    private void ShortageAllRpt_NeedDataSource(object sender, EventArgs e)
    {
        var report = (Telerik.Reporting.Processing.Report)sender;

        var families = ((object[])report.Parameters[FieldConstants.Family].Value).OfType<string>().ToList();
        var reportType = report.Parameters[FieldConstants.ReportType].Value.ToString().ToInt();
        var bUpdateQuantity = report.Parameters["UpdateQuantities"].Value.ToBoolean();

        // Get date range from _plans (populated by UpdateFamiliesNos)
        // Extract min/max dates from the plans that were loaded
        var fromDate = _plans.Any() && _plans.Any(p => p.DueDate.HasValue)
            ? _plans.Where(p => p.DueDate.HasValue).Min(p => p.DueDate.Value)
            : DateTime.Now.Date;
        var toDate = _plans.Any() && _plans.Any(p => p.DueDate.HasValue)
            ? _plans.Where(p => p.DueDate.HasValue).Max(p => p.DueDate.Value)
            : DateTime.Now.Date;

        // Use stored procedure for better performance
        var planService = Bootstrapper.Get<IPlanService>();
        var familiesString = string.Join(",", families);
        
        // Use RunAsync helper method to avoid deadlocks by executing async code on thread pool thread
        var dataSource = RunAsync(() => planService.ExecuteStoredProcedureAsync<ShortageAllReportDto>(
            "GetShortageAllReport", fromDate, toDate, familiesString, reportType));

        //txtHeader.Value = report.Parameters[FieldConstants.ReportType]?.Label.ToString();

        table1.DataSource = dataSource is { Count: > 0 } ? dataSource : null;
    }

    private void ShortageAllRpt_ItemDataBound(object sender, EventArgs e)
    {
        /*if (sender is not Telerik.Reporting.Processing.Report report) return;

        var pageHeader = report.PageHeader;
        var txtHeaderP = (TextBox)ElementTreeHelper.GetChildByName(pageHeader, "txtHeader");
        txtHeaderP.Value = report.Parameters[FieldConstants.ReportType]?.Label.ToString();*/
    }

    #endregion
}

public class ShortageAllReportDto
{
    public DateTime? DueDate { get; set; }
    public string OneLineItemCode { get; set; }
    public string WarehouseOrderNo { get; set; }
    public string Family { get; set; }
    public string Position { get; set; }
    public string Description { get; set; }
    public string Color { get; set; }
    public double? OrderQuantity { get; set; }
    public double? ScanQuantity { get; set; }
}