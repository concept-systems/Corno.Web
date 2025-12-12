using Corno.Web.Models.Base;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Corno.Web.Areas.Admin.Models;

public class AspNetRole : IdentityRole, ICornoModel
{
    #region -- Properties --
    public int? CompanyId { get; set; }
    //public int? SerialNo { get; set; }
    //public string Code { get; set; }
    //[Key] 
    //public new string Id { get; set; }
    //public string Status { get; set; }
    //public string CreatedBy { get; set; }
    //public DateTime? CreatedDate { get; set; }
    //public string ModifiedBy { get; set; }
    //public DateTime? ModifiedDate { get; set; }
    //public string DeletedBy { get; set; }
    //public DateTime? DeletedDate { get; set; }

    public string Description { get; set; }
    #endregion

    #region -- Methods --
    public void Reset()
    {
        Description = string.Empty;
    }
    #endregion
}