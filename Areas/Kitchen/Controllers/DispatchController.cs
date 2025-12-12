using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Corno.Web.Areas.Kitchen.Dto;
using Corno.Web.Areas.Kitchen.Dto.Carton;
using Corno.Web.Areas.Kitchen.Services.Interfaces;
using Corno.Web.Attributes;
using Corno.Web.Controllers;
using Corno.Web.Globals;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;


namespace Corno.Web.Areas.Kitchen.Controllers;

[System.Web.Http.Authorize]
public class DispatchController : SuperController
{
    #region -- Constructors --
    public DispatchController(INonWeighingPackingService nonWeighingPackingService,IDispatchService dispatchService)
    {
        _nonWeighingPackingService = nonWeighingPackingService;
        _dispatchService = dispatchService;

        const string viewPath = "~/Areas/Kitchen/Views/Dispatch";
        _createPath = $"{viewPath}/Create.cshtml";
        _indexPath = $"{viewPath}/Index.cshtml";
        _createPath = $"{viewPath}/Create.cshtml";
    }
    #endregion

    #region -- Data Members --

    private readonly INonWeighingPackingService _nonWeighingPackingService;
    private readonly IDispatchService _dispatchService;
    #endregion
    #region -- Data Mambers --
    private readonly string _indexPath;
    private readonly string _createPath;
    #endregion
    #region -- Actions --
    public ActionResult Index()
    {
        return View(new CartonIndexDto());
    }

    public virtual ActionResult Create()
    {
        try
        {
            try
            {
                Session[FieldConstants.Label] = null;
                return View(new DispatchViewDto());
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
    [HttpPost]
    //[ValidateAntiForgeryToken]
    [MultipleButton(Name = "action", Argument = "Save")]
    public ActionResult Create(DispatchViewDto viewModel)
    {
        if (!ModelState.IsValid)
            return View(viewModel);

        try
        {
            return View(_createPath, viewModel);
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }

        return View(_createPath, viewModel);
    }

    [HttpPost]
    //[ValidateAntiForgeryToken]
    [MultipleButton(Name = "action", Argument = "Scan")]
    //public ActionResult Scan(DispatchViewDto viewModel)
    //{
    //    if (!ModelState.IsValid)
    //        return View(_createPath, viewModel);
    //    try
    //    {
    //        // Execute operation
    //        var operationRequest = Session[FieldConstants.Operation] as OperationRequest;
    //        if (null == operationRequest)
    //        {
    //            operationRequest = new OperationRequest
    //            {
    //                Operation = Operation.PalletInFg,
    //                Action = Action.Scan
    //            };
    //        }

    //        var oldStatus = new List<string>
    //            { StatusConstants.Printed, StatusConstants.PalletIn, StatusConstants.RackIn, StatusConstants.RackOut };
    //        operationRequest.Set(FieldConstants.Barcodes, new List<string> { viewModel.CartonBarcode });
    //        operationRequest.Set(FieldConstants.LoadNo, viewModel.LoadNo);
    //        operationRequest.Set(FieldConstants.WarehouseOrderNo, viewModel.WarehouseOrderNo);
    //        operationRequest.Set(FieldConstants.OldStatus, oldStatus);
    //        operationRequest.Set(FieldConstants.NewStatus, StatusConstants.Dispatch);
    //        _dispatchService.Dispatch(operationRequest);
    //        // Post Dispatch In
    //        viewModel.CartonDispatchDtos.Add(new CartonDispatchDto
    //        {
    //            CartonBarcode = viewModel.CartonBarcode,
    //            LoadNo = viewModel.LoadNo,
    //            WarehouseOrderNo = viewModel.WarehouseOrderNo,
    //        });
    //    }
    //    catch (Exception exception)
    //    {
    //        HandleControllerException(exception);
    //    }
    //    viewModel.CartonBarcode = string.Empty;
    //    //viewModel.WarehouseOrderNo = string.Empty;
    //    //viewModel.LoadNo = string.Empty;
    //    return View(_createPath, viewModel);
    //}

    //public async Task<ActionResult> GetIndexViewDto([DataSourceRequest] DataSourceRequest request)
    //{
    //    try
    //    {
    //        var query = _nonWeighingPackingService.GetQuery();
    //        query = query?.Where(p => p.Status == StatusConstants.PalletIn);
    //        var data = from carton in query
    //            select new CartonIndexDto()
    //            {
    //                Id = carton.Id,
    //                CartonNo = carton.CartonNo.ToString(), // Now safe
    //                PackingDate = carton.PackingDate,
    //                SoNo = carton.SoNo,
    //                WarehouseOrderNo = carton.WarehouseOrderNo,
    //                CartonBarcode = carton.CartonBarcode,
    //                Status = carton.Status
    //            };
    //        var result = await data.ToDataSourceResultAsync(request);
    //        return Json(result, JsonRequestBehavior.AllowGet);
    //    }
    //    catch (Exception exception)
    //    {
    //        HandleControllerException(exception);
    //        ModelState.AddModelError("Error", exception.Message);
    //        return Json(new DataSourceResult { Errors = ModelState });
    //    }
    //}


    public ActionResult GetIndexViewDto([DataSourceRequest] DataSourceRequest request)
    {
        try
        {
            var query = _nonWeighingPackingService.GetQuery().AsNoTracking();
            var data = from label in query
                select new CartonIndexDto
                {
                    Id = label.Id,
                    //LabelDate = label.LabelDate,
                    //LotNo = label.LotNo,
                    WarehouseOrderNo = label.WarehouseOrderNo,
                    //Position = label.Position,
                    //OrderQuantity = label.OrderQuantity ?? 0,
                    //Quantity = label.Quantity ?? 0,
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