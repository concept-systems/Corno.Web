using Microsoft.AspNet.Identity.EntityFramework;

namespace Corno.Web.Areas.Admin.Models;

public class AspNetUserRole : IdentityUserRole<string>
{
    public virtual AspNetUser User { get; set; }
}