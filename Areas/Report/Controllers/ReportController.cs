using System;
using System.Web.Mvc;
using Corno.Web.Controllers;
using Corno.Web.Globals;
using Corno.Web.Logger;
using Corno.Web.Services.Interfaces;
using Corno.Web.Services.Report.Interfaces;
using Kendo.Mvc.Extensions;

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

            // CRITICAL: Use AssemblyQualifiedName, NOT FullName
            ViewBag.ReportTypeName = type.AssemblyQualifiedName;  // e.g., "Corno.Web.Areas.Kitchen.Reports.HandoverRpt, Corno.Web"
            ViewBag.BoxTitle = type.Name.SplitPascalCase().Replace("Rpt", "Report");

            /*// Pass the report type name to the view - HTML5 viewer will use TypeReportSourceResolver
            // This is the standard practice - no need to store in Session
            ViewBag.ReportTypeName = type.FullName;*/
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