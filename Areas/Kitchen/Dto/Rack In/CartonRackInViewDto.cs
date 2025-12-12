using Corno.Web.Dtos;

namespace Corno.Web.Areas.Kitchen.Dto.Rack_In;

public class CartonRackInViewDto : BaseDto
{
    #region -- Properties --
    public string PalletNo { get; set; }
    public string Barcode { get; set; }
    public int? CartonId { get; set; }
    public string RackNo { get; set; }
    #endregion
}