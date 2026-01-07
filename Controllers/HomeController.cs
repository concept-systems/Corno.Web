using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Corno.Web.Areas.Admin.Dto;
using Corno.Web.Areas.Admin.Services.Interfaces;
using Corno.Web.Globals;
using Corno.Web.Services.Interfaces;
using Corno.Web.Logger;
using Corno.Web.Services.Masters.Interfaces;
using Kendo.Mvc;
using Kendo.Mvc.Extensions;
using Microsoft.AspNet.Identity;
using SiteMapNode = Kendo.Mvc.SiteMapNode;

namespace Corno.Web.Controllers;

[Authorize]
public class HomeController : Controller
{
    #region -- Constructors --

    public HomeController(IProjectService projectService, IMenuService menuService, IPermissionService permissionService, IUserService userService, IMenuMigrationService migrationService)
    {
        _projectService = projectService;
        _menuService = menuService;
        _permissionService = permissionService;
        _userService = userService;
        _migrationService = migrationService;
    }

    #endregion

    #region -- Data Members --

    private readonly IProjectService _projectService;
    private readonly IMenuService _menuService;
    private readonly IPermissionService _permissionService;
    private readonly IUserService _userService;
    private readonly IMenuMigrationService _migrationService;

    #endregion

    #region -- Private Methods --
    // XML sitemap methods removed - system now uses database menus exclusively
    #endregion

    #region -- Actions --
    public async Task<ActionResult> Index()
    {
        try
        {
            var project = await _projectService.GetProjectAsync("Active").ConfigureAwait(false);
            ApplicationGlobals.ProjectId = project.Id;

            var userId = User.Identity.GetUserId();

            // Check if menus exist in database
            var menuCount = await _menuService.GetMenuCountAsync().ConfigureAwait(false);
            
            // If menus exist but default login module menus don't, create them
            if (menuCount > 0)
            {
                // Check if Dashboard menu exists (as indicator of default menus)
                // Use GetVisibleMenusAsync to check all menus, not just tree structure
                var allVisibleMenus = await _menuService.GetVisibleMenusAsync().ConfigureAwait(false);
                var hasDashboard = allVisibleMenus?.Any(m => m.MenuName == "Dashboard") ?? false;
                
                if (!hasDashboard)
                {
                    // Default login module menus are missing, create them
                    var createResult = await _migrationService.CreateDefaultLoginModuleMenusAsync(userId).ConfigureAwait(false);
                    LogHandler.LogInfo($"Created default login module menus: {createResult.CreatedMenus} created, {createResult.SkippedMenus} skipped. Message: {createResult.Message}");
                }
            }

            // If no menus exist, check if user is admin and show migration prompt
            if (menuCount == 0)
            {
                var isAdmin = await _userService.IsAdministratorAsync(userId).ConfigureAwait(false);
                if (isAdmin && !string.IsNullOrEmpty(project.MenuXml))
                {
                    // Check if migration was already attempted (prevent repeated prompts)
                    var migrationAttempted = Session["MenuMigrationAttempted"] as bool? ?? false;
                    if (!migrationAttempted)
                    {
                        ViewBag.ShowMigrationPrompt = true;
                        ViewBag.ProjectName = "Active";
                    }
                }
            }

            // Get all visible menus from database
            var allMenus = await _menuService.GetVisibleMenusAsync().ConfigureAwait(false);

            // Debug: Log menu information
            LogHandler.LogInfo($"HomeController.Index: Found {allMenus?.Count ?? 0} visible menus. Menu names: {string.Join(", ", allMenus?.Select(m => m.MenuName) ?? new List<string>())}");

            // If no menus, return empty sitemap
            var sitemapName = User.Identity.GetUserId();
            if (allMenus == null || !allMenus.Any())
            {
                LogHandler.LogInfo("HomeController.Index: No visible menus found, returning empty sitemap");
                var emptySiteMap = new XmlSiteMap();
                emptySiteMap.RootNode = new SiteMapNode
                {
                    Title = "",
                    ControllerName = "",
                    ActionName = ""
                };
                SiteMapManager.SiteMaps.Remove(sitemapName);
                SiteMapManager.SiteMaps.Add(new KeyValuePair<string, SiteMapBase>(sitemapName, emptySiteMap));
                ViewBag.SiteMapRoot = emptySiteMap.RootNode;
                return View();
            }

            // Convert database menus to Kendo SiteMapNode structure (with permission filtering)
            var siteMap = new XmlSiteMap();
            siteMap.RootNode = await ConvertMenusToSiteMapAsync(allMenus, userId, null).ConfigureAwait(false);

            // Remove existing entry if it exists
            SiteMapManager.SiteMaps.Remove(sitemapName);

            // Now add the new sitemap
            SiteMapManager.SiteMaps.Add(new KeyValuePair<string, SiteMapBase>(sitemapName, siteMap));

            // Pass sitemap root node to view for menu display
            ViewBag.SiteMapRoot = siteMap.RootNode;

            //ViewBag.ShowMigrationPrompt = true;

            return View();
        }
        catch(Exception exception)
        {
            LogHandler.LogError(exception);
            
            // Get user-friendly error message
            var errorMessage = LogHandler.GetDetailException(exception)?.Message ?? exception.Message;
            ViewBag.ErrorMessage = errorMessage;
            
            // Set up empty sitemap so view can still render
            var sitemapName = User.Identity.GetUserId();
            var emptySiteMap = new XmlSiteMap();
            emptySiteMap.RootNode = new SiteMapNode
            {
                Title = "",
                ControllerName = "",
                ActionName = ""
            };
            SiteMapManager.SiteMaps.Remove(sitemapName);
            SiteMapManager.SiteMaps.Add(new KeyValuePair<string, SiteMapBase>(sitemapName, emptySiteMap));
            ViewBag.SiteMapRoot = emptySiteMap.RootNode;
            
            return View();
        }
    }

