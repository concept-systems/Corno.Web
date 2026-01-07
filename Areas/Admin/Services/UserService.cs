using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web;
using Corno.Web.Areas.Admin.Dto;
using Corno.Web.Areas.Admin.Models;
using Corno.Web.Areas.Admin.Services.Interfaces;
using Corno.Web.Globals;
using Corno.Web.Repository.Interfaces;
using Corno.Web.Services;
using Mapster;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace Corno.Web.Areas.Admin.Services;

public class UserService : CornoService<AspNetUser>, IUserService
{
    #region -- Consctructors --
    public UserService(IGenericRepository<AspNetUser> genericRepository, /*IIdentityService identityService,*/
       IRoleService roleService, IUserRoleService userRoleService, 
       IGenericRepository<AspNetLoginHistory> loginHistoryRepository) : base(genericRepository)
    {
        //_identityService = identityService;

        _roleService = roleService;
        _userRoleService = userRoleService;
        _loginHistoryRepository = loginHistoryRepository;

        UserManager = HttpContext.Current?.GetOwinContext().GetUserManager<ApplicationUserManager>();
        SignInManager = HttpContext.Current?.GetOwinContext().Get<ApplicationSignInManager>();
    }
    #endregion

    #region -- Data Members --
    //private readonly IIdentityService _identityService;
    private readonly IRoleService _roleService;
    private readonly IUserRoleService _userRoleService;
    private readonly IGenericRepository<AspNetLoginHistory> _loginHistoryRepository;

    private ApplicationUserManager UserManager; //=> HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
    private ApplicationSignInManager SignInManager;// => HttpContext.Current.GetOwinContext().Get<ApplicationSignInManager>();
    #endregion

    #region -- Public Methods --
    public async Task<IEnumerable> GetViewModelListAsync(Expression<Func<AspNetUser, bool>> filter = null,
        Func<IQueryable<AspNetUser>, IOrderedQueryable<AspNetUser>> orderBy = null,
        string includeProperties = "")
    {
        var results = await GetAsync(filter, u => new
        {
            u.Id,
            u.UserName,
            Name = u.FirstName + " " + u.LastName
        }, orderBy).ConfigureAwait(false);
        return results;
    }

    public async Task<IEnumerable<AspNetUser>> GetListAsync<TDest>(Expression<Func<AspNetUser, bool>> filter = null,
        Expression<Func<AspNetUser, TDest>> select = null,
        string includeProperties = null,
        Func<IQueryable<AspNetUser>, IOrderedQueryable<AspNetUser>> orderBy = null)
    {
        if (filter != null)
            return await GetAsync(filter, g => g, orderBy).ConfigureAwait(false);
        var all = await GetAsync<AspNetUser>(null, g => g, orderBy).ConfigureAwait(false);
        return all;
    }

    public async Task<AspNetUser> GetByIdAsync(string id)
    {
        return await FirstOrDefaultAsync(u => u.Id == id, u => u).ConfigureAwait(false);
    }

    public async Task<AspNetUser> GetByUserNameAsync(string userName)
    {
        return await FirstOrDefaultAsync(u => u.UserName == userName, u => u).ConfigureAwait(false);
    }

    public async Task<AspNetUser> GetOrCreateAsync(string userName, string password, string firstName, string lastName)
    {
        var user = await FirstOrDefaultAsync(u => u.UserName == userName, u => u).ConfigureAwait(false);
        if (user != null)
            return user;

        user = new AspNetUser
        {
            UserName = userName,
            Password = password,
            FirstName = firstName,
            LastName = lastName,
            PhoneNumberConfirmed = true,
        };

        await UserManager.CreateAsync(user).ConfigureAwait(false);
        return user;
    }

    public async Task ChangePasswordAsync(string userName, string newPassword)
    {
        var user = await UserManager.FindByNameAsync(userName).ConfigureAwait(false);
        if (null == user)
            throw new Exception("User not found");


        await UserManager.RemovePasswordAsync(user.Id).ConfigureAwait(false);
        await UserManager.AddPasswordAsync(user.Id, newPassword).ConfigureAwait(false);
        
        await UpdateAndSaveAsync(user).ConfigureAwait(false);
    }

