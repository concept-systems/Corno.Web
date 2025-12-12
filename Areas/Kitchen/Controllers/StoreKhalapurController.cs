using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Corno.Web.Areas.BoardStore.Services.Interfaces;
using Corno.Web.Controllers;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using Corno.Web.Areas.Kitchen.Dto.StoreLabel;
using Corno.Web.Areas.Kitchen.Services.Interfaces;
using Corno.Web.Areas.Kitchen.Dto.Label;
using Corno.Web.Attributes;
using Corno.Web.Globals;
using Corno.Web.Models.Packing;
using Corno.Web.Models.Plan;
using Mapster;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Corno.Web.Areas.Kitchen.Controllers;

[Authorize]
public class StoreKhalapurController : SuperController
{
    #region -- Constructors --
    public StoreKhalapurController(IStoreKhalapurLabelService storeKhalapurLabelService, IItemService itemService, IPlanService planService)
    {
        _storeKhalapurLabelService = storeKhalapurLabelService;
        _planService = planService;
        _itemService = itemService;

        const string viewPath = "~/Areas/Kitchen/Views/StoreKhalapur";
        _createPath = $"{viewPath}/Create.cshtml";

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
    private readonly string _createPath;
    private readonly IStoreKhalapurLabelService _storeKhalapurLabelService;
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
    #endregion
    #region -- Actions --

    public ActionResult Index()
    {
        return View(new StoreLabelCrudDto());
    }

    public ActionResult Create(string warehouseOrderNo)
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
    //[ValidateAntiForgeryToken]
    [MultipleButton(Name = "action", Argument = "Print")]
    public async Task<ActionResult> Create(StoreLabelCrudDto dto)
    {
        if (!ModelState.IsValid)
            return View(_createPath, dto);
        try
        {
            // Get Plan
            var plan = await GetPlanAsync(dto.WarehouseOrderNo).ConfigureAwait(false);
            // Create Labels
            var labels = await _storeKhalapurLabelService.CreateLabelsAsync(dto, plan).ConfigureAwait(false);
            // Create Label Reports
            Session[FieldConstants.Label] = await _storeKhalapurLabelService.CreateLabelReportAsync(labels, false).ConfigureAwait(false);
            // Save in database
            await _storeKhalapurLabelService.UpdateDatabaseAsync(labels, plan).ConfigureAwait(false);
            dto.PrintToPrinter = true;
            dto.Clear();
            ModelState.Clear();
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }

        return View(_createPath, dto);
    }

    [HttpPost]
    [MultipleButton(Name = "action", Argument = "Preview")]
    public async Task<ActionResult> Preview(StoreLabelCrudDto dto)
    {
        if (!ModelState.IsValid)
            return View(_createPath, dto);
        try
        {
            //dto.Clear();
            // Get Plan
            var plan = await GetPlanAsync(dto.WarehouseOrderNo).ConfigureAwait(false);
            // Create Labels
            var labels = await _storeKhalapurLabelService.CreateLabelsAsync(dto, plan).ConfigureAwait(false);
            // Create Label Reports
            Session[FieldConstants.Label] = await _storeKhalapurLabelService.CreateLabelReportAsync(labels, false).ConfigureAwait(false);
            dto.PrintToPrinter = false;
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }
        return View(_createPath, dto);
    }

    public async Task<ActionResult> GetIndexViewDto([DataSourceRequest] DataSourceRequest request)
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

    //public ActionResult GetIndexViewDto([DataSourceRequest] DataSourceRequest request)
    //{
    //    try
    //    {
    //        var query = _storeKhalapurLabelService.GetQuery();
    //        var result = query.ToDataSourceResult(request);
    //        return Json(result, JsonRequestBehavior.AllowGet);
    //    }
    //    catch (Exception exception)
    //    {
    //        HandleControllerException(exception);
    //    }
    //    return Json(null, JsonRequestBehavior.AllowGet);
    //}

    public async Task<ActionResult> GetLotNos([DataSourceRequest] DataSourceRequest request, DateTime dueDate)
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

    public async Task<ActionResult> GetWarehouseOrderNos([DataSourceRequest] DataSourceRequest request, string lotNo)
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

    public async Task<ActionResult> GetFamilies([DataSourceRequest] DataSourceRequest request, string warehouseOrderNo)
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

    public async Task<ActionResult> GetItems([DataSourceRequest] DataSourceRequest request, string warehouseOrderNo,
        string family)
    {
        if (string.IsNullOrEmpty(warehouseOrderNo))
            return Json(null, JsonRequestBehavior.AllowGet);

        try
        {
            var plan = await GetPlanAsync(warehouseOrderNo).ConfigureAwait(false);
            /*var labelCrudDetailDtos = plan.PlanItemDetails
                .Where(d => d.Group.Equals(family) && d.ItemType.Equals(FieldConstants.Bo))
                .Select(d => new StoreLabelCrudDetailDto
                {
                    ItemCode = d.ItemCode,
                    ItemName = d.Description,
                    Position = d.Position,
                    IsSelected = false,
                })
                .ToList();*/
            var items = await _storeKhalapurLabelService.GetPendingItemsAsync(plan, family).ConfigureAwait(false);

            return Json((await items.ToDataSourceResultAsync(request).ConfigureAwait(false)).Data);
        }
        catch (Exception exception)
        {
            return GetComboBoxErrorResult(exception);
        }
    }
    #endregion
}