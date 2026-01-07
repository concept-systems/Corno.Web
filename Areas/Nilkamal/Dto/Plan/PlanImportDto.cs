using System;
using Ganss.Excel;

namespace Corno.Web.Areas.Nilkamal.Dto.Plan;

public class PlanImportDto
{
    [Column("Due Date")]
    public DateTime? DueDate { get; set; }
    
    [Column("Product Code")]
    public string ProductCode { get; set; }

    [Column("Product Description")]
    public string ProductDescription { get; set; }

    [Column("Production Order No")]
    public string ProductionOrderNo { get; set; }

    [Column("So No")]
    public string SoNo { get; set; }

    [Column("Plan QTY")]
    public double? PlanQty { get; set; }
    
    public string Status { get; set; }
    public string Remark { get; set; }
}