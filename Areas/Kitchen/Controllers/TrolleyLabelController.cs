using System;
using System.Linq;
using System.Web.Mvc;
using Corno.Web.Controllers;
using Kendo.Mvc.UI;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using Corno.Web.Areas.Kitchen.Dto.Label;
using Corno.Web.Areas.Kitchen.Dto.TrolleyLabel;
using Corno.Web.Areas.Kitchen.Services.Interfaces;
using Corno.Web.Globals;
using Corno.Web.Models.Packing;
using Corno.Web.Models.Plan;
using Corno.Web.Attributes;
using Mapster;
using Kendo.Mvc.Extensions;

namespace Corno.Web.Areas.Kitchen.Controllers;

[Authorize]
public class TrolleyLabelController : SuperController
{
    #region -- Constructors --
    public TrolleyLabelController(ITrolleyLabelService trolleyLabelService,
        IPlanService planService)
    {
        _trolleyLabelService = trolleyLabelService;
        _planService = planService;

        const string viewPath = "~/Areas/Kitchen/Views/TrolleyLabel";
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
    private readonly ITrolleyLabelService _trolleyLabelService;
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

    public virtual ActionResult Create(string warehouseOrderNo)
    {
        if (string.IsNullOrEmpty(warehouseOrderNo))
            Session[FieldConstants.Label] = null;

        return View(new TrolleyLabelCrudDto());
    }

    [HttpPost]
    //[ValidateAntiForgeryToken]
    [MultipleButton(Name = "action", Argument = "Print")]
    public async Task<ActionResult> Create(TrolleyLabelCrudDto dto)
    {
        if (!ModelState.IsValid)
            return View(_createPath, dto);
        try
        {
            // Get Plan
            var plan = await GetPlanAsync(dto.WarehouseOrderNo).ConfigureAwait(false);
            // Create Labels
            var labels = await _trolleyLabelService.CreateLabelsAsync(dto, plan).ConfigureAwait(false);
            // Create Label Reports
            Session[FieldConstants.Label] = await _trolleyLabelService.CreateLabelReportAsync(labels, false).ConfigureAwait(false);
            // Save in database
            await _trolleyLabelService.UpdateDatabaseAsync(labels, plan).ConfigureAwait(false);
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
    public async Task<ActionResult> Preview(TrolleyLabelCrudDto dto)
    {
        if (!ModelState.IsValid)
            return View(_createPath, dto);
        try
        {
            //dto.Clear();
            // Get Plan
            var plan = await GetPlanAsync(dto.WarehouseOrderNo).ConfigureAwait(false);
            // Create Labels
            var labels = await _trolleyLabelService.CreateLabelsAsync(dto, plan).ConfigureAwait(false);
            // Create Label Reports
           Session[FieldConstants.Label] = await _trolleyLabelService.CreateLabelReportAsync(labels, false).ConfigureAwait(false);
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

            var dto = await _trolleyLabelService.CreateViewDtoAsync(id).ConfigureAwait(false);
            Session[FieldConstants.Label] = dto.LabelReport;

            return View(dto);
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }

        return RedirectToAction("Index");
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
            // Trolley labels use specific selected groups
            var selectedGroups = new List<string> { "FGWNO1", "FGWN04", "FGWN06", "FGWN10" };
            var families = await _planService.GetPendingFamiliesAsync(plan, selectedGroups).ConfigureAwait(false);
            return Json(families, JsonRequestBehavior.AllowGet);
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
            var lotNos = await _planService.GetPendingLotNosAsync(dueDate).ConfigureAwait(false);
            return Json(lotNos, JsonRequestBehavior.AllowGet);
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
            var query = _trolleyLabelService.GetQuery().AsNoTracking();
            var data = from label in query
                select new LabelIndexDto
                {
                    Id = label.Id,
                    LabelDate = label.LabelDate,
                    LotNo = label.LotNo,
                    WarehouseOrderNo = label.WarehouseOrderNo,
                    CarcassCode = label.CarcassCode,
                    OrderQuantity = label.OrderQuantity ?? 0,
                    Quantity = label.Quantity ?? 0,
                    Status = label.Status
                };
            var result = await data.ToDataSourceResultAsync(request);
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