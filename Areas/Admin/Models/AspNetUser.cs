using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;
using Corno.Web.Areas.Admin.Dto;
using Corno.Web.Models.Base;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Corno.Web.Areas.Admin.Models;

//[NotMapped]
public sealed class AspNetUser : IdentityUser, ICornoModel
{
    #region -- Constructors --

    public AspNetUser()
    {
        AspNetLoginHistories = new List<AspNetLoginHistory>();
        //UserRoles = new List<AspNetUserRole>();
    }
    #endregion

    #region -- Properties --
    public string FirstName { get; set; }
    public string LastName { get; set; }
    //public int? DeviceId { get; set; }

    [NotMapped]
    public string Password { get; set; }
    [NotMapped]
    public string ConfirmPassword { get; set; }
    [NotMapped]
    public string OldPassword { get; set; }
    [NotMapped]
    public bool RememberMe { get; set; }
    [NotMapped]
    public int? AccountNo { get; set; }
    [NotMapped]
    public double? Wallet { get; set; }
    [NotMapped]
    public string Otp { get; set; }
    [NotMapped]
    public string UserType { get; set; }
        
    public ICollection<AspNetLoginHistory> AspNetLoginHistories { get; set; }
    #endregion

    #region -- Methods --

    public void Reset()
    {
        FirstName = string.Empty;
        LastName = string.Empty;
    }

    public async System.Threading.Tasks.Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<AspNetUser> manager)
    {
        // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
        var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
        // Add custom user claims here
        return userIdentity;
    }

    public string GetFullName()
    {
        return FirstName + " " + LastName;
    }
    #endregion
}

public class AspNetUserViewModel
{
    #region -- Constructors --

    public AspNetUserViewModel()
    {
        AspNetRoleViewModels = new List<AspNetRoleDto>();
    }
    #endregion

    #region -- Properties --
    public string UserId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }

    public List<AspNetRoleDto> AspNetRoleViewModels { get; set; }
    #endregion
}