    public async Task<bool> IsAdministratorAsync(string userId)
    {
        return await UserManager.IsInRoleAsync(userId, RoleNames.Administrator).ConfigureAwait(false);
    }

    public async Task<IList<string>> GetUserRolesAsync(string userId)
    {
        return await UserManager.GetRolesAsync(userId).ConfigureAwait(false);
    }

    public async Task<UserCrudDto> GetViewModelWithRolesAsync(string id)
    {
        var user = await GetByIdAsync(id).ConfigureAwait(false);

        var dto = new UserCrudDto();
        user.Adapt(dto);
        var roles = (await _roleService.GetAsync<AspNetRole>(null, r => r).ConfigureAwait(false)).ToList();
        dto.Roles = roles.Select(role => new UserRoleDto
        {
            IsSelected = false,
            RoleName = role.Name
        }).ToList();

        var userRoleIds = await _userRoleService.GetAsync(p => p.UserId == id,
                p => p.RoleId).ConfigureAwait(false);
        var userRoles = (await _roleService.GetAsync<AspNetRole>(p => userRoleIds.Contains(p.Id), p => p).ConfigureAwait(false)).ToList();
        dto.Roles.ForEach(d => d.IsSelected = null != userRoles.FirstOrDefault(x => x.Name == d.RoleName));

        return dto;
    }

    public async Task<AspNetUser> CreateAsync(UserCrudDto dto)
    {
        if (!dto.Password.Equals(dto.ConfirmPassword, StringComparison.InvariantCulture))
            throw new Exception("Password and confirm password doesn't match");

        var existing = await FirstOrDefaultAsync(p =>
            p.UserName.Equals(dto.UserName, StringComparison.OrdinalIgnoreCase), p => p).ConfigureAwait(false);
        if (existing != null)
            throw new Exception($"User with user name {dto.UserName} already exists.");

        var aspNetUser = new AspNetUser();
        dto.Adapt(aspNetUser);

        var passwordHasher = new PasswordHasher();
        aspNetUser.Id = Guid.NewGuid().ToString();
        aspNetUser.SecurityStamp = Guid.NewGuid().ToString();
        aspNetUser.PasswordHash = passwordHasher.HashPassword(dto.Password);

        await AddAndSaveAsync(aspNetUser).ConfigureAwait(false);

        return aspNetUser;
    }

    public async Task<AspNetUser> EditAsync(UserCrudDto dto)
    {
        var existing = await FirstOrDefaultAsync(p =>
            p.Id.Equals(dto.Id, StringComparison.OrdinalIgnoreCase), p => p).ConfigureAwait(false);
        if (existing == null)
            throw new Exception($"User with user name {dto.UserName} does not exist.");

        existing.FirstName = dto.FirstName;
        existing.LastName = dto.LastName;
        existing.Email = dto.Email;
        existing.PhoneNumber = dto.PhoneNumber;

        await UpdateAndSaveAsync(existing).ConfigureAwait(false);

        var selectedRoles = dto.Roles.Where(r => r.IsSelected)
            .Select(r => r.RoleName)
            .ToList();
        await _userRoleService.AddRolesAsync(existing.Id, selectedRoles).ConfigureAwait(false);

        return existing;
    }

    public async Task<AspNetUser> ChangePasswordAsync(UserCrudDto viewModel)
    {
        if (!viewModel.Password.Equals(viewModel.ConfirmPassword, StringComparison.InvariantCulture))
            throw new Exception("Password and confirm password doesn't match");

        var existing = await FirstOrDefaultAsync(p =>
            p.Id.Equals(viewModel.Id, StringComparison.OrdinalIgnoreCase), p => p).ConfigureAwait(false);
        if (existing == null)
            throw new Exception($"User with user name {viewModel.UserName} does not exist.");

        var passwordHasher = new PasswordHasher();
        existing.PasswordHash = passwordHasher.HashPassword(viewModel.Password);

        await UpdateAndSaveAsync(existing).ConfigureAwait(false);

        return existing;
    }

