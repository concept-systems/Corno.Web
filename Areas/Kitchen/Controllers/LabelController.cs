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
using Corno.Web.Globals.Enums;
using Corno.Web.Logger;
using Corno.Web.Models.Packing;
using Corno.Web.Models.Plan;
using Corno.Web.Windsor;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Mapster;

namespace Corno.Web.Areas.Kitchen.Controllers;

[Authorize]
public class LabelController : SuperController
{
    #region -- Constructors --
    public LabelController()
    {
        _labelService = Bootstrapper.Get<ILabelService>();
        PlanService = Bootstrapper.Get<IPlanService>();
        ItemService = Bootstrapper.Get<IItemService>();

        PartLabelService = Bootstrapper.Get<IPartLabelService>();
        StoreKhalapurLabelService = Bootstrapper.Get<IStoreKhalapurLabelService>();
        StoreShirwalLabelService = Bootstrapper.Get< IStoreShirwalLabelService>();
        StiffenerLabelService = Bootstrapper.Get<IStiffenerLabelService>();

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

    private readonly ILabelService _labelService;
    protected readonly IPartLabelService PartLabelService;
    protected readonly IStoreKhalapurLabelService StoreKhalapurLabelService;
    protected readonly IStoreShirwalLabelService StoreShirwalLabelService;
    protected readonly IStiffenerLabelService StiffenerLabelService;
    protected readonly IPlanService PlanService;
    protected readonly IItemService ItemService;
    #endregion

    #region -- Private Methods --
    private async Task<Plan> GetPlanAsync(string warehouseOrderNo)
    {
        if (Session[FieldConstants.Plan] is Plan plan &&
            plan.WarehouseOrderNo.Equals(warehouseOrderNo))
            return plan;

        plan = await PlanService.GetByWarehouseOrderNoAsync(warehouseOrderNo).ConfigureAwait(false);
        Session[FieldConstants.Plan] = plan ?? throw new Exception($"Warehouse order {warehouseOrderNo} not found");

        return plan;
    }

    private async Task<List<Plan>> GetPlansAsync(string lotNo, string drawingNo)
    {
        if (Session[FieldConstants.Plans] is List<Plan> plans &&
            plans.Any(d => !d.LotNo.Equals(lotNo)))
            return plans;
        plans = (await PlanService.GetAsync<Plan>(p => p.LotNo == lotNo &&
                                      p.PlanItemDetails.Any(d =>
                                          d.DrawingNo == drawingNo), p => p).ConfigureAwait(false)).ToList();
        Session[FieldConstants.Plans] = plans;

        return plans;
    }
    #endregion

        #region -- Index Action --
    public virtual ActionResult Index()
    {
        return View(new LabelViewDto());
    }

    public virtual async Task<ActionResult> GetIndexViewDto([DataSourceRequest] DataSourceRequest request)
    {
        try
        {
            var result = await _labelService.GetIndexDataSourceAsync(request)
                .ConfigureAwait(false);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
            ModelState.AddModelError("Error", exception.Message);
            return Json(new DataSourceResult { Errors = ModelState });
        }
    }

    public virtual async Task<ActionResult> View(int? id, string labelType = null)
    {
        try
        {
            if (id == null)
                throw new Exception("Invalid Id");

            // Parse labelType string to LabelType enum
            LabelType? parsedLabelType = null;
            if (!string.IsNullOrEmpty(labelType))
            {
                if (Enum.TryParse<LabelType>(labelType, true, out var parsedType))
                {
                    parsedLabelType = parsedType;
                }
            }

            var dto = await _labelService.CreateViewDtoAsync(id, parsedLabelType).ConfigureAwait(false);
            
            // Determine the view path based on the labelType
            string viewPath = null;
            
            if (parsedLabelType.HasValue)
            {
                switch (parsedLabelType.Value)
                {
                    case LabelType.Part:
                        viewPath = "~/Areas/Kitchen/Views/PartLabel/View.cshtml";
                        break;
                    case LabelType.StoreKhalapur:
                        viewPath = "~/Areas/Kitchen/Views/StoreKhalapur/View.cshtml";
                        break;
                    case LabelType.StoreShirwal:
                        viewPath = "~/Areas/Kitchen/Views/StoreShirwal/View.cshtml";
                        break;
                    case LabelType.Stiffener:
                        viewPath = "~/Areas/Kitchen/Views/StiffenerLabel/View.cshtml";
                        break;
                }
            }
            
            // If a specific view path was determined, return that view; otherwise use default view resolution
            return viewPath != null ? View(viewPath, dto) : View(dto);
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
            var labels = await PartLabelService.CreateLabelsAsync(dto, plan).ConfigureAwait(false);
            Session[FieldConstants.Label] = await PartLabelService.CreateLabelReportAsync(labels, false).ConfigureAwait(false);
            await PartLabelService.UpdateDatabaseAsync(labels, plan).ConfigureAwait(false);
            dto.Clear();
            ModelState.Clear();

            var report = await PartLabelService.CreateLabelReportAsync(labels, false).ConfigureAwait(false);
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
            var labels = await PartLabelService.CreateLabelsAsync(dto, plan).ConfigureAwait(false);
            var report = await PartLabelService.CreateLabelReportAsync(labels, false).ConfigureAwait(false);
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
            var pendingItems = await PartLabelService.GetPendingItemsAsync(plan).ConfigureAwait(false);

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
            var result = await Task.Run(() => PartLabelService.GetIndexDataSourceAsync(request)).ConfigureAwait(false);
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
            var labels = await StoreKhalapurLabelService.CreateLabelsAsync(dto, plan).ConfigureAwait(false);
            Session[FieldConstants.Label] = await StoreKhalapurLabelService.CreateLabelReportAsync(labels, false).ConfigureAwait(false);
            await StoreKhalapurLabelService.UpdateDatabaseAsync(labels, plan).ConfigureAwait(false);
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
            var labels = await StoreKhalapurLabelService.CreateLabelsAsync(dto, plan).ConfigureAwait(false);
            Session[FieldConstants.Label] = await StoreKhalapurLabelService.CreateLabelReportAsync(labels, false).ConfigureAwait(false);
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
            var query = StoreKhalapurLabelService.GetQuery().AsNoTracking();
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
            var lotNos = await PlanService.GetPendingLotNosAsync(dueDate).ConfigureAwait(false);
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
            var data = await PlanService.GetPendingWarehouseOrdersAsync(lotNo).ConfigureAwait(false);
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
            var families = await PlanService.GetPendingFamiliesAsync(plan).ConfigureAwait(false);
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
            var items = await StoreKhalapurLabelService.GetPendingItemsAsync(plan, family).ConfigureAwait(false);

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
            var labels = await StoreShirwalLabelService.CreateLabelsAsync(dto, plan).ConfigureAwait(false);
            Session[FieldConstants.Label] = await StoreShirwalLabelService.CreateLabelReportAsync(labels, false).ConfigureAwait(false);
            await StoreShirwalLabelService.UpdateDatabaseAsync(labels, plan).ConfigureAwait(false);
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
            var labels = await StoreShirwalLabelService.CreateLabelsAsync(dto, plan).ConfigureAwait(false);
            Session[FieldConstants.Label] = await StoreShirwalLabelService.CreateLabelReportAsync(labels, false).ConfigureAwait(false);
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
            var families = await PlanService.GetPendingFamiliesAsync(plan).ConfigureAwait(false);
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
            var result = await Task.Run(() => StoreShirwalLabelService.GetIndexDataSourceAsync(request)).ConfigureAwait(false);
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
            var labels = await StiffenerLabelService.CreateLabelsAsync(dto, plans).ConfigureAwait(false);
            Session[FieldConstants.Label] = await StiffenerLabelService.CreateLabelReportAsync(labels, false).ConfigureAwait(false);
            await StiffenerLabelService.UpdateDatabaseAsync(labels, new Plan()).ConfigureAwait(false);
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
            var labels = await StiffenerLabelService.CreateLabelsAsync(dto, plans).ConfigureAwait(false);
            Session[FieldConstants.Label] = await StiffenerLabelService.CreateLabelReportAsync(labels, false).ConfigureAwait(false);
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
            var plans = (await PlanService.GetAsync<Plan>(p => DbFunctions.TruncateTime(p.DueDate) == DbFunctions.TruncateTime(dueDate)
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
            var result = await Task.Run(() => StiffenerLabelService.GetIndexDataSourceAsync(request)).ConfigureAwait(false);
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

