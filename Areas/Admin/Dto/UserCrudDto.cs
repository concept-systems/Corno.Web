using System.Collections.Generic;

namespace Corno.Web.Areas.Admin.Dto;

public class UserCrudDto
{
    #region -- Constructors --
    public UserCrudDto()
    {
        Roles = new List<UserRoleDto>();
    }
    #endregion

    #region -- Properties --
    public string Id { get; set; }
    public string UserName { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public bool Locked { get; set; }

    public string Password { get; set; }
    public string ConfirmPassword { get; set; }

    public List<UserRoleDto> Roles { get; set; }
#endregion
}