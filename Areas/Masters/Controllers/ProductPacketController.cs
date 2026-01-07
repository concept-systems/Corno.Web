using System;
using System.Web.Mvc;
using Corno.Web.Controllers;
using Corno.Web.Models.Masters;
using Corno.Web.Services.Masters.Interfaces;

namespace Corno.Web.Areas.Masters.Controllers
{
    [Authorize]
    public sealed class ProductPacketController : MasterController<Product>
    {
        #region -- Constructors --
        public ProductPacketController(IProductService productService) : base(productService)
        {
        }
        #endregion

        #region -- Data Members --

        #endregion

        
        #region -- Actions --
        // GET: /Product/Create
        public ActionResult Edit()
        {
            try
            {
                return View(new Product());
            }
            catch (Exception exception)
            {
                HandleControllerException(exception);
            }
            return View();
        }
        #endregion
    }
}
