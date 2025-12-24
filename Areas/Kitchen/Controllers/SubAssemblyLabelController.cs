using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Corno.Web.Areas.Kitchen.Dto.Label;
using Corno.Web.Areas.Kitchen.Services.Interfaces;
using Corno.Web.Controllers;
using Corno.Web.Extensions;
using Corno.Web.Globals;
using Corno.Web.Models.Packing;
using Corno.Web.Models.Plan;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Mapster;

namespace Corno.Web.Areas.Kitchen.Controllers;

[Authorize]
public class SubAssemblyLabelController : SuperController
{
    #region -- Constructors --
    public SubAssemblyLabelController(IAssemblyLabelService assemblyLabelService, IPlanService planService)
    {
        _assemblyLabelService = assemblyLabelService;
        _planService = planService;

        const string viewPath = "~/Areas/Kitchen/Views/SubAssemblyLabel";
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
    private readonly IAssemblyLabelService _assemblyLabelService;
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

    public virtual ActionResult Create()
    {
        try
        {
            Session[FieldConstants.Label] = null;
            return View(new SubAssemblyCrudDto());
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
            return View();
        }
    }

    [HttpPost]
    public async Task<ActionResult> Print(SubAssemblyCrudDto dto)
    {
        if (!ModelState.IsValid)
            return View(_createPath, dto);

        try
        {
            var oldStatus = new[] { StatusConstants.Bent, StatusConstants.Sorted };
            var labels = await _assemblyLabelService.GetAsync(p => p.Barcode == dto.Barcode1 &&
                                                 oldStatus.Contains(p.Status), p => p,
                                                 q => q.OrderByDescending(x => x.Id), true).ConfigureAwait(false);
            var label1 = labels.FirstOrDefault();
            if (string.IsNullOrEmpty(label1?.AssemblyCode) || label1.AssemblyCode == FieldConstants.NA)
                throw new Exception($"Invalid assembly code {label1?.AssemblyCode}");

            var plan = await GetPlanAsync(label1.WarehouseOrderNo).ConfigureAwait(false);

            // Create Labels
            var createdLabels = await _assemblyLabelService.CreateLabelsAsync(dto, plan, label1, true).ConfigureAwait(false);

            /*// Persist changes only for Print
            await _assemblyLabelService.UpdateDatabaseAsync(label1, createdLabels, plan).ConfigureAwait(false);*/

            // Clear DTO for next input
            dto.Clear();
            ModelState.Clear();

            // Create Label Report and return as PDF (do not store in session)
            var report = await _assemblyLabelService.CreateLabelReportAsync(createdLabels, false).ConfigureAwait(false);
            return File(report.ToDocumentBytes(), "application/pdf");
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
            return Json(new { Success = false, exception.GetBaseException().Message },
                JsonRequestBehavior.AllowGet);
        }
    }

    [HttpPost]
    public async Task<ActionResult> Preview(SubAssemblyCrudDto dto)
    {
        if (!ModelState.IsValid)
            return View(_createPath, dto);

        try
        {
            var oldStatus = new[] { StatusConstants.Bent, StatusConstants.Sorted };
            var labels = await _assemblyLabelService.GetAsync(p => p.Barcode == dto.Barcode1 &&
                                                 oldStatus.Contains(p.Status), p => p,
                                                 q => q.OrderByDescending(x => x.Id), true).ConfigureAwait(false);
            var label1 = labels.FirstOrDefault();
            if (string.IsNullOrEmpty(label1?.AssemblyCode) || label1.AssemblyCode == FieldConstants.NA)
                throw new Exception($"Invalid assembly code {label1?.AssemblyCode}");

            var plan = await GetPlanAsync(label1.WarehouseOrderNo).ConfigureAwait(false);

            // Create Labels
            var createdLabels = await _assemblyLabelService.CreateLabelsAsync(dto, plan, label1, false).ConfigureAwait(false);

            // Create Label Report for preview only (no DB update, no session)
            var report = await _assemblyLabelService.CreateLabelReportAsync(createdLabels, false).ConfigureAwait(false);
            return File(report.ToDocumentBytes(), "application/pdf");
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
            return Json(new { Success = false, exception.GetBaseException().Message },
                JsonRequestBehavior.AllowGet);
        }
    }

    public async Task<ActionResult> View(int? id)
    {
        try
        {
            if (id == null)
                throw new Exception("Invalid Id");

            var dto = await _assemblyLabelService.CreateViewDtoAsync(id).ConfigureAwait(false);
            Session[FieldConstants.Label] = dto.LabelReport;

            return View(dto);
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }

        return RedirectToAction("Index");
    }

    public async Task<ActionResult> GetIndexViewDto([DataSourceRequest] DataSourceRequest request)
    {
        try
        {
            var query = _assemblyLabelService.GetQuery().AsNoTracking();
            query = query.Where(p => p.Status == StatusConstants.SubAssembled);
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
