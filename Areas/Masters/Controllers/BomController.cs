using Corno.Concept.Modules.Masters.Models;
using Corno.Concept.Portal.Controllers;
using Corno.Services.Progress.Interfaces;


namespace Corno.Concept.Portal.Areas.Masters.Controllers
{
    public class BomController : MasterController <Item>
    {
        #region -- Constructors --
        public BomController(IBomService bomService,
            IWebProgressService progressService) 
            : base(bomService, progressService)
        {
            _BomService = bomService;
        }
        #endregion

        #region -- Data Mambers --
        private readonly IBomService _BomService;
        #endregion

        #region -- Actions --


        // GET: /Product/
        //[HttpPost]
        //public ActionResult Index(int? page)
        //{
        //    try
        //    {
        //        // For Admin users
        //        // if (User.IsInRole(RoleNames.Admin))
        //        return View(_itemService.GetList());
        //    }
        //    catch (Exception exception)
        //    {
        //        HandleControllerException(exception);
        //    }
        //    return View();
        //}


        //// GET: /Product/Create
        //[HttpPost]
        //public ActionResult Create()
        //{
        //    try
        //    {
        //        return View(new Item());
        //    }
        //    catch (Exception exception)
        //    {
        //        HandleControllerException(exception);
        //    }
        //    return View();
        //}


        //// POST: /Product/Create
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create(Item model)
        //{
        //    if (!ModelState.IsValid)
        //        return View(model);

        //    try
        //    {
        //        // Check whether Name Already Exists
        //        var nameExisting = _itemService.Get(t => t.Name == model.Name).FirstOrDefault();
        //        if (null != nameExisting)
        //            throw new Exception("Product Already Exists");
        //        _itemService.Add(model);
        //        _itemService.Save();

        //        return RedirectToAction("Index");
        //    }
        //    catch (Exception exception)
        //    {
        //        HandleControllerException(exception);
        //    }

        //    return View(model);
        //}
     

        //// GET: /Product/Edit/5
        //public ActionResult Edit(int? id)
        //{
        //    try
        //    {
        //        var model = _itemService.GetById(id);
        //        return View(model);
        //    }
        //    catch (Exception exception)
        //    {
        //        HandleControllerException(exception);
        //    }

        //    return View();
        //}

        //// POST: /Product/Edit/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit(Item model)
        //{
        //    try
        //    {
        //        if (!ModelState.IsValid)
        //            return View(model);

        //        // model.ModifiedBy = User.Identity.GetUserId();
        //        model.ModifiedDate = DateTime.Now;

        //        _itemService.Update(model);
        //        _itemService.Save();

        //        return RedirectToAction("Index");
        //    }
        //    catch (Exception exception)
        //    {
        //        HandleControllerException(exception);
        //    }

        //    return View(model);
        //}

        //// GET: /Product/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    try
        //    {
        //        return View(_itemService.GetById(id));
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
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    try
        //    {
        //        var model = _itemService.GetById(id);

        //        //model.DeletedBy = User.Identity.GetUserId();

        //        model.DeletedDate = DateTime.Now;

        //        //model.Status = StatusConstants.Cancelled;

        //        _itemService.Delete(model);
        //        _itemService.Save();
        //    }
        //    catch (Exception exception)
        //    {
        //        HandleControllerException(exception);
        //    }

        //    return RedirectToAction("Index");
        //}

        #endregion


    }
}