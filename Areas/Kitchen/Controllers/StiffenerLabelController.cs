using System;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.UI;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using Corno.Web.Attributes;
using Corno.Web.Areas.Kitchen.Dto.StiffenerLabel;
using Corno.Web.Extensions;
using Corno.Web.Globals;
using Corno.Web.Logger;
using Corno.Web.Models.Plan;

namespace Corno.Web.Areas.Kitchen.Controllers;

[Authorize]
public class StiffenerLabelController : LabelController
{
    #region -- Constructors --
    public StiffenerLabelController()
    {
        const string viewPath = "~/Areas/Kitchen/Views/StiffenerLabel";
        _createPath = $"{viewPath}/Create.cshtml";
    }
    #endregion

    #region -- Data Members --
    private readonly string _createPath;
    #endregion

    #region -- Private Methods --
    private async Task<List<Plan>> GetPlansAsync(StiffenerLabelCrudDto dto)
    {
        if (Session[FieldConstants.Plans] is List<Plan> plans &&
            plans.Any(d => !d.LotNo.Equals(dto.LotNo)))
            return plans;
        plans = (await PlanService.GetAsync(p => p.LotNo == dto.LotNo &&
                                      p.PlanItemDetails.Any(d => d.DrawingNo == dto.DrawingNo), p => p)
            .ConfigureAwait(false));
        Session[FieldConstants.Plans] = plans;

        return plans;
    }
    #endregion

    #region -- Actions --
    // Index and View actions are inherited from LabelController

    public ActionResult Create()
    {
        return View(new StiffenerLabelCrudDto());
    }

    [HttpPost]
    public async Task<ActionResult> Print(StiffenerLabelCrudDto dto)
    {
        if (!ModelState.IsValid)
            return View(_createPath, dto);
        try
        {
            // Get Plan
            var plans = await GetPlansAsync(dto).ConfigureAwait(false);
            // Create Labels
            var labels = await StiffenerLabelService.CreateLabelsAsync(dto, plans).ConfigureAwait(false);
            // Save in database
            foreach(var plan in plans)
                await StiffenerLabelService.UpdateDatabaseAsync(labels, plan).ConfigureAwait(false);
            var report = await StiffenerLabelService.CreateLabelReportAsync(labels, false, Globals.Enums.LabelType.Stiffener).ConfigureAwait(false);
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
    public async Task<ActionResult> Preview(StiffenerLabelCrudDto dto)
    {
        if (!ModelState.IsValid)
            return View(_createPath, dto);
        try
        {
            // Get Plan
            var plans = await GetPlansAsync(dto).ConfigureAwait(false);
            // Create Labels
            var labels = await StiffenerLabelService.CreateLabelsAsync(dto, plans).ConfigureAwait(false);
            // Create Label Reports
            var report = await StiffenerLabelService.CreateLabelReportAsync(labels, false, Globals.Enums.LabelType.Stiffener).ConfigureAwait(false);
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

    public async Task<ActionResult> GetDrawingNo([DataSourceRequest] DataSourceRequest request, string lotNo)
    {
        try
        {
            var plans = Session[FieldConstants.Plans] as List<Plan>;
            var plns = plans?.Where(p => p.LotNo == lotNo).ToList();
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
            var plans = await PlanService.GetAsync(p => DbFunctions.TruncateTime(p.DueDate) == DbFunctions.TruncateTime(dueDate)
                                              && p.PlanItemDetails.Any(d => (d.OrderQuantity ?? 0) > (d.PrintQuantity ?? 0)),
                p => p).ConfigureAwait(false);

            Session[FieldConstants.Plans] = plans;

            var dataSource = plans.Select(p => new { p.LotNo }).Distinct();
            return Json(dataSource, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            return GetComboBoxErrorResult(exception);
        }
    }

    public async Task<ActionResult> GetPositions([DataSourceRequest] DataSourceRequest request, string lotNo, string drawingNo)
    {
        try
        {
            var plans = Session[FieldConstants.Plans] as List<Plan>;
            if (plans == null || string.IsNullOrEmpty(lotNo) || string.IsNullOrEmpty(drawingNo))
                return Json(new List<object>(), JsonRequestBehavior.AllowGet);

            var positions = plans
                .Where(p => p.LotNo == lotNo)
                .SelectMany(p => p.PlanItemDetails
                    .Where(d => d.DrawingNo == drawingNo && (d.OrderQuantity ?? 0) > (d.PrintQuantity ?? 0))
                    .Select(d => new
                    {
                        WarehouseOrderNo = p.WarehouseOrderNo,
                        Position = d.Position,
                        OrderQuantity = d.OrderQuantity ?? 0,
                        PrintQuantity = d.PrintQuantity ?? 0,
                        PendingQuantity = (d.OrderQuantity ?? 0) - (d.PrintQuantity ?? 0),
                        ItemCode = d.ItemCode,
                        Name = d.Description,
                        Family = d.Group,
                        DrawingNo = d.DrawingNo
                    }))
                .OrderBy(p => p.WarehouseOrderNo)
                .ThenBy(p => p.Position)
                .ToList();

            return Json(positions, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            return GetComboBoxErrorResult(exception);
        }
    }
    // GetIndexViewDto is inherited from LabelController

    #endregion
}
