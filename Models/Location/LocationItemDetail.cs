using System.ComponentModel.DataAnnotations;
using Corno.Web.Models.Base;
using Mapster;

namespace Corno.Web.Models.Location;

public class LocationItemDetail : BaseModel 
{
    public int? LocationId { get; set; }
    public int? ItemId { get; set; }
    public int? ProductId { get; set; }
    public double? MaxQuantity { get; set; }

    [Required]
    [AdaptIgnore]
    public virtual Location Location { get; set; }
}