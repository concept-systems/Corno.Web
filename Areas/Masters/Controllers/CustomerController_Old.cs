using System;
using System.Linq;
using System.Web.Mvc;
using Corno.Concept.Modules.Masters.Models;
using Corno.Concept.Portal.Controllers;
using Corno.Services.Base.Interfaces;

namespace Corno.Concept.Portal.Areas.Masters.Controllers
{
    public class CustomerController_Old : MasterController<Customer>
    {
        #region -- Constructors --
        public CustomerController_Old(IMasterService<Customer> customerService)
        : base(null, null)
        {
            _customerService = customerService;

        }
        #endregion

        #region -- Data Mambers --
        private readonly IMasterService<Customer> _customerService;

        #endregion

        #region -- Actions --


        // GET: /Product/
        public ActionResult Index(int? page)
        {
            try
            {
                // For Admin users
                // if (User.IsInRole(RoleNames.Admin))
                var models = _customerService.Get().ToList();
                return View(models);
            }
            catch (Exception exception)
            {
                HandleControllerException(exception);
            }
            return View();
        }


        // GET: /Product/Create
        public ActionResult Create()
        {
            try
            {
                return View(new Customer());
            }
            catch (Exception exception)
            {
                HandleControllerException(exception);
            }
            return View();
        }


        // POST: /Product/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Customer model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                // Check whether Name Already Exists
                var nameExisting = _customerService.Get(t => t.Name == model.Name).FirstOrDefault();
                if (null != nameExisting)
                    throw new Exception("Customer Already Exists");
                _customerService.Add(model);
                _customerService.Save();

                return RedirectToAction("Index");
            }
            catch (Exception exception)
            {
                HandleControllerException(exception);
            }

            return View(model);
        }

        // GET: /Product/Edit/5
        public ActionResult Edit(int? id)
        {
            try
            {
                var model = _customerService.GetById(id);
                return View(model);
            }
            catch (Exception exception)
            {
                HandleControllerException(exception);
            }

            return View();
        }

        // POST: /Product/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Customer model)
        {
            if (ModelState.IsValid)
            {
                try
                {

                    // model.ModifiedBy = User.Identity.GetUserId();
                    model.ModifiedDate = DateTime.Now;

                    _customerService.Update(model);
                    _customerService.Save();

                    return RedirectToAction("Index");
                }
                catch (Exception exception)
                {
                    HandleControllerException(exception);
                }
            }

            return View(model);
        }
        // GET: /Product/Delete/5
        public ActionResult Delete(int? id)
        {
            try
            {
                return View(_customerService.GetById(id));
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
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                var model = _customerService.GetById(id);

                //model.DeletedBy = User.Identity.GetUserId();

                model.DeletedDate = DateTime.Now;

                //model.Status = StatusConstants.Cancelled;

                _customerService.Delete(model);
                _customerService.Save();
            }
            catch (Exception exception)
            {
                HandleControllerException(exception);
            }

            return RedirectToAction("Index");
        }

        #endregion
    }
}