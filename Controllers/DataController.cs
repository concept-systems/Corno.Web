using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Corno.Web.Areas.Admin.Services.Interfaces;
using Corno.Web.Globals;
using Corno.Web.Helper;
using Corno.Web.Models;
using Corno.Web.Models.Masters;
using Corno.Web.Services.Interfaces;
using Corno.Web.Services.Masters.Interfaces;
using Kendo.Mvc.UI;
using MoreLinq;
using Volo.Abp.Data;

namespace Corno.Web.Controllers;

public class DataController : SuperController
{
    #region -- Constructors --
    public DataController(IMiscMasterService miscMasterService, IMasterService<Product> productService, 
        IMasterService<Customer> customerService, 
        IMasterService<Supplier> supplierService, IBaseItemService itemService, IUserService userService)
    {
        _miscMasterService = miscMasterService;
        _productService = productService;
        _customerService = customerService;
        _supplierService = supplierService;
        _itemService = itemService;
        _userService = userService;
    }
    #endregion

    #region -- Data Members --
    private readonly IMiscMasterService _miscMasterService;
    private readonly IMasterService<Product> _productService;
    private readonly IMasterService<Customer> _customerService;
    private readonly IMasterService<Supplier> _supplierService;
    private readonly IBaseItemService _itemService;
    private readonly IUserService _userService;

    #endregion

    #region -- Actions --

    public async Task<JsonResult> GetMiscMasters(string miscType)
    {
        try
        {
            var list = await _miscMasterService.GetViewModelListAsync(m => m.MiscType == miscType);
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            return GetComboBoxErrorResult(exception);
        }
    }

    public async Task<JsonResult> GetUsers()
    {
        try
        {
            var users = await _userService.GetViewModelListAsync();
            return Json(users, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            return GetComboBoxErrorResult(exception);
        }
    }

    public async Task<JsonResult> GetProducts()
    {
        try
        {
            var list = await _productService.GetViewModelListAsync();
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            return GetComboBoxErrorResult(exception);
        }
    }

    public async Task<JsonResult> GetSuppliers()
    {
        try
        {
            var list = await _supplierService.GetViewModelListAsync();
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            return GetComboBoxErrorResult(exception);
        }
    }

    public async Task<JsonResult> GetCustomers()
    {
        try
        {
            var list = await _customerService.GetViewModelListAsync();
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            return GetComboBoxErrorResult(exception);
        }
    }

    public async Task<JsonResult> GetItems()
    {
        try
        {
            var list = await _itemService.GetViewModelListAsync();
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            return GetComboBoxErrorResult(exception);
        }
    }


    public virtual async Task<JsonResult> GetCustomerProducts(int? customerId)
    {
        if (customerId == null)
            return Json(null, JsonRequestBehavior.AllowGet);

        try
        {
            var customer = await _customerService.GetByIdAsync(customerId);
            // Optimize: Get product IDs in one query instead of loading full customer entity
            var productIds = customer.CustomerProductDetails.Select(x => x.ProductId).ToList();
            // Optimize: Single ToList() call, already optimized in GetViewModelList
            var products = await _productService.GetViewModelListAsync(p => productIds.Contains(p.Id));
            return Json(products, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            return GetComboBoxErrorResult(exception);
        }
    }

    public virtual async Task<JsonResult> GetProductPackingTypes(int? productId)
    {
        if (productId == null)
            return Json(null, JsonRequestBehavior.AllowGet);

        try
        {
            var product = await _productService.GetByIdAsync(productId);
            // Optimize: Get packing type IDs in one query
            var packingTypeIds = product.ProductPacketDetails.Select(x => x.PackingTypeId)
                .Distinct().ToList();
            // Optimize: Single ToList() call, already optimized in GetViewModelList
            var packingTypes = await _miscMasterService.GetViewModelListAsync(p => packingTypeIds.Contains(p.Id));
            var result = product.ProductPacketDetails.Select(d =>
            {
                var packingType = packingTypes.FirstOrDefault(p => p.Id == d.PackingTypeId);
                var mrp = d.GetProperty(FieldConstants.Mrp, 0);
                return new
                {
                    packingType?.Id,
                    packingType?.Name,
                    NameWithCode = $"{packingType?.NameWithCode} (MRP:{mrp}, Weight:{d.Quantity})",
                    Weight = d.Quantity,
                    MRP = mrp
                };
            });
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            return GetComboBoxErrorResult(exception);
        }
    }

    public async Task<JsonResult> ReadDimensionsHttp([DataSourceRequest] DataSourceRequest request)
    {
        try
        {
            var dimensionScale = await Task.Run(() => new Worker().ReadDimensionsHttp() ?? new DimensionScale { ActualLength = -1, ActualWidth = -1 }).ConfigureAwait(false);
            return Json(dimensionScale, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            return GetGridErrorResult(exception);
        }
    }

    public async Task<JsonResult> ReadDimensionsModbus([DataSourceRequest] DataSourceRequest request)
    {
        try
        {
            var dimensionScale = await Task.Run(() => new Worker().ReadDimensionsModbus() ?? new DimensionScale { ActualLength = -1, ActualWidth = -1 }).ConfigureAwait(false);
            return Json(dimensionScale, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            return GetGridErrorResult(exception);
        }
    }

    public async Task<JsonResult> GetCustomerData(int customerId)
    {
        if (customerId <= 0)
            return Json(null, JsonRequestBehavior.AllowGet);

        try
        {
            var customer = await _customerService.GetByIdAsync(customerId);
            return Json(new
            {
                address = customer?.Address,
            }, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            return GetComboBoxErrorResult(exception);
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            _miscMasterService.Dispose(true);

        base.Dispose(disposing);
    }
    #endregion
}