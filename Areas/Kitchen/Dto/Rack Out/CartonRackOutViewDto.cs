using Corno.Web.Dtos;

namespace Corno.Web.Areas.Kitchen.Dto.Rack_Out;

public class CartonRackOutViewDto : BaseDto
{
    #region -- Properties --
    public string WarehouseOrderNo { get; set; }
    public string CartonBarcode { get; set; }
    #endregion
}