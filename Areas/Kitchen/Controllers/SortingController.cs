using Corno.Web.Areas.Kitchen.Dto.Label;
using Corno.Web.Areas.Kitchen.Dto.Sorting;
using Corno.Web.Areas.Kitchen.Services.Interfaces;
using Corno.Web.Controllers;
using Corno.Web.Globals;
using Corno.Web.Hubs;
using Corno.Web.Models;
using Corno.Web.Services.Progress.Interfaces;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Corno.Web.Areas.Kitchen.Controllers;

[System.Web.Http.Authorize]
public class SortingController : SuperController
{
    #region -- Constructors --
    public SortingController(ILabelService labelService, IWebProgressService progressService)
    {
        _labelService = labelService;
        _progressService = progressService;

        //progressService.SetWebProgress();
        progressService.OnProgressChanged += OnProgressChanged;
    }
    #endregion

    #region -- Data Members --
    private readonly ILabelService _labelService;
    private readonly IWebProgressService _progressService;
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
                return View(new SortingCrudDto());
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
    //[MultipleButton(Name = "action", Argument = "Scan")]
    public async Task<ActionResult> Create(SortingCrudDto dto)
    {
        if (!ModelState.IsValid)
            return View(dto);

        try
        {
            await _labelService.PerformSorting(dto, User.Identity.GetUserId()).ConfigureAwait(false);
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
    //        var file = files?.FirstOrDefault();
    //        if (file == null)
    //            throw new Exception("No file selected for import");

    //        _progressService.SetWebProgress();
    //        var uploadPath = Server.MapPath("~/Upload");
    //        if (!Directory.Exists(uploadPath))
    //        {
    //            Directory.CreateDirectory(uploadPath);
    //        }
    //        var fileName = Path.GetFileName(file.FileName);
    //        var filePath = Path.Combine(Server.MapPath("~/Upload"), fileName);

    //        file.SaveAs(filePath);

    //        Session[FieldConstants.FilePath] = filePath;
    //        Session[FieldConstants.OldStatus] = new List<string> { StatusConstants.Bent };
    //        Session[FieldConstants.Operation] = OperationConstants.Sorting;
    //        Session[FieldConstants.NewStatus] = StatusConstants.Sorted;

    //        var importModels = _labelService.Import(file.InputStream, FieldConstants.Operation, FieldConstants.NewStatus,
    //             _progressService);
    //        return Json(new { success = true, importModels = importModels }, JsonRequestBehavior.AllowGet);
    //        //_labelService.Import(operationRequest, _progressService);
    //    }
    //    catch (Exception exception)
    //    {
    //        HandleControllerException(exception);

    //        return Json(new { success = false, message = exception.Message }, JsonRequestBehavior.AllowGet);
    //    }
    //}

    [HttpPost]
    public ActionResult Import(IEnumerable<HttpPostedFileBase> files)
    {
        try
        {
            var file = files?.FirstOrDefault();
            if (file == null)
                throw new Exception("No file selected for import");

            var uploadPath = Server.MapPath("~/Upload");
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }
            var fileName = Path.GetFileName(file.FileName);
            var filePath = Path.Combine(Server.MapPath("~/Upload"), fileName);

            file.SaveAs(filePath);

            Session[FieldConstants.FilePath] = filePath;
            Session[FieldConstants.OldStatus] = new List<string> { StatusConstants.Bent };
            Session[FieldConstants.Operation] = OperationConstants.Sorting;
            Session[FieldConstants.NewStatus] = StatusConstants.Sorted;

            _progressService.SetWebProgress();

            var importModels = _labelService.Import(file.InputStream, FieldConstants.Operation, StatusConstants.Sorted,
                             _progressService, User.Identity.GetUserId());
            return Json(new { success = true, importModels }, JsonRequestBehavior.AllowGet);


            //var importModels = _planService.Import(file.InputStream, file.FileName, _progressService,
            //User.Identity.GetUserId());

            //return Json(new { success = true, importModels }, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);

            return Json(new { success = false, message = exception.Message }, JsonRequestBehavior.AllowGet);
        }
    }


    private void OnProgressChanged(object sender, ProgressModel progressModel)
    {
        /*if (progressModel.MessageType != MessageType.Info && progressModel.Maximum <= 0)
            return;*/
        var context = GlobalHost.ConnectionManager.GetHubContext<ProgressHub>();
        context.Clients.All.receiveProgress(progressModel);
    }

    public ActionResult GetIndexViewDto([DataSourceRequest] DataSourceRequest request)
    {
        try
        {
            var query = _labelService.GetQuery();
            query = query.Where(p => p.Status == StatusConstants.Sorted);
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