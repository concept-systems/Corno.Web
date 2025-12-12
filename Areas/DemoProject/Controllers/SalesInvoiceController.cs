using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Corno.Web.Areas.DemoProject.Dtos;
using Corno.Web.Areas.DemoProject.Services.Interfaces;
using Corno.Web.Controllers;
using Corno.Web.Globals;
using Corno.Web.Models.Sales;
using Corno.Web.Services.Masters.Interfaces;
using Corno.Web.Services.Sales.Interfaces;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Mapster;
using Volo.Abp.Data;

namespace Corno.Web.Areas.DemoProject.Controllers;

public class SalesInvoiceController : SuperController
{
    #region -- Constructors --
    public SalesInvoiceController(IBaseSalesInvoiceService salesInvoiceService, 
        IProductLabelService labelService, IProductService productService)
    {
        _salesInvoiceService = salesInvoiceService;
        _labelService = labelService;
        _productService = productService;

        ConfigureMapping();
    }
    #endregion

    #region -- Mapping --
    private void ConfigureMapping()
    {
        // Map header
        TypeAdapterConfig<SalesInvoiceDto, SalesInvoice>
            .NewConfig()
            .IgnoreNullValues(true)
            .Map(dest => dest.Code, src => src.InvoiceNo)
            // Map details collection
            .Map(dest => dest.SalesInvoiceDetails, src => (src.Details ?? new List<SalesInvoiceDetailDto>()).Select(d => d.Adapt<SalesInvoiceDetail>()).ToList())
            .AfterMapping((src, dest) =>
            {
                // Set audit/default fields similar to Kitchen PlanService
                dest.ModifiedDate = DateTime.Now;
                dest.ModifiedBy = User?.Identity?.Name ?? "System";
                if (dest.Id <= 0)
                {
                    dest.CreatedDate = DateTime.Now;
                    dest.CreatedBy = dest.ModifiedBy;
                }
                if (string.IsNullOrEmpty(dest.Status))
                {
                    dest.Status = StatusConstants.Active;
                }

                dest.SetProperty(FieldConstants.Mobile, src.MobileNo);
            });

        // Map detail dto -> entity
        TypeAdapterConfig<SalesInvoiceDetailDto, SalesInvoiceDetail>
            .NewConfig()
            .IgnoreNullValues(true)
            .Map(dest => dest.ProductId, src => src.ProductId)
            //.Map(dest => dest.ProductName, src => src.ProductName)
            .Map(dest => dest.Quantity, src => src.Quantity)
            .Map(dest => dest.Rate, src => src.Rate)
            //.Map(dest => dest.Amount, src => src.Amount)
            .Map(dest => dest.Barcode, src => src.Barcode)
            .AfterMapping((_, dest) =>
            {
                dest.ModifiedDate = DateTime.Now;
                dest.ModifiedBy = User?.Identity?.Name ?? "System";
                if (dest.Id <= 0)
                {
                    dest.CreatedDate = DateTime.Now;
                    dest.CreatedBy = dest.ModifiedBy;
                }
                dest.Status = string.IsNullOrEmpty(dest.Status) ? StatusConstants.Active : dest.Status;
            });
    }
    #endregion

    #region -- Data Mambers --

    private readonly IBaseSalesInvoiceService _salesInvoiceService;
    private readonly IProductLabelService _labelService;
    private readonly IProductService _productService;

    #endregion

    #region -- Private Methods --

    /// <summary>
    /// Validates the SalesInvoiceDto for required fields and business rules
    /// </summary>
    /// <param name="dto">The DTO to validate</param>
    /// <returns>True if validation passes, false otherwise</returns>
    private void ValidateSalesInvoiceDto(SalesInvoiceDto dto)
    {
        // Validate Mobile
        if (string.IsNullOrWhiteSpace(dto.MobileNo))
            throw new Exception("Mobile is required.");

        // Validate Customer Name
        if (string.IsNullOrWhiteSpace(dto.CustomerName))
            throw new Exception("Customer name is required.");

        // Validate Payment Mode
        if (string.IsNullOrWhiteSpace(dto.PaymentMode))
            throw new Exception("Payment mode is required.");

        // Validate Paid Amount
        if (dto.PaidAmount is null or <= 0)
            throw new Exception("Payment Mode is required."); 

        // Validate at least one product in details
        if (dto.Details is not { Count: > 0 })
            throw new Exception(@"At least one product is required in the invoice details.");
    }

    #endregion

    #region -- Actions --
    public ActionResult Index()
    {
        return View();
    }

