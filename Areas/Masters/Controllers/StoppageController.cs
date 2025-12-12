using System;
using Corno.Concept.Modules.Masters.Models;
using Corno.Concept.Portal.Controllers;
using Corno.Services.Base.Interfaces;
using Corno.Services.Progress.Interfaces;

namespace Corno.Concept.Portal.Areas.Masters.Controllers;

public class StoppageController : MasterController<Supplier>
{
    #region -- Constructors --
    public StoppageController(IMasterService<Supplier> supplierService, 
        IWebProgressService progressService)
        : base(supplierService, progressService)
    {
        _supplierService = supplierService;
    }
    #endregion

    #region -- Data Mambers --
    private readonly IMasterService<Supplier> _supplierService;
    #endregion

    #region -- Actions --
    // GET: /Product/
    //public override ActionResult Index(int? page)
    //{
    //    try
    //    {
    //        // For Admin users
    //        // if (User.IsInRole(RoleNames.Admin))
    //        //var models = _supplierService.Get().ToList();
    //        return View(_supplierService.GetList());
    //    }
    //    catch (Exception exception)
    //    {
    //        HandleControllerException(exception);
    //    }
    //    return View();
    //}


    // GET: /Product/Create
    //public override ActionResult Create()
    //{
    //    try
    //    {
    //        return View(new Supplier());
    //    }
    //    catch (Exception exception)
    //    {
    //        HandleControllerException(exception);
    //    }
    //    return View();
    //}

    // POST: /Product/Create
    //[HttpPost]
    //[ValidateAntiForgeryToken]
    //public ActionResult Create(Supplier model)
    //{
    //    if (!ModelState.IsValid)
    //        return View(model);

    //    try
    //    {
    //        // Check whether Name Already Exists
    //        if (_supplierService.Exists(model.Code))
    //            throw new Exception($"Supplier with code {model.Code} already exists.");

    //        _supplierService.AddAndSave(model);

    //        return RedirectToAction("Index");
    //    }
    //    catch (Exception exception)
    //    {
    //        HandleControllerException(exception);
    //    }

    //    return View(model);
    //}

    // GET: /Product/Edit/5
    //public override ActionResult Edit(int? id)
    //{
    //    try
    //    {
    //        return View(_supplierService.GetById(id));
    //    }
    //    catch (Exception exception)
    //    {
    //        HandleControllerException(exception);
    //    }

    //    return View();
    //}

    protected override Supplier EditPost(Supplier model)
    {
        var existing = _supplierService.GetById(model.Id);
        if (null == existing)
            throw new Exception("Something went wrong Supplier controller.");

        model.Id = existing.Id;
        model.CopyPropertiesTo(existing);

        return existing;
    }

    // POST: /Product/Edit/5
    //[HttpPost]
    //[ValidateAntiForgeryToken]
    //public override ActionResult Edit(Supplier model)
    //{
    //    if (!ModelState.IsValid)
    //        return View(model);

    //    try
    //    {
    //        model.ModifiedDate = DateTime.Now;

    //        _supplierService.UpdateAndSave(model);

    //        return RedirectToAction("Index");
    //    }
    //    catch (Exception exception)
    //    {
    //        HandleControllerException(exception);
    //    }

    //    return View(model);
    //}
    // GET: /Product/Delete/5
    //public override ActionResult Delete(int? id)
    //{
    //    try
    //    {
    //        return View(_supplierService.GetById(id));
    //    }
    //    catch (Exception exception)
    //    {
    //        HandleControllerException(exception);
    //    }

    //    return View();
    //}

    //// POST: /Product/Delete/5
    //[HttpPost, ActionName("Delete")]
    //[ValidateAntiForgeryToken]
    //public override ActionResult DeleteConfirmed(int id)
    //{
    //    try
    //    {
    //        var model = _supplierService.GetById(id);

    //        //model.DeletedBy = User.Identity.GetUserId();

    //        model.DeletedDate = DateTime.Now;

    //        //model.Status = StatusConstants.Cancelled;

    //        _supplierService.Delete(model);
    //        _supplierService.Save();
    //    }
    //    catch (Exception exception)
    //    {
    //        HandleControllerException(exception);
    //    }

    //    return RedirectToAction("Index");
    //}

    #endregion
}