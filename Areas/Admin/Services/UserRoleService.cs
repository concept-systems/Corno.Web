using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Corno.Web.Areas.Admin.Models;
using Corno.Web.Areas.Admin.Services.Interfaces;
using Corno.Web.Repository.Interfaces;
using Corno.Web.Services;

namespace Corno.Web.Areas.Admin.Services;

public class UserRoleService : CornoService<AspNetUserRole>, IUserRoleService
{
    #region -- Constructors --

    public UserRoleService(IGenericRepository<AspNetUserRole> genericRepository, IRoleService roleService)
    : base(genericRepository)
    {
        _roleService = roleService;
    }
    #endregion

    #region -- Data Members --
    private readonly IRoleService _roleService;
    #endregion

    #region -- Public Methods --

    public async Task AddRolesAsync(string userId, List<string> roleNames)
    {
        var roleIds = await _roleService.GetAsync(p => roleNames.Contains(p.Name), p => p.Id).ConfigureAwait(false);

        var userRoles = await GetAsync<AspNetUserRole>(p => p.UserId == userId, p => p).ConfigureAwait(false);
        foreach (var roleId in roleIds)
        {
            if (userRoles.FirstOrDefault(p => p.RoleId == roleId) == null)
                await AddAsync(new AspNetUserRole { UserId = userId, RoleId = roleId }).ConfigureAwait(false);
        }

        await SaveAsync().ConfigureAwait(false);
    }
    #endregion
}