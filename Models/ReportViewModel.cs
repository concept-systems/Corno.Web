using System;

namespace Corno.Web.Models;

public class ReportViewModel
{
    public int SerialNo { get; set; }
    public DateTime? DueDate { get; set; }
    public string ProductionOrderNo { get; set; }
    public string ChildProductionOrdeNo { get; set; }
    public string PackingTypeName { get; set; }
    public string ProductCode { get; set; }
    public string ProductName { get; set; }
    public string ItemCode { get; set; }
    public string ItemName { get; set; }
    public string CartonNo { get; set; }
    public double? NoOfCartons { get; set; }     

    public double? OrderQuantity { get; set; }
    public double? TotalQuantity { get; set; }
    public double? ProcessedQuantity { get; set; }
    public double? PendingQuantity { get; set; }
    public double? RejectedQuantity { get; set; }
    public double? DrillQuantity { get; set; }
    public double? EdgeBandQuantity { get; set; }
    public double? RoutingQuantity { get; set; }
    public double? SubAssemblyQuantity { get; set; }
    public double? PackQuantity { get; set; }
    public double? TotalShortfallQuantity { get; set; }
    public double? OnHandQuantity { get; set; }
    public double? QuantityDel { get; set; }
    public double? BPR { get; set; }
  
    public double? CompleteQuanity { get; set; }
    public double? ShortfallQuantity { get; set; }
    public double? ItemShortfallQuantity { get; set; }
}