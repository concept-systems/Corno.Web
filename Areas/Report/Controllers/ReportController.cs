using System;
using System.Web.Mvc;
using Corno.Web.Controllers;
using Corno.Web.Globals;
using Corno.Web.Logger;
using Corno.Web.Services.Interfaces;
using Corno.Web.Services.Report.Interfaces;

namespace Corno.Web.Areas.Report.Controllers;

[Authorize]
public class ReportController : SuperController
{
    #region -- Constructors --
    public ReportController(IAssemblyService assemblyService, IReportFactory reportFactory)
    {
        _assemblyService = assemblyService;
        _reportFactory = reportFactory;
    }
    #endregion

    #region -- Data Members --
    private readonly IAssemblyService _assemblyService;
    private readonly IReportFactory _reportFactory;
    #endregion

    #region -- Actions --
    public ActionResult ShowReport(string reportName, string title)
    {
        try
        {
            ViewBag.Title = title;
            var type = _assemblyService.GetType(reportName);
            if (null == type)
                throw new Exception("Report (" + reportName + ") is not assigned to selected form in site-map or not part of assembly.");

            var report = _reportFactory.CreateReport(type);
            Session[FieldConstants.Label] = report ?? throw new Exception("Invalid report type " + reportName);
            /*var report = (BaseReport)DependencyResolver.Current.GetService(type);
            Session[FieldConstants.Label] = report ?? throw new Exception("Invalid report type " + reportName);*/
            /*var report = Bootstrapper.Get<BaseReport>(type);
            Session[FieldConstants.Label] = report ?? throw new Exception("Invalid report type " + reportName);*/
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }
        return View();
    }

    public virtual ActionResult RefreshTimes()
    {
        ActionResult jsonResult;
        try
        {
            //UpdateTimes(productionOrderNo, workOrderNo);

            jsonResult = null;
        }
        catch (Exception exception)
        {
            LogHandler.LogError(exception);

            jsonResult = Json(new
            {
                error = true,
                message = LogHandler.GetDetailException(exception)?.Message
            }, JsonRequestBehavior.AllowGet);
        }
        return jsonResult;
    }

    #endregion
}