using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Corno.Web.Areas.Kitchen.Dto.Kitting;
using Corno.Web.Areas.Kitchen.Dto.Label;
using Corno.Web.Areas.Kitchen.Services.Interfaces;
using Corno.Web.Controllers;
using Corno.Web.Globals;
using Corno.Web.Hubs;
using Corno.Web.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.SignalR;


namespace Corno.Web.Areas.Kitchen.Controllers;

[System.Web.Mvc.Authorize]
public class KittingController : SuperController
{
    #region -- Constructors --
    public KittingController(
        ILabelService labelService,
        IPlanService planService)
    {
        _labelService = labelService;
        _planService = planService;
    }
    #endregion

    #region -- Data Members --
    private readonly ILabelService _labelService;
    private readonly IPlanService _planService;

    #endregion

    #region -- Actions --

    public ActionResult Index()
    {
        return View(new LabelViewDto());
    }

    public virtual ActionResult Create()
    {
        try
        {
            try
            {
                Session[FieldConstants.Label] = null;
                return View(new KittingCrudDto());
            }
            catch (Exception exception)
            {
                HandleControllerException(exception);
            }
            return View();
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }
        return View();
    }


    //[ValidateAntiForgeryToken]
    [HttpPost]
    public async Task<ActionResult> Create(KittingCrudDto dto)
    {
        if (!ModelState.IsValid)
            return View(dto);

        try
        {
            await _labelService.PerformKittingAsync(dto, User.Identity.GetUserId()).ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }

        return View(dto);
    }

    public virtual ActionResult Import()
    {
        try
        {
            return View();
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }
        return View();
    }

    //[HttpPost]
    //public virtual ActionResult Import(IEnumerable<HttpPostedFileBase> files)
    //{
    //    try
    //    {
    //        var httpPostedFileBases = files?.ToList();
    //        if (httpPostedFileBases?.FirstOrDefault() == null)
    //            throw new Exception("No file selected for import");

    //        var file = httpPostedFileBases.FirstOrDefault();
    //        var filePath = Server.MapPath("~/Upload/" + file?.FileName);

    //        // Save file
    //        file?.SaveAs(filePath);

    //        Session[FieldConstants.FilePath] = filePath;
    //        Session[FieldConstants.OldStatus] = new List<string> { StatusConstants.Active, StatusConstants.Printed };
    //        Session[FieldConstants.Operation] = OperationConstants.Bending;
    //        Session[FieldConstants.NewStatus] = StatusConstants.Bent;

    //        var importModels = _labelService.Import(file, FieldConstants.Operation, FieldConstants.NewStatus,
    //            _progressService);
    //        return Json(new { success = true, importModels = importModels }, JsonRequestBehavior.AllowGet);
    //        //_labelService.Import(operationRequest, _progressService);
    //    }
    //    catch (Exception exception)
    //    {
    //        HandleControllerException(exception);

    //        return Json(new { success = false, message = exception.Message }, JsonRequestBehavior.AllowGet);
    //    }
    //}

    public ActionResult GetIndexViewDto([DataSourceRequest] DataSourceRequest request)
    {
        try
        {
            var query = _labelService.GetQuery().AsNoTracking();
            query = query?.Where(p => p.Status == StatusConstants.Bent);
            var data = from label in query
                select new LabelIndexDto
                {
                    Id = label.Id,
                    LabelDate = label.LabelDate,
                    LotNo = label.LotNo,
                    WarehouseOrderNo = label.WarehouseOrderNo,
                    Position = label.Position,
                    OrderQuantity = label.OrderQuantity ?? 0,
                    Quantity = label.Quantity ?? 0,
                    Status = label.Status
                };
            var result = data.ToDataSourceResult(request);
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