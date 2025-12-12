using Corno.Web.Areas.Masters.ViewModels;
using Corno.Web.Areas.Masters.ViewModels.MiscMaster;
using Corno.Web.Controllers;
using Corno.Web.Globals;
using Corno.Web.Models.Masters;
using Corno.Web.Services.Interfaces;
using Corno.Web.Services.Progress.Interfaces;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Corno.Web.Areas.Masters.Controllers;

public class MiscMasterController : SuperController
{
    #region -- Constructors --
    public MiscMasterController(IMiscMasterService miscMasterService,
        IWebProgressService progressService)
    {
        _miscMasterService = miscMasterService;
        _progressService = progressService;
    }
    #endregion

    #region -- Data Members --
    private readonly IMiscMasterService _miscMasterService;
    private readonly IWebProgressService _progressService;
    #endregion

    #region -- Actions --
    public async Task<ActionResult> Index(string miscType)
    {
        ViewBag.MiscType = miscType;
        var data = await _miscMasterService.GetViewModelListAsync(miscType).ConfigureAwait(false);
        return View(data);

        /*if (string.IsNullOrEmpty(miscType))
            throw new Exception("Invalid Misc Type.");

        ViewBag.MiscType = miscType;
        return View(_miscMasterService.GetViewModelList(miscType));*/
    }

    // GET: /Location/Create
    public ActionResult Create(string miscType)
    {
        try
        {
            return View(new MiscMasterDto
            {
                MiscType = miscType,
            });
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }
        return View();
    }

    //POST: /Product/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Create(MiscMasterDto viewModel)
    {
        if (!ModelState.IsValid)
            return View(viewModel);

        try
        {
            // Check whether Name Already Exists
            if (await _miscMasterService.ExistsAsync(viewModel.Code).ConfigureAwait(false))
                throw new Exception($"Misc Master with code {viewModel.Code} already exists.");

            await AddAsync(viewModel).ConfigureAwait(false);

            return RedirectToAction("Index", new { miscType = viewModel.MiscType });
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }

        return View(viewModel);
    }

    public async Task<ActionResult> Edit(int? id)
    {
        var miscMaster = await _miscMasterService.GetByIdAsync(id ?? 0).ConfigureAwait(false);

        var miscMasterDto = new MiscMasterDto
        {
            Id = miscMaster.Id,
            Code = miscMaster.Code,
            Name = miscMaster.Name,
            Description = miscMaster.Description,

            MiscType = miscMaster.MiscType,

            // General Tab
            Weight = miscMaster.Weight,
            Tolerance = miscMaster.Tolerance
        };

        return View(miscMasterDto);

    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Edit(MiscMasterDto model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }
        try
        {
            if (model.MiscType == null)
                throw new Exception("Invalid Misc Type.");

            await UpdateAsync(model).ConfigureAwait(false);

            return RedirectToAction("Index", new { miscType = model.MiscType });
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }

        return View(model);
    }

    // GET: /Product/Delete/5
    public async Task<ActionResult> Delete(int? id)
    {
        try
        {
            return await Task.FromResult(View(new MiscMasterDto()));
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }

        return View();
    }

    // POST: /Product/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> DeleteConfirmed(int id)
    {
        try
        {
            var model = await _miscMasterService.GetByIdAsync(id).ConfigureAwait(false);

            //model.DeletedBy = User.Identity.GetUserId();

            model.DeletedDate = DateTime.Now;

            //model.Status = StatusConstants.Cancelled;

            model.Status = StatusConstants.Deleted;

            await _miscMasterService.DeleteAsync(model).ConfigureAwait(false);
            await _miscMasterService.SaveAsync().ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }

        return RedirectToAction("Index");
    }
    #endregion

    #region -- Private Methods --
    private async Task AddAsync(MiscMasterDto viewModel)
    {
        var miscMaster = new MiscMaster
        {
            Code = viewModel.Code,
            Name = viewModel.Name,
            Description = viewModel.Description,

            MiscType = viewModel.MiscType,

            // General Tab
            Weight = viewModel.Weight,
            Tolerance = viewModel.Tolerance,

            Status = StatusConstants.Active,
        };

        await _miscMasterService.AddAndSaveAsync(miscMaster).ConfigureAwait(false);
    }

    private async Task UpdateAsync(MiscMasterDto viewModel)
    {
        var miscMaster = await _miscMasterService.GetByCodeAsync(viewModel.Code).ConfigureAwait(false);

        if (miscMaster == null)
            throw new Exception($"Misc Master not found for code '{viewModel.Code}' to update.");

        miscMaster.Name = viewModel.Name;
        miscMaster.Description = viewModel.Description;
        miscMaster.MiscType = viewModel.MiscType;

        miscMaster.Weight = viewModel.Weight;
        miscMaster.Tolerance = viewModel.Tolerance;

        await _miscMasterService.UpdateAndSaveAsync(miscMaster).ConfigureAwait(false);
    }

