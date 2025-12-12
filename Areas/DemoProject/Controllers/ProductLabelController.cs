using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Corno.Web.Areas.Admin.Services.Interfaces;
using Corno.Web.Areas.DemoProject.Dtos;
using Corno.Web.Areas.DemoProject.Services.Interfaces;
using Corno.Web.Controllers;
using Corno.Web.Dtos;
using Corno.Web.Extensions;
using Corno.Web.Globals;
using Corno.Web.Logger;
using Corno.Web.Models.Masters;
using Corno.Web.Models.Packing;
using Corno.Web.Services.Masters.Interfaces;
using Corno.Web.Windsor;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNet.Identity;
using Volo.Abp.Data;

namespace Corno.Web.Areas.DemoProject.Controllers;

public class ProductLabelController : SuperController
{
    #region -- Constructors --
    public ProductLabelController(IProductLabelService labelService, IProductService productService)
    {
        _labelService = labelService;
        _productService = productService;

        const string viewPath = "~/Areas/DemoProject/views/ProductLabel/";
        _indexPath = $"{viewPath}/Index.cshtml";
        _createPath = $"{viewPath}/Create.cshtml";
        _viewPath = $"{viewPath}/View.cshtml";
    }
    #endregion

    #region -- Data Mambers --

    private readonly IProductLabelService _labelService;
    private readonly IProductService _productService;

    private readonly string _indexPath;
    private readonly string _createPath;
    private readonly string _viewPath;
    #endregion

    #region -- Private Methods --

    private async Task<LabelDto> CreateLabelDtoAsync(int? id)
    {
        var label = await _labelService.FirstOrDefaultAsync<Label>(l => l.Id == id, l => l);
        if (label == null)
            throw new Exception("Label not found.");

        var product = await _productService.FirstOrDefaultAsync<Product>(p => p.Id == (label.ProductId ?? 0), p => p);

        var userIds = label.LabelDetails.Select(d => d.ModifiedBy).ToList();
        var userService = Bootstrapper.Get<IUserService>();
        var users = await userService.GetAsync(p => userIds.Contains(p.Id), p => p);

        var dto = new LabelDto
        {
            ProductId = product?.Id,
            ProductName = product?.Name,
            LabelDate = label.LabelDate,
            Weight = label.NetWeight ?? 0,
            Rate = label.GetProperty(FieldConstants.Rate, 0),
            ManufacturingDate = label.GetProperty(FieldConstants.ManufacturingDate, DateTime.MinValue),
            ExpiryDate = label.GetProperty("ExpiryDate", DateTime.MinValue),

            PrintToPrinter = false,

            Details = label.LabelDetails.Select(d => new LabelDetailDto
            {
                ScanDate = d.ScanDate,
                ModifiedBy = users.FirstOrDefault(p => p.Id == d.ModifiedBy)?.UserName,
                ModifiedDate = d.ModifiedDate,
                Status = d.Status
            }).ToList()
        };

        return dto;
    }
    #endregion

    #region -- Actions --
    public ActionResult Index()
    {
        return View();
    }

    public virtual ActionResult Create()
    {
        Session[FieldConstants.Label] = null;
        return View(_createPath, new LabelDto());
    }

    [HttpPost]
    public async Task<ActionResult> Print(LabelDto dto)
    {
        if (!ModelState.IsValid)
            return View(_createPath, dto);
        try
        {
            // Store LabelFormatId before clearing
            var labelFormatId = dto.LabelFormatId;
            
            // Create Labels
            var labels = await _labelService.CreateLabelsAsync(dto, User.Identity.GetUserId()).ConfigureAwait(false);
            
            // Get Product for report
            var product = await _productService.FirstOrDefaultAsync<Product>(p => p.Id == (dto.ProductId ?? 0), p => p).ConfigureAwait(false);
            
            // Create Label Reports
            Session[FieldConstants.Label] = await _labelService.CreateLabelReportAsync(labels, product, labelFormatId, false).ConfigureAwait(false);
            
            // Save in database
            await _labelService.UpdateDatabaseAsync(labels).ConfigureAwait(false);
            
            dto.Clear();
            ModelState.Clear();

            var report = await _labelService.CreateLabelReportAsync(labels, product, labelFormatId, false).ConfigureAwait(false);
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
    public async Task<ActionResult> Preview(LabelDto dto)
    {
        if (!ModelState.IsValid)
            return View(_createPath, dto);
        try
        {
            // Get Product
            var product = await _productService.FirstOrDefaultAsync<Product>(p => p.Id == (dto.ProductId ?? 0), p => p).ConfigureAwait(false);
            
            // Create Labels
            var labels = await _labelService.CreateLabelsAsync(dto, User.Identity.GetUserId()).ConfigureAwait(false);
            
            // Create Label Reports
            var report = await _labelService.CreateLabelReportAsync(labels, product, dto.LabelFormatId, false).ConfigureAwait(false);
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

    public async Task<ActionResult> View(int? id)
    {
        try
        {
            var label = await _labelService.FirstOrDefaultAsync<Label>(l => l.Id == id, l => l);
            if (null == label)
                throw new Exception($"Label with Id '{id}' not found.");

            var product = await _productService.FirstOrDefaultAsync<Product>(p => p.Id == (label.ProductId ?? 0), p => p);

            // Create Label Report (using default format for view - small label) (async)
            var report = await _labelService.CreateLabelReportAsync(new List<Label> { label }, product, 3, true).ConfigureAwait(false);
            Session[FieldConstants.Label] = report;

            var dto = await CreateLabelDtoAsync(id).ConfigureAwait(false);
            return View(_viewPath, dto);
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }

        return RedirectToAction("Index");
    }

    public JsonResult GetLabelFormats()
    {
        try
        {
            var labelFormats = new List<MasterDto>
            {
                new() { Id = 1, Name = "Big Label" },
                new() { Id = 2, Name = "Medium Label" },
                new() { Id = 3, Name = "Small Label" },
            };
            return Json(labelFormats, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            return GetJsonResult(exception);
        }
    }

    public async Task<ActionResult> GetIndexViewModels([DataSourceRequest] DataSourceRequest request)
    {
        try
        {
            var query = (IEnumerable<Label>)_labelService.GetQuery();

            var productQuery = (IEnumerable<Product>)_productService.GetQuery();

            var data = from label in query
                       where (label.ProductId ?? 0) > 0
                       join product in productQuery on label.ProductId equals product.Id
                       select new LabelDto
                       {
                           Id = label.Id,
                           LabelDate = label.LabelDate,
                           ProductName = product?.Name,
                           Weight = label.Quantity,
                           Rate = label.GetProperty(FieldConstants.Rate, 0) > 0 ? label.GetProperty(FieldConstants.Rate, 0) : product?.Mrp,
                           ExpiryDate = label.GetProperty("ExpiryDate", DateTime.MinValue),
                           ManufacturingDate = label.GetProperty(FieldConstants.ManufacturingDate, DateTime.MinValue),
                       };
            var result = await data.ToDataSourceResultAsync(request).ConfigureAwait(false);
            //var result = await query.ToDataSourceResultAsync(request);

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            return Json(new DataSourceResult { Errors = exception.Message });
        }
    }

    #endregion
}