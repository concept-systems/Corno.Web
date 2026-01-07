using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Corno.Web.Areas.Admin.Dto;
using Corno.Web.Areas.Admin.Models;
using Corno.Web.Areas.Admin.Services.Interfaces;
using Corno.Web.Repository.Interfaces;
using Corno.Web.Services;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Web;
using System.Data.Entity;
using Mapster;

namespace Corno.Web.Areas.Admin.Services;

public class PermissionService : CornoService<AccessControl>, IPermissionService
{
    private readonly IGenericRepository<Menu> _menuRepository;
    private readonly IGenericRepository<PermissionType> _permissionTypeRepository;
    private readonly IUserService _userService;

    public PermissionService(
        IGenericRepository<AccessControl> accessControlRepository,
        IGenericRepository<Menu> menuRepository,
        IGenericRepository<PermissionType> permissionTypeRepository,
        IUserService userService) : base(accessControlRepository)
    {
        _menuRepository = menuRepository;
        _permissionTypeRepository = permissionTypeRepository;
        _userService = userService;
    }

    #region -- Menu Level Permissions --
    public async Task<bool> HasMenuAccessAsync(string userId, int menuId)
    {
        // Administrators have full access by default
        var isAdmin = await _userService.IsAdministratorAsync(userId).ConfigureAwait(false);
        if (isAdmin)
            return true;

        var userRoles = (await _userService.GetUserRolesAsync(userId).ConfigureAwait(false)).ToList();

        // Check user-specific permissions first (highest priority)
        var userPermission = await FirstOrDefaultAsync(
            ac => ac.UserId == userId
            && ac.PermissionTypeId == (int)PermissionTypeEnum.Menu
            && ac.MenuId == menuId,
            ac => ac
        ).ConfigureAwait(false);

        if (userPermission != null)
            return userPermission.IsAllowed;

        // Check role-based permissions
        var rolePermissions = await GetAsync(
            ac => ac.RoleId != null
            && userRoles.Contains(ac.RoleId)
            && ac.PermissionTypeId == (int)PermissionTypeEnum.Menu
            && ac.MenuId == menuId,
            ac => ac
        ).ConfigureAwait(false);

        if (rolePermissions != null && rolePermissions.Any())
        {
            if (rolePermissions.Any(rp => !rp.IsAllowed))
                return false;
            return rolePermissions.Any(rp => rp.IsAllowed);
        }

        // Check parent menu access
        var menu = await _menuRepository.FirstOrDefaultAsync(m => m.Id == menuId, m => m).ConfigureAwait(false);
        if (menu?.ParentMenuId != null)
        {
            return await HasMenuAccessAsync(userId, menu.ParentMenuId.Value).ConfigureAwait(false);
        }

        // Default: Allow access if no permissions are configured (for initial setup)
        // This allows menus to be visible after migration before permissions are set up
        return true;
    }

    public async Task<bool> HasMenuAccessAsync(string userId, string menuName)
    {
        var menu = await _menuRepository.FirstOrDefaultAsync(m => m.MenuName == menuName, m => m).ConfigureAwait(false);
        if (menu == null)
            return false;

        return await HasMenuAccessAsync(userId, menu.Id).ConfigureAwait(false);
    }

    public async Task<List<AccessControlDto>> GetMenuPermissionsAsync(int? menuId, string userId = null, string roleId = null)
    {
        var permissions = await GetAsync(
            ac => ac.PermissionTypeId == (int)PermissionTypeEnum.Menu
            && (menuId == null || ac.MenuId == menuId)
            && (userId == null || ac.UserId == userId)
            && (roleId == null || ac.RoleId == roleId),
            ac => ac
        ).ConfigureAwait(false);
        return permissions.Adapt<List<AccessControlDto>>();
    }

    public async Task SaveMenuPermissionsAsync(string userId, string roleId, Dictionary<int, bool> permissions)
    {
        foreach (var permission in permissions)
        {
            var existing = await FirstOrDefaultAsync(
                ac => ac.PermissionTypeId == (int)PermissionTypeEnum.Menu
                && ac.MenuId == permission.Key
                && (userId != null ? ac.UserId == userId : ac.RoleId == roleId),
                ac => ac
            ).ConfigureAwait(false);

            if (existing != null)
            {
                existing.IsAllowed = permission.Value;
                existing.ModifiedDate = DateTime.Now;
                await UpdateAndSaveAsync(existing).ConfigureAwait(false);
            }
            else
            {
                var accessControl = new AccessControl
                {
                    PermissionTypeId = (int)PermissionTypeEnum.Menu,
                    MenuId = permission.Key,
                    UserId = userId,
                    RoleId = roleId,
                    IsAllowed = permission.Value,
                    CreatedBy = userId ?? "System",
                    CreatedDate = DateTime.Now
                };
                await AddAndSaveAsync(accessControl).ConfigureAwait(false);
            }
        }
    }
    #endregion

