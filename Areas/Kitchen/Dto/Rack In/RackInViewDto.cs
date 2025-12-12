using System.Collections.Generic;

namespace Corno.Web.Areas.Kitchen.Dto.Rack_In;

public class RackInViewDto
{
    public string WarehouseOrderNo { get; set; }
    public string PalletNo { get; set; }
    public string RackNo { get; set; }
    public string LoadNo { get; set; }
    public string CartonBarcode { get; set; }
    public virtual List<CartonRackInViewDto> RackInDetails { get; set; } = new();
}