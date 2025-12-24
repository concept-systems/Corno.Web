using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Corno.Web.Areas.DemoProject.Dtos;
using Corno.Web.Areas.DemoProject.Reports;
using Corno.Web.Areas.DemoProject.Services.Interfaces;
using Corno.Web.Controllers;
using Corno.Web.Extensions;
using Corno.Web.Globals;
using Corno.Web.Logger;
using Corno.Web.Models.Sales;
using Corno.Web.Services.Interfaces;
using Corno.Web.Services.Masters.Interfaces;
using Corno.Web.Services.Sales.Interfaces;
using Corno.Web.Windsor;
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
            // Map details collection (type difference)
            .Map(dest => dest.SalesInvoiceDetails, src => (src.SalesInvoiceDetailDtos ?? new List<SalesInvoiceDetailDto>()).Select(d => d.Adapt<SalesInvoiceDetail>()).ToList())
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
            .AfterMapping((src, dest) =>
            {
                // Update 
                dest.Rate = src.Mrp.ToDouble();
                dest.Amount = src.Amount.ToDouble();

                dest.ModifiedDate = DateTime.Now;
                dest.ModifiedBy = User?.Identity?.Name ?? "System";
                if (dest.Id <= 0)
                {
                    dest.CreatedDate = DateTime.Now;
                    dest.CreatedBy = dest.ModifiedBy;
                }
                dest.Status = string.IsNullOrEmpty(dest.Status) ? StatusConstants.Active : dest.Status;
            });

        // Reverse mapping: entity -> dto (including details)
        TypeAdapterConfig<SalesInvoice, SalesInvoiceDto>
            .NewConfig()
            .IgnoreNullValues(true)
            // Name difference
            .Map(dest => dest.MobileNo, src => src.GetProperty(FieldConstants.Mobile, string.Empty))
            .Map(dest => dest.SalesInvoiceDetailDtos, src => (src.SalesInvoiceDetails ?? new List<SalesInvoiceDetail>()).Select(d => d.Adapt<SalesInvoiceDetailDto>()).ToList());

        // Reverse mapping: detail entity -> dto
        TypeAdapterConfig<SalesInvoiceDetail, SalesInvoiceDetailDto>
            .NewConfig()
            .IgnoreNullValues(true)
            .Map(dest => dest.Id, src => src.Id)
            // Name difference
            .Map(dest => dest.Mrp, src => src.Rate);
            // PackingTypeId and NetWeight
        TypeAdapterConfig<SalesInvoiceDetail, SalesInvoiceDetailDto>
            .ForType()
            .Map(dest => dest.PackingTypeId, src => src.PackingTypeId)
            .Map(dest => dest.NetWeight, src => src.NetWeight);
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
        /*if (dto.PaidAmount is null or <= 0)
            throw new Exception("Payment Mode is required."); */

        // Validate at least one product in details
        if (dto.SalesInvoiceDetailDtos is not { Count: > 0 })
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
            InvoiceDate = DateTime.Now
        };
        Session[FieldConstants.Label] = null;
        return View(dto);
    }

    [HttpPost]
    /*[ValidateAntiForgeryToken]*/
    //[MultipleButton(Name = "action", Argument = "Create")]
    public async Task<ActionResult> Create(SalesInvoiceDto dto)
    {
        /*if (!ModelState.IsValid)
            return View(dto);

        try
        {
            // Validate DTO using common validation method
            ValidateSalesInvoiceDto(dto);

            // Map dto (including details) to entity and save
            var entity = dto.Adapt<SalesInvoice>();
            // Set serial number to next max only for new invoices (when not already set)
            if (entity.SerialNo <= 0)
                entity.SerialNo = await _salesInvoiceService.GetNextSerialNoAsync().ConfigureAwait(false);
            await _salesInvoiceService.AddAndSaveAsync(entity).ConfigureAwait(false);

            return RedirectToAction("Index");
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }*/

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

            // Populate names for product and packing type before save (optional display enrichment)
            var miscMasterService = Bootstrapper.Get<IMiscMasterService>();
            var productIds = salesInvoiceDto.SalesInvoiceDetailDtos.Select(d => d.ProductId).Distinct().ToList();
            var products = await _productService.GetViewModelListAsync(p => productIds.Contains(p.Id));
            var packingTypeIds = salesInvoiceDto.SalesInvoiceDetailDtos.Select(d => d.PackingTypeId).Distinct().ToList();
            var packingTypes = await miscMasterService.GetViewModelListAsync(p => packingTypeIds.Contains(p.Id));
            salesInvoiceDto.SalesInvoiceDetailDtos.ForEach(d => {
                d.ProductName = products.FirstOrDefault(x => x.Id == d.ProductId)?.Name;
                d.PackingTypeShortName = packingTypes.FirstOrDefault(x => x.Id == d.PackingTypeId)?.Description;
            });

            return View(salesInvoiceDto);
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }

        return View();
    }

    [HttpPost]
    //[ValidateAntiForgeryToken]
    //[MultipleButton(Name = "action", Argument = "Edit")]
    public async Task<ActionResult> Edit(SalesInvoiceDto dto)
    {
        /*if (!ModelState.IsValid)
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

            // Merge details by Id: update existing, add new, remove deleted
            var incomingDetails = (dto.SalesInvoiceDetailDtos ?? new List<SalesInvoiceDetailDto>())
                .Select(d => d.Adapt<SalesInvoiceDetail>()).ToList();

            // Update existing rows
            foreach (var inc in incomingDetails)
            {
                if (inc.Id > 0)
                {
                    var existingDetail = existing.SalesInvoiceDetails.FirstOrDefault(x => x.Id == inc.Id);
                    if (existingDetail != null)
                        inc.Adapt(existingDetail);
                    else
                        existing.SalesInvoiceDetails.Add(inc);
                }
                else
                {
                    // New row
                    existing.SalesInvoiceDetails.Add(inc);
                }
            }

            // Remove rows that are not in incoming list (deleted by user)
            var incomingIds = new HashSet<int>(incomingDetails.Where(x => x.Id > 0).Select(x => x.Id));
            existing.SalesInvoiceDetails.RemoveAll(x => x.Id > 0 && !incomingIds.Contains(x.Id));

            await _salesInvoiceService.UpdateAndSaveAsync(existing).ConfigureAwait(false);

            return RedirectToAction("Index");
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }*/

        return View(dto);
    }

    public async Task<ActionResult> View(int? id)
    {
        try
        {
            var model = await _salesInvoiceService.FirstOrDefaultAsync(s => s.Id == id, s => s).ConfigureAwait(false);
            if (null == model)
                throw new Exception("Invalid Id");

            // Ensure reverse mapping config is applied before adaptation
            ConfigureMapping();
            var dto = model.Adapt<SalesInvoiceDto>();

            // Update Product Names & Packing Type Names
            var miscMasterService = Bootstrapper.Get<IMiscMasterService>();
            var productIds = dto.SalesInvoiceDetailDtos.Select(d => d.ProductId).Distinct().ToList();
            var products = await _productService.GetViewModelListAsync(p => productIds.Contains(p.Id));
            var packingTypeIds = dto.SalesInvoiceDetailDtos.Select(d => d.PackingTypeId).Distinct().ToList();
            var packingTypes = await miscMasterService.GetViewModelListAsync(p => packingTypeIds.Contains(p.Id));
            dto.SalesInvoiceDetailDtos.ForEach(d => {
                d.ProductName = products.FirstOrDefault(x => x.Id == d.ProductId)?.Name;
                d.PackingTypeShortName = packingTypes.FirstOrDefault(x => x.Id == d.PackingTypeId)?.Description;
            });

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
            // MRP from label extra properties
            var mrp = label.GetProperty(FieldConstants.Mrp, 0);
            var miscService = Bootstrapper.Get<IMiscMasterService>();
            var packingType = await miscService.GetViewModelAsync(label.PackingTypeId ?? 0).ConfigureAwait(false);
            var netWeight = label.NetWeight;

            var quantity = 1; // Default quantity for barcode scan
            return Json(new
            {
                Success = true,
                SalesInvoiceDetailDto = new SalesInvoiceDetailDto
                {
                    Id = 0,
                    ProductId = label.ProductId,
                    ProductName = product?.Name,
                    PackingTypeId = label.PackingTypeId,
                    PackingTypeShortName = packingType?.Description,
                    Barcode = label.Barcode,
                    Quantity = quantity,
                    Mrp = mrp,
                    Amount = mrp.ToDouble() * quantity,
                    NetWeight = netWeight.ToDouble()
                },
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

    [HttpPost]
    public async Task<ActionResult> Preview(SalesInvoiceDto dto)
    {
        try
        {
            // Validate DTO
            ValidateSalesInvoiceDto(dto);

            // Create report
            var report = await CreateSalesInvoiceReportAsync(dto);
            return File(report.ToDocumentBytes(), "application/pdf");
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
            var message = LogHandler.GetDetailException(exception)?.Message;
            return Json(new { Success = false, Message = message },
                JsonRequestBehavior.AllowGet);
        }
    }

    [HttpPost]
    public async Task<ActionResult> Save(SalesInvoiceDto dto)
    {
        try
        {
            // Validate DTO (common rules)
            ValidateSalesInvoiceDto(dto);

            // Delegate business logic to service
            var entity = await _salesInvoiceService.SaveInvoiceAsync(dto).ConfigureAwait(false);

            // Ensure DTO reflects persisted entity
            dto.Id = entity.Id;
            dto.Code = entity.Code;

            return Json(new
            {
                Success = true,
                Id = entity.Id,
                Code = entity.Code
            }, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
            var message = LogHandler.GetDetailException(exception)?.Message;
            return Json(new { Success = false, Message = message },
                JsonRequestBehavior.AllowGet);
        }
    }

    [HttpPost]
    public async Task<ActionResult> Print(SalesInvoiceDto dto)
    {
        /*if (!ModelState.IsValid)
            return Json(new { Success = false, Message = "Invalid model state" }, JsonRequestBehavior.AllowGet);*/
        
        try
        {
            // Validate DTO
            ValidateSalesInvoiceDto(dto);

            // Use common service method so Save & Save & Print share logic
            var entity = await _salesInvoiceService.SaveInvoiceAsync(dto).ConfigureAwait(false);

            // Ensure DTO is in sync for report generation
            dto.Id = entity.Id;
            dto.Code = entity.Code;

            // Create report
            var report = await CreateSalesInvoiceReportAsync(dto);
            return File(report.ToDocumentBytes(), "application/pdf");
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
            var message = LogHandler.GetDetailException(exception)?.Message;
            return Json(new { Success = false, Message = message },
                JsonRequestBehavior.AllowGet);
        }
    }

    private async Task<SalesInvoiceReceiptRpt> CreateSalesInvoiceReportAsync(SalesInvoiceDto dto)
    {
        return new SalesInvoiceReceiptRpt(dto);
    }

    #endregion
}