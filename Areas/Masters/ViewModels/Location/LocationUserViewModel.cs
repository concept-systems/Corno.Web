using Corno.Web.Models.Base;

namespace Corno.Web.Areas.Masters.ViewModels.Location;

public class LocationUserViewModel : BaseModel
{

    public int? LocationId { get; set; }
    public string UserId { get; set; }
    public string UserName { get; set; }

    public virtual LocationViewModel Location { get; set; }


}