using Corno.Web.Models.Base;

namespace Corno.Web.Areas.Masters.Dtos.Location;

public class LocationUserDto : BaseModel
{

    public int? LocationId { get; set; }
    public string UserId { get; set; }
    public string UserName { get; set; }

}