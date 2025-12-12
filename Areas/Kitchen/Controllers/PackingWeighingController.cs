using System;
using System.Web.Mvc;
using Corno.Concept.Portal.Areas.Adm.Services.Interfaces;
using Corno.Concept.Portal.Areas.Kitchen.Models;
using Corno.Concept.Portal.Areas.Kitchen.Services.Interfaces;
using Corno.Concept.Portal.Controllers;
using Corno.Concept.Portal.Logger;
using Corno.Concept.Portal.Models.Packing;
using Corno.Concept.Portal.Services.Masters.Interfaces;
using Corno.Concept.Portal.Services.Packing.Interfaces;
using Kendo.Mvc.UI;

namespace Corno.Concept.Portal.Areas.Kitchen.Controllers;

public class PackingWeighingController : CornoController<Carton>
{
    #region -- Constructors --
    public PackingWeighingController(ILabelService labelService, IBaseItemService itemService, IRackInService packingService,
        IBaseCartonService cartonService)
        : base(cartonService)
    {
        _itemService = itemService;
        _labelService = labelService;
    }

    private readonly ILabelService _labelService;
    private readonly IBaseItemService _itemService;
    #endregion

    #region -- Data Mambers --
    private readonly string _viewPath;
    #endregion

    #region -- Action Methods --
    protected override ActionResult IndexGet(int? pageNo, string type)
    {
        return View(new OperationViewModel());
    }
    #endregion

    #region -- Actions --
    [HttpPost]
    public ActionResult Scan([DataSourceRequest] DataSourceRequest request, string barcode)
    {
        JsonResult jsonResult;
        try
        {
            // Execute operation
            var operationRequest = ExecuteOperation(barcode, Action.Scan);
            // Show Grid
            jsonResult = Json(operationRequest, JsonRequestBehavior.AllowGet);
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

    public ActionResult GetGridDataSource([DataSourceRequest] DataSourceRequest request, string barcode)
    {
        JsonResult jsonResult;
        try
        {
            //Execute operation
            var operationRequest = ExecuteOperation(barcode, Action.Scan);
            jsonResult = Json(operationRequest.GridDataSource.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
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

    public virtual ActionResult GetLayoutDataSource([DataSourceRequest] DataSourceRequest request, string barcode)
    {
        JsonResult jsonResult;
        try
        {
            var operationRequest = ExecuteOperation(barcode, Action.Scan);
            jsonResult = Json(operationRequest.LayoutDataSource.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
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
    public ActionResult ClearControls([DataSourceRequest] DataSourceRequest request, string barcode)
    {
        JsonResult jsonResult;
        try
        {
            // Clear data layout
            var operationRequest = ExecuteOperation(barcode, Action.Cancel);

            // Cancel packing operation
            _operationService.ExecuteOperation(new OperationRequest
            {
                Operation = Operation.PackBomWeight,
                Action = Action.Cancel
            });
            jsonResult = Json(operationRequest, JsonRequestBehavior.AllowGet);
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

    public ActionResult GetPrint([DataSourceRequest] DataSourceRequest request, string barcode)
    {

        JsonResult jsonResult;
        try
        {
            // Execute operation
            var operationRequest = ExecuteOperation(barcode, Action.Print);
            jsonResult = Json(operationRequest, JsonRequestBehavior.AllowGet);
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