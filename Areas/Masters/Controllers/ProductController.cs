using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Corno.Web.Areas.Masters.Dtos.Product;
using Corno.Web.Controllers;
using Corno.Web.Globals;
using Corno.Web.Models.Masters;
using Corno.Web.Services.Interfaces;
using Corno.Web.Services.Masters.Interfaces;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Mapster;

namespace Corno.Web.Areas.Masters.Controllers;

public class ProductController : SuperController
{
    #region -- Constructors --
    public ProductController(IProductService productService,
        IMiscMasterService miscMasterService, IBaseItemService itemService,
        ICustomerService customerService)
    {
        _productService = productService;
        _miscMasterService = miscMasterService;
        _itemService = itemService;
        _customerService = customerService;

        const string viewPath = "~/Areas/Masters/views/Product/";
        _indexPath = $"{viewPath}/Index.cshtml";
        _createPath = $"{viewPath}/Create.cshtml";
        _editPath = $"{viewPath}/Edit.cshtml";
    }
    #endregion

    #region -- Data Mambers --
    private readonly IProductService _productService;
    private readonly IMiscMasterService _miscMasterService;
    private readonly IBaseItemService _itemService;
    private readonly ICustomerService _customerService;

    private readonly string _indexPath;
    private readonly string _createPath;
    private readonly string _editPath;
    #endregion

    #region -- Actions --

    public ActionResult Index()
    {
        return View(_indexPath, new List<ProductDto>());
    }

    // GET: /Location/Create
    public ActionResult Create()
    {
        return View(_createPath, new ProductDto());
    }

    //POST: /Product/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Create(ProductDto dto)
    {
        if (!ModelState.IsValid)
            return View(_createPath, dto);

        try
        {
            // Check whether Name Already Exists
            if (await _productService.ExistsAsync(dto.Code).ConfigureAwait(false))
                throw new Exception($"Product with code {dto.Code} already exists.");

            //var product = dto.CreateModel();
            var product = dto.Adapt<Product>();
            product.Status = StatusConstants.Active;
            await _productService.AddAndSaveAsync(product).ConfigureAwait(false);

            return RedirectToAction("Index");
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }

        return View(_createPath, dto);
    }

    public async Task<ActionResult> Edit(int? id)
    {
        var product = await _productService.GetByIdAsync(id).ConfigureAwait(false);

        var itemIds = product.ProductItemDetails.Select(d => d.ItemId).ToList();
        var items = await _itemService.GetViewModelListAsync(p => itemIds.Contains(p.Id)).ConfigureAwait(false);
        var packingTypeIds = product.ProductPacketDetails.Select(d => d.PackingTypeId).ToList();
        var packingTypes = await _miscMasterService.GetViewModelListAsync(p => packingTypeIds.Contains(p.Id)).ConfigureAwait(false);
        var customerIds = product.ProductStockDetails.Select(d => d.CustomerId).ToList();
        var customers =  await _customerService.GetViewModelListAsync(p => customerIds.Contains(p.Id)).ConfigureAwait(false);

        var productDto = product.Adapt<ProductDto>();
        productDto.ProductItemDtos.ForEach(d => d.ItemName = items.FirstOrDefault(x => x.Id == d.ItemId)?.Name);
        productDto.ProductPacketDtos.ForEach(d => d.PackingTypeName = packingTypes.FirstOrDefault(x => x.Id == d.PackingTypeId)?.Name);
        productDto.ProductStockDtos.ForEach(d => d.CustomerName = customers.FirstOrDefault(x => x.Id == d.CustomerId)?.Name);
        return View(_editPath, productDto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Edit(ProductDto dto)
    {
        if (!ModelState.IsValid)
        {
            return View(_editPath, dto);
        }
        try
        {
            var product = await _productService.GetByIdAsync(dto.Id).ConfigureAwait(false);
            if (product == null) 
                throw new Exception("Product not found.");

            dto.Adapt(product);
            product.Status = StatusConstants.Active;
            await _productService.UpdateAndSaveAsync(product).ConfigureAwait(false);

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
        var product = await _productService.GetByIdAsync(id ?? 0).ConfigureAwait(false);

        var productViewModel = new ProductDto
        {
            Code = product.Code,
            Name = product.Name,
            Description = product.Description
        };

        return View(productViewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Delete(ProductDto dto)
    {
        if (!ModelState.IsValid)
        {
            return View(dto);
        }
        try
        {
            var product = await _productService.GetByIdAsync(dto.Id).ConfigureAwait(false);
            product.Status = StatusConstants.Deleted;
            await _productService.UpdateAndSaveAsync(product).ConfigureAwait(false);

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

    public async Task<ActionResult> GetIndexViewModels([DataSourceRequest] DataSourceRequest request)
    {
        try
        {
            var query = (IEnumerable<Product>)_productService.GetQuery();
        var data = query.Select(p => new ProductIndexDto
        {
            Id = p.Id,
            Code = p.Code,
            Name = p.Name,
            Status = p.Status
        });
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

    [AcceptVerbs(HttpVerbs.Post)]
    public async Task<ActionResult> Inline_Create_Update_Destroy_Item([DataSourceRequest] DataSourceRequest request, ProductItemDto model)
    {
        var item = await _itemService.GetViewModelAsync(model.ItemId).ConfigureAwait(false);
        model.ItemName = item?.Name;

        return Json(new[] { model }.ToDataSourceResultAsync(request, ModelState));
    }
    
    [AcceptVerbs(HttpVerbs.Post)]
    public async Task<ActionResult> Inline_Create_Update_Destroy_Packet([DataSourceRequest] DataSourceRequest request, ProductPacketDetail dto)
    {
        var packingType  = await _miscMasterService.GetViewModelAsync(dto.PackingTypeId).ConfigureAwait(false);
        dto.PackingTypeName = packingType?.Name;

        return Json(new[] { dto }.ToDataSourceResultAsync(request, ModelState));
    }

    [AcceptVerbs(HttpVerbs.Post)]
    public async Task<ActionResult> Inline_Create_Update_Destroy_Stock([DataSourceRequest] DataSourceRequest request, ProductStockDto dto)
    {
        return Json(new[] { dto }.ToDataSourceResult(request, ModelState));
    }

    #endregion
}