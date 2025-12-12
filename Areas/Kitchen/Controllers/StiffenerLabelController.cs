using System;
using System.Linq;
using System.Web.Mvc;
using Corno.Web.Controllers;
using Kendo.Mvc.UI;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using Corno.Web.Areas.BoardStore.Services.Interfaces;
using Corno.Web.Areas.Kitchen.Dto.Label;
using Corno.Web.Areas.Kitchen.Services.Interfaces;
using Kendo.Mvc.Extensions;
using Mapster;
using Corno.Web.Attributes;
using Corno.Web.Areas.Kitchen.Dto.StiffenerLabel;
using Corno.Web.Globals;
using Corno.Web.Models.Packing;
using Corno.Web.Models.Plan;

namespace Corno.Web.Areas.Kitchen.Controllers;

[Authorize]
public class StiffenerLabelController : SuperController
{
    #region -- Constructors --
    public StiffenerLabelController(IStiffenerLabelService stiffenerLabelService, IItemService itemService, IPlanService planService)
    {
        _stiffenerLabelService = stiffenerLabelService;
        _planService = planService;
        _itemService = itemService;

        const string viewPath = "~/Areas/Kitchen/Views/StiffenerLabel";
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
    private readonly IStiffenerLabelService _stiffenerLabelService;
    private readonly IPlanService _planService;
    private readonly IItemService _itemService;


    #endregion

    #region -- Private Methods --
    private async Task<List<Plan>> GetPlansAsync(StiffenerLabelCrudDto dto)
    {
        if (Session[FieldConstants.Plans] is List<Plan> plans &&
            plans.Any(d => !d.LotNo.Equals(dto.LotNo)))
            return plans;
        plans = (await _planService.GetAsync<Plan>(p => p.LotNo == dto.LotNo &&
                                      p.PlanItemDetails.Any(d =>
                                          d.DrawingNo == dto.DrawingNo), p => p).ConfigureAwait(false)).ToList();
        Session[FieldConstants.Plans] = plans;

        return plans;
    }
    #endregion

    #region -- Methods --
    public ActionResult Index()
    {
        return View(new StiffenerLabelCrudDto());
    }

    #endregion

    #region -- Actions --
    public ActionResult Create()
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
    public async Task<ActionResult> Create(StiffenerLabelCrudDto dto)
    {
        if (!ModelState.IsValid)
            return View(_createPath, dto);
        try
        {
            // Get Plan
            var plan = await GetPlansAsync(dto).ConfigureAwait(false);
            // Create Labels
            var labels = await _stiffenerLabelService.CreateLabelsAsync(dto,plan).ConfigureAwait(false);
            // Create Label Reports
            Session[FieldConstants.Label] = await _stiffenerLabelService.CreateLabelReportAsync(labels, false).ConfigureAwait(false);
            // Save in database
            await _stiffenerLabelService.UpdateDatabaseAsync(labels, new Plan()).ConfigureAwait(false);
            dto.PrintToPrinter = true;
            //dto.Clear();
            //ModelState.Clear();
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }

        return View(_createPath, dto);
    }

    [HttpPost]
    [MultipleButton(Name = "action", Argument = "Preview")]
    public async Task<ActionResult> Preview(StiffenerLabelCrudDto dto)
    {
        if (!ModelState.IsValid)
            return View(_createPath, dto);
        try
        {
            //dto.Clear();
            // Get Plan
            var plans = await GetPlansAsync(dto).ConfigureAwait(false);
            // Create Labels
            var labels = await _stiffenerLabelService.CreateLabelsAsync(dto, plans).ConfigureAwait(false);
            // Create Label Reports
            Session[FieldConstants.Label] = await _stiffenerLabelService.CreateLabelReportAsync(labels, false).ConfigureAwait(false);
            dto.PrintToPrinter = false;
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }
        return View(_createPath, dto);
    }
    public async Task<ActionResult> GetDrawingNo([DataSourceRequest] DataSourceRequest request, string lotNo)
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
    
    public async Task<ActionResult> GetLotNos([DataSourceRequest] DataSourceRequest request, DateTime dueDate)
    {
        try
        {
            var plans = (await _planService.GetAsync<Plan>(p => DbFunctions.TruncateTime(p.DueDate) == DbFunctions.TruncateTime(dueDate)
                                              && p.PlanItemDetails.Any(d => (d.OrderQuantity ?? 0) > (d.PrintQuantity ?? 0)),
                p => p).ConfigureAwait(false)).ToList();

            Session[FieldConstants.Plans] = plans;
                
            var dataSource = plans.Select(p => new {p.LotNo}).Distinct();
            return Json(dataSource, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            return GetComboBoxErrorResult(exception);
        }
    }

    public async Task<ActionResult> GetIndexViewDto([DataSourceRequest] DataSourceRequest request)
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
}