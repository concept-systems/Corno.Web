using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Corno.Web.Areas.Admin.Dto;
using Corno.Web.Areas.Admin.Models;
using Corno.Web.Areas.Admin.Services.Interfaces;
using Corno.Web.Controllers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace Corno.Web.Areas.Admin.Controllers;

public class UserController : SuperController
{
    #region -- Constructors --
    public UserController(IUserService userService, IRoleService roleService)
    {
        _userService = userService;
        _roleService = roleService;

        const string viewPath = "~/Areas/admin/views/User/";
        _indexPath = $"{viewPath}/Index.cshtml";
        _createPath = $"{viewPath}/Create.cshtml";
        _editPath = $"{viewPath}/Edit.cshtml";
        _changePasswordPath = $"{viewPath}/ChangePassword.cshtml";
    }
    #endregion

    #region -- Data Members --

    private readonly IUserService _userService;
    private readonly IRoleService _roleService;

    private readonly string _indexPath;
    private readonly string _createPath;
    private readonly string _editPath;
    private readonly string _changePasswordPath;

    #endregion

    #region -- Actions --
    [Authorize]
    public ActionResult Index(int? page)
    {
        return View(_indexPath, null);
    }

    public async Task<ActionResult> Create()
    {
        var dto = new UserCrudDto
        {
            Roles = (await _roleService.GetListAsync().ConfigureAwait(false))
                .Select(p => new UserRoleDto{IsSelected = false, RoleName = p.Name})
                .ToList()
        };

        return View(_createPath, dto);
    }

    [HttpPost] [ValidateAntiForgeryToken]
    public async Task<ActionResult> Create(UserCrudDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
                return View(_createPath, dto);

            //_userService.Roles
           /* var identityService = Bootstrapper.Get<IIdentityService>();
            _userRoles = identityService.GetUserRoles(User.Identity.GetUserId()).ToList();*/

            // Create user.
            await _userService.CreateAsync(dto).ConfigureAwait(false);
            
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

        var dto = await _userService.GetViewModelWithRolesAsync(id).ConfigureAwait(false);
        /*dto.Roles = _roleService.GetList()
            .Select(p => new UserRoleDto { IsSelected = false, RoleName = p.Name })
            .ToList();

        var identityService = Bootstrapper.Get<IIdentityService>();
        _userRoles = identityService.GetUserRoles(User.Identity.GetUserId()).ToList();
        for (var index = 0; index < _userRoles.Count; index++)
        {
            _userRoles[index] = _userRoles[index].Trim();
            _userRoles[index] = _userRoles[index].ToLower();
        }
        foreach (var role in dto.Roles.Where(role => _userRoles.Contains(role.RoleName)))
        {
            role.IsSelected = true;
        }*/

        return View(_editPath, dto);
    }

    // POST: /AspNetUser/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Edit(UserCrudDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
                return View(_editPath, dto);

            // Edit user
            await _userService.EditAsync(dto).ConfigureAwait(false);

            /*var userRoleService = Bootstrapper.Get<IUserRoleService>();
            var selectedRoles = dto.Roles.Where(r => r.IsSelected)
                .Select(r => r.RoleName)
                .ToList();
            userRoleService.AddRoles(dto.Id, selectedRoles);*/

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
        var user = await _userService.GetByIdAsync(id ?? 0).ConfigureAwait(false);

        var dto = new UserCrudDto
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            UserName = user.UserName,
        };

        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Delete(UserCrudDto dto)
    {
        if (!ModelState.IsValid)
        {
            return View(dto);
        }
        try
        {
            var user = await _userService.GetByIdAsync(dto.Id).ConfigureAwait(false);
            //user.Status = StatusConstants.Deleted;
            await _userService.UpdateAndSaveAsync(user).ConfigureAwait(false);

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

    public async Task<ActionResult> ChangePassword(string id)
    {
        if (null == id)
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

        var viewModel = await _userService.GetViewModelWithRolesAsync(id).ConfigureAwait(false);

        return View(_changePasswordPath, viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> ChangePassword(UserCrudDto dto)
    {
        try
        {
            // ChangePassword
            await _userService.ChangePasswordAsync(dto).ConfigureAwait(false);

            return RedirectToAction("Index");
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }
        return View(_changePasswordPath, dto);
    }

    public ActionResult GetIndexModels([DataSourceRequest] DataSourceRequest request)
    {
        var query = ((IEnumerable<AspNetUser>)_userService.GetQuery())
            .Select(p => new UserIndexDto()
            {
                Id = p.Id,
                UserName = p.UserName,
                FirstName = p.FirstName,
                LastName = p.LastName,
                Email = p.Email,
                PhoneNumber = p.PhoneNumber,
                Locked = p.LockoutEnabled
            });
        var result = query.ToDataSourceResult(request);
        return Json(result, JsonRequestBehavior.AllowGet);
    }

    [AcceptVerbs(HttpVerbs.Post)]
    public ActionResult Inline_Create_Update_Destroy([DataSourceRequest] DataSourceRequest request, UserCrudDto dto)
    {
        return Json(new[] { dto }.ToDataSourceResult(request, ModelState));
    }

    #endregion
}