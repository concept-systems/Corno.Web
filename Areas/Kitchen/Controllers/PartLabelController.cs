using Corno.Web.Areas.Kitchen.Dto.Label;
using Corno.Web.Areas.Kitchen.Services.Interfaces;
using Corno.Web.Controllers;
using Corno.Web.Globals;
using Corno.Web.Logger;
using Corno.Web.Models.Packing;
using Corno.Web.Models.Plan;
using Kendo.Mvc.UI;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Corno.Web.Extensions;
using Kendo.Mvc.Extensions;

namespace Corno.Web.Areas.Kitchen.Controllers;

public class PartLabelController : SuperController
{
    #region -- Constructors --
    public PartLabelController(IPartLabelService partLabelService, IPlanService planService)
    {
        _partLabelService = partLabelService;
        _planService = planService;

         const string viewPath = "~/Areas/Kitchen/Views/PartLabel";
        _createPath = $"{viewPath}/Create.cshtml";

        TypeAdapterConfig<LabelViewDto, Label>
            .NewConfig()
            .Map(dest => dest.LabelDetails, src => src.LabelViewDetailDto.Adapt<List<LabelDetail>>())
            .AfterMapping((src, dest) =>
            {
                dest.Id = src.Id;
                for (var index = 0; index < src.LabelViewDetailDto.Count; index++)
                    dest.LabelDetails[index].Id = src.LabelViewDetailDto[index].Id;
            });
        TypeAdapterConfig<Label, LabelViewDto>
            .NewConfig()
            .Map(dest => dest.LabelViewDetailDto, src => src.LabelDetails.Adapt<List<LabelViewDetailDto>>())
            .AfterMapping((src, dest) =>
            {
                dest.Id = src.Id;
                for (var index = 0; index < src.LabelDetails.Count; index++)
                    dest.LabelViewDetailDto[index].Id = src.LabelDetails[index].Id;
            });
    }
    #endregion

    #region -- Data Mambers --
    private readonly string _createPath;
    private readonly IPartLabelService _partLabelService;
    private readonly IPlanService _planService;
    #endregion

    #region -- Private Methods --
    private async Task<Plan> GetPlanAsync(string warehouseOrderNo)
    {
        if (Session[FieldConstants.Plan] is Plan plan &&
            plan.WarehouseOrderNo.Equals(warehouseOrderNo)) 
            return plan;

        plan = await _planService.GetByWarehouseOrderNoAsync(warehouseOrderNo).ConfigureAwait(false);
        Session[FieldConstants.Plan] = plan ?? throw new Exception($"Warehouse order {warehouseOrderNo} not found");

        return plan;
    }
    #endregion

    #region -- Actions --
    public ActionResult Index()
    {
        return View(new LabelViewDto());
    }

    public Task<ActionResult> Create(string warehouseOrderNo)
    {
        if (string.IsNullOrEmpty(warehouseOrderNo))
            Session[FieldConstants.Label] = null;

        return Task.FromResult<ActionResult>(View(new PartLabelCrudDto()));
    }

    [HttpPost]
    //[MultipleButton(Name = "action", Argument = "Print")]
    public async Task<ActionResult> Print(PartLabelCrudDto dto)
    {
        if (!ModelState.IsValid)
            return View(_createPath, dto);
        try
        {
            // Get Plan
            var plan = await GetPlanAsync(dto.WarehouseOrderNo).ConfigureAwait(false);
            // Create Labels
            var labels = await _partLabelService.CreateLabelsAsync(dto, plan).ConfigureAwait(false);
            // Create Label Reports
            Session[FieldConstants.Label] = await _partLabelService.CreateLabelReportAsync(labels, false).ConfigureAwait(false);
            // Save in database
            await _partLabelService.UpdateDatabaseAsync(labels, plan).ConfigureAwait(false);
            //dto.PrintToPrinter = true;
            dto.Clear();
            ModelState.Clear();

            var report = await _partLabelService.CreateLabelReportAsync(labels, false).ConfigureAwait(false);
            return File(report.ToDocumentBytes(), "application/pdf");
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
            var message = LogHandler.GetDetailException(exception)?.Message;
            return Json(new { Success = false, Message = message },
                JsonRequestBehavior.AllowGet);
        }
        //return View(_createPath, dto);
    }

    [HttpPost]
    //[MultipleButton(Name = "action", Argument = "Preview")]
    public async Task<ActionResult> Preview(PartLabelCrudDto dto)
    {
        if (!ModelState.IsValid)
            return View(_createPath, dto);
        try
        {
            //dto.Clear();
            // Get Plan
            var plan = await GetPlanAsync(dto.WarehouseOrderNo).ConfigureAwait(false);
            // Create Labels
            var labels = await _partLabelService.CreateLabelsAsync(dto, plan).ConfigureAwait(false);
            // Create Label Reports
            //Session[FieldConstants.Label] = _partLabelService.CreateLabelReport(labels, false);
            var report = await _partLabelService.CreateLabelReportAsync(labels, false).ConfigureAwait(false);
            return File(report.ToDocumentBytes(), "application/pdf");
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
            var message = LogHandler.GetDetailException(exception)?.Message;
            return Json(new { Success = false, Message = message },
                JsonRequestBehavior.AllowGet);
        }
        //return View(_createPath, dto);
    }

    public async Task<ActionResult> View(int? id)
    {
        try
        {
            if (id == null)
                throw new Exception("Invalid Id");

            // If CreateViewDto has an async version, use that:
            var dto = await _partLabelService.CreateViewDtoAsync(id);

            // Otherwise, wrap in Task.Run for async behavior:
            //var dto = await Task.Run(() => _partLabelService.CreateViewDtoAsync(id));

            //Session[FieldConstants.Label] = dto.LabelReport;

            return View(dto);
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }

        return RedirectToAction("Index");
    }

    public async Task<ActionResult> GetItems([DataSourceRequest] DataSourceRequest request, string warehouseOrderNo)
    {
        try
        {
            if (string.IsNullOrEmpty(warehouseOrderNo))
                return Json(null, JsonRequestBehavior.AllowGet);

            var plan = await GetPlanAsync(warehouseOrderNo).ConfigureAwait(false);
            var pendingItems = await _partLabelService.GetPendingItemsAsync(plan).ConfigureAwait(false);
            
            // Convert to list, then to queryable for proper filtering
            var itemsList = pendingItems.Cast<object>().ToList();
            var queryableItems = itemsList.AsQueryable();
            
            // Debug: Check if filter is received
            // You can check request.Filters in debugger to see if filter is being sent
            // If request.Filters is null or empty, the filter is not being sent from client
            
            // Apply filter, sorting, and paging from DataSourceRequest
            // MultiColumnComboBox expects just the Data array, not the full DataSourceResult
            var result = queryableItems.ToDataSourceResult(request);
            return Json(result.Data, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            return GetComboBoxErrorResult(exception);
        }
    }

    public async Task<ActionResult> GetIndexViewDto([DataSourceRequest] DataSourceRequest request)
    {
        try
        {
            var result = await Task.Run(() => _partLabelService.GetIndexDataSource(request)).ConfigureAwait(false);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
            ModelState.AddModelError("Error", exception.Message);
            return Json(new DataSourceResult { Errors = ModelState });
        }
    }
    #endregion
}