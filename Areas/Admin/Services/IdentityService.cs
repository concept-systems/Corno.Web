using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Corno.Concept.Portal.Areas.Admin.Models;
using Corno.Concept.Portal.Areas.Admin.Services.Interfaces;
using Corno.Concept.Portal.Windsor.Context;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;

namespace Corno.Concept.Portal.Areas.Admin.Services;

public class IdentityService : IIdentityService
{
    #region -- Constructors --
    public IdentityService(BaseDbContext baseContext)
    {
        _roleManager ??= new RoleManager<AspNetRole, string>(new RoleStore<AspNetRole, string, IdentityUserRole>(baseContext));
        _userManager ??= new UserManager<AspNetUser, string>(new UserStore<AspNetUser, AspNetRole, string, IdentityUserLogin, IdentityUserRole,
            IdentityUserClaim>(baseContext));
        _dbContext = baseContext;
        /*if (null == _signInManager)
        {
            //_signInManager = HttpContext.Current?.GetOwinContext().Get<SignInManager<AspNetUser, string>>();

            //_signInManager = new SignInManager<AspNetUser, string>(_userManager, 
            //    new AuthenticationManager());
        }*/
    }

    #endregion

    #region -- Data Members --

    private static RoleManager<AspNetRole, string> _roleManager;
    private static UserManager<AspNetUser, string> _userManager;

    private readonly BaseDbContext _dbContext;

    //private static SignInManager<AspNetUser, string> _signInManager;

    #endregion

    #region -- Methods (Roles) --

    public IEnumerable<AspNetRole> GetRoles()
    {
        return _roleManager.Roles;
    }

    public AspNetRole FindRoleById(string id)
    {
        return _roleManager.FindById(id);
    }

    public bool RoleExists(string name)
    {
        return _roleManager.RoleExists(name);
    }

    public bool CreateRole(AspNetRole role)
    {
        var idResult = _roleManager.Create(role);
        if (false == idResult.Succeeded)
            throw new Exception(idResult.Errors.FirstOrDefault());

        return idResult.Succeeded;
    }

    public bool EditRole(AspNetRole role)
    {
        var idResult = _roleManager.Update(role);
        if (false == idResult.Succeeded)
            throw new Exception(idResult.Errors.FirstOrDefault());

        return idResult.Succeeded;
    }

    public bool DeleteRole(AspNetRole role)
    {
        var idResult = _roleManager.Delete(role);
        if (false == idResult.Succeeded)
            throw new Exception(idResult.Errors.FirstOrDefault());

        return idResult.Succeeded;
    }

    #endregion

    #region -- Methods (Users) --

    //private async Task SignInAsync(string userId, string password)
    //{
    //    await _userManager.AddPasswordAsync(userId, password);
    //}

    /*private Task<SignInStatus> SignInAsync(AspNetUser user, bool isPersistent)
    {
        return _signInManager.PasswordSignInAsync(user.UserName, user.Password, false, false);
        //try
        //{
        //    //AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
        //    //var identity = await _userManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
        //    //AuthenticationManager.SignIn(new AuthenticationProperties { IsPersistent = isPersistent }, identity);

        //    await _userManager.AddPasswordAsync(user.Id, user.Password);
        //}
        //catch
        //{
        //    // Ignore
        //}
    }*/

    /*public Task<SignInStatus> Login(AspNetUser userViewModel)
    {
        var user = _userManager.Find(userViewModel.UserName, userViewModel.Password);
        if (user == null) return null;
        return SignInAsync(user, false);
    }

    public Task<SignInStatus> Login(string userName, string password)
    {
        var user = _userManager.Find(userName, password);
        if (user == null) return null;
        return SignInAsync(user, false);
    }*/

    public string HashPassword(string password)
    {
        return _userManager.PasswordHasher.HashPassword(password);
    }

    public async Task<bool> ChangePassword(AspNetUser viewModel, string newPassword)
    {
        var user = _userManager.FindByName(viewModel.UserName);
        if (user == null) return false;
        //var code = await _userManager.GeneratePasswordResetTokenAsync(user.Id);
        //var result = _userManager.ResetPasswordAsync(user.Id, code,
        //    newPassword);
        await _userManager.RemovePasswordAsync(user.Id).ConfigureAwait(false);
        var result = await _userManager.AddPasswordAsync(user.Id, newPassword).ConfigureAwait(false);

        UpdateUser(user);

        return result.Succeeded;
    }

    public IEnumerable<AspNetUser> GetUsers()
    {
        return _userManager.Users;
    }

    public ICollection<AspNetUser> GetUsersInRole(string roleName)
    {
        var identityUserRoles = _roleManager.FindById(roleName).Users;
        ICollection<AspNetUser> users = new List<AspNetUser>();
        foreach (var identityUserRole in identityUserRoles)
            users.Add(_userManager.FindById(identityUserRole.UserId));

        return users;
    }

