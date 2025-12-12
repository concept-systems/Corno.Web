using Corno.Web.Dtos;

namespace Corno.Web.Areas.Kitchen.Dto.Carton;

public class CartonDispatchDto : BaseDto
{
    #region -- Properties --
    public string WarehouseOrderNo { get; set; }
    public string LoadNo { get; set; }
    public string CartonBarcode { get; set; }
    #endregion
}