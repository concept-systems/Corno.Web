using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Corno.Web.Areas.BoardStore.Services.Interfaces;
using Corno.Web.Areas.Kitchen.Dto.Label;
using Corno.Web.Areas.Kitchen.Dto.StiffenerLabel;
using Corno.Web.Areas.Kitchen.Dto.StoreLabel;
using Corno.Web.Areas.Kitchen.Dto.StoreShirwalLabel;
using Corno.Web.Areas.Kitchen.Services.Interfaces;
using Corno.Web.Attributes;
using Corno.Web.Controllers;
using Corno.Web.Extensions;
using Corno.Web.Globals;
using Corno.Web.Logger;
using Corno.Web.Models.Packing;
using Corno.Web.Models.Plan;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Mapster;

namespace Corno.Web.Areas.Kitchen.Controllers;

[Authorize]
public class LabelController : SuperController
{
    #region -- Constructors --
    public LabelController(
        IPartLabelService partLabelService,
        IStoreKhalapurLabelService storeKhalapurLabelService,
        IStoreShirwalLabelService storeShirwalLabelService,
        IStiffenerLabelService stiffenerLabelService,
        IItemService itemService,
        IPlanService planService)
    {
        _partLabelService = partLabelService;
        _storeKhalapurLabelService = storeKhalapurLabelService;
        _storeShirwalLabelService = storeShirwalLabelService;
        _stiffenerLabelService = stiffenerLabelService;
        _planService = planService;
        _itemService = itemService;

        // Configure Mapster for all label types
        TypeAdapterConfig<LabelViewDto, Label>
            .NewConfig()
            .Map(dest => dest.LabelDetails, src => src.LabelViewDetailDto.Adapt<List<LabelDetail>>())
            .AfterMapping((src, dest) =>
            {
                dest.Id = src.Id;
                for (var index = 0; index < src.LabelViewDetailDto.Count; index++)
                    dest.LabelDetails[index].Id = src.LabelViewDetailDto[index].Id;
            });
        TypeAdapterConfig<Label, LabelViewDto>
            .NewConfig()
            .Map(dest => dest.LabelViewDetailDto, src => src.LabelDetails.Adapt<List<LabelViewDetailDto>>())
            .AfterMapping((src, dest) =>
            {
                dest.Id = src.Id;
                for (var index = 0; index < src.LabelDetails.Count; index++)
                    dest.LabelViewDetailDto[index].Id = src.LabelDetails[index].Id;
            });
    }
    #endregion

    #region -- Data Members --
    private readonly IPartLabelService _partLabelService;
    private readonly IStoreKhalapurLabelService _storeKhalapurLabelService;
    private readonly IStoreShirwalLabelService _storeShirwalLabelService;
    private readonly IStiffenerLabelService _stiffenerLabelService;
    private readonly IPlanService _planService;
    private readonly IItemService _itemService;
    #endregion

    #region -- Private Methods --
    private async Task<Plan> GetPlanAsync(string warehouseOrderNo)
    {
        if (Session[FieldConstants.Plan] is Plan plan &&
            plan.WarehouseOrderNo.Equals(warehouseOrderNo))
            return plan;

        plan = await _planService.GetByWarehouseOrderNoAsync(warehouseOrderNo).ConfigureAwait(false);
        Session[FieldConstants.Plan] = plan ?? throw new Exception($"Warehouse order {warehouseOrderNo} not found");

        return plan;
    }

    private async Task<List<Plan>> GetPlansAsync(string lotNo, string drawingNo)
    {
        if (Session[FieldConstants.Plans] is List<Plan> plans &&
            plans.Any(d => !d.LotNo.Equals(lotNo)))
            return plans;
        plans = (await _planService.GetAsync<Plan>(p => p.LotNo == lotNo &&
                                      p.PlanItemDetails.Any(d =>
                                          d.DrawingNo == drawingNo), p => p).ConfigureAwait(false)).ToList();
        Session[FieldConstants.Plans] = plans;

        return plans;
    }
    #endregion

    #region -- Index Action --
    public ActionResult Index()
    {
        return View(new LabelViewDto());
    }

