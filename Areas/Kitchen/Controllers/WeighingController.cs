using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Corno.Concept.Portal.Areas.Adm.Labels;
using Corno.Concept.Portal.Areas.Kitchen.Dto.Carton;
using Corno.Concept.Portal.Areas.Kitchen.Services.Interfaces;
using Corno.Concept.Portal.Attributes;
using Corno.Concept.Portal.Controllers;
using Corno.Concept.Portal.Extensions;
using Corno.Concept.Portal.Globals;
using Corno.Concept.Portal.Models.Packing;
using Corno.Concept.Portal.Models.Plan;
using Corno.Concept.Portal.Reports;
using Corno.Concept.Portal.Services.Masters.Interfaces;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace Corno.Concept.Portal.Areas.Kitchen.Controllers;

public class WeighingController : SuperController
{
    #region -- Constructors --
    public WeighingController(ICartonService cartonService,IWeighingPackingService weighingPackingService, IPlanService planService,ICarcassPackingService carcassPackingService,
        IOperationService operationService, ILabelService labelService, IBaseItemService itemService)
           
    {
        _weighingPackingService = weighingPackingService;
        _cartonService = cartonService;
        _planService = planService;
        _carcassPackingService = carcassPackingService;
        _itemService = itemService;
        _labelService = labelService;
        _operationService = operationService;
        // _viewPath = @"~/Areas/Kitchen/Views/PackingWeighing/Index.cshtml";

        const string viewPath = "~/Areas/Kitchen/Views/Weighing/";
        _createPath = $"{viewPath}/Create.cshtml";
        _editPath = $"{viewPath}/Edit.cshtml";
        _indexPath = $"{viewPath}/Index.cshtml";
        _createPath = $"{viewPath}/Create.cshtml";
        _previewPath = $"{viewPath}/View.cshtml";
    }
    private readonly IWeighingPackingService _weighingPackingService;
    private readonly ILabelService _labelService;
    private readonly ICartonService _cartonService;
    private readonly ICarcassPackingService _carcassPackingService;
    private readonly IPlanService _planService;
    private readonly IOperationService _operationService;
    private readonly IBaseItemService _itemService;
    #endregion

    #region -- Data Mambers --
    private readonly string _indexPath;
    private readonly string _createPath;
    private readonly string _editPath;
    private readonly string _previewPath;
    #endregion

    #region -- Private Methods --
        

    private void ClearControls()
    {
        var operationRequest = Session[FieldConstants.Operation] as OperationRequest;
        if (null == operationRequest)
            throw new Exception("Invalid operation");

        operationRequest.Action = Action.Cancel;
        var operationResponse = _operationService.ExecuteOperation(operationRequest);

        Session[FieldConstants.Label] = null;
        ModelState.ClearFields(new string[] { FieldConstants.WarehouseOrderNo });
        operationRequest.Clear();

        ModelState.Clear();
    }
    #endregion

    #region -- Action Methods --
    public ActionResult Index()
    {
        return View(new CartonCrudDto());
    }
    #endregion

    #region -- Actions --
    public virtual ActionResult Create()
    {
        Session[FieldConstants.Label] = null;
        return View(new CartonCrudDto());

    }

    [HttpPost]
    //[ValidateAntiForgeryToken]
    [MultipleButton(Name = "action", Argument = "Save")]
    public ActionResult Create(CartonCrudDto viewModel)
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
    public async Task<ActionResult> Scan(CartonCrudDto viewModel)

    {
        if (!ModelState.IsValid)
            return View(_createPath, viewModel);
        try
        {
            // Execute operation
            var operationRequest = Session[FieldConstants.Operation] as OperationRequest;
            if (null == operationRequest || string.IsNullOrEmpty(viewModel.WarehouseOrderNo))
            {
                operationRequest = new OperationRequest
                {
                    ApplicationType = Corno.Globals.Enums.ApplicationType.Web,
                    Operation = Operation.PackBomWeight,
                    Action = Action.Scan
                };

                if (string.IsNullOrEmpty(viewModel.WarehouseOrderNo))
                {
                    var label = await _labelService.GetByBarcodeAsync(viewModel.Barcode).ConfigureAwait(false);
                    viewModel.WarehouseOrderNo = label?.WarehouseOrderNo;
                }
                operationRequest.Set(FieldConstants.WarehouseOrderNo, viewModel.WarehouseOrderNo);
            }

            operationRequest.Set(FieldConstants.Barcode, viewModel.Barcode);
            operationRequest.Set(FieldConstants.ActualWeight, viewModel.ActualWeight);
            _operationService.ExecuteOperation(operationRequest);

            var carton = operationRequest.Get<Carton>(FieldConstants.Carton);
            var plan = operationRequest.Get<Plan>(FieldConstants.Plan);

            if (null == carton)
                return View(_createPath, viewModel);

            viewModel.WarehouseOrderNo = carton.WarehouseOrderNo;
            viewModel.PackingDate = carton.PackingDate;

            viewModel.CartonDetailsDtos.Clear();

            foreach (var cartonDetail in carton.CartonDetails)
            {
                var planItemDetail = plan?.PlanItemDetails.FirstOrDefault(d => d.Position == cartonDetail.Position);
                viewModel.CartonDetailsDtos.Add(new CartonDetailsDto
                {
                    Position = cartonDetail.Position,
                    CarcassCode = planItemDetail?.CarcassCode,
                    AssemblyCode = planItemDetail?.AssemblyCode,
                    ItemCode = planItemDetail?.ItemCode,
                    Description = planItemDetail?.Description,
                    Barcode = cartonDetail.Barcode,
                    Quantity = cartonDetail.Quantity,
                    NetWeight = cartonDetail.NetWeight,
                    SystemWeight = cartonDetail.SystemWeight,
                    Tolerance = cartonDetail.Tolerance,
                    Status = cartonDetail.Status,
                });
            }

            Session[FieldConstants.Operation] = operationRequest;
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }
        viewModel.Barcode = string.Empty;
        ModelState.ClearFields(new string[] { FieldConstants.Barcode });
        ModelState.ClearFields(new string[] { FieldConstants.WarehouseOrderNo });
        return View(_createPath, viewModel);
    }

    [HttpPost]
    //[ValidateAntiForgeryToken]
    [MultipleButton(Name = "action", Argument = "Print")]
    public ActionResult Print(CartonCrudDto viewModel)
    {
        try
        {
            var operationRequest = Session[FieldConstants.Operation] as OperationRequest;
            if (null == operationRequest)
                throw new Exception("Invalid operation");

            operationRequest.Action = Action.Print;

            var operationResponse = _operationService.ExecuteOperation(operationRequest);

            var labeReport = operationRequest.Get<BaseReport>(FieldConstants.Label);
            Session[FieldConstants.Label] = labeReport;

            viewModel = new CartonCrudDto();
            viewModel.PrintToPrinter = true;

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
    [MultipleButton(Name = "action", Argument = "Cancel Carcass")]
    public ActionResult ClearControls(CartonCrudDto viewModel, string barcode)
    {
        try
        {
            Session[FieldConstants.Label] = null;
            ClearControls();

            return View(_createPath, new CartonCrudDto());
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }
        return View(_createPath, viewModel);
    }

    public async Task<ActionResult> View(int? id)
    {
        try
        {
            var carton = await _cartonService.GetByIdAsync(id).ConfigureAwait(false);
            if (null == carton)
                throw new Exception($"Label with Id '{id}' not found.");

            var plan = await _planService.GetByWarehouseOrderNoAsync(carton.WarehouseOrderNo).ConfigureAwait(false);

            var viewModel = new CartonViewDto
            {
                WarehouseOrderNo = carton.WarehouseOrderNo,
                SoNo = carton.SoNo,
                LotNo = plan?.LotNo,
                DueDate = plan?.DueDate,
                OrderQuantity = plan?.OrderQuantity,
                PrintQuantity = plan?.PrintQuantity,
                CartonDetailsDtos = carton.CartonDetails.Select(d =>
                {

                    var planItemDetail = plan?.PlanItemDetails.FirstOrDefault(x => x.Position == d.Position);
                    return new CartonDetailsDto
                    {
                        Position = d.Position,
                        WarehousePosition = d.WarehousePosition,
                        AssemblyCode = planItemDetail?.AssemblyCode,
                        CarcassCode = planItemDetail?.CarcassCode,
                        NetWeight = d.NetWeight,
                        Tolerance = d.Tolerance,
                        Quantity = d.Quantity,
                        OrderQuantity = d.OrderQuantity ?? 0
                    };
                }).ToList(),
                CartonRackingDetailDtos = carton.CartonRackingDetails.Select(d => new CartonRackingDetailDto
                {
                    ScanDate = d.ScanDate,
                    PalletNo = d.PalletNo,
                    RackNo = d.RackNo,
                    Status = d.Status
                }).ToList()
            };

            var operationRequest = new OperationRequest();
            operationRequest.Set(FieldConstants.Plan, plan);
            operationRequest.Set(FieldConstants.Cartons, new List<Carton> { carton });
            //var cartonReports = _cartonService.GetDuplicateLabelRpts(operationRequest);
            var cartonLabelReport = new CartonLabelRpt(operationRequest, true);
            Session[FieldConstants.Label] = cartonLabelReport;

            return View(viewModel);
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }

        return View(new CartonViewDto());
    }

    public async Task<ActionResult> DeleteCarton(int? id)
    {
        try
        {
            var carton = await _cartonService.GetByIdAsync(id).ConfigureAwait(false);
            if (null == carton)
                throw new Exception($"Label with Id '{id}' not found.");

            var cartons = await _cartonService.GetAsync(c => c.CartonNo == carton.CartonNo, c => c).ConfigureAwait(false);

        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
            //return Json(new { success = false, message = exception.Message }, JsonRequestBehavior.AllowGet);
        }
        return View("Index");
        //return Json(new { success = true }, JsonRequestBehavior.AllowGet);
    }

    public async Task<ActionResult> GetIndexViewModels([DataSourceRequest] DataSourceRequest request)
    {
        var query = from carton in _cartonService.GetQuery() as IQueryable<Carton>
            select new
            {
                carton.Id,
                //CartonNo = $"C{carton.CartonNo.ToString().PadLeft(3, '0')}",
                carton.CartonNo,
                carton.PackingDate,
                carton.SoNo,
                carton.WarehouseOrderNo,
                carton.CartonBarcode,
                carton.Status
            };
        var result = await query.ToDataSourceResultAsync(request).ConfigureAwait(false);
        return Json(result, JsonRequestBehavior.AllowGet);
    }

    [AcceptVerbs(HttpVerbs.Post)]
    public ActionResult Inline_Create_Update_Destroy([DataSourceRequest] DataSourceRequest request, CartonDetailsDto model)
    {
        return Json(new[] { model }.ToDataSourceResult(request, ModelState));
    }
    #endregion
}