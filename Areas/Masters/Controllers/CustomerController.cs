using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Corno.Web.Areas.Masters.ViewModels;
using Corno.Web.Areas.Masters.ViewModels.Customer;
using Corno.Web.Controllers;
using Corno.Web.Globals;
using Corno.Web.Models.Masters;
using Corno.Web.Services.Interfaces;
using Corno.Web.Services.Masters.Interfaces;
using Corno.Web.Services.Progress.Interfaces;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace Corno.Web.Areas.Masters.Controllers;

public class CustomerController : SuperController
{
    #region -- Constructors --
    public CustomerController(ICustomerService customerService, IBaseItemService itemService, IProductService productService,
        IWebProgressService progressService, IMiscMasterService miscMasterService)
    {
        _productService = productService;
        _miscMasterService = miscMasterService;
        _itemService = itemService;
        _customerService = customerService;
    }
    #endregion

    #region -- Data Mambers --
    private readonly IProductService _productService;
    private readonly IMiscMasterService _miscMasterService;
    private readonly IBaseItemService _itemService;
    private readonly ICustomerService _customerService;

    #endregion

    #region -- Action Methods --

    public async Task<ActionResult> Index()
    {
        return await Task.FromResult(View(new List<CustomerViewModel>())).ConfigureAwait(false);
    }

    // GET: /Location/Create
    public async Task<ActionResult> Create()
    {
        try
        {
            return await Task.FromResult(View(new CustomerViewModel())).ConfigureAwait(false);
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
    public async Task<ActionResult> Create(CustomerViewModel viewModel)
    {
        if (!ModelState.IsValid)
            return View(viewModel);

        try
        {
            // Check whether Name Already Exists
            if (await _customerService.ExistsAsync(viewModel.Code).ConfigureAwait(false))
                throw new Exception($"Customer with code {viewModel.Code} already exists.");

            await AddAsync(viewModel).ConfigureAwait(false);

            return RedirectToAction("Index");
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }

        return View(viewModel);
    }

    public async Task<ActionResult> Edit(int? id)
    {
        var customer = await _customerService.GetByIdAsync(id ?? 0).ConfigureAwait(false);

        var customerViewModel = new CustomerViewModel
        {
            Code = customer.Code,
            Name = customer.Name,
            Description = customer.Description,

            // General tab
            GSTIN = customer.GSTIN,
            SupplierCode = customer.SupplierCode,
            CreditDays = customer.CreditDays ?? 0,
            CreditLimit = customer.CreditLimit ?? 0,

            // Contact Info 
            Address = customer.Address,
            ContactPerson = customer.ContactPerson,
            Phone = customer.Phone,
            Mobile = customer.Mobile,
            Fax = customer.Fax,
            Email = customer.Email,
            Website = customer.Website
        };

        foreach (var detail in customer.CustomerProductDetails)
        {
            var productViewModel = _productService != null ? await _productService.GetViewModelAsync(detail.ProductId).ConfigureAwait(false)
    : null;
            var machineViewModel = _miscMasterService != null
                ? await _miscMasterService.GetViewModelAsync(detail.MachineId).ConfigureAwait(false)
                : null;
            customerViewModel.CustomerProductDetails.Add(new CustomerProductDetailViewModel
            {
                Id = detail.Id,
                ProductId = detail.ProductId,
                ProductName = productViewModel?.NameWithCode,
                MachineId = detail.MachineId,
                //MachineName = _machineService?.GetViewModel(detail.MachineId)?.NameWithCode,
                PrinterName = detail.PrinterName,
            });
        }

        return View(customerViewModel);

    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Edit(CustomerViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }
        try
        {
            await UpdateAsync(model).ConfigureAwait(false);

            return RedirectToAction("Index");
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }

        return View(model);
    }

    public async Task<ActionResult> Delete(int? id)
    {
        var customer = await _customerService.GetByIdAsync(id ?? 0).ConfigureAwait(false);

        var customerViewModel = new CustomerViewModel
        {
            Code = customer.Code,
            Name = customer.Name,
            Description = customer.Description
        };

        return View(customerViewModel);

    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Delete(CustomerViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }
        try
        {
            await UpdateAsync(model).ConfigureAwait(false);

            return RedirectToAction("Index");
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }

        return View(model);
    }

    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> DeleteConfirmed(int id)
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
        /*var query = _customerService.GetQuery();
        var result = query.ToDataSourceResult(request);
        return Json(result, JsonRequestBehavior.AllowGet);*/

        try
        {
            var query = (IEnumerable<Customer>)_customerService.GetQuery();
            var data = query.Select(p => new CustomerIndexModel
            {
                Id = p.Id,
                Code = p.Code,
                Name = p.Name,
                Status = p.Status
            });
            var result = data.ToDataSourceResult(request);
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
    public async Task<ActionResult> Inline_Create_Update_Destroy_Product([DataSourceRequest] DataSourceRequest request, CustomerProductDetail model)
    {
        var productViewModel = await _productService.GetViewModelAsync(model.ProductId).ConfigureAwait(false);
        var machineViewModel = await _miscMasterService.GetViewModelAsync(model.MachineId).ConfigureAwait(false);
        model.ProductName = productViewModel?.NameWithCode;
        model.MachineName = machineViewModel?.Name;

        return Json(new[] { model }.ToDataSourceResult(request, ModelState));
    }
    #endregion

    #region -- Private Methods --
    private async Task AddAsync(CustomerViewModel viewModel)
    {
        var customer = new Customer
        {
            Code = viewModel.Code,
            Name = viewModel.Name,
            Description = viewModel.Description,

            // General tab
            GSTIN = viewModel.GSTIN,
            SupplierCode = viewModel.SupplierCode,
            CreditDays = viewModel.CreditDays,
            CreditLimit = viewModel.CreditLimit,

            // Contact Info 
            Address = viewModel.Address,
            ContactPerson = viewModel.ContactPerson,
            Phone = viewModel.Phone,
            Mobile = viewModel.Mobile,
            Fax = viewModel.Fax,
            Email = viewModel.Email,
            Website = viewModel.Website,

            Status = StatusConstants.Active
        };

        foreach (var customerProductDetailViewModel in viewModel.CustomerProductDetails)
        {
            customer.CustomerProductDetails.Add(new CustomerProductDetail
            {
                ProductId = customerProductDetailViewModel.ProductId,
                MachineId = customerProductDetailViewModel.MachineId,
                PrinterName = customerProductDetailViewModel.PrinterName
            });
        }

        await _customerService.AddAndSaveAsync(customer).ConfigureAwait(false);
    }

    private async Task UpdateAsync(CustomerViewModel viewModel)
    {
        var customer = await _customerService.GetByCodeAsync(viewModel.Code).ConfigureAwait(false);

        if (customer == null)
        {
            throw new Exception("Customer not found.");
        }

        // Update the properties of the existing customer
        customer.Name = viewModel.Name;
        customer.Description = viewModel.Description;

        // General tab
        customer.GSTIN = viewModel.GSTIN;
        customer.SupplierCode = viewModel.SupplierCode;
        customer.CreditDays = viewModel.CreditDays;
        customer.CreditLimit = viewModel.CreditLimit;

        // Contact Info 
        customer.Address = viewModel.Address;
        customer.ContactPerson = viewModel.ContactPerson;
        customer.Phone = viewModel.Phone;
        customer.Mobile = viewModel.Mobile;
        customer.Fax = viewModel.Fax;
        customer.Email = viewModel.Email;
        customer.Website = viewModel.Website;

        customer.Status = StatusConstants.Active;

        //customer.CustomerProductDetails.Clear();
        foreach (var customerProductDetailViewModel in viewModel?.CustomerProductDetails)
        {
            var existing = customer.CustomerProductDetails.FirstOrDefault(d =>
                d.ProductId == customerProductDetailViewModel.ProductId);
            if (null != existing)
            {
                existing.ProductId = customerProductDetailViewModel.ProductId;
                existing.MachineId = customerProductDetailViewModel.MachineId;
                existing.PrinterName = customerProductDetailViewModel.PrinterName;
                continue;
            }

            customer.CustomerProductDetails.Add(new CustomerProductDetail
            {
                ProductId = customerProductDetailViewModel.ProductId,
                MachineId = customerProductDetailViewModel.MachineId,
                PrinterName = customerProductDetailViewModel.PrinterName,
            });
        }

        await _customerService.UpdateAndSaveAsync(customer).ConfigureAwait(false);
    }
    #endregion
}