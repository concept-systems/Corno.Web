using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Corno.Concept.Modules.Masters.Services.Interfaces;
using Corno.Concept.Modules.Planning.Models;
using Corno.Concept.Portal.Controllers;
using Corno.Services.Progress.Interfaces;
using Kendo.Mvc.UI;

namespace Corno.Concept.Portal.Areas.Masters.Controllers;

public class StandardBomController : TransactionController<Plan>
{
    #region -- Constructors --
    public StandardBomController( ICustomerService customerService, IPlanService planService, IWebProgressService progressService) 
        : base(planService)
    {
           
        _customerService = customerService;
        _planService = planService;
        _progressService = progressService;
    }
    #endregion

    #region -- Data Mambers --

    private readonly ICustomerService _customerService;
    private readonly IPlanService _planService;
    private readonly IWebProgressService _progressService;

    #endregion

    #region -- Action Methods --

    protected override ActionResult IndexGet(int? pageNo, string type)
    {
        return View(_planService.GetList());
    }


    protected override Plan EditGet(int? id)
    {
        var plan = _planService.GetById(id);
        if (null == plan)
            throw new Exception($"Plan Id ({id}) not found.");
        return plan;
    }

    protected override Plan EditPost(Plan model)
    {
        var existing = _planService.GetById(model.Id);
        if (null == existing)
            throw new Exception("Something went wrong Product controller.");

        model.CopyPropertiesTo(existing);

        existing.ModifiedDate = DateTime.Now;
        return existing;
    }

    #endregion

    #region -- Actions --

    [AcceptVerbs(HttpVerbs.Post)]
    public virtual ActionResult Inline_Create_Update_Destroy([DataSourceRequest] DataSourceRequest request, PlanItemDetail model)
    {
        return View();
        //return Json(new[] { model }.ToDataSourceResult(request, ModelState));
    }

    public virtual ActionResult ImportPlan(IEnumerable<HttpPostedFileBase> files)
    {
        ActionResult jsonResult = Json(new { error = false }, JsonRequestBehavior.AllowGet);
        try
        {
            var httpPostedFileBases = files.ToList();
            if (httpPostedFileBases.FirstOrDefault() == null)
                throw new Exception("No file selected for import");

            var fileBase = httpPostedFileBases.FirstOrDefault();
            // Save file
            var filePath = Server.MapPath("~/Upload/" + fileBase?.FileName);
            //if(System.IO.File.Exists(filePath))
            //    System.IO.File.Delete(filePath);
            fileBase?.SaveAs(filePath);

            // Import file
            _progressService.SetWebProgress();
            _planService.Import(filePath, _progressService);
        }
        catch (Exception exception)
        {
            jsonResult = Json(new
            {
                error = true,
                message = exception.Message
            }, JsonRequestBehavior.AllowGet);
        }
        return jsonResult;
    }

    #endregion
}