using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Corno.Web.Controllers;
using Kendo.Mvc.UI;
using Corno.Web.Areas.Kitchen.Dto.Label;
using Corno.Web.Areas.Kitchen.Services.Interfaces;
using Corno.Web.Globals;
using Corno.Web.Models.Packing;
using Mapster;
using Corno.Web.Models.Plan;
using Corno.Web.Attributes;
using Kendo.Mvc.Extensions;
using Corno.Web.Areas.Kitchen.Dto.StoreShirwalLabel;

namespace Corno.Web.Areas.Kitchen.Controllers;

[Authorize]
public class StoreShirwalController : SuperController
{
    #region -- Constructors --
    public StoreShirwalController(IStoreShirwalLabelService storeShirwalLabelService, IPlanService planService)
    {
        _storeShirwalLabelService = storeShirwalLabelService;
        _planService = planService;

        const string viewPath = "~/Areas/Kitchen/Views/StoreShirwal";
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
    private readonly IStoreShirwalLabelService _storeShirwalLabelService;
    private readonly IPlanService _planService;
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
        return View(new LabelViewDto());
    }

    public ActionResult Create()
    {
        var viewModel = new StoreShirwalCrudDto
        {
            //LabelReports = new List<BaseReport>()
        };
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
    //[ValidateAntiForgeryToken]
    [MultipleButton(Name = "action", Argument = "Print")]
    public async Task<ActionResult> Create(StoreShirwalCrudDto dto)
    {
        if (!ModelState.IsValid)
            return View(_createPath, dto);
        try
        {
            // Get Plan
            var plan = await GetPlanAsync(dto.WarehouseOrderNo).ConfigureAwait(false);
            // Create Labels
            var labels = await _storeShirwalLabelService.CreateLabelsAsync(dto, plan).ConfigureAwait(false);
            // Create Label Reports
            Session[FieldConstants.Label] = await _storeShirwalLabelService.CreateLabelReportAsync(labels, false).ConfigureAwait(false);
            // Save in database
            await _storeShirwalLabelService.UpdateDatabaseAsync(labels, plan).ConfigureAwait(false);
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
    public async Task<ActionResult> Preview(StoreShirwalCrudDto dto)
    {
        if (!ModelState.IsValid)
            return View(_createPath, dto);
        try
        {
            //dto.Clear();
            // Get Plan
            var plan = await GetPlanAsync(dto.WarehouseOrderNo).ConfigureAwait(false);
            // Create Labels
            var labels = await _storeShirwalLabelService.CreateLabelsAsync(dto, plan).ConfigureAwait(false);
            // Create Label Reports
            Session[FieldConstants.Label] = await _storeShirwalLabelService.CreateLabelReportAsync(labels, false).ConfigureAwait(false);
            dto.PrintToPrinter = false;
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }
        return View(_createPath, dto);
    }

    public async Task<ActionResult> View(int? id)
    {
        try
        {
            if (id == null)
                throw new Exception("Invalid Id");

            var dto = await _storeShirwalLabelService.CreateViewDtoAsync(id).ConfigureAwait(false);
            Session[FieldConstants.Label] = dto.LabelReport;

            return View(dto);
            //return View(viewModel);
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }

        return RedirectToAction("Index");
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


    //public ActionResult GetFamilies([DataSourceRequest] DataSourceRequest request, string warehouseOrderNo)
    //{

    //    var families = _planService.GetFamilies(warehouseOrderNo,
    //        new List<string> { FieldConstants.Bo}, 1);

    //    return Json(families, JsonRequestBehavior.AllowGet);
    //}


    public async Task<ActionResult> GetIndexViewDto([DataSourceRequest] DataSourceRequest request)
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



    //public async Task<ActionResult> GetIndexViewModels([DataSourceRequest] DataSourceRequest request)
    //{
    //    try
    //    {
    //        var query = _storeShirwalLabelService.GetQuery();
    //        var result = await query.ToDataSourceResultAsync(request);
    //        return Json(result, JsonRequestBehavior.AllowGet);
    //    }
    //    catch (Exception exception)
    //    {
    //        return Json(new DataSourceResult { Errors = exception.Message });
    //    }
    //}
    #endregion
}