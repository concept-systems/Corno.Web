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
       IRoleService roleService, IUserRoleService userRoleService) : base(genericRepository)
    {
        //_identityService = identityService;

        _roleService = roleService;
        _userRoleService = userRoleService;
    }
    #endregion

    #region -- Data Members --
    //private readonly IIdentityService _identityService;
    private readonly IRoleService _roleService;
    private readonly IUserRoleService _userRoleService;

    private ApplicationUserManager UserManager =>
        HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
    private ApplicationSignInManager SignInManager =>
        HttpContext.Current.GetOwinContext().Get<ApplicationSignInManager>();
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

    #endregion
}