    #region -- Page Level Permissions --
    public async Task<bool> HasPageAccessAsync(string userId, string controller, string action, string area = null)
    {
        // Administrators have full access by default
        var isAdmin = await _userService.IsAdministratorAsync(userId).ConfigureAwait(false);
        if (isAdmin)
            return true;

        // Check menu access first
        var menu = await _menuRepository.FirstOrDefaultAsync(
            m => m.ControllerName == controller
            && m.ActionName == action
            && (area == null || m.Area == area),
            m => m
        ).ConfigureAwait(false);

        if (menu != null)
        {
            if (!await HasMenuAccessAsync(userId, menu.Id).ConfigureAwait(false))
                return false;
        }

        var userRoles = (await _userService.GetUserRolesAsync(userId).ConfigureAwait(false)).ToList();

        // Check user-specific page permissions
        var userPermission = await FirstOrDefaultAsync(
            ac => ac.UserId == userId
            && ac.PermissionTypeId == (int)PermissionTypeEnum.Page
            && ac.ControllerName == controller
            && ac.ActionName == action
            && (area == null || ac.Area == area),
            ac => ac
        ).ConfigureAwait(false);

        if (userPermission != null)
            return userPermission.IsAllowed;

        // Check role-based page permissions
        var rolePermissions = await GetAsync(
            ac => ac.RoleId != null
            && userRoles.Contains(ac.RoleId)
            && ac.PermissionTypeId == (int)PermissionTypeEnum.Page
            && ac.ControllerName == controller
            && ac.ActionName == action
            && (area == null || ac.Area == area),
            ac => ac
        ).ConfigureAwait(false);

        if (rolePermissions != null && rolePermissions.Any())
        {
            if (rolePermissions.Any(rp => !rp.IsAllowed))
                return false;
            return rolePermissions.Any(rp => rp.IsAllowed);
        }

        return true; // Default allow if menu access is granted
    }

    public async Task<List<AccessControlDto>> GetPagePermissionsAsync(string controller, string action, string userId = null, string roleId = null)
    {
        var query = await GetAsync<AccessControl>(
            ac => ac.PermissionTypeId == (int)PermissionTypeEnum.Page
            && ac.ControllerName == controller
            && ac.ActionName == action
            && (userId == null || ac.UserId == userId)
            && (roleId == null || ac.RoleId == roleId)
        ).ConfigureAwait(false);
        var permissions = query;//await query.ToListAsync().ConfigureAwait(false);

        return permissions.Adapt<List<AccessControlDto>>();
    }

    public async Task SavePagePermissionsAsync(string userId, string roleId, string controller, string action, string area, bool isAllowed)
    {
        var existing = await FirstOrDefaultAsync(
            ac => ac.PermissionTypeId == (int)PermissionTypeEnum.Page
            && ac.ControllerName == controller
            && ac.ActionName == action
            && (area == null || ac.Area == area)
            && (userId != null ? ac.UserId == userId : ac.RoleId == roleId),
            ac => ac
        ).ConfigureAwait(false);

        if (existing != null)
        {
            existing.IsAllowed = isAllowed;
            existing.ModifiedDate = DateTime.Now;
            await UpdateAndSaveAsync(existing).ConfigureAwait(false);
        }
        else
        {
            var accessControl = new AccessControl
            {
                PermissionTypeId = (int)PermissionTypeEnum.Page,
                ControllerName = controller,
                ActionName = action,
                Area = area,
                UserId = userId,
                RoleId = roleId,
                IsAllowed = isAllowed,
                CreatedBy = userId ?? "System",
                CreatedDate = DateTime.Now
            };
            await AddAndSaveAsync(accessControl).ConfigureAwait(false);
        }
    }
    #endregion

    #region -- Control Level Permissions --
    public async Task<bool> HasControlAccessAsync(string userId, string controlId, string controller = null, string action = null)
    {
        // Check page access first
        if (controller != null && action != null)
        {
            if (!await HasPageAccessAsync(userId, controller, action).ConfigureAwait(false))
                return false;
        }

        var userRoles = (await _userService.GetUserRolesAsync(userId).ConfigureAwait(false)).ToList();

        // Check user-specific control permissions
        var userPermission = await FirstOrDefaultAsync(
            ac => ac.UserId == userId
            && ac.PermissionTypeId == (int)PermissionTypeEnum.Control
            && ac.ControlId == controlId
            && (controller == null || ac.ControllerName == controller)
            && (action == null || ac.ActionName == action),
            ac => ac
        ).ConfigureAwait(false);

        if (userPermission != null)
            return userPermission.IsAllowed;

        // Check role-based control permissions
        var rolePermissions = await GetAsync(
            ac => ac.RoleId != null
            && userRoles.Contains(ac.RoleId)
            && ac.PermissionTypeId == (int)PermissionTypeEnum.Control
            && ac.ControlId == controlId
            && (controller == null || ac.ControllerName == controller)
            && (action == null || ac.ActionName == action),
            ac => ac
        ).ConfigureAwait(false);

        if (rolePermissions != null && rolePermissions.Any())
        {
            if (rolePermissions.Any(rp => !rp.IsAllowed))
                return false;
            return rolePermissions.Any(rp => rp.IsAllowed);
        }

        return true; // Default allow if page access is granted
    }

