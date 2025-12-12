using System;
using System.Collections.Generic;
using Corno.Web.Dtos;

namespace Corno.Web.Areas.Kitchen.Dto.Plan;

public class PlanViewDto : BaseDto
{
    // Header
    public DateTime? DueDate { get; set; }
    public DateTime? PlanDate { get; set; }
    public string WarehouseOrderNo { get; set; }
    public string SoNo { get; set; }
    public string LotNo { get; set; }
    public string OneLineItemCode { get; set; }
    public double? OrderQuantity { get; set; }
    public double? PackedQuantity { get; set; }
    public double? PrintQuantity { get; set; }
    public string WarehouseName { get; set; }
    public string ImportFileName { get; set; }

    // Items
    public virtual List<PlanViewItemDto> PlanViewItemDtos { get; set; } = new();
    public virtual List<PlanViewLabelChartDto> PlanViewChartDtos { get; set; } = new();
    public virtual List<CartonViewChartDto> CartonViewChartDtos { get; set; } = new();
}