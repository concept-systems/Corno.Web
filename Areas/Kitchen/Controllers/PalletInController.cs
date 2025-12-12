using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Corno.Web.Areas.Kitchen.Dto.Carton;
using Corno.Web.Areas.Kitchen.Dto.Pallet;
using Corno.Web.Areas.Kitchen.Services.Interfaces;
using Corno.Web.Attributes;
using Corno.Web.Controllers;
using Corno.Web.Globals;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;


namespace Corno.Web.Areas.Kitchen.Controllers;

[System.Web.Http.Authorize]
public class PalletInController : SuperController
{
    #region -- Constructors --
    public PalletInController(INonWeighingPackingService nonWeighingPackingService,IPalletInService palletInService)
    {
        _nonWeighingPackingService = nonWeighingPackingService;
        _palletInService = palletInService;

        const string viewPath = "~/Areas/Kitchen/Views/PalletIn";
        _createPath = $"{viewPath}/Create.cshtml";
        _indexPath = $"{viewPath}/Index.cshtml";
        _createPath = $"{viewPath}/Create.cshtml";
    }
    #endregion

    #region -- Data Members --

    private readonly INonWeighingPackingService _nonWeighingPackingService;
    private readonly IPalletInService _palletInService;
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
                return View(new PalletInViewDto());
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
    public ActionResult Create(PalletInViewDto viewModel)
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
    public ActionResult Scan(PalletInViewDto viewModel)
    {
        if (!ModelState.IsValid)
            return View(_createPath, viewModel);
        try
        {
            Session[OperationConstants.PalletInFg] = viewModel.ToString();
            var oldStatus = new List<string>
                { StatusConstants.Printed, StatusConstants.Packed, StatusConstants.UnLoad, StatusConstants.RackIn };
            Session[FieldConstants.Barcodes] = new  List<string> { viewModel.CartonBarcode };
            Session[FieldConstants.PalletNo] = viewModel.PalletNo;
            Session[FieldConstants.OldStatus] = oldStatus;
            Session[FieldConstants.PalletNo] = viewModel.PalletNo;
            Session[FieldConstants.NewStatus] = StatusConstants.PalletIn;
            //_palletInService(OperationConstants.PalletInFg);
            // Post Pallet In
            viewModel.PalletDetails.Add(new CartonPalletViewDto
            {
                Barcode = viewModel.CartonBarcode,
                PalletNo = viewModel.PalletNo
            });
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }
        viewModel.CartonBarcode = string.Empty;
        //viewModel.PalletNo = string.Empty;
        return View(_createPath, viewModel);
    }

    public async Task<ActionResult> GetIndexViewDto([DataSourceRequest] DataSourceRequest request)
    {
        try
        {
            var query = _nonWeighingPackingService.GetQuery().AsNoTracking();
            query = query?.Where(p => p.Status == StatusConstants.PalletIn);
            var data = from carton in query
                select new CartonIndexDto()
                {
                    Id = carton.Id,
                    CartonNo = carton.CartonNo.ToString(), // Now safe
                    PackingDate = carton.PackingDate,
                    SoNo = carton.SoNo,
                    WarehouseOrderNo = carton.WarehouseOrderNo,
                    CartonBarcode = carton.CartonBarcode,
                    Status = carton.Status
                };
            var result = await data.ToDataSourceResultAsync(request).ConfigureAwait(false);
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