    public string GetUserName(string id)
    {
        var user = _userManager.FindById(id);
        if (null != user)
            return user.UserName;
        return string.Empty;
    }

    public AspNetUser GetUser(string userId)
    {
        return _userManager.FindById(userId);
    }

    public AspNetUser GetUserByName(string userName)
    {
        return _userManager.FindByName(userName);
    }

    public AspNetUser FindUserById(string id)
    {
        return _userManager.FindById(id);
    }

    public AspNetUser FindUserByName(string name)
    {
        return _userManager.FindByName(name);
    }

    public bool IsUserInRole(string userId, string roleName)
    {
        return _userManager.IsInRole(userId, roleName);
    }

    public bool CreateUser(AspNetUser user)
    {
        var idResult = _userManager.Create(user, user.Password);
        if (false == idResult.Succeeded)
            throw new Exception(idResult.Errors.FirstOrDefault());

        return idResult.Succeeded;
    }

    public bool UpdateUser(AspNetUser user)
    {
        var idResult = _userManager.Update(user);
        if (false == idResult.Succeeded)
            throw new Exception(idResult.Errors.FirstOrDefault());

        return idResult.Succeeded;
    }

    public bool DeleteUser(AspNetUser user)
    {
        var idResult = _userManager.Delete(user);
        if (false == idResult.Succeeded)
            throw new Exception(idResult.Errors.FirstOrDefault());

        return idResult.Succeeded;
    }

    public AspNetUser VerifyUserNamePassword(string userName, string password)
    {
        return _userManager.Find(userName, password);
    }

    public void UpdateLoginHistory(string userName, string ipAddress, LoginResult loginResult)
    {
        var user = _userManager.FindByName(userName);

        if (null == user) return;

        var loginHistory = user.AspNetLoginHistories.FirstOrDefault(a => a.LogoutTime == null);
        if (null != loginHistory)
            throw new Exception("User is already logged in.");

        // save login info
        user.AspNetLoginHistories.Add(new AspNetLoginHistory
        {
            Id = Guid.NewGuid().ToString(),
            AspNetUserId = user.Id,
            UserName = user.UserName,
            LoginTime = DateTime.Now,
            LogoutTime = null,
            IpAddress = ipAddress,
            MachineName = Environment.MachineName,
            HostName = System.Net.Dns.GetHostName(),
            LoginResult = LoginResult.Success
        });
        UpdateUser(user);
    }

    public void UpdateLogoutHistory(string userId)
    {
        var user = _userManager.FindById(userId);

        if (null == user) return;
        // save login info
        var loginHistory = user.AspNetLoginHistories.FirstOrDefault(a => a.LogoutTime == null);
        if (null == loginHistory)
            throw new Exception("User is not logged in");

        loginHistory.LogoutTime = DateTime.Now;
        UpdateUser(user);
    }

    public IList<string> GetUserRoles(string userId)
    {
        //var roles = _roleManager.Roles;
        //return  _roleManager.Roles.Where(r => r.Users.Any(u => u.UserId == userId))
        //    .Select(r => r.Name);
        return _userManager.GetRoles(userId);
    }

    public bool AddUserToRole(string userId, string roleName)
    {
        var idResult = _userManager.AddToRole(userId, roleName);
        return idResult.Succeeded;
    }

    public void ClearUserRoles(string userId)
    {
        var user = _userManager.FindById(userId);
        var currentRoles = new List<IdentityUserRole>();
        currentRoles.AddRange(user.Roles);
        foreach (var role in currentRoles)
            _userManager.RemoveFromRole(userId, _roleManager.FindById(role.RoleId).Name);
    }

    public int? GetAccountNo(string userId)
    {
        var user = _userManager.FindById(userId);
        return user?.AccountNo;
    }

    public AspNetUser GetUserByDeviceId(int deviceId)
    {
        var user = _userManager.Users.FirstOrDefault(u => u.DeviceId == deviceId);
        return user;
    }
    public double? GetWallet(string userId)
    {
        var user = _userManager.FindById(userId);
        return user?.Wallet;
    }

    public void UpdateWallet(string userId, double? wallet)
    {
        var user = _userManager.FindById(userId);
        user.Wallet = wallet;
        _userManager.Update(user);
    }

    #endregion
}

public class ApplicationSignInManager : SignInManager<AspNetUser, string>
{
    public ApplicationSignInManager(UserManager<AspNetUser, string> userManager, IAuthenticationManager authenticationManager)
        : base(userManager, authenticationManager)
    {
    }

    //public override Task<ClaimsIdentity> CreateUserIdentityAsync(AspNetUser user)
    //{
    //    return user.GenerateUserIdentityAsync((UserManager<user, string>)UserManager);
    //}

    public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
    {
        return new ApplicationSignInManager(context.GetUserManager<UserManager<AspNetUser, string>>(), context.Authentication);
    }
}