    private async Task<SiteMapNode> ConvertMenusToSiteMapAsync(List<MenuDto> menus, string userId, int? parentMenuId)
    {
        var rootNode = new SiteMapNode
        {
            Title = "",
            ControllerName = "",
            ActionName = ""
        };

        // Filter menus by parent
        var childMenus = menus.Where(m => 
            (parentMenuId == null && m.ParentMenuId == null) ||
            (parentMenuId != null && m.ParentMenuId == parentMenuId)
        ).OrderBy(m => m.DisplayOrder).ToList();

        foreach (var menu in childMenus)
        {
            // Check menu-level permission
            var hasMenuAccess = await _permissionService.HasMenuAccessAsync(userId, menu.Id).ConfigureAwait(false);
            if (!hasMenuAccess)
            {
                LogHandler.LogInfo($"HomeController: Menu '{menu.MenuName}' (ID: {menu.Id}) filtered out - no menu access");
                continue;
            }

            // If has controller/action, check page-level permission
            if (!string.IsNullOrEmpty(menu.ControllerName) && !string.IsNullOrEmpty(menu.ActionName))
            {
                var hasPageAccess = await _permissionService.HasPageAccessAsync(userId, menu.ControllerName, menu.ActionName, menu.Area).ConfigureAwait(false);
                if (!hasPageAccess)
                {
                    LogHandler.LogInfo($"HomeController: Menu '{menu.MenuName}' (ID: {menu.Id}) filtered out - no page access for {menu.ControllerName}/{menu.ActionName}");
                    continue;
                }
            }

            var siteMapNode = new SiteMapNode
            {
                Title = menu.DisplayName,
                ControllerName = menu.ControllerName,
                ActionName = menu.ActionName ?? "Index"
            };

            // Add area to route values
            if (!string.IsNullOrEmpty(menu.Area))
                siteMapNode.RouteValues.Add("area", menu.Area);

            // Add icon
            if (!string.IsNullOrEmpty(menu.IconClass))
                siteMapNode.Attributes.Add(new KeyValuePair<string, object>("icon", menu.IconClass));

            // Add menu name
            if (!string.IsNullOrEmpty(menu.MenuName))
                siteMapNode.Attributes.Add(new KeyValuePair<string, object>("name", menu.MenuName));

            // Parse and add route values from JSON
            if (!string.IsNullOrEmpty(menu.RouteValues))
            {
                try
                {
                    var routeValues = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(menu.RouteValues);
                    if (routeValues != null)
                    {
                        foreach (var kvp in routeValues)
                        {
                            if (kvp.Key == "misctype")
                                siteMapNode.RouteValues.Add(FieldConstants.MiscType, kvp.Value);
                            else if (kvp.Key == "reportName")
                                siteMapNode.RouteValues.Add(FieldConstants.ReportName, kvp.Value);
                            else
                                siteMapNode.RouteValues.Add(kvp.Key, kvp.Value);
                        }
                    }
                }
                catch
                {
                    // Ignore JSON parse errors
                }
            }
            else
            {
                // If no route values, add "web" = "true" by default for system menus
                // This ensures they appear in the treeview and home page
                siteMapNode.RouteValues.Add("web", "true");
            }

            // Process children recursively
            var childSiteMapNode = await ConvertMenusToSiteMapAsync(menus, userId, menu.Id).ConfigureAwait(false);
            if (childSiteMapNode.ChildNodes.Any())
            {
                siteMapNode.ChildNodes.AddRange(childSiteMapNode.ChildNodes);
            }

            rootNode.ChildNodes.Add(siteMapNode);
        }

        return rootNode;
    }

    public async Task<ActionResult> About()
    {
        ViewBag.Message = "Your application description page.";

        return await Task.FromResult(View()).ConfigureAwait(false);
    }

    public async Task<ActionResult> Contact()
    {
        ViewBag.Message = "Your contact page.";

        return await Task.FromResult(View()).ConfigureAwait(false);
    }

    public async Task<ActionResult> Chat()
    {
        return await Task.FromResult(View()).ConfigureAwait(false);
    }
    #endregion
}