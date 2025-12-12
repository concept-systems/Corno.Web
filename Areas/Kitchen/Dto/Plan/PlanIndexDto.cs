using System;

namespace Corno.Web.Areas.Kitchen.Dto.Plan;

public class PlanIndexDto
{
    public int Id { get; set; }
    public string LotNo { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? PlanDate { get; set; }
    public string SoNo { get; set; }
    public string WarehouseOrderNo { get; set; }
    public double OrderQuantity { get; set; }
    public double? PrintQuantity { get; set; }
    public double? SortQuantity { get; set; }
    public double? BendQuantity { get; set; }
    public double? SubAssemblyQuantity { get; set; }
    public double? PackedQuantity { get; set; }
    public double? PalletInQuantity { get; set; }
}