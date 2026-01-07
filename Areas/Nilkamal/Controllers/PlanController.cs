using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Corno.Web.Areas.Nilkamal.Dto.Plan;
using Corno.Web.Areas.Nilkamal.Services.Interfaces;
using Corno.Web.Models.Plan;
using Corno.Web.Services.Import;
using Corno.Web.Services.Import.Interfaces;
using Kendo.Mvc.UI;

namespace Corno.Web.Areas.Nilkamal.Controllers;

public class PlanController : BaseImportController<PlanImportDto>
{
    #region -- Constructors --
    public PlanController(IFileImportService<PlanImportDto> importService, IPlanService planService)
        : base(importService)
    {
        _planService = planService;

        const string viewPath = "~/Areas/Nilkamal/Views/Plan";
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
        return View(new PlanIndexDto());
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
                throw new Exception("Invalid Id or production order.");
            var plan = await _planService.FirstOrDefaultAsync(p => p.Id == id, p => p).ConfigureAwait(false);
            if (null == plan)
            {
                plan = await _planService.GetByWarehouseOrderNoAsync(productionOrderNo).ConfigureAwait(false);
                if(null == plan)
                    throw new Exception($"Plan with Id ({id}) or Order ({productionOrderNo}) not found");
            }

            var dto = await _planService.GetViewModelAsync(plan).ConfigureAwait(false);

            /*foreach (var planItemDetail in plan.PlanItemDetails)
            {
                dto.PlanViewItemDtos.Add(new PlanViewItemDto
                {
                    Id = planItemDetail.Id,
                    Description = planItemDetail.Description,

                    OrderQuantity = planItemDetail.OrderQuantity ?? 0,
                });
            }*/

            return View(dto);
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
            throw;
        }
    }

    public async Task<ActionResult> PlanDetailView(string warehouseOrderNo, string position)
    {
        try
        {
            var positionView = new PositionDetailDto
            {
                WarehouseOrderNo = warehouseOrderNo,
                Position = position,
            };
            await _planService.FillLabelInformationAsync(positionView).ConfigureAwait(false);
            await _planService.FillCartonInformationAsync(positionView).ConfigureAwait(false);

            return View(positionView);
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }
        return View();
    }

    public async Task<ActionResult> DeletePosition(string warehouseOrderNo, string position)
    {
        try
        {
            if (null == position)
                return Json(new { success = false, message = "Warehouse Order cannot be null." });

            var viewModel = await _planService.DeletePositionAsync(warehouseOrderNo, position).ConfigureAwait(false);

            return View(_previewPath, viewModel);
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
            return Json(new { success = false, message = exception.Message }, JsonRequestBehavior.AllowGet);
        }
    }

    [HttpPost]
    public async Task<ActionResult> DeletePlan(string productionOrderNo)
    {
        try
        {
            await _planService.DeletePlanAsync(productionOrderNo).ConfigureAwait(false);
            return Json(new { success = true, message = "Plan deleted successfully." }, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
            return Json(new { success = false, message = exception.Message }, JsonRequestBehavior.AllowGet);
        }
    }

    [HttpPost]
    public async Task<ActionResult> UpdateQuantities(string warehouseOrderNo, string dueDate)
    {
        try
        {
            Plan plan = null;
            List<Plan> plans = null;
            var searchCriteria = "";

            if (string.IsNullOrEmpty(warehouseOrderNo) && string.IsNullOrEmpty(dueDate))
                throw new Exception("Either Warehouse Order No or Due Date is required");

            if (!string.IsNullOrEmpty(warehouseOrderNo) && !string.IsNullOrEmpty(dueDate))
                throw new Exception("Please provide either Warehouse Order No OR Due Date, not both");

            if (!string.IsNullOrEmpty(warehouseOrderNo))
            {
                plan = await _planService.GetByWarehouseOrderNoAsync(warehouseOrderNo).ConfigureAwait(false);
                searchCriteria = $"Warehouse Order No: {warehouseOrderNo}";
            }
            else if (!string.IsNullOrEmpty(dueDate))
            {
                if (!DateTime.TryParse(dueDate, out var parsedDueDate))
                    throw new Exception("Invalid Due Date format");

                var lotNos = await _planService.GetLotNosAsync(parsedDueDate).ConfigureAwait(false);
                var selectedLotNos = lotNos.Cast<dynamic>().Select(x => (string)x.LotNo).ToList();
                plans = (await _planService.GetAsync(p => selectedLotNos.Contains(p.LotNo), p => p).ConfigureAwait(false)).ToList();
                searchCriteria = $"Due Date: {dueDate}";
            }

            if (plan == null && (plans == null || plans.Count == 0))
                throw new Exception($"No plans found for the given criteria ({searchCriteria})");

            if (plan != null)
            {
                await _planService.UpdateQuantitiesAsync(plan).ConfigureAwait(false);
            }
            else
            {
                foreach (var p in plans)
                {
                    await _planService.UpdateQuantitiesAsync(p).ConfigureAwait(false);
                }
            }

            return Json(new { success = true, message = $"Quantities updated successfully for {searchCriteria}" }, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
            return Json(new { success = false, message = exception.Message }, JsonRequestBehavior.AllowGet);
        }
    }

    public ActionResult UpdateQuantity(IEnumerable<HttpPostedFileBase> files)
    {
        try
        {
            var httpPostedFileBases = files?.ToList();
            if (httpPostedFileBases?.FirstOrDefault() == null)
                throw new Exception("No file selected for import");
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
            return Json(new { success = false, message = exception.Message }, JsonRequestBehavior.AllowGet);
        }

        return Json(new { success = true }, JsonRequestBehavior.AllowGet);
    }

    [HttpPost]
    public async Task<ActionResult> UpdateDueDate(string warehouseOrderNo, DateTime dueDate)
    {
        try
        {
            await _planService.UpdateDueDateAsync(warehouseOrderNo, dueDate).ConfigureAwait(false);
            return Json(new { success = true, message = "Due date updated successfully." }, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
            return Json(new { success = false, message = exception.Message }, JsonRequestBehavior.AllowGet);
        }
    }

    [HttpPost]
    public async Task<ActionResult> UpdateLotNo(string warehouseOrderNo, string lotNo)
    {
        try
        {
            await _planService.UpdateLotNoAsync(warehouseOrderNo, lotNo).ConfigureAwait(false);
            return Json(new { success = true, message = "Lot No updated successfully." }, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
            return Json(new { success = false, message = exception.Message }, JsonRequestBehavior.AllowGet);
        }
    }


    [HttpPost]
    public ActionResult Remove(string[] fileNames)
    {
        // Handle file removal
        foreach (var file in fileNames)
        {
            var filePath = System.IO.Path.Combine(Server.MapPath("~/App_Data/uploads"), file);
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }
        return Json(new { success = true });
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

    public async Task<ActionResult> GetLabelsForPlan([DataSourceRequest] DataSourceRequest request, string warehouseOrderNo)
    {
        try
        {
            var result = await _planService.GetLabelsForPlanAsync(request, warehouseOrderNo).ConfigureAwait(false);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
            ModelState.AddModelError("Error", exception.Message);
            return Json(new DataSourceResult { Errors = ModelState }, JsonRequestBehavior.AllowGet);
        }
    }

    public async Task<ActionResult> GetCartonsForPlan([DataSourceRequest] DataSourceRequest request, string warehouseOrderNo)
    {
        try
        {
            var result = await _planService.GetCartonsForPlanAsync(request, warehouseOrderNo).ConfigureAwait(false);
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