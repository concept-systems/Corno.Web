using System.ComponentModel.DataAnnotations;
using Corno.Web.Models.Base;
using Mapster;

namespace Corno.Web.Areas.Kitchen.Models.Put_To_Light;

public class TrolleyConfigDetail : BaseModel
{
    #region -- Properties --
    public int? TrolleyConfigId { get; set; }
    public int LocationId { get; set; }
    public int ColorId { get; set; }

    [Required]
    [AdaptIgnore]
    public TrolleyConfig TrolleyConfig { get; set; }
    #endregion
}