    public virtual ActionResult Create()
    {
        var dto = new SalesInvoiceDto
        {
            PrintToPrinter = false,
            InvoiceDate = DateTime.Now
        };
        Session[FieldConstants.Label] = null;
        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    //[MultipleButton(Name = "action", Argument = "Create")]
    public async Task<ActionResult> Create(SalesInvoiceDto dto)
    {
        if (!ModelState.IsValid)
            return View(dto);

        try
        {
            // Validate DTO using common validation method
            ValidateSalesInvoiceDto(dto);

            // Map dto (including details) to entity and save
            var entity = dto.Adapt<SalesInvoice>();
            await _salesInvoiceService.AddAndSaveAsync(entity).ConfigureAwait(false);

            return RedirectToAction("Index");
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }

        return View(dto);
    }

    public async Task<ActionResult> Edit(int? id)
    {
        try
        {
            var salesInvoice = await _salesInvoiceService.FirstOrDefaultAsync(s => s.Id == id, s => s).ConfigureAwait(false);
            if (null == salesInvoice)
                throw new Exception("Invalid Id");

            var salesInvoiceDto = salesInvoice.Adapt<SalesInvoiceDto>();
            salesInvoiceDto.PrintToPrinter = false;
            Session[FieldConstants.Label] = null;

            return View(salesInvoiceDto);
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    //[MultipleButton(Name = "action", Argument = "Edit")]
    public async Task<ActionResult> Edit(SalesInvoiceDto dto)
    {
        if (!ModelState.IsValid)
            return View(dto);

        try
        {
            // Validate DTO using common validation method
            ValidateSalesInvoiceDto(dto);

            var existing = await _salesInvoiceService.FirstOrDefaultAsync(s => s.Id == dto.Id, s => s).ConfigureAwait(false);
            if (existing == null)
                throw new Exception("Invalid Id");

            // Adapt header fields
            dto.Adapt(existing);

            // Replace details with adapted ones
            existing.SalesInvoiceDetails = (dto.Details ?? new List<SalesInvoiceDetailDto>())
                .Select(d => d.Adapt<SalesInvoiceDetail>()).ToList();

            await _salesInvoiceService.UpdateAndSaveAsync(existing).ConfigureAwait(false);

            return RedirectToAction("Index");
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }

        return View(dto);
    }

    public async Task<ActionResult> View(int? id)
    {
        try
        {
            var model = await _salesInvoiceService.FirstOrDefaultAsync(s => s.Id == id, s => s).ConfigureAwait(false);
            if (null == model)
                throw new Exception("Invalid Id");

            var dto = model.Adapt<SalesInvoiceDto>();
            dto.PrintToPrinter = false;
            Session[FieldConstants.Label] = null;

            return View(dto);
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }

        return View();
    }

    [HttpGet]
    public async Task<ActionResult> ValidateBarcode(string barcode)
    {
        try
        {
            var invoiceId = await _salesInvoiceService.FirstOrDefaultAsync(p => 
                p.SalesInvoiceDetails.Any(d => d.Barcode == barcode),
                p => p.Id).ConfigureAwait(false);
            if (invoiceId > 0)
                throw new Exception($"barcode '{barcode}' is already scanned");
            var label = await _labelService.FirstOrDefaultAsync(b => b.Barcode == barcode, b => b).ConfigureAwait(false);
            if (null == label)
                throw new Exception($"barcode '{barcode}' is not found");
            var product = await _productService.FirstOrDefaultAsync(p => p.Id == label.ProductId, p => p).ConfigureAwait(false);
            return Json(new
            {
                Success = true,
                label.ProductId,
                ProductName = product?.Name,
                Quantity = 1,
            }, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            return Json(new { Success = false, exception.Message }, JsonRequestBehavior.AllowGet);
        }
    }

    public async Task<ActionResult> GetIndexDtos([DataSourceRequest] DataSourceRequest request)
    {
        var query = (IEnumerable<SalesInvoice>)_salesInvoiceService.GetQuery();
        var data = query.Select(p => new SalesInvoiceIndexDto
        {
            Id = p.Id,
            InvoiceNo = p.Code,
            InvoiceDate = p.InvoiceDate,
            CustomerName = p.CustomerName,
            Mobile = p.GetProperty(FieldConstants.Mobile, string.Empty),
            PaymentMode = p.PaymentMode,
            PaidAmount = p.PaidAmount,
            Status = p.Status,
        }).OrderByDescending(p => p.InvoiceDate);
        var result = await data.ToDataSourceResultAsync(request).ConfigureAwait(false);

        return Json(result, JsonRequestBehavior.AllowGet);
    }

    [AcceptVerbs(HttpVerbs.Post)]
    public ActionResult EditingInline_Create([DataSourceRequest] DataSourceRequest request, SalesInvoiceDetailDto detail)
    {
        return Json(new[] { detail }.ToDataSourceResult(request, ModelState));
    }

    #endregion
}