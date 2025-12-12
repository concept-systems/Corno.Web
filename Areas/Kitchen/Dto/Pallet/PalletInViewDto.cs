using System.Collections.Generic;

namespace Corno.Web.Areas.Kitchen.Dto.Pallet;

public class PalletInViewDto
{
    public string WarehouseOrderNo { get; set; }
    public string PalletNo { get; set; }
    public string RackNo { get; set; }
    public string LoadNo { get; set; }
    public string CartonBarcode { get; set; }
    public virtual List<CartonPalletViewDto> PalletDetails { get; set; } = new();
}