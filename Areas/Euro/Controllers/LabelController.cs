using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Corno.Web.Areas.Euro.Dto.Label;
using Corno.Web.Areas.Euro.Services.Interfaces;
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

namespace Corno.Web.Areas.Euro.Controllers;

[Authorize]
public class LabelController : SuperController
{
    #region -- Constructors --
    public LabelController()
    {
        PlanService = Bootstrapper.Get<IPlanService>();
        PartLabelService = Bootstrapper.Get<IPartLabelService>();

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

    protected readonly IPartLabelService PartLabelService;
    protected readonly IPlanService PlanService;
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
            var result = await PartLabelService.GetIndexDataSourceAsync(request)
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

            var dto = await PartLabelService.CreateViewDtoAsync(id, parsedLabelType).ConfigureAwait(false);
            
            // Determine the view path based on the labelType
            string viewPath = null;
            
            if (parsedLabelType.HasValue)
            {
                switch (parsedLabelType.Value)
                {
                    case LabelType.Part:
                        viewPath = "~/Areas/Euro/Views/PartLabel/View.cshtml";
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

        return Task.FromResult<ActionResult>(View(new PartLabelCrudDto { ProductionOrderNo = warehouseOrderNo }));
    }

    [HttpPost]
    public async Task<ActionResult> PrintPartLabel(PartLabelCrudDto dto)
    {
        if (!ModelState.IsValid)
            return View("CreatePartLabel", dto);
        try
        {
            var plan = await GetPlanAsync(dto.ProductionOrderNo).ConfigureAwait(false);
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
            var plan = await GetPlanAsync(dto.ProductionOrderNo).ConfigureAwait(false);
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

            var result = await queryableItems.ToDataSourceResultAsync(request);
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
}