    public async Task<List<AccessControlDto>> GetControlPermissionsAsync(string controlId, string userId = null, string roleId = null)
    {
        var query = await GetAsync<AccessControl>(
            ac => ac.PermissionTypeId == (int)PermissionTypeEnum.Control
            && ac.ControlId == controlId
            && (userId == null || ac.UserId == userId)
            && (roleId == null || ac.RoleId == roleId)
        ).ConfigureAwait(false);
        var permissions = query;// await query.ToListAsync().ConfigureAwait(false);

        return permissions.Adapt<List<AccessControlDto>>();
    }

    public async Task SaveControlPermissionsAsync(string userId, string roleId, string controlId, string controlName, bool isAllowed, string controller = null, string action = null)
    {
        var existing = await FirstOrDefaultAsync(
            ac => ac.PermissionTypeId == (int)PermissionTypeEnum.Control
            && ac.ControlId == controlId
            && (controller == null || ac.ControllerName == controller)
            && (action == null || ac.ActionName == action)
            && (userId != null ? ac.UserId == userId : ac.RoleId == roleId),
            ac => ac
        ).ConfigureAwait(false);

        if (existing != null)
        {
            existing.IsAllowed = isAllowed;
            existing.ControlName = controlName;
            existing.ModifiedDate = DateTime.Now;
            await UpdateAndSaveAsync(existing).ConfigureAwait(false);
        }
        else
        {
            var accessControl = new AccessControl
            {
                PermissionTypeId = (int)PermissionTypeEnum.Control,
                ControlId = controlId,
                ControlName = controlName,
                ControllerName = controller,
                ActionName = action,
                UserId = userId,
                RoleId = roleId,
                IsAllowed = isAllowed,
                CreatedBy = userId ?? "System",
                CreatedDate = DateTime.Now
            };
            await AddAndSaveAsync(accessControl).ConfigureAwait(false);
        }
    }
    #endregion

    #region -- Combined Methods --
    public async Task<Dictionary<string, bool>> GetAllUserPermissionsAsync(string userId)
    {
        var userRoles = (await _userService.GetUserRolesAsync(userId).ConfigureAwait(false)).ToList();
        var permissions = new Dictionary<string, bool>();

        // Get all permissions for user and roles
        var allPermissions = await GetAsync(
            ac => (ac.UserId == userId || (ac.RoleId != null && userRoles.Contains(ac.RoleId))),
            ac => ac
        ).ConfigureAwait(false);

        foreach (var perm in allPermissions)
        {
            var key = $"{perm.PermissionTypeId}_{perm.MenuId}_{perm.ControllerName}_{perm.ActionName}_{perm.ControlId}";
            if (!permissions.ContainsKey(key) || perm.IsAllowed) // User permissions override role permissions
            {
                permissions[key] = perm.IsAllowed;
            }
        }

        return permissions;
    }

    public async Task<PermissionAssignmentDto> GetPermissionAssignmentAsync(int menuId, string userId = null, string roleId = null)
    {
        var menu = await _menuRepository.FirstOrDefaultAsync(m => m.Id == menuId, m => m).ConfigureAwait(false);
        if (menu == null)
            return null;

        var assignment = new PermissionAssignmentDto
        {
            MenuId = menuId,
            MenuName = menu.MenuName,
            MenuPath = menu.MenuPath,
            PagePermissions = new Dictionary<string, bool>(),
            ControlPermissions = new Dictionary<string, bool>()
        };

        // Get menu access
        var menuPermissions = await GetMenuPermissionsAsync(menuId, userId, roleId).ConfigureAwait(false);
        assignment.HasMenuAccess = menuPermissions.Any(p => p.IsAllowed) && !menuPermissions.Any(p => !p.IsAllowed);

        // Get page permissions
        if (!string.IsNullOrEmpty(menu.ControllerName) && !string.IsNullOrEmpty(menu.ActionName))
        {
            var pagePermissions = await GetPagePermissionsAsync(menu.ControllerName, menu.ActionName, userId, roleId).ConfigureAwait(false);
            assignment.PagePermissions[menu.ActionName] = pagePermissions.Any(p => p.IsAllowed) && !pagePermissions.Any(p => !p.IsAllowed);
        }

        return assignment;
    }
    #endregion
}

// Permission Type Enum
public enum PermissionTypeEnum
{
    Menu = 1,
    Page = 2,
    Control = 3
}