    public async Task ChangePasswordAsync(string userId, string oldPassword, string newPassword)
    {
        var user = await GetByIdAsync(userId).ConfigureAwait(false);
        if (user == null)
            throw new Exception("User not found");

        // Verify old password using UserManager
        var isValidPassword = await UserManager.CheckPasswordAsync(user, oldPassword).ConfigureAwait(false);
        if (!isValidPassword)
            throw new Exception("Current password is incorrect");

        // Change password
        await UserManager.RemovePasswordAsync(userId).ConfigureAwait(false);
        await UserManager.AddPasswordAsync(userId, newPassword).ConfigureAwait(false);
        
        await UpdateAndSaveAsync(user).ConfigureAwait(false);
    }

    public async Task<ProfileDto> GetProfileAsync(string userId)
    {
        var user = await GetByIdAsync(userId).ConfigureAwait(false);
        if (user == null)
            throw new Exception("User not found");

        var profile = new ProfileDto();
        user.Adapt(profile);
        
        // Get user roles
        profile.Roles = (await GetUserRolesAsync(userId).ConfigureAwait(false)).ToList();
        
        // Get last login info
        var allLogins = await _loginHistoryRepository.GetAsync<AspNetLoginHistory>(
            h => h.AspNetUserId == userId && h.LoginResult == LoginResult.Success,
            h => h
        ).ConfigureAwait(false);
        var lastLogin = allLogins?.OrderByDescending(x => x.LoginTime).FirstOrDefault();
        
        if (lastLogin != null)
        {
            profile.LastLoginTime = lastLogin.LoginTime;
            profile.LastIpAddress = lastLogin.IpAddress;
        }
        
        // Get active sessions count
        var activeSessions = await _loginHistoryRepository.GetAsync<AspNetLoginHistory>(
            h => h.AspNetUserId == userId && h.LogoutTime == null,
            h => h
        ).ConfigureAwait(false);
        profile.ActiveSessions = activeSessions?.Count() ?? 0;
        
        return profile;
    }

    public async Task UpdateProfileAsync(string userId, ProfileDto dto)
    {
        var user = await GetByIdAsync(userId).ConfigureAwait(false);
        if (user == null)
            throw new Exception("User not found");

        user.FirstName = dto.FirstName;
        user.LastName = dto.LastName;
        user.Email = dto.Email;
        user.PhoneNumber = dto.PhoneNumber;
        
        await UpdateAndSaveAsync(user).ConfigureAwait(false);
    }

    #region -- Session Management Methods --

    /// <summary>
    /// Checks if user has an active session
    /// </summary>
    public async Task<bool> HasActiveSessionAsync(string userId)
    {
        // Query only active sessions directly from AspNetLoginHistory table (much faster than loading all 38k records)
        var activeSession = await _loginHistoryRepository.FirstOrDefaultAsync<AspNetLoginHistory>(
            h => h.AspNetUserId == userId && h.LogoutTime == null,
            h => h).ConfigureAwait(false);

        if (activeSession == null) return false;

        // Check if session is stale (e.g., older than 24 hours without activity)
        // You can adjust this timeout as needed
        var sessionTimeout = TimeSpan.FromHours(24);
        if (activeSession.LoginTime.HasValue &&
            DateTime.Now - activeSession.LoginTime.Value > sessionTimeout)
        {
            // Mark stale session as logged out
            activeSession.LogoutTime = activeSession.LoginTime.Value.Add(sessionTimeout);
            await _loginHistoryRepository.UpdateAsync(activeSession).ConfigureAwait(false);
            await _loginHistoryRepository.SaveAsync().ConfigureAwait(false);
            return false;
        }

        return true;
    }

