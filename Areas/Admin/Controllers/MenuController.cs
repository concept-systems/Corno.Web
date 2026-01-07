using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Corno.Web.Areas.Admin.Dto;
using Corno.Web.Areas.Admin.Services.Interfaces;
using Corno.Web.Controllers;
using Corno.Web.Logger;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNet.Identity;

namespace Corno.Web.Areas.Admin.Controllers;

public class MenuController : SuperController
{
    #region -- Constructors --
    public MenuController(IMenuService menuService, IAuditLogService auditLogService, IMenuMigrationService migrationService)
    {
        _menuService = menuService;
        _auditLogService = auditLogService;
        _migrationService = migrationService;

        const string viewPath = "~/Areas/Admin/Views/Menu/";
        _indexPath = $"{viewPath}Index.cshtml";
        _createPath = $"{viewPath}Create.cshtml";
        _editPath = $"{viewPath}Edit.cshtml";
    }
    #endregion

    #region -- Data Members --
    private readonly IMenuService _menuService;
    private readonly IAuditLogService _auditLogService;
    private readonly IMenuMigrationService _migrationService;
    private readonly string _indexPath;
    private readonly string _createPath;
    private readonly string _editPath;
    #endregion

    #region -- Actions --
    [Authorize]
    public ActionResult Index()
    {
        return View(_indexPath);
    }

    public async Task<ActionResult> Create()
    {
        var dto = new MenuDto
        {
            IsVisible = true,
            IsActive = true,
            DisplayOrder = 0
        };
        return View(_createPath, dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Create(MenuDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
                return View(_createPath, dto);

            var userId = User.Identity.GetUserId();
            await _menuService.CreateAsync(dto, userId).ConfigureAwait(false);

            // Audit log
            await _auditLogService.LogAsync(
                userId,
                "Create",
                "Menu",
                null,
                dto.DisplayName,
                $"Created menu: {dto.DisplayName}",
                Request.UserHostAddress,
                Request.UserAgent
            ).ConfigureAwait(false);

            return RedirectToAction("Index");
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }
        return View(_createPath, dto);
    }

    public async Task<ActionResult> Edit(int? id)
    {
        if (id == null)
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

        var dto = await _menuService.GetByIdAsync(id.Value).ConfigureAwait(false);
        if (dto == null)
            return HttpNotFound();

        return View(_editPath, dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Edit(MenuDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
                return View(_editPath, dto);

            var userId = User.Identity.GetUserId();
            await _menuService.UpdateAsync(dto, userId).ConfigureAwait(false);

            // Audit log
            await _auditLogService.LogAsync(
                userId,
                "Edit",
                "Menu",
                dto.Id.ToString(),
                dto.DisplayName,
                $"Updated menu: {dto.DisplayName}",
                Request.UserHostAddress,
                Request.UserAgent
            ).ConfigureAwait(false);

            return RedirectToAction("Index");
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }
        return View(_editPath, dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            var menu = await _menuService.GetByIdAsync(id).ConfigureAwait(false);
            if (menu == null)
                return HttpNotFound();

            await _menuService.DeleteAsync(id).ConfigureAwait(false);

            // Audit log
            var userId = User.Identity.GetUserId();
            await _auditLogService.LogAsync(
                userId,
                "Delete",
                "Menu",
                id.ToString(),
                menu.DisplayName,
                $"Deleted menu: {menu.DisplayName}",
                Request.UserHostAddress,
                Request.UserAgent
            ).ConfigureAwait(false);

            return RedirectToAction("Index");
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
            return RedirectToAction("Index");
        }
    }

    [HttpPost]
    public async Task<ActionResult> GetMenuTree([DataSourceRequest] DataSourceRequest request)
    {
        try
        {
            var menus = await _menuService.GetMenuTreeAsync().ConfigureAwait(false);
            var result = menus.ToDataSourceResult(request);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            return GetGridErrorResult(exception);
        }
    }

    [HttpGet]
    public async Task<ActionResult> GetMenusForDropdown(int? excludeMenuId = null)
    {
        try
        {
            var menus = await _menuService.GetAllMenusAsync(excludeMenuId).ConfigureAwait(false);
            return Json(menus, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            return GetJsonResult(exception);
        }
    }

    [HttpPost]
    public async Task<ActionResult> MoveMenu(int menuId, int? newParentId, int newOrder)
    {
        try
        {
            await _menuService.MoveMenuAsync(menuId, newParentId, newOrder).ConfigureAwait(false);
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            return GetJsonResult(exception);
        }
    }

    [HttpPost]
    [Authorize(Roles = "Administrator")]
    public async Task<ActionResult> CreateDefaultMenus()
    {
        try
        {
            var userId = User.Identity.GetUserId();
            var result = await _migrationService.CreateDefaultLoginModuleMenusAsync(userId).ConfigureAwait(false);

            // Audit log
            await _auditLogService.LogAsync(
                userId,
                "CreateDefaultMenus",
                "Menu",
                null,
                "Default Menu Creation",
                $"Created {result.CreatedMenus} default menus. Skipped: {result.SkippedMenus}",
                Request.UserHostAddress,
                Request.UserAgent
            ).ConfigureAwait(false);

            return Json(new 
            { 
                success = result.Success, 
                message = result.Message,
                createdMenus = result.CreatedMenus,
                updatedMenus = result.UpdatedMenus,
                skippedMenus = result.SkippedMenus,
                errors = result.Errors
            }, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            var errorMessage = exception.Message;
            var innerException = exception.InnerException?.Message;
            var fullError = $"{errorMessage}{(string.IsNullOrEmpty(innerException) ? "" : $" | Inner: {innerException}")}";
            
            return Json(new 
            { 
                success = false, 
                message = $"Failed to create default menus: {fullError}",
                error = fullError
            }, JsonRequestBehavior.AllowGet);
        }
    }

    [HttpPost]
    [Authorize(Roles = "Administrator")]
    public async Task<ActionResult> MigrateFromXml(string projectName = "Active")
    {
        try
        {
            var userId = User.Identity.GetUserId();
            var result = await _migrationService.MigrateFromXmlAsync(projectName, userId).ConfigureAwait(false);

            // Audit log
            await _auditLogService.LogAsync(
                userId,
                "MigrateMenus",
                "Menu",
                null,
                "Menu Migration",
                $"Migrated {result.CreatedMenus} menus from XML. Total: {result.TotalMenus}, Errors: {result.Errors.Count}",
                Request.UserHostAddress,
                Request.UserAgent
            ).ConfigureAwait(false);

            // After XML migration (or even if skipped), create default login module menus
            var defaultMenusResult = await _migrationService.CreateDefaultLoginModuleMenusAsync(userId).ConfigureAwait(false);
            
            // Log the result for debugging
            LogHandler.LogInfo($"CreateDefaultLoginModuleMenus result: Success={defaultMenusResult.Success}, Created={defaultMenusResult.CreatedMenus}, Skipped={defaultMenusResult.SkippedMenus}, Updated={defaultMenusResult.UpdatedMenus}, Message={defaultMenusResult.Message}");
            if (defaultMenusResult.Errors != null && defaultMenusResult.Errors.Any())
            {
                foreach (var error in defaultMenusResult.Errors)
                {
                    LogHandler.LogError(error);
                }
            }

            if (result.Success)
            {
                var totalCreated = result.CreatedMenus + defaultMenusResult.CreatedMenus;
                var totalSkipped = result.SkippedMenus + defaultMenusResult.SkippedMenus;
                
                return Json(new 
                { 
                    success = true, 
                    message = result.Message ?? $"Successfully migrated {result.CreatedMenus} menus from XML sitemap. Created {defaultMenusResult.CreatedMenus} default login module menus.",
                    totalMenus = result.TotalMenus + defaultMenusResult.TotalMenus,
                    createdMenus = totalCreated,
                    updatedMenus = result.UpdatedMenus,
                    skippedMenus = totalSkipped,
                    duplicateMenus = result.DuplicateMenus,
                    defaultMenusCreated = defaultMenusResult.CreatedMenus,
                    defaultMenusSkipped = defaultMenusResult.SkippedMenus
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new 
                { 
                    success = false, 
                    message = result.Message ?? "Migration completed with errors.",
                    errors = result.Errors,
                    totalMenus = result.TotalMenus,
                    createdMenus = result.CreatedMenus,
                    updatedMenus = result.UpdatedMenus,
                    skippedMenus = result.SkippedMenus,
                    duplicateMenus = result.DuplicateMenus,
                    defaultMenusCreated = defaultMenusResult.CreatedMenus,
                    defaultMenusSkipped = defaultMenusResult.SkippedMenus,
                    defaultMenusUpdated = defaultMenusResult.UpdatedMenus,
                    defaultMenusMessage = defaultMenusResult.Message,
                    defaultMenusErrors = defaultMenusResult.Errors
                }, JsonRequestBehavior.AllowGet);
            }
        }
        catch (Exception exception)
        {
            // Return detailed error information for migration
            var errorMessage = exception.Message;
            var innerException = exception.InnerException?.Message;
            var fullError = $"{errorMessage}{(string.IsNullOrEmpty(innerException) ? "" : $" | Inner: {innerException}")}";
            
            return Json(new 
            { 
                success = false, 
                message = $"Migration failed: {fullError}",
                error = fullError,
                errorType = exception.GetType().Name,
                stackTrace = exception.StackTrace
            }, JsonRequestBehavior.AllowGet);
        }
    }
    #endregion
}

