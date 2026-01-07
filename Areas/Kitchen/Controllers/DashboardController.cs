using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using Corno.Web.Areas.Kitchen.Dto.Dashboard;
using Corno.Web.Areas.Kitchen.Services.Interfaces;
using Corno.Web.Controllers;

namespace Corno.Web.Areas.Kitchen.Controllers;

public class DashboardController : SuperController
{
    #region -- Constructors --
    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }
    #endregion

    #region -- Data Members --
    private readonly IDashboardService _dashboardService;
    #endregion

    #region -- Actions --
    public async Task<ActionResult> Index()
    {
        try
        {
            var dashboard = await _dashboardService.GetMainDashboardAsync().ConfigureAwait(false);
            return View(dashboard);
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
            return View(new MainDashboardDto());
        }
    }

    public async Task<ActionResult> Plan()
    {
        try
        {
            var dashboard = await _dashboardService.GetPlanDashboardAsync().ConfigureAwait(false);
            return View(dashboard);
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
            return View(new PlanDashboardDto());
        }
    }

    public async Task<ActionResult> Label()
    {
        try
        {
            var dashboard = await _dashboardService.GetLabelDashboardAsync().ConfigureAwait(false);
            return View(dashboard);
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
            return View(new LabelDashboardDto());
        }
    }

    public async Task<ActionResult> Carton()
    {
        try
        {
            var dashboard = await _dashboardService.GetCartonDashboardAsync().ConfigureAwait(false);
            return View(dashboard);
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
            return View(new CartonDashboardDto());
        }
    }

    // AJAX endpoints for real-time updates
    [HttpPost]
    public async Task<ActionResult> GetPlanDashboardData(DateTime? startDate, DateTime? endDate)
    {
        try
        {
            var data = await _dashboardService.GetPlanDashboardDataAsync(startDate, endDate).ConfigureAwait(false);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
            return Json(new { success = false, message = exception.Message }, JsonRequestBehavior.AllowGet);
        }
    }

    [HttpPost]
    public async Task<ActionResult> GetLabelDashboardData(DateTime? startDate, DateTime? endDate)
    {
        try
        {
            var data = await _dashboardService.GetLabelDashboardDataAsync(startDate, endDate).ConfigureAwait(false);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
            return Json(new { success = false, message = exception.Message }, JsonRequestBehavior.AllowGet);
        }
    }

    [HttpPost]
    public async Task<ActionResult> GetCartonDashboardData(DateTime? startDate, DateTime? endDate)
    {
        try
        {
            var data = await _dashboardService.GetCartonDashboardDataAsync(startDate, endDate).ConfigureAwait(false);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
            return Json(new { success = false, message = exception.Message }, JsonRequestBehavior.AllowGet);
        }
    }
    #endregion
}

