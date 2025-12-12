using System.Web.Mvc;
using System.Windows.Forms;
using Corno.Concept.Modules.Masters.Services.Interfaces;
using Corno.Concept.Portal.Controllers;
using Corno.Services.Progress.Interfaces;

namespace Corno.Concept.Portal.Areas.Masters.Controllers;

public class StateController : MasterController<AxHost.State>
{
    #region -- Constructors --
    public StateController(ICustomerService customerService, IBaseItemService itemService, IProductService productService,
        IWebProgressService progressService)
        : base(null,progressService)
    {
        _productService = productService;
        _itemService = itemService;
        _customerService = customerService;
    }
    #endregion
    #region -- Data Mambers --
    private readonly IProductService _productService;
    private readonly IBaseItemService _itemService;
    private readonly ICustomerService _customerService;

    #endregion
    #region -- Action Methods --
    protected ActionResult Index()
    {
        return View();
    }
    //protected override Customer EditGet(int? id)
    //{
    //    var customer = _customerService.GetById(id ?? 0);
    //    customer.CustomerCategoryId.ForEach(d =>
    //    {
    //        d.customer = _customerService.GetViewModel(d.customer ?? 0);
    //        //d.PackingType = _miscMasterService.GetViewModel(d.PackingTypeId ?? 0);
    //        d.Item = _itemService.GetViewModel(d.ItemId ?? 0);
    //    });
    //    //product.ProductStockDetails.ForEach(d =>
    //    //{
    //    //    d.Customer = _customerService.GetViewModel(d.CustomerId ?? 0);
    //    //});
    //    //product.ProductPacketDetails.ForEach(d =>
    //    //{
    //    //    d.PackingType = _miscMasterService.GetViewModel(d.PackingTypeId ?? 0);
    //    //});
    //    return customer;
    //}

   /* protected override AxHost.State EditPost(AxHost.State model)
    {
        var existing = _stateService.GetById(model.Id);
        if (null == existing)
            throw new Exception("Something went wrong State controller.");

        model.Id = existing.Id;
        model.CopyPropertiesTo(existing);

        return existing;
    }*/

    #endregion
}