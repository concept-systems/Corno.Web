using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using Corno.Web.Areas.Admin.Dto;
using Corno.Web.Areas.Admin.Services.Interfaces;
using Corno.Web.Controllers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace Corno.Web.Areas.Admin.Controllers;

public class DashboardController : SuperController
{
    #region -- Constructors --
    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
        _indexPath = "~/Areas/Admin/Views/Dashboard/Index.cshtml";
    }
    #endregion

    #region -- Data Members --
    private readonly IDashboardService _dashboardService;
    private readonly string _indexPath;
    #endregion

    #region -- Actions --
    [Authorize]
    public async Task<ActionResult> Index()
    {
        var dashboardData = await _dashboardService.GetDashboardDataAsync().ConfigureAwait(false);
        return View(_indexPath, dashboardData);
    }

    [HttpPost]
    public async Task<ActionResult> GetDashboardData()
    {
        try
        {
            var data = await _dashboardService.GetDashboardDataAsync().ConfigureAwait(false);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        catch (System.Exception exception)
        {
            return GetJsonResult(exception);
        }
    }

    [HttpPost]
    public async Task<ActionResult> GetRecentActivities([DataSourceRequest] DataSourceRequest request)
    {
        try
        {
            var dashboardData = await _dashboardService.GetDashboardDataAsync().ConfigureAwait(false);
            var activities = dashboardData?.RecentActivities ?? new System.Collections.Generic.List<RecentActivityDto>();
            
            var result = activities.ToDataSourceResult(request);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            return GetGridErrorResult(exception);
        }
    }
    #endregion
}

