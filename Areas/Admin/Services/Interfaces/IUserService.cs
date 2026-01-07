using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Corno.Web.Areas.Admin.Dto;
using Corno.Web.Areas.Admin.Models;
using Corno.Web.Services.Interfaces;

namespace Corno.Web.Areas.Admin.Services.Interfaces;

public interface IUserService : ICornoService<AspNetUser>
{
    #region -- Methods --

    Task<IEnumerable> GetViewModelListAsync(Expression<Func<AspNetUser, bool>> filter = null,
        Func<IQueryable<AspNetUser>, IOrderedQueryable<AspNetUser>> orderBy = null,
        string includeProperties = "");

    Task<IEnumerable<AspNetUser>> GetListAsync<TDest>(Expression<Func<AspNetUser, bool>> filter = null,
        Expression<Func<AspNetUser, TDest>> select = null,
        string includeProperties = null,
        Func<IQueryable<AspNetUser>, IOrderedQueryable<AspNetUser>> orderBy = null);

    Task<AspNetUser> GetByIdAsync(string id);
    Task<AspNetUser> GetByUserNameAsync(string userName);

    Task<AspNetUser> GetOrCreateAsync(string userName, string password, string firstName, string lastName);

    Task ChangePasswordAsync(string userName, string newPassword);
    Task<bool> IsAdministratorAsync(string userId);
    Task<IList<string>> GetUserRolesAsync(string userId);

    Task<UserCrudDto> GetViewModelWithRolesAsync(string id);
    Task<AspNetUser> CreateAsync(UserCrudDto dto);
    Task<AspNetUser> EditAsync(UserCrudDto dto);
    Task<AspNetUser> ChangePasswordAsync(UserCrudDto viewModel);
    Task ChangePasswordAsync(string userId, string oldPassword, string newPassword);
    
    // Profile Management Methods
    Task<ProfileDto> GetProfileAsync(string userId);
    Task UpdateProfileAsync(string userId, ProfileDto dto);
    
    // Session Management Methods
    Task<bool> HasActiveSessionAsync(string userId);
    Task<AspNetLoginHistory> GetActiveSessionAsync(string userId);
    Task<List<AspNetLoginHistory>> GetActiveSessionsAsync(string userId);
    Task InvalidateAllSessionsAsync(string userId);
    Task InvalidateSessionAsync(string userId, string sessionId);
    Task UpdateLoginHistoryAsync(string userName, string ipAddress, LoginResult loginResult);
    Task UpdateLogoutHistoryAsync(string userId);


    #endregion
}