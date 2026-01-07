using System;
using System.Web.Mvc;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using Corno.Web.Areas.Kitchen.Dto.StoreLabel;
using Corno.Web.Attributes;
using Corno.Web.Globals;
using Corno.Web.Logger;
using Corno.Web.Models.Plan;
using System.Threading.Tasks;
using Corno.Web.Extensions;
using Corno.Web.Globals.Enums;

namespace Corno.Web.Areas.Kitchen.Controllers;

[Authorize]
public class StoreKhalapurController : LabelController
{
    #region -- Constructors --
    public StoreKhalapurController()
    {
        const string viewPath = "~/Areas/Kitchen/Views/StoreKhalapur";
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

    public ActionResult Create(string warehouseOrderNo)
    {
        var dto = new StoreLabelCrudDto();

        try
        {
            if (string.IsNullOrEmpty(warehouseOrderNo))
                Session[FieldConstants.Label] = null;

            return View(dto);
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }
        return View(dto);
    }

    [HttpPost]
    public async Task<ActionResult> Preview(StoreLabelCrudDto dto)
    {
        if (!ModelState.IsValid)
            return View(_createPath, dto);
        try
        {
            // Get Plan
            var plan = await GetPlanAsync(dto.WarehouseOrderNo).ConfigureAwait(false);
            // Create Labels
            var labels = await StoreKhalapurLabelService.CreateLabelsAsync(dto, plan).ConfigureAwait(false);
            // Create Label Reports
            var report = await StoreKhalapurLabelService.CreateLabelReportAsync(labels, false, LabelType.StoreKhalapur).ConfigureAwait(false);
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
    public async Task<ActionResult> Print(StoreLabelCrudDto dto)
    {
        if (!ModelState.IsValid)
            return View(_createPath, dto);
        try
        {
            // Get Plan
            var plan = await GetPlanAsync(dto.WarehouseOrderNo).ConfigureAwait(false);
            // Create Labels
            var labels = await StoreKhalapurLabelService.CreateLabelsAsync(dto, plan).ConfigureAwait(false);
            // Save in database
            await StoreKhalapurLabelService.UpdateDatabaseAsync(labels, plan).ConfigureAwait(false);
            var report = await StoreKhalapurLabelService.CreateLabelReportAsync(labels, false, LabelType.StoreKhalapur).ConfigureAwait(false);
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

    // GetIndexViewDto is inherited from LabelController
    public async Task<ActionResult> GetLotNos([DataSourceRequest] DataSourceRequest request, DateTime dueDate)
    {
        try
        {
            var lotNos = await PlanService.GetPendingLotNosAsync(dueDate).ConfigureAwait(false);
            return Json(lotNos, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            return GetComboBoxErrorResult(exception);
        }
    }

    public async Task<ActionResult> GetWarehouseOrderNos([DataSourceRequest] DataSourceRequest request, string lotNo)
    {
        try
        {
            var data = await PlanService.GetPendingWarehouseOrdersAsync(lotNo).ConfigureAwait(false);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            return GetComboBoxErrorResult(exception);
        }
    }

    public async Task<ActionResult> GetFamilies([DataSourceRequest] DataSourceRequest request, string warehouseOrderNo)
    {
        try
        {
            var plan = await GetPlanAsync(warehouseOrderNo).ConfigureAwait(false);
            var families = await PlanService.GetPendingFamiliesAsync(plan).ConfigureAwait(false);
            return Json(families, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            return GetComboBoxErrorResult(exception);
        }
    }

    public async Task<ActionResult> GetItems([DataSourceRequest] DataSourceRequest request, string warehouseOrderNo,
        string family)
    {
        if (string.IsNullOrEmpty(warehouseOrderNo))
            return Json(null, JsonRequestBehavior.AllowGet);

        try
        {
            var plan = await GetPlanAsync(warehouseOrderNo).ConfigureAwait(false);
            var items = await StoreKhalapurLabelService.GetPendingItemsAsync(plan, family).ConfigureAwait(false);

            return Json((await items.ToDataSourceResultAsync(request).ConfigureAwait(false)).Data, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            return GetComboBoxErrorResult(exception);
        }
    }
    #endregion
}