    /// <summary>
    /// Gets active session information
    /// </summary>
    public async Task<AspNetLoginHistory> GetActiveSessionAsync(string userId)
    {
        // Query only active sessions directly from AspNetLoginHistory table
        return await _loginHistoryRepository.FirstOrDefaultAsync<AspNetLoginHistory>(
            h => h.AspNetUserId == userId && h.LogoutTime == null,
            h => h).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets all active sessions for a user
    /// </summary>
    public async Task<List<AspNetLoginHistory>> GetActiveSessionsAsync(string userId)
    {
        var sessions = await _loginHistoryRepository.GetAsync<AspNetLoginHistory>(
            h => h.AspNetUserId == userId && h.LogoutTime == null,
            h => h,
            q => q.OrderByDescending(x => x.LoginTime)
        ).ConfigureAwait(false);
        return sessions?.ToList() ?? new List<AspNetLoginHistory>();
    }

    /// <summary>
    /// Invalidates all active sessions for a user (logs them out)
    /// </summary>
    public async Task InvalidateAllSessionsAsync(string userId)
    {
        // Use direct SQL update to avoid Adapt() hang in UpdateAsync/UpdateRangeAsync
        // This bypasses the Mapster Adapt() issue with AspNetUser navigation property
        // and is much more efficient than loading and updating each session individually
        var logoutTime = DateTime.Now;
        var sql = "UPDATE AspNetLoginHistories SET LogoutTime = @p0 WHERE AspNetUserId = @p1 AND LogoutTime IS NULL";
        
        await _loginHistoryRepository.ExecuteSqlCommandAsync(sql, logoutTime, userId).ConfigureAwait(false);
    }

    /// <summary>
    /// Invalidates a specific session
    /// </summary>
    public async Task InvalidateSessionAsync(string userId, string sessionId)
    {
        var session = await _loginHistoryRepository.FirstOrDefaultAsync<AspNetLoginHistory>(
            h => h.AspNetUserId == userId && h.Id == sessionId && h.LogoutTime == null,
            h => h).ConfigureAwait(false);

        if (session == null)
            throw new Exception("Session not found or already invalidated");

        session.LogoutTime = DateTime.Now;
        await _loginHistoryRepository.UpdateAsync(session).ConfigureAwait(false);
        await _loginHistoryRepository.SaveAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Updates login history with session tracking
    /// </summary>
    public async Task UpdateLoginHistoryAsync(string userName, string ipAddress, LoginResult loginResult)
    {
        var user = await GetByUserNameAsync(userName).ConfigureAwait(false);
        if (null == user) return;

        // Invalidate any existing active sessions first
        await InvalidateAllSessionsAsync(user.Id).ConfigureAwait(false);

        // Save new login info directly to AspNetLoginHistory table
        var now = DateTime.Now;
        var loginHistory = new AspNetLoginHistory
        {
            Id = Guid.NewGuid().ToString(),
            AspNetUserId = user.Id,
            UserName = user.UserName,
            LoginTime = now,
            LogoutTime = null,
            IpAddress = ipAddress,
            MachineName = Environment.MachineName,
            HostName = System.Net.Dns.GetHostName(),
            LoginResult = loginResult,
            IsActive = loginResult == LoginResult.Success, // Set IsActive to true for successful logins
            LastActivityTime = now // Initialize last activity time
        };

        // Try to capture session ID if HttpContext is available (may be null in async contexts)
        try
        {
            if (System.Web.HttpContext.Current != null && System.Web.HttpContext.Current.Session != null)
            {
                loginHistory.SessionId = System.Web.HttpContext.Current.Session.SessionID;
            }
        }
        catch
        {
            // Session ID capture is optional, continue without it
        }

        await _loginHistoryRepository.AddAsync(loginHistory).ConfigureAwait(false);
        await _loginHistoryRepository.SaveAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Updates logout history
    /// </summary>
    public async Task UpdateLogoutHistoryAsync(string userId)
    {
        // Query only active sessions directly from AspNetLoginHistory table
        var loginHistory = await _loginHistoryRepository.FirstOrDefaultAsync<AspNetLoginHistory>(
            h => h.AspNetUserId == userId && h.LogoutTime == null,
            h => h).ConfigureAwait(false);

        if (null == loginHistory)
            return; // User is not logged in, nothing to update

        loginHistory.LogoutTime = DateTime.Now;
        loginHistory.IsActive = false; // Mark session as inactive
        await _loginHistoryRepository.UpdateAsync(loginHistory).ConfigureAwait(false);
        await _loginHistoryRepository.SaveAsync().ConfigureAwait(false);
    }

    #endregion


    #endregion
}