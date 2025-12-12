using System.Collections.Generic;

namespace Corno.Web.Areas.Kitchen.Dto.Rack_Out;

public class RackOutViewDto
{
    public string WarehouseOrderNo { get; set; }
    public string CartonBarcode { get; set; }
    public virtual List<CartonRackOutViewDto> RackOutDetails { get; set; } = new();
}