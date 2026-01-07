using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Corno.Web.Areas.Admin.Dto;
using Corno.Web.Areas.Admin.Services.Interfaces;
using Corno.Web.Windsor;
using Microsoft.AspNet.Identity;

namespace Corno.Web.Helpers;

/// <summary>
/// Helper class for menu rendering and permission checking
/// </summary>
public static class MenuHelper
{
    /// <summary>
    /// Checks if user has access to a menu item
    /// </summary>
    public static bool HasMenuAccess(this HtmlHelper helper, int menuId)
    {
        var userId = HttpContext.Current?.User?.Identity?.GetUserId();
        if (string.IsNullOrEmpty(userId))
            return false;

        var permissionService = Bootstrapper.Get<IPermissionService>();
        return permissionService.HasMenuAccessAsync(userId, menuId).Result;
    }

    /// <summary>
    /// Checks if user has access to a menu by menu name
    /// </summary>
    public static bool HasMenuAccess(this HtmlHelper helper, string menuName)
    {
        var userId = HttpContext.Current?.User?.Identity?.GetUserId();
        if (string.IsNullOrEmpty(userId))
            return false;

        var permissionService = Bootstrapper.Get<IPermissionService>();
        return permissionService.HasMenuAccessAsync(userId, menuName).Result;
    }

    /// <summary>
    /// Gets user menus filtered by permissions
    /// </summary>
    public static List<MenuDto> GetUserMenus(this HtmlHelper helper)
    {
        var userId = HttpContext.Current?.User?.Identity?.GetUserId();
        if (string.IsNullOrEmpty(userId))
            return new List<MenuDto>();

        var menuService = Bootstrapper.Get<IMenuService>();
        return menuService.GetUserMenusAsync(userId).Result;
    }

    /// <summary>
    /// Renders menu tree HTML
    /// </summary>
    public static MvcHtmlString RenderMenuTree(this HtmlHelper helper, List<MenuDto> menus, int? parentId = null)
    {
        if (menus == null || !menus.Any())
            return MvcHtmlString.Empty;

        var childMenus = menus.Where(m => m.ParentMenuId == parentId).OrderBy(m => m.DisplayOrder).ToList();
        if (!childMenus.Any())
            return MvcHtmlString.Empty;

        var html = new System.Text.StringBuilder();
        html.Append("<ul class=\"menu-tree\">");

        foreach (var menu in childMenus)
        {
            // Check permission
            if (!helper.HasMenuAccess(menu.Id))
                continue;

            html.Append("<li>");
            
            if (!string.IsNullOrEmpty(menu.ControllerName) && !string.IsNullOrEmpty(menu.ActionName))
            {
                var url = string.IsNullOrEmpty(menu.Area)
                    ? helper.Action(menu.ActionName, menu.ControllerName).ToString()
                    : helper.Action(menu.ActionName, menu.ControllerName, new { area = menu.Area }).ToString();
                
                html.Append($"<a href=\"{url}\">");
                if (!string.IsNullOrEmpty(menu.IconClass))
                    html.Append($"<i class=\"{menu.IconClass}\"></i> ");
                html.Append(menu.DisplayName);
                html.Append("</a>");
            }
            else
            {
                html.Append("<span>");
                if (!string.IsNullOrEmpty(menu.IconClass))
                    html.Append($"<i class=\"{menu.IconClass}\"></i> ");
                html.Append(menu.DisplayName);
                html.Append("</span>");
            }

            // Render children recursively
            var children = helper.RenderMenuTree(menus, menu.Id);
            if (!string.IsNullOrEmpty(children.ToString()))
            {
                html.Append(children);
            }

            html.Append("</li>");
        }

        html.Append("</ul>");
        return new MvcHtmlString(html.ToString());
    }
}

