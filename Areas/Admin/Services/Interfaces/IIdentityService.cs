using System.Collections.Generic;
using System.Threading.Tasks;
using Corno.Concept.Portal.Areas.Admin.Models;
using Corno.Concept.Portal.Services.Interfaces;
using Microsoft.AspNet.Identity.Owin;

namespace Corno.Concept.Portal.Areas.Admin.Services.Interfaces;

public interface IIdentityService : IService
{
    #region -- Methods (Roles) --
    IEnumerable<AspNetRole> GetRoles();
    AspNetRole FindRoleById(string id);
    bool RoleExists(string name);
    bool CreateRole(AspNetRole role);
    bool EditRole(AspNetRole role);
    bool DeleteRole(AspNetRole role);
    #endregion

    #region -- Methods (Users) --

    /*Task<SignInStatus> Login(AspNetUser userViewModel);
    Task<SignInStatus> Login(string userName, string password);*/
    string HashPassword(string password);
    Task<bool> ChangePassword(AspNetUser user, string newPassword);
    IEnumerable<AspNetUser> GetUsers();
    ICollection<AspNetUser> GetUsersInRole(string roleName);
    string GetUserName(string id);
    AspNetUser GetUser(string userId);
    AspNetUser GetUserByName(string userName);
    AspNetUser FindUserById(string id);
    AspNetUser FindUserByName(string name);
    bool IsUserInRole(string userId, string roleName);
    bool CreateUser(AspNetUser user);
    bool UpdateUser(AspNetUser user);
    bool DeleteUser(AspNetUser user);
    AspNetUser VerifyUserNamePassword(string userName, string password);
    void UpdateLoginHistory(string userName, string ipAddress, LoginResult loginResult);
    void UpdateLogoutHistory(string userId);
    IList<string> GetUserRoles(string userId);
    bool AddUserToRole(string userId, string roleName);
    void ClearUserRoles(string userId);
    int? GetAccountNo(string userId);
    AspNetUser GetUserByDeviceId(int deviceId);
    double? GetWallet(string userId);
    void UpdateWallet(string userId, double? wallet);

    #endregion
}