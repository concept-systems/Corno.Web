using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Corno.Web.Areas.Admin.Dto;
using Corno.Web.Areas.Admin.Models;
using Corno.Web.Areas.Admin.Services.Interfaces;
using Corno.Web.Controllers;
using Microsoft.AspNet.Identity;

namespace Corno.Web.Areas.Admin.Controllers;

public class AccessControlController : SuperController
{
    #region -- Constructors --
    public AccessControlController(
        IPermissionService permissionService,
        IMenuService menuService,
        IUserService userService,
        IRoleService roleService,
        IAuditLogService auditLogService)
    {
        _permissionService = permissionService;
        _menuService = menuService;
        _userService = userService;
        _roleService = roleService;
        _auditLogService = auditLogService;

        _indexPath = "~/Areas/Admin/Views/AccessControl/Index.cshtml";
    }
    #endregion

    #region -- Data Members --
    private readonly IPermissionService _permissionService;
    private readonly IMenuService _menuService;
    private readonly IUserService _userService;
    private readonly IRoleService _roleService;
    private readonly IAuditLogService _auditLogService;
    private readonly string _indexPath;
    #endregion

    #region -- Actions --
    [Authorize]
    public ActionResult Index()
    {
        return View(_indexPath);
    }

    [HttpPost]
    public async Task<ActionResult> GetRoles()
    {
        try
        {
            var roles = await _roleService.GetListAsync().ConfigureAwait(false);
            return Json(roles, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            return GetJsonResult(exception);
        }
    }

    [HttpPost]
    public async Task<ActionResult> GetUsers()
    {
        try
        {
            var users = await _userService.GetListAsync<AspNetUser>(null, u => u).ConfigureAwait(false);
            return Json(users.Select(u => new { Id = u.Id, UserName = u.UserName }), JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            return GetJsonResult(exception);
        }
    }

    [HttpPost]
    public async Task<ActionResult> GetMenuTree()
    {
        try
        {
            var menus = await _menuService.GetMenuTreeAsync().ConfigureAwait(false);
            return Json(menus, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            return GetJsonResult(exception);
        }
    }

    [HttpPost]
    public async Task<ActionResult> GetMenuPermissions(string userId, string roleId)
    {
        try
        {
            var permissions = await _permissionService.GetMenuPermissionsAsync(null, userId, roleId).ConfigureAwait(false);
            return Json(permissions, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            return GetJsonResult(exception);
        }
    }

    [HttpPost]
    public async Task<ActionResult> SaveMenuPermissions(string userId, string roleId, Dictionary<int, bool> permissions)
    {
        try
        {
            await _permissionService.SaveMenuPermissionsAsync(userId, roleId, permissions).ConfigureAwait(false);

            // Audit log
            var currentUserId = User.Identity.GetUserId();
            var targetType = string.IsNullOrEmpty(userId) ? "Role" : "User";
            var targetId = string.IsNullOrEmpty(userId) ? roleId : userId;
            await _auditLogService.LogAsync(
                currentUserId,
                "SavePermissions",
                "AccessControl",
                targetId,
                targetType,
                $"Saved menu permissions for {targetType}: {targetId}",
                Request.UserHostAddress,
                Request.UserAgent
            ).ConfigureAwait(false);

            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            return GetJsonResult(exception);
        }
    }

    [HttpPost]
    public async Task<ActionResult> GetPagePermissions(string controller, string action, string userId, string roleId)
    {
        try
        {
            var permissions = await _permissionService.GetPagePermissionsAsync(controller, action, userId, roleId).ConfigureAwait(false);
            return Json(permissions, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            return GetJsonResult(exception);
        }
    }

    [HttpPost]
    public async Task<ActionResult> SavePagePermissions(string userId, string roleId, string controller, string action, string area, bool isAllowed)
    {
        try
        {
            await _permissionService.SavePagePermissionsAsync(userId, roleId, controller, action, area, isAllowed).ConfigureAwait(false);

            // Audit log
            var currentUserId = User.Identity.GetUserId();
            var targetType = string.IsNullOrEmpty(userId) ? "Role" : "User";
            var targetId = string.IsNullOrEmpty(userId) ? roleId : userId;
            await _auditLogService.LogAsync(
                currentUserId,
                "SavePermissions",
                "AccessControl",
                targetId,
                targetType,
                $"Saved page permission for {targetType}: {targetId}, Controller: {controller}, Action: {action}",
                Request.UserHostAddress,
                Request.UserAgent
            ).ConfigureAwait(false);

            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            return GetJsonResult(exception);
        }
    }

    [HttpPost]
    public async Task<ActionResult> GetControlPermissions(string controlId, string userId, string roleId)
    {
        try
        {
            var permissions = await _permissionService.GetControlPermissionsAsync(controlId, userId, roleId).ConfigureAwait(false);
            return Json(permissions, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            return GetJsonResult(exception);
        }
    }

    [HttpPost]
    public async Task<ActionResult> SaveControlPermissions(string userId, string roleId, string controlId, string controlName, bool isAllowed, string controller = null, string action = null)
    {
        try
        {
            await _permissionService.SaveControlPermissionsAsync(userId, roleId, controlId, controlName, isAllowed, controller, action).ConfigureAwait(false);

            // Audit log
            var currentUserId = User.Identity.GetUserId();
            var targetType = string.IsNullOrEmpty(userId) ? "Role" : "User";
            var targetId = string.IsNullOrEmpty(userId) ? roleId : userId;
            await _auditLogService.LogAsync(
                currentUserId,
                "SavePermissions",
                "AccessControl",
                targetId,
                targetType,
                $"Saved control permission for {targetType}: {targetId}, Control: {controlId}",
                Request.UserHostAddress,
                Request.UserAgent
            ).ConfigureAwait(false);

            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            return GetJsonResult(exception);
        }
    }
    #endregion
}

