using Corno.Web.Areas.Kitchen.Dto.Label;
using Corno.Web.Globals;
using Corno.Web.Logger;
using Corno.Web.Models.Plan;
using Kendo.Mvc.UI;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Corno.Web.Extensions;
using Corno.Web.Globals.Enums;
using Kendo.Mvc.Extensions;

namespace Corno.Web.Areas.Kitchen.Controllers;

public class PartLabelController : LabelController
{
    #region -- Constructors --
    public PartLabelController()
    {
        const string viewPath = "~/Areas/Kitchen/Views/PartLabel";
        _createPath = $"{viewPath}/Create.cshtml";
    }
    #endregion

    #region -- Data Members --
    private readonly string _createPath;
    #endregion

    #region -- Private Methods --
    private async Task<Plan> GetPlanAsync(string warehouseOrderNo)
    {
        if (Session[FieldConstants.Plan] is Plan plan &&
            plan.WarehouseOrderNo.Equals(warehouseOrderNo)) 
            return plan;

        plan = await PlanService.GetByWarehouseOrderNoAsync(warehouseOrderNo).ConfigureAwait(false);
        Session[FieldConstants.Plan] = plan ?? throw new Exception($"Warehouse order {warehouseOrderNo} not found");

        return plan;
    }
    #endregion

    #region -- Actions --
    // Index and View actions are inherited from LabelController

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
            var labels = await PartLabelService.CreateLabelsAsync(dto, plan).ConfigureAwait(false);
            // Save in database
            await PartLabelService.UpdateDatabaseAsync(labels, plan).ConfigureAwait(false);
            var report = await PartLabelService.CreateLabelReportAsync(labels, false, LabelType.Part).ConfigureAwait(false);
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
            var labels = await PartLabelService.CreateLabelsAsync(dto, plan).ConfigureAwait(false);
            // Create Label Reports
            //Session[FieldConstants.Label] = _partLabelService.CreateLabelReport(labels, false);
            var report = await PartLabelService.CreateLabelReportAsync(labels, false, LabelType.Part).ConfigureAwait(false);
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

    public async Task<ActionResult> GetItems([DataSourceRequest] DataSourceRequest request, string warehouseOrderNo)
    {
        try
        {
            if (string.IsNullOrEmpty(warehouseOrderNo))
                return Json(null, JsonRequestBehavior.AllowGet);

            var plan = await GetPlanAsync(warehouseOrderNo).ConfigureAwait(false);
            var pendingItems = await PartLabelService.GetPendingItemsAsync(plan).ConfigureAwait(false);
            
            // Convert to list, then to queryable for proper filtering
            var itemsList = pendingItems.Cast<object>().ToList();
            var queryableItems = itemsList.AsQueryable();
            
            // Debug: Check if filter is received
            // You can check request.Filters in debugger to see if filter is being sent
            // If request.Filters is null or empty, the filter is not being sent from client
            
            // Apply filter, sorting, and paging from DataSourceRequest
            // MultiColumnComboBox expects just the Data array, not the full DataSourceResult
            var result = await queryableItems.ToDataSourceResultAsync(request);
            return Json(result.Data, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            return GetComboBoxErrorResult(exception);
        }
    }
    // GetIndexViewDto is inherited from LabelController
    #endregion
}
