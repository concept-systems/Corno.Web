using System;
using System.Web.Mvc;
using Corno.Concept.Modules.Masters.Services.Interfaces;
using Corno.Concept.Portal.Areas.Masters.Models;
using Corno.Concept.Portal.Controllers;
using Corno.Services.Progress.Interfaces;

namespace Corno.Concept.Portal.Areas.Masters.Controllers;

public class HsnController : MasterController<Hsn>
{
    #region -- Constructors --
    public HsnController(IHsnService hsnService, IStateService stateService, ICustomerService customerService, IBaseItemService itemService, IProductService productService,
        IWebProgressService progressService)
        : base(hsnService, progressService)
    {
        _productService = productService;
        _itemService = itemService;
        _customerService = customerService;
        _stateService = stateService;
        _hsnService = hsnService;
    }
    #endregion
    #region -- Data Mambers --
    private readonly IProductService _productService;
    private readonly IBaseItemService _itemService;
    private readonly ICustomerService _customerService;
    private readonly IStateService _stateService;
    private readonly IHsnService _hsnService;

    #endregion
    #region -- Action Methods --
    protected ActionResult Index()
    {
        return View();
    }

    protected override ActionResult IndexGet(int? pageNo, string type)
    {
        var viewModels = _hsnService.Get(null, h => new HsnViewModel
        {
            SerialNo = h.SerialNo, Id = h.Id, Code = h.Code, Name = h.Name,
            Cgst = h.Cgst, Sgst = h.Sgst, Igst = h.Igst
        });
        return View(viewModels);
    }

    protected override Hsn EditPost(Hsn model)
    {
        var existing = _hsnService.GetById(model.Id);
        if (null == existing)
            throw new Exception("Something went wrong State controller.");

        model.Id = existing.Id;
        model.CopyPropertiesTo(existing);

        return existing;
    }

    #endregion
}