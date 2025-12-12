using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Corno.Web.Areas.Admin.Dto;
using Corno.Web.Areas.Admin.Services.Interfaces;
using Corno.Web.Controllers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Mapster;

namespace Corno.Web.Areas.Admin.Controllers;

public class RoleController : SuperController
{
    #region -- Constructors --
    public RoleController(IRoleService roleService)
    {
        _roleService = roleService;

        const string viewPath = "~/Areas/admin/views/Role/";
        _indexPath = $"{viewPath}/Index.cshtml";
        _createPath = $"{viewPath}/Create.cshtml";
        _editPath = $"{viewPath}/Edit.cshtml";
        _setSitemapPath = $"{viewPath}/SetSitemap.cshtml";
    }
    #endregion

    #region -- Data Members --

    private readonly IRoleService _roleService;

    private readonly string _indexPath;
    private readonly string _createPath;
    private readonly string _editPath;
    private readonly string _setSitemapPath;
    #endregion

    #region -- Actions --
    [Authorize]
    public ActionResult Index(int? page)
    {
        return View(_indexPath, null);
    }

    public ActionResult Create()
    {
        return View(_createPath, new RoleDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Create(RoleDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
                return View(_createPath, dto);

            // Create Role
            await _roleService.CreateAsync(dto).ConfigureAwait(false);

            return RedirectToAction("Index");
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }
        return View(_createPath, dto);
    }

    public async Task<ActionResult> Edit(string id)
    {
        if (null == id)
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

        // Get AspNetRole
        var role = await _roleService.GetByIdAsync(id);

        var viewModel = new RoleDto();
        role.Adapt(viewModel);
         
        return View(_editPath, viewModel);
    }

    // POST: /AspNetRole/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Edit(RoleDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
                return View(_editPath, dto);

            // Edit role
            await _roleService.EditAsync(dto).ConfigureAwait(false);

            return RedirectToAction("Index");
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }
        return View(_editPath, dto);
    }

    public async Task<ActionResult> Delete(int? id)
    {
        var role = await _roleService.GetByIdAsync(id ?? 0).ConfigureAwait(false);

        var dto = new RoleDto
        {
            Name = role.Name,
        };

        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Delete(RoleDto dto)
    {
        if (!ModelState.IsValid)
        {
            return View(dto);
        }
        try
        {
            var role = await _roleService.GetByIdAsync(dto.Id).ConfigureAwait(false);
            //role.Status = StatusConstants.Deleted;
            await _roleService.UpdateAndSaveAsync(role).ConfigureAwait(false);

            return RedirectToAction("Index");
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }

        return View(dto);
    }

    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public ActionResult DeleteConfirmed(int id)
    {
        try
        {
            /* var location = _locationService.GetById(id);
             location.Status = StatusConstants.Deleted;
             _locationService.UpdateAndSave(location);*/

            return RedirectToAction("Index");
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }
        return View();
    }

    public async Task<ActionResult> SetRole(string id)
    {
        if (null == id)
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

        // Get AspNetRole
        var role = await _roleService.GetByIdAsync(id).ConfigureAwait(false);

        var viewModel = new RoleDto();
        role.Adapt(viewModel);

        return View(_setSitemapPath, viewModel);
    }

    public ActionResult GetIndexModels([DataSourceRequest] DataSourceRequest request)
    {
        var query = _roleService.GetQuery();
        var result = query.ToDataSourceResult(request);
        return Json(result, JsonRequestBehavior.AllowGet);
    }

    #endregion
}