using Corno.Web.Models.Base;

namespace Corno.Web.Areas.Kitchen.Dto.Put_To_Light;

public class TrolleyConfigDetailDto : BaseModel
{
    #region -- Properties --
    public int TrolleyConfigId { get; set; }
    public int LocationId { get; set; }
    public string LocationName { get; set; }
    public int ColorId { get; set; }
    public string ColorName { get; set; }
    #endregion
}