using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using Kendo.Mvc.UI;
using Corno.Web.Globals;
using Corno.Web.Models.Plan;
using Corno.Web.Attributes;
using Corno.Web.Areas.Kitchen.Dto.StoreShirwalLabel;

namespace Corno.Web.Areas.Kitchen.Controllers;

[Authorize]
public class StoreShirwalController : LabelController
{
    #region -- Constructors --
    public StoreShirwalController()
    {
        const string viewPath = "~/Areas/Kitchen/Views/StoreShirwal";
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

    public ActionResult Create()
    {
        var viewModel = new StoreShirwalCrudDto
        {
            //LabelReports = new List<BaseReport>()
        };
        try
        {
            Session[FieldConstants.Label] = null;
            return View(viewModel);
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }
        return View(viewModel);
    }

    [HttpPost]
    //[ValidateAntiForgeryToken]
    [MultipleButton(Name = "action", Argument = "Print")]
    public async Task<ActionResult> Create(StoreShirwalCrudDto dto)
    {
        if (!ModelState.IsValid)
            return View(_createPath, dto);
        try
        {
            // Get Plan
            var plan = await GetPlanAsync(dto.WarehouseOrderNo).ConfigureAwait(false);
            // Create Labels
            var labels = await StoreShirwalLabelService.CreateLabelsAsync(dto, plan).ConfigureAwait(false);
            // Create Label Reports
            Session[FieldConstants.Label] = await StoreShirwalLabelService.CreateLabelReportAsync(labels, false).ConfigureAwait(false);
            // Save in database
            await StoreShirwalLabelService.UpdateDatabaseAsync(labels, plan).ConfigureAwait(false);
            dto.PrintToPrinter = true;
            dto.Clear();
            ModelState.Clear();
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }

        return View(_createPath, dto);
    }


    [HttpPost]
    [MultipleButton(Name = "action", Argument = "Preview")]
    public async Task<ActionResult> Preview(StoreShirwalCrudDto dto)
    {
        if (!ModelState.IsValid)
            return View(_createPath, dto);
        try
        {
            //dto.Clear();
            // Get Plan
            var plan = await GetPlanAsync(dto.WarehouseOrderNo).ConfigureAwait(false);
            // Create Labels
            var labels = await StoreShirwalLabelService.CreateLabelsAsync(dto, plan).ConfigureAwait(false);
            // Create Label Reports
            Session[FieldConstants.Label] = await StoreShirwalLabelService.CreateLabelReportAsync(labels, false).ConfigureAwait(false);
            dto.PrintToPrinter = false;
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }
        return View(_createPath, dto);
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
    // GetIndexViewDto is inherited from LabelController
    #endregion
}