    private async Task Update(MiscMasterDto viewModel)
    {
        var miscMaster = await _miscMasterService.GetByCodeAsync(viewModel.Code).ConfigureAwait(false);

        if (miscMaster == null)
            throw new Exception($"Misc Master not found for code '{viewModel.Code}' to update.");

        // Update the properties of the existing product
        miscMaster.Name = viewModel.Name;
        miscMaster.Description = viewModel.Description;

        miscMaster.MiscType = viewModel.MiscType;

        // General Tab
        miscMaster.Weight = viewModel.Weight;
        miscMaster.Tolerance = viewModel.Tolerance;

        await _miscMasterService.UpdateAndSaveAsync(miscMaster);
    }
    #endregion

    #region -- Methods --   
    /*protected override RouteValueDictionary GetRouteValues(MiscMaster model)
    {
        return new RouteValueDictionary { { "miscType", model.MiscType } };
    }*/

    /*protected override ActionResult IndexGet(int? pageNo, string miscType)
    {
        if (string.IsNullOrEmpty(miscType))
            throw new Exception("Invalid Misc Type.");

        ViewBag.MiscType = miscType;
        return View(_miscMasterService.GetViewModelList(miscType));
    }

    protected override MiscMaster CreateGet(string miscType)
    {
        if (miscType == null)
            throw new Exception("Invalid Misc Type.");

        return new MiscMaster { MiscType = miscType };
    }

    protected override void CreatePost(MiscMaster model)
    {
        if (model.MiscType == null)
            throw new Exception("Invalid Misc Type.");

        // Check whether Name Already Exists
        if (_miscMasterService.Exists(model.Code))
            throw new Exception($"{model.MiscType} {model.Code} already exists.");
    }

    protected override MiscMaster EditPost(MiscMaster model)
    {
        if (model.MiscType == null)
            throw new Exception("Invalid Misc Type.");
        return model;
    }*/

    #endregion

    #region -- Actions --

    public virtual async Task<ActionResult> ImportMiscMaster(IEnumerable<HttpPostedFileBase> files)
    {
        ActionResult jsonResult = Json(new { error = false }, JsonRequestBehavior.AllowGet);
        try
        {
            var httpPostedFileBases = files.ToList();
            if (httpPostedFileBases.FirstOrDefault() == null)
                throw new Exception("No file selected for import");

            var fileBase = httpPostedFileBases.FirstOrDefault();
            // Save file
            var filePath = Server.MapPath("~/Upload/" + fileBase?.FileName);
            fileBase?.SaveAs(filePath);

            // Import file
            _progressService.SetWebProgress();
            await _miscMasterService.ImportAsync(filePath, _progressService).ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            jsonResult = Json(new
            {
                error = true,
                message = exception.Message
            }, JsonRequestBehavior.AllowGet);
        }
        return jsonResult;
    }

    /*protected override MiscMaster EditGet(int? id)
    {
        var product = _miscMasterService.GetById(id ?? 0);

        return product;
    }
        
    //POST : / MiscMaster/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public override ActionResult Edit(MiscMaster model)
    {
        try
        {
            if (!ModelState.IsValid)
                return View(model);

            if (model.MiscType == null)
                throw new Exception("Invalid Misc Type.");

            _miscMasterService.Update(model);
            _miscMasterService.Save();
            return RedirectToAction("Index", new { miscType = model.MiscType });
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }
        return View(model);
    }*/

    public async Task<ActionResult> GetIndexViewModels([DataSourceRequest] DataSourceRequest request, string miscType)
    {
        try
        {
            var query = (IQueryable<MiscMaster>)(await _miscMasterService.GetAsync<MiscMaster>(p => p.MiscType == miscType, p => p).ConfigureAwait(false)).AsQueryable();
            var data = from miscMaster in query
                       select new MiscMasterIndexModel
                       {
                           Id = miscMaster.Id,
                           Code = miscMaster.Code,
                           Name = miscMaster.Name,
                           Description = miscMaster.Description,
                           Status = miscMaster.Status
                       };
            var result = await data.ToDataSourceResultAsync(request).ConfigureAwait(false);

            return Json(result, JsonRequestBehavior.AllowGet);


        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
            ModelState.AddModelError("Error", exception.Message);
            return Json(new DataSourceResult { Errors = ModelState });
        }
    }

    #endregion
}