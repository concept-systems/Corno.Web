using System;

namespace Corno.Web.Models;

public class PlanViewModel
{
    public DateTime PlanDate { get; set; }
    public DateTime Date { get; set; }
    public DateTime DueDate { get; set; }
    public string SoNo { get; set; }
    public string WarehouseOrderNo { get; set; }
    public string ProductionOrderNo { get; set; }
    public string ProductName { get; set; }
    public double OrderQuantity { get; set; }
    public double CutQuantity { get; set; }
    public double EdgeBandQuantity { get; set; }
    public double BoringQuantity { get; set; }
    public double RoutingQuantity { get; set; }
    public double SubAssemblyQuantity { get; set; }
    public double PackQuantity { get; set; }

    public double? PackedQuantity { get; set; }
    public double? PrintQuantity { get; set; }
}