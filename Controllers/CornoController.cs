using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Corno.Web.Attributes;
using Corno.Web.Controllers;
using Corno.Web.Dtos;
using Corno.Web.Globals;
using Corno.Web.Models.Base;
using Corno.Web.Services.Interfaces;
using Microsoft.AspNet.Identity;
using Telerik.Reporting;

namespace Corno.Web.Controllers;

[Authorize]
[Compress]
[SessionExpire]
public class CornoController<TEntity> : SuperController 
    where TEntity : class, new()
{
    #region -- Constructors --
    public CornoController(ICornoService<TEntity> cornoService)
    {
        _cornoService = cornoService;
    }
    #endregion

    #region -- Data Members --
    private readonly ICornoService<TEntity> _cornoService;
    #endregion

    #region -- Action Methods --
    protected virtual RouteValueDictionary GetRouteValues(TEntity model)
    {
        return null;
    }

    protected virtual async Task<ActionResult> IndexGetAsync(int? pageNo, string type)
    {
        throw new NotImplementedException("Perform Index method not implemented.");
    }

    protected virtual async Task<List<Report>> PrintGetAsync(int? id)
    {
        throw new NotImplementedException("Perform Index method not implemented.");
    }

    protected virtual async Task<TEntity> CreateGetAsync(string type)
    {
        return await Task.FromResult(new TEntity()).ConfigureAwait(false);
    }

    protected virtual void CreatePost(TEntity model)
    {
    }

    protected virtual async Task<TEntity> EditGetAsync(int? id)
    {
        return await _cornoService.GetByIdAsync(id).ConfigureAwait(false);
    }

    protected virtual async Task<TEntity> EditGetAsync(string id)
    {
        throw new NotImplementedException("Perform Edit Get method not implemented.");
    }

    protected virtual TEntity EditPost(TEntity model)
    {
        throw new NotImplementedException("EditPost not implemented");
    }

    protected virtual async Task<TEntity> DeleteGetAsync(string id)
    {
        throw new NotImplementedException("Perform Edit Get method not implemented.");
    }

    protected virtual void DeletePost(TEntity model)
    {
    }
    #endregion

    #region -- Common Updations --

    protected virtual void UpdateCommonCreateFields(TEntity model)
    {
    }

    protected virtual void UpdateCommonEditFields(TEntity model)
    {
    }

    protected virtual void UpdateCommonDeleteFields(TEntity model)
    {
    }
    #endregion

    //[AcceptVerbs(HttpVerbs.Post)]
    //public virtual ActionResult Inline_Create([DataSourceRequest] DataSourceRequest request, ProductItemDetail model)
    //{
    //    return Json(new[] { model }.ToDataSourceResult(request, ModelState));
    //}

    //[AcceptVerbs(HttpVerbs.Post)]
    //public virtual ActionResult Inline_Update([DataSourceRequest] DataSourceRequest request, BaseModel model)
    //{
    //    return Json(new[] { model }.ToDataSourceResult(request, ModelState));
    //}

    //[AcceptVerbs(HttpVerbs.Post)]
    //public virtual ActionResult Inline_Destroy([DataSourceRequest] DataSourceRequest request, BaseModel model)
    //{
    //    return Json(new[] { model }.ToDataSourceResult(request, ModelState));
    //}

    protected override void OnException(ExceptionContext filterContext)
    {
        if (filterContext.ExceptionHandled)
            return;

        var exception = filterContext.Exception;

        var result = View("~/Views/Error/Error.cshtml", new HandleErrorInfo(exception,
            filterContext.RouteData.Values["controller"].ToString(),
            filterContext.RouteData.Values["action"].ToString()));
        filterContext.Result = result;
        filterContext.ExceptionHandled = true;
    }

    public virtual async Task<ActionResult> Index(int? page, string miscType)
    {
        try
        {
            return await IndexGetAsync(page, miscType).ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }
        return View();
    }

    public virtual async Task<ActionResult> Print(int? id)
    {
        try
        {
            var reports = await PrintGetAsync(id).ConfigureAwait(false);

            Session[FieldConstants.Label] = reports.FirstOrDefault();

            //return PartialView("Partials/ReportViewer", reports.FirstOrDefault()?.GetType().AssemblyQualifiedName);
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }
        return View();
    }

    public virtual async Task<ActionResult> Create(string miscType)
    {
        try
        {
            return View(await CreateGetAsync(miscType).ConfigureAwait(false));
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public virtual async Task<ActionResult> Create(TEntity model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            _cornoService.ValidateModel(model);

            CreatePost(model);

            // Update Common Crate Fields
            UpdateCommonCreateFields(model);

            await _cornoService.AddAndSaveAsync(model).ConfigureAwait(false);

            return RedirectToAction("Index", GetRouteValues(model));
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }
        return View(model);
    }

    public virtual async Task<ActionResult> Create()
    {
        return await Task.FromResult(View()).ConfigureAwait(false);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public virtual async Task<ActionResult> Create(CornoDto dto)
    {
        return await Task.FromResult(View()).ConfigureAwait(false);
    }

    public virtual async Task<ActionResult> Edit(int? id)
    {
        try
        {
            return View(await EditGetAsync(id).ConfigureAwait(false));
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }
        return View();
    }

    public virtual async Task<ActionResult> EditString(string id)
    {
        try
        {
            return View("Edit", await EditGetAsync(id).ConfigureAwait(false));
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public virtual async Task<ActionResult> Edit(TEntity model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }
        try
        {
            var updatedModel = EditPost(model);

            // Update Common Crate Fields
            UpdateCommonEditFields(updatedModel);

            await _cornoService.UpdateAndSaveAsync(updatedModel).ConfigureAwait(false);

            return RedirectToAction("Index", GetRouteValues(updatedModel));
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }
        return View(model);
    }

    public virtual async Task<ActionResult> Delete(int? id)
    {
        try
        {
            return View(await _cornoService.GetByIdAsync(id).ConfigureAwait(false));
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }
        return View();
    }

    public virtual async Task<ActionResult> DeleteString(string id)
    {
        try
        {
            return View("Delete", await DeleteGetAsync(id).ConfigureAwait(false));
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }
        return View();
    }

    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public virtual async Task<ActionResult> DeleteConfirmed(int id)
    {
        var entity = await _cornoService.GetByIdAsync(id).ConfigureAwait(false);
        try
        {
            DeletePost(entity);

            // Deleting by Id. Because Delete by entity was not working
            //await _cornoService.DeleteAsync(id);

            // We will not delete the entity. Instead will be marked as deleted
            if (entity is BaseModel baseModel)
            {
                baseModel.Status = StatusConstants.Deleted;
                baseModel.DeletedBy = User.Identity.GetUserId();
                baseModel.DeletedDate = DateTime.Now;
                    
                await _cornoService.UpdateAsync(entity).ConfigureAwait(false);
            }

            await _cornoService.SaveAsync().ConfigureAwait(false);

            return RedirectToAction("Index", GetRouteValues(entity));
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }
        return View(entity);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public virtual async Task<ActionResult> DeleteConfirmedString(string id)
    {
        var val = await DeleteGetAsync(id).ConfigureAwait(false);
        try
        {
            await _cornoService.DeleteAsync(val).ConfigureAwait(false);
            await _cornoService.SaveAsync().ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }
        return RedirectToAction("Index", GetRouteValues(val));
    }

    [HttpPost]
    public virtual async Task<ActionResult> ImportMaster(IEnumerable<HttpPostedFileBase> files)
    {
        // Do Nothing
        ModelState.AddModelError("Error", @"Import Method not implemented");
        return await Task.FromResult(View()).ConfigureAwait(false);
    }

    [HttpPost]
    public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
    {
        var fileContents = Convert.FromBase64String(base64);

        return File(fileContents, contentType, fileName);
    }
}