    public async Task<ActionResult> GetAllLabelsIndexViewDto([DataSourceRequest] DataSourceRequest request)
    {
        try
        {
            // Since all services query the same Label table, we can use any service's GetQuery
            // But to get all labels regardless of type, we'll use one service's query
            // All services inherit from ILabelService which provides GetQuery()
            var query = _partLabelService.GetQuery().AsNoTracking();

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
                    Status = label.Status,
                    LabelType = label.LabelType,
                    CarcassCode = label.CarcassCode,
                    AssemblyCode = label.AssemblyCode,
                    Barcode = label.Barcode
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

    public async Task<ActionResult> View(int? id, string labelType = null)
    {
        try
        {
            if (id == null)
                throw new Exception("Invalid Id");

            LabelViewDto dto = null;

            // Determine which service to use based on labelType or try to detect from label
            if (string.IsNullOrEmpty(labelType))
            {
                // Try to get label from any service to determine type
                var label = _partLabelService.GetQuery().FirstOrDefault(l => l.Id == id) ??
                           _storeKhalapurLabelService.GetQuery().FirstOrDefault(l => l.Id == id) ??
                           _storeShirwalLabelService.GetQuery().FirstOrDefault(l => l.Id == id) ??
                           _stiffenerLabelService.GetQuery().FirstOrDefault(l => l.Id == id);

                if (label != null)
                {
                    labelType = label.LabelType;
                }
            }

            // Get DTO from appropriate service based on label type
            if (!string.IsNullOrEmpty(labelType))
            {
                switch (labelType.ToLower())
                {
                    case "partlabel":
                    case "part":
                        dto = await _partLabelService.CreateViewDtoAsync(id).ConfigureAwait(false);
                        break;
                    case "storekhalapur":
                    case "store khalapur":
                        // StoreKhalapur doesn't have CreateViewDtoAsync, try PartLabel as fallback
                        dto = await _partLabelService.CreateViewDtoAsync(id).ConfigureAwait(false);
                        break;
                    case "storeshirwal":
                    case "store shirwal":
                        dto = await _storeShirwalLabelService.CreateViewDtoAsync(id).ConfigureAwait(false);
                        break;
                    case "stiffenerlabel":
                    case "stiffener":
                        dto = await _stiffenerLabelService.CreateViewDtoAsync(id).ConfigureAwait(false);
                        break;
                }
            }

            // Fallback: try PartLabel first
            if (dto == null)
            {
                dto = await _partLabelService.CreateViewDtoAsync(id).ConfigureAwait(false);
            }

            return View(dto);
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }

        return RedirectToAction("Index");
    }
    #endregion

    #region -- PartLabel Actions --
    public Task<ActionResult> CreatePartLabel(string warehouseOrderNo)
    {
        if (string.IsNullOrEmpty(warehouseOrderNo))
            Session[FieldConstants.Label] = null;

        return Task.FromResult<ActionResult>(View(new PartLabelCrudDto { WarehouseOrderNo = warehouseOrderNo }));
    }

    [HttpPost]
    public async Task<ActionResult> PrintPartLabel(PartLabelCrudDto dto)
    {
        if (!ModelState.IsValid)
            return View("CreatePartLabel", dto);
        try
        {
            var plan = await GetPlanAsync(dto.WarehouseOrderNo).ConfigureAwait(false);
            var labels = await _partLabelService.CreateLabelsAsync(dto, plan).ConfigureAwait(false);
            Session[FieldConstants.Label] = await _partLabelService.CreateLabelReportAsync(labels, false).ConfigureAwait(false);
            await _partLabelService.UpdateDatabaseAsync(labels, plan).ConfigureAwait(false);
            dto.Clear();
            ModelState.Clear();

            var report = await _partLabelService.CreateLabelReportAsync(labels, false).ConfigureAwait(false);
            return File(report.ToDocumentBytes(), "application/pdf");
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
            var message = LogHandler.GetDetailException(exception)?.Message;
            return Json(new { Success = false, Message = message },
                JsonRequestBehavior.AllowGet);
        }
    }

    [HttpPost]
    public async Task<ActionResult> PreviewPartLabel(PartLabelCrudDto dto)
    {
        if (!ModelState.IsValid)
            return View("CreatePartLabel", dto);
        try
        {
            var plan = await GetPlanAsync(dto.WarehouseOrderNo).ConfigureAwait(false);
            var labels = await _partLabelService.CreateLabelsAsync(dto, plan).ConfigureAwait(false);
            var report = await _partLabelService.CreateLabelReportAsync(labels, false).ConfigureAwait(false);
            return File(report.ToDocumentBytes(), "application/pdf");
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
            var message = LogHandler.GetDetailException(exception)?.Message;
            return Json(new { Success = false, Message = message },
                JsonRequestBehavior.AllowGet);
        }
    }

    public async Task<ActionResult> GetPartLabelItems([DataSourceRequest] DataSourceRequest request, string warehouseOrderNo)
    {
        try
        {
            if (string.IsNullOrEmpty(warehouseOrderNo))
                return Json(null, JsonRequestBehavior.AllowGet);

            var plan = await GetPlanAsync(warehouseOrderNo).ConfigureAwait(false);
            var pendingItems = await _partLabelService.GetPendingItemsAsync(plan).ConfigureAwait(false);

            var itemsList = pendingItems.Cast<object>().ToList();
            var queryableItems = itemsList.AsQueryable();

            var result = queryableItems.ToDataSourceResult(request);
            return Json(result.Data, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            return GetComboBoxErrorResult(exception);
        }
    }

    public async Task<ActionResult> GetPartLabelIndexViewDto([DataSourceRequest] DataSourceRequest request)
    {
        try
        {
            var result = await Task.Run(() => _partLabelService.GetIndexDataSource(request)).ConfigureAwait(false);
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

    #region -- StoreKhalapur Actions --
    public ActionResult CreateStoreKhalapur(string warehouseOrderNo)
    {
        var dto = new StoreLabelCrudDto();

        try
        {
            if (string.IsNullOrEmpty(warehouseOrderNo))
                Session[FieldConstants.Label] = null;

            return View(dto);
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }
        return View(dto);
    }

    [HttpPost]
    [MultipleButton(Name = "action", Argument = "Print")]
    public async Task<ActionResult> CreateStoreKhalapur(StoreLabelCrudDto dto)
    {
        if (!ModelState.IsValid)
            return View("CreateStoreKhalapur", dto);
        try
        {
            var plan = await GetPlanAsync(dto.WarehouseOrderNo).ConfigureAwait(false);
            var labels = await _storeKhalapurLabelService.CreateLabelsAsync(dto, plan).ConfigureAwait(false);
            Session[FieldConstants.Label] = await _storeKhalapurLabelService.CreateLabelReportAsync(labels, false).ConfigureAwait(false);
            await _storeKhalapurLabelService.UpdateDatabaseAsync(labels, plan).ConfigureAwait(false);
            dto.PrintToPrinter = true;
            dto.Clear();
            ModelState.Clear();
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }

        return View("CreateStoreKhalapur", dto);
    }

    [HttpPost]
    [MultipleButton(Name = "action", Argument = "Preview")]
    public async Task<ActionResult> PreviewStoreKhalapur(StoreLabelCrudDto dto)
    {
        if (!ModelState.IsValid)
            return View("CreateStoreKhalapur", dto);
        try
        {
            var plan = await GetPlanAsync(dto.WarehouseOrderNo).ConfigureAwait(false);
            var labels = await _storeKhalapurLabelService.CreateLabelsAsync(dto, plan).ConfigureAwait(false);
            Session[FieldConstants.Label] = await _storeKhalapurLabelService.CreateLabelReportAsync(labels, false).ConfigureAwait(false);
            dto.PrintToPrinter = false;
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }
        return View("CreateStoreKhalapur", dto);
    }

    public async Task<ActionResult> GetStoreKhalapurIndexViewDto([DataSourceRequest] DataSourceRequest request)
    {
        try
        {
            var query = _storeKhalapurLabelService.GetQuery().AsNoTracking();
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

    public async Task<ActionResult> GetStoreKhalapurLotNos([DataSourceRequest] DataSourceRequest request, DateTime dueDate)
    {
        try
        {
            var lotNos = await _planService.GetPendingLotNosAsync(dueDate).ConfigureAwait(false);
            return Json(lotNos, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            return GetComboBoxErrorResult(exception);
        }
    }

    public async Task<ActionResult> GetStoreKhalapurWarehouseOrderNos([DataSourceRequest] DataSourceRequest request, string lotNo)
    {
        try
        {
            var data = await _planService.GetPendingWarehouseOrdersAsync(lotNo).ConfigureAwait(false);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            return GetComboBoxErrorResult(exception);
        }
    }

    public async Task<ActionResult> GetStoreKhalapurFamilies([DataSourceRequest] DataSourceRequest request, string warehouseOrderNo)
    {
        try
        {
            var plan = await GetPlanAsync(warehouseOrderNo).ConfigureAwait(false);
            var families = await _planService.GetPendingFamiliesAsync(plan).ConfigureAwait(false);
            return Json(families, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            return GetComboBoxErrorResult(exception);
        }
    }

    public async Task<ActionResult> GetStoreKhalapurItems([DataSourceRequest] DataSourceRequest request, string warehouseOrderNo,
        string family)
    {
        if (string.IsNullOrEmpty(warehouseOrderNo))
            return Json(null, JsonRequestBehavior.AllowGet);

        try
        {
            var plan = await GetPlanAsync(warehouseOrderNo).ConfigureAwait(false);
            var items = await _storeKhalapurLabelService.GetPendingItemsAsync(plan, family).ConfigureAwait(false);

            return Json((await items.ToDataSourceResultAsync(request).ConfigureAwait(false)).Data);
        }
        catch (Exception exception)
        {
            return GetComboBoxErrorResult(exception);
        }
    }
    #endregion

    #region -- StoreShirwal Actions --
    public ActionResult CreateStoreShirwal()
    {
        var viewModel = new StoreShirwalCrudDto();
        try
        {
            Session[FieldConstants.Label] = null;
            return View(viewModel);
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }
        return View(viewModel);
    }

    [HttpPost]
    [MultipleButton(Name = "action", Argument = "Print")]
    public async Task<ActionResult> CreateStoreShirwal(StoreShirwalCrudDto dto)
    {
        if (!ModelState.IsValid)
            return View("CreateStoreShirwal", dto);
        try
        {
            var plan = await GetPlanAsync(dto.WarehouseOrderNo).ConfigureAwait(false);
            var labels = await _storeShirwalLabelService.CreateLabelsAsync(dto, plan).ConfigureAwait(false);
            Session[FieldConstants.Label] = await _storeShirwalLabelService.CreateLabelReportAsync(labels, false).ConfigureAwait(false);
            await _storeShirwalLabelService.UpdateDatabaseAsync(labels, plan).ConfigureAwait(false);
            dto.PrintToPrinter = true;
            dto.Clear();
            ModelState.Clear();
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }

        return View("CreateStoreShirwal", dto);
    }

    [HttpPost]
    [MultipleButton(Name = "action", Argument = "Preview")]
    public async Task<ActionResult> PreviewStoreShirwal(StoreShirwalCrudDto dto)
    {
        if (!ModelState.IsValid)
            return View("CreateStoreShirwal", dto);
        try
        {
            var plan = await GetPlanAsync(dto.WarehouseOrderNo).ConfigureAwait(false);
            var labels = await _storeShirwalLabelService.CreateLabelsAsync(dto, plan).ConfigureAwait(false);
            Session[FieldConstants.Label] = await _storeShirwalLabelService.CreateLabelReportAsync(labels, false).ConfigureAwait(false);
            dto.PrintToPrinter = false;
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }
        return View("CreateStoreShirwal", dto);
    }

    public async Task<ActionResult> GetStoreShirwalFamilies([DataSourceRequest] DataSourceRequest request, string warehouseOrderNo)
    {
        try
        {
            var plan = await GetPlanAsync(warehouseOrderNo).ConfigureAwait(false);
            var families = await _planService.GetPendingFamiliesAsync(plan).ConfigureAwait(false);
            return Json(families, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            return GetComboBoxErrorResult(exception);
        }
    }

    public async Task<ActionResult> GetStoreShirwalIndexViewDto([DataSourceRequest] DataSourceRequest request)
    {
        try
        {
            var query = _storeShirwalLabelService.GetQuery().AsNoTracking();
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

    #region -- StiffenerLabel Actions --
    public ActionResult CreateStiffenerLabel()
    {
        var dto = new StiffenerLabelCrudDto();
        try
        {
            Session[FieldConstants.Label] = null;
            return View(dto);
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }
        return View(dto);
    }

    [HttpPost]
    [MultipleButton(Name = "action", Argument = "Print")]
    public async Task<ActionResult> CreateStiffenerLabel(StiffenerLabelCrudDto dto)
    {
        if (!ModelState.IsValid)
            return View("CreateStiffenerLabel", dto);
        try
        {
            var plans = await GetPlansAsync(dto.LotNo, dto.DrawingNo).ConfigureAwait(false);
            var labels = await _stiffenerLabelService.CreateLabelsAsync(dto, plans).ConfigureAwait(false);
            Session[FieldConstants.Label] = await _stiffenerLabelService.CreateLabelReportAsync(labels, false).ConfigureAwait(false);
            await _stiffenerLabelService.UpdateDatabaseAsync(labels, new Plan()).ConfigureAwait(false);
            dto.PrintToPrinter = true;
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }

        return View("CreateStiffenerLabel", dto);
    }

    [HttpPost]
    [MultipleButton(Name = "action", Argument = "Preview")]
    public async Task<ActionResult> PreviewStiffenerLabel(StiffenerLabelCrudDto dto)
    {
        if (!ModelState.IsValid)
            return View("CreateStiffenerLabel", dto);
        try
        {
            var plans = await GetPlansAsync(dto.LotNo, dto.DrawingNo).ConfigureAwait(false);
            var labels = await _stiffenerLabelService.CreateLabelsAsync(dto, plans).ConfigureAwait(false);
            Session[FieldConstants.Label] = await _stiffenerLabelService.CreateLabelReportAsync(labels, false).ConfigureAwait(false);
            dto.PrintToPrinter = false;
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }
        return View("CreateStiffenerLabel", dto);
    }

    public async Task<ActionResult> GetStiffenerLabelDrawingNo([DataSourceRequest] DataSourceRequest request, string lotNo)
    {
        try
        {
            var plans = Session[FieldConstants.Plans] as List<Plan>;
            var lotNoPlans = plans?.Where(p => p.LotNo == lotNo && p.PlanItemDetails.Any(d => (d.OrderQuantity ?? 0) > (d.PrintQuantity ?? 0)))
                .ToList();

            Session[FieldConstants.Plans] = lotNoPlans;

            var drawingNos = lotNoPlans?.SelectMany(p => p.PlanItemDetails.Where(d =>
                    (d.OrderQuantity ?? 0) > (d.PrintQuantity ?? 0)), (_, d) => new { d.DrawingNo })
                .Distinct().ToList();
            return Json(drawingNos, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            return GetComboBoxErrorResult(exception);
        }
    }

    public async Task<ActionResult> GetStiffenerLabelLotNos([DataSourceRequest] DataSourceRequest request, DateTime dueDate)
    {
        try
        {
            var plans = (await _planService.GetAsync<Plan>(p => DbFunctions.TruncateTime(p.DueDate) == DbFunctions.TruncateTime(dueDate)
                                              && p.PlanItemDetails.Any(d => (d.OrderQuantity ?? 0) > (d.PrintQuantity ?? 0)),
                p => p).ConfigureAwait(false)).ToList();

            Session[FieldConstants.Plans] = plans;

            var dataSource = plans.Select(p => new { p.LotNo }).Distinct();
            return Json(dataSource, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            return GetComboBoxErrorResult(exception);
        }
    }

    public async Task<ActionResult> GetStiffenerLabelIndexViewDto([DataSourceRequest] DataSourceRequest request)
    {
        try
        {
            var query = _stiffenerLabelService.GetQuery().AsNoTracking();
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

    #region -- Common Actions --
    public ActionResult LoadPartialView(string partialViewName)
    {
        // This method loads partial views for report viewer
        // The partialViewName is typically "Partials/ReportViewer_New"
        return PartialView(partialViewName, Session[FieldConstants.Label]);
    }
    #endregion
}

