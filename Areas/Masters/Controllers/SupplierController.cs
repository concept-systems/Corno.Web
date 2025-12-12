using Corno.Web.Areas.Masters.Dtos.Item;
using Corno.Web.Areas.Masters.ViewModels.Supplier;
using Corno.Web.Controllers;
using Corno.Web.Globals;
using Corno.Web.Models.Masters;
using Corno.Web.Services.Interfaces;
using Corno.Web.Services.Masters.Interfaces;
using Corno.Web.Services.Progress.Interfaces;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Corno.Web.Areas.Masters.Controllers;

public class SupplierController : SuperController
{
    #region -- Constructors --
    public SupplierController(IMasterService<Supplier> supplierService, 
        IWebProgressService progressService, IBaseItemService itemService)
    {
        _supplierService = supplierService;
        _itemService = itemService;
    }
    #endregion

    #region -- Data Mambers --
    private readonly IMasterService<Supplier> _supplierService;
    private readonly IBaseItemService _itemService;

    #endregion

    #region -- Actions --
    public async Task<ActionResult> Index()
    {
        return await Task.FromResult(View(new List<SupplierViewModel>()));
    }

    // GET: /Location/Create
    public ActionResult Create()
    {
        try
        {
            return View(new SupplierViewModel());
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
    public async Task<ActionResult> Create(SupplierViewModel viewModel)
    {
        if (!ModelState.IsValid)
            return View(viewModel);

        try
        {
            // Check whether Name Already Exists
            if (await _supplierService.ExistsAsync(viewModel.Code).ConfigureAwait(false))
                throw new Exception($"Supplier with code {viewModel.Code} already exists.");

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
        var supplier = await _supplierService.GetByIdAsync(id ?? 0).ConfigureAwait(false);

        var supplierViewModel = new SupplierViewModel
        {
            Code = supplier.Code,
            Name = supplier.Name,
            Description = supplier.Description,

            // General Tab
            GSTIN = supplier.GSTIN,
            CreditDays = supplier.CreditDays,
            CreditLimit = supplier.CreditLimit,

            // Contact Info
            Address = supplier.Address,
            ContactPerson = supplier.ContactPerson,
            Phone = supplier.Phone,
            Mobile = supplier.Mobile,
            Fax = supplier.Fax,
            Email = supplier.Email,
            Website = supplier.Website,

            //SupplierItemDetails = new List<SupplierItemDetailViewModel>()

        };

        var itemIds = supplier.SupplierItemDetails.Select(d => d.ItemId).ToList();
        var items = await _itemService.GetViewModelListAsync(p => itemIds.Contains(p.Id)).ConfigureAwait(false);

        foreach (var detail in supplier.SupplierItemDetails)
        {
            supplierViewModel.SupplierItemDetails.Add(new SupplierItemDetailViewModel
            {
                Id = detail.Id,
                ItemId = detail.ItemId ?? 0,
                ItemName = items.FirstOrDefault(x => x.Id == detail.ItemId)?.Name,
            });
        }

        return View(supplierViewModel);
        
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Edit(SupplierViewModel model)
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
        var supplier = await _supplierService.GetByIdAsync(id ?? 0).ConfigureAwait(false);

        var supplierViewModel = new SupplierViewModel
        {
            Code = supplier.Code,
            Name = supplier.Name,
            Description = supplier.Description
        };

        return View(supplierViewModel);

    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Delete(SupplierViewModel model)
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

    public async Task<ActionResult> GetIndexViewModels([DataSourceRequest] DataSourceRequest request)
    {
        try
        {
            var query = (IEnumerable<Supplier>)_supplierService.GetQuery();
        var data = query.Select(p => new ItemIndexDto
        {
            Id = p.Id,
            Name = p.Name,
            Code = p.Code,
            Description = p.Description,
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
    public async Task<ActionResult> Inline_Create_Update_Destroy_Product([DataSourceRequest] DataSourceRequest request, SupplierItemDetailViewModel model)
    {
        var item = await _itemService.GetViewModelAsync(model.ItemId).ConfigureAwait(false);
        model.ItemName = item?.Name;

        return Json(new[] { model }.ToDataSourceResultAsync(request, ModelState));
    }

    #endregion

    #region -- Private Methods --
    private async Task AddAsync(SupplierViewModel viewModel)
    {
        var supplier = new Supplier
        {
            Code = viewModel.Code,
            Name = viewModel.Name,
            Description = viewModel.Description,

            // General Tab
            GSTIN = viewModel.GSTIN,
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

        foreach (var supplierItemDetailViewModel in viewModel?.SupplierItemDetails)
        {
            var existing = supplier.SupplierItemDetails.FirstOrDefault(d => d.Id == supplierItemDetailViewModel.Id);
            if (null != existing)
                continue;
            supplier.SupplierItemDetails.Add(new SupplierItemDetail
            {
                ItemId = supplierItemDetailViewModel.ItemId,
                //Quantity = supplierItemDetailViewModel.Q
            });
        }

        await _supplierService.AddAndSaveAsync(supplier).ConfigureAwait(false);
    }

    private async Task UpdateAsync(SupplierViewModel viewModel)
    {
        var supplier = await _supplierService.FirstOrDefaultAsync<Supplier>(s => s.Code == viewModel.Code, s => s).ConfigureAwait(false);

        if (supplier == null)
        {
            throw new Exception("Supplier not found.");
        }

        // Update the properties of the existing supplier
        supplier.Name = viewModel.Name;
        supplier.Description = viewModel.Description;
        // General Tab
        supplier.GSTIN = viewModel.GSTIN;
        supplier.CreditDays = viewModel.CreditDays;
        supplier.CreditLimit = viewModel.CreditLimit;

        // Contact Info
        supplier.Address = viewModel.Address;
        supplier.ContactPerson = viewModel.ContactPerson;
        supplier.Phone = viewModel.Phone;
        supplier.Mobile = viewModel.Mobile;
        supplier.Fax = viewModel.Fax;
        supplier.Email = viewModel.Email;
        supplier.Website = viewModel.Website;

        supplier.Status = StatusConstants.Active;

        foreach (var supplierItemDetailViewModel in viewModel?.SupplierItemDetails)
        {
            var existing = supplier.SupplierItemDetails.FirstOrDefault(d => d.Id == supplierItemDetailViewModel.Id);
            if (null != existing)
                continue;
            supplier.SupplierItemDetails.Add(new SupplierItemDetail
            {
                ItemId = supplierItemDetailViewModel.ItemId,
            });
        }

        await _supplierService.UpdateAndSaveAsync(supplier).ConfigureAwait(false);
    }
    #endregion
}