using System.ComponentModel.DataAnnotations;
using Corno.Web.Models.Base;
using Mapster;

namespace Corno.Web.Models.Location;

public class LocationUserDetail : BaseModel 
{
    public int? LocationId { get; set; }
    public string UserId { get; set; }

    [Required]
    [AdaptIgnore]
    public virtual Location Location { get; set; }
}