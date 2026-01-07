using System;

namespace Corno.Web.Areas.Nilkamal.Dto.Plan;

public class PlanIndexDto
{
    public int Id { get; set; }
    public DateTime? PlanDate { get; set; }
    public string ProductionOrderNo { get; set; }
    public DateTime? DueDate { get; set; }
    public double OrderQuantity { get; set; }
    public double? PrintQuantity { get; set; }
    public double? PackedQuantity { get; set; }
    public string Status { get; set; }
}