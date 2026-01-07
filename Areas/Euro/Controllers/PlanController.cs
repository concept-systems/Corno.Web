using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using Corno.Web.Areas.Euro.Dto.Plan;
using Corno.Web.Areas.Euro.Dto.Label;
using Corno.Web.Areas.Euro.Services.Interfaces;
using Corno.Web.Services.Import;
using Corno.Web.Services.Import.Interfaces;
using Kendo.Mvc.UI;

namespace Corno.Web.Areas.Euro.Controllers;

public class PlanController : BaseImportController<LabelImportDto>
{
    #region -- Constructors --
    public PlanController(IFileImportService<LabelImportDto> importService, IPlanService planService)
        : base(importService)
    {
        _planService = planService;

        const string viewPath = "~/Areas/Euro/Views/Plan";
        _createPath = $"{viewPath}/Create.cshtml";
        _editPath = $"{viewPath}/Edit.cshtml";
        _indexPath = $"{viewPath}/Index.cshtml";
        _createPath = $"{viewPath}/Create.cshtml";
        _previewPath = $"{viewPath}/View.cshtml";
    }

    #endregion

    #region -- Data Members --
    private readonly IPlanService _planService;

    private readonly string _indexPath;
    private readonly string _createPath;
    private readonly string _editPath;
    private readonly string _previewPath;
    #endregion

    #region -- BaseImportController Implementation --
    protected override string ControllerName => "Plan";
    #endregion
       
    #region -- Actions --
    public ActionResult Index()
    {
        return View(_indexPath);
    }

    public ActionResult GetTotalCountData()
    {
        var data = new List<LabelViewChartDto>
        {
            new() { LabelDate = new DateTime(2025, 3, 26), Count = 100 },
            new() { LabelDate = new DateTime(2025, 3, 27), Count = 150 },
            new() { LabelDate = new DateTime(2025, 3, 28), Count = 90 }
        };
        return Json(data, JsonRequestBehavior.AllowGet);
    }

    public async Task<ActionResult> View(int? id, string productionOrderNo)
    {
        try
        {
            if(null == id && string.IsNullOrEmpty(productionOrderNo))
                throw new Exception("Invalid Id or Production Order.");
            var plan = await _planService.FirstOrDefaultAsync(p => p.Id == id, p => p).ConfigureAwait(false);
            if (null == plan)
            {
                plan = await _planService.GetByWarehouseOrderNoAsync(productionOrderNo).ConfigureAwait(false);
                if(null == plan)
                    throw new Exception($"Plan with Id ({id}) or Order ({productionOrderNo}) not found");
            }

            var dto = await _planService.GetViewModelAsync(plan).ConfigureAwait(false);
            return View(dto);
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
            throw;
        }
    }

    public async Task<ActionResult> PlanDetailView(string productionOrderNo, string position)
    {
        try
        {
            var positionView = new PositionDetailDto
            {
                ProductionOrderNo = productionOrderNo,
                Position = position,
            };
            await _planService.FillLabelInformationAsync(positionView).ConfigureAwait(false);
            await _planService.FillCartonInformationAsync(positionView).ConfigureAwait(false);

            return View(_previewPath, positionView);
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }
        return View();
    }

    public async Task<ActionResult> GetIndexViewDto([DataSourceRequest] DataSourceRequest request)
    {
        try
        {
            var result = await _planService.GetIndexViewDtoAsync(request).ConfigureAwait(false);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
            ModelState.AddModelError("Error", exception.Message);
            return Json(new DataSourceResult { Errors = ModelState });
        }
    }

    public async Task<ActionResult> GetLabelsForPlan([DataSourceRequest] DataSourceRequest request, string productionOrderNo)
    {
        try
        {
            var result = await _planService.GetLabelsForPlanAsync(request, productionOrderNo).ConfigureAwait(false);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
            ModelState.AddModelError("Error", exception.Message);
            return Json(new DataSourceResult { Errors = ModelState }, JsonRequestBehavior.AllowGet);
        }
    }

    public async Task<ActionResult> GetCartonsForPlan([DataSourceRequest] DataSourceRequest request, string productionOrderNo)
    {
        try
        {
            var result = await _planService.GetCartonsForPlanAsync(request, productionOrderNo).ConfigureAwait(false);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
            ModelState.AddModelError("Error", exception.Message);
            return Json(new DataSourceResult { Errors = ModelState }, JsonRequestBehavior.AllowGet);
        }
    }

    #endregion
}
