using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Corno.Concept.Portal.Areas.Quality.Models;
using Corno.Concept.Portal.Controllers;
using Corno.Logger;
using Corno.Models.ViewModels;
using Kendo.Mvc.UI;

namespace Corno.Concept.Portal.Areas.Masters.Controllers;

[System.Web.Mvc.Authorize]
public class FinancialYearController : MasterController<FinancialYear>
{
    #region -- Constructors --
    public FinancialYearController(IFinancialYearService financialYearService) 
        : base(financialYearService, null)
    {
        _financialYearService = financialYearService;
            
    }
    #endregion

    #region -- Data Mambers --

    private readonly IFinancialYearService _financialYearService;
    #endregion
    #region -- Action Methods --
    protected ActionResult Index()
    {
        return View();
    }

    //[WebMethod]
    [System.Web.Mvc.AllowAnonymous]
    [System.Web.Mvc.OverrideAuthorization]
    public JsonResult GetViewModelList([DataSourceRequest] DataSourceRequest request, int? companyId)
    {
        LogHandler.LogInfo($@"In GetViewModelList, CompanyId : {companyId}");
        JsonResult jsonResult;
        try
        {
            var financialYears = _financialYearService.GetViewModelList(f => f.CompanyId == companyId);
            jsonResult = Json(financialYears, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            LogHandler.LogInfo(exception);
            jsonResult = Json(new List<MasterDto>(), JsonRequestBehavior.AllowGet);
            //return Json(null, JsonRequestBehavior.AllowGet);
            //jsonResult = Json(new DataSourceResult
            //{
            //    Errors = new {error = "fail", message = Logger.LogHandler.GetDetailException(exception)}
            //});
        }

        return jsonResult;
    }
}

#endregion