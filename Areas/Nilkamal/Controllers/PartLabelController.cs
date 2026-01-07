using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;
using Corno.Web.Areas.Nilkamal.Dto.PartLabel;
using Corno.Web.Areas.Nilkamal.Labels;
using Corno.Web.Areas.Nilkamal.Services.Interfaces;
using Corno.Web.Controllers;
using Corno.Web.Extensions;
using Corno.Web.Globals;
using Corno.Web.Logger;
using Corno.Web.Models.Masters;
using Corno.Web.Models.Packing;
using Corno.Web.Models.Plan;
using Corno.Web.Services.Masters.Interfaces;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Mapster;

namespace Corno.Web.Areas.Nilkamal.Controllers;

public class PartLabelController : SuperController
{
    #region -- Constructors --
    public PartLabelController(IPartLabelService partLabelService, IPlanService planService, IBaseItemService itemService)
    {
        _partLabelService = partLabelService;
        _planService = planService;
        _itemService = itemService;

        const string viewPath = "~/Areas/Nilkamal/Views/PartLabel";
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

    #region -- Data Mambers --
    private readonly string _createPath;
    private readonly IPartLabelService _partLabelService;
    private readonly IPlanService _planService;
    private readonly IBaseItemService _itemService;

    #endregion

    #region -- Private Methods --
    private async Task<Plan> GetPlanAsync(string productionOrderNo)
    {
        if (Session[FieldConstants.Plan] is Plan plan &&
            plan.ProductionOrderNo.Equals(productionOrderNo))
            return plan;

        plan = await _planService.GetByProductionOrderNoAsync(productionOrderNo).ConfigureAwait(false);
        Session[FieldConstants.Plan] = plan ?? throw new Exception($"Production order {productionOrderNo} not found");

        return plan;
    }
    #endregion

    #region -- Actions --
    public ActionResult Index()
    {
        return View(new LabelViewDto());
    }

    public Task<ActionResult> Create(string productionOrderNo)
    {
        if (string.IsNullOrEmpty(productionOrderNo))
            Session[FieldConstants.Label] = null;

        return Task.FromResult<ActionResult>(View(new PartLabelCrudDto()));
    }

    [System.Web.Http.HttpPost]
    //[MultipleButton(Name = "action", Argument = "Print")]
    public async Task<ActionResult> Print(PartLabelCrudDto dto)
    {
        if (!ModelState.IsValid)
            return View(_createPath, dto);
        try
        {
            // Get Plan
            var plan = await GetPlanAsync(dto.ProductionOrderNo).ConfigureAwait(false);
            // Create Labels
            var labels = await _partLabelService.CreateLabelsAsync(dto, plan).ConfigureAwait(false);
            // Create Label Reports
            Session[FieldConstants.Label] = await _partLabelService.CreateLabelReportAsync(labels, false).ConfigureAwait(false);
            // Save in database
            await _partLabelService.UpdateDatabaseAsync(labels, plan).ConfigureAwait(false);
            //dto.PrintToPrinter = true;
            dto.Clear();
            ModelState.Clear();

            var report = await _partLabelService.CreateLabelReportAsync(labels, false).ConfigureAwait(false);
            return File(report.ToDocumentBytes(), "application/pdf");
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
            var message = LogHandler.GetDetailException(exception)?.Message;
            return Json(new { Success = false, Message = message },
                JsonRequestBehavior.AllowGet);
        }
        //return View(_createPath, dto);
    }

    [System.Web.Http.HttpPost]
    //[MultipleButton(Name = "action", Argument = "Preview")]
    public async Task<ActionResult> Preview(PartLabelCrudDto dto)
    {
        if (!ModelState.IsValid)
            return View(_createPath, dto);
        try
        {
            //dto.Clear();
            // Get Plan
            var plan = await GetPlanAsync(dto.ProductionOrderNo).ConfigureAwait(false);
            // Create Labels
            var labels = await _partLabelService.CreateLabelsAsync(dto, plan).ConfigureAwait(false);
            // Create Label Reports
            //Session[FieldConstants.Label] = _partLabelService.CreateLabelReport(labels, false);
            var report = await _partLabelService.CreateLabelReportAsync(labels, false).ConfigureAwait(false);
            return File(report.ToDocumentBytes(), "application/pdf");
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
            var message = LogHandler.GetDetailException(exception)?.Message;
            return Json(new { Success = false, Message = message },
                JsonRequestBehavior.AllowGet);
        }
        //return View(_createPath, dto);
    }

    public async Task<ActionResult> View(int? id)
    {
        try
        {
            if (id == null)
                throw new Exception("Invalid Id");

            // If CreateViewDto has an async version, use that:
            var dto = await _partLabelService.CreateViewDtoAsync(id);

            return View(dto);
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
            throw;
        }
    }

    [System.Web.Http.HttpPost]
    public async Task<ActionResult> ReprintLabels([FromBody] List<int> ids)
    {
        try
        {
            // Assuming _cuttingLabelService.GetList() can be async. 
            // If not, you can wrap it in Task.Run to avoid blocking.
            var allLabels = await Task.Run(() => _partLabelService.GetListAsync());

            var labels = allLabels.Where(x => ids.Contains(x.Id))
                .ToList();

            var report = new PartLabelRpt(labels);

            // If ToDocumentBytes() is synchronous, wrap it as well
            var pdfBytes = await Task.Run(() => report.ToDocumentBytes());

            return File(pdfBytes, "application/pdf");
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
            return Json(new { Success = false, Message = (object)exception.Message },
                JsonRequestBehavior.AllowGet);
        }
    }

    public async Task<ActionResult> GetItems([DataSourceRequest] DataSourceRequest request, string productionOrderNo)
    {
        try
        {
            if (string.IsNullOrEmpty(productionOrderNo))
                return Json(null, JsonRequestBehavior.AllowGet);

            var plan = await GetPlanAsync(productionOrderNo).ConfigureAwait(false);
            var pendingItems = await _partLabelService.GetPendingItemsAsync(plan).ConfigureAwait(false);

            // Convert to list, then to queryable for proper filtering
            var itemsList = pendingItems.Cast<object>().ToList();
            var queryableItems = itemsList.AsQueryable();

            // Debug: Check if filter is received
            // You can check request.Filters in debugger to see if filter is being sent
            // If request.Filters is null or empty, the filter is not being sent from client

            // Apply filter, sorting, and paging from DataSourceRequest
            // MultiColumnComboBox expects just the Data array, not the full DataSourceResult
            var result = queryableItems.ToDataSourceResult(request);
            return Json(result.Data, JsonRequestBehavior.AllowGet);
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
            var query = (IQueryable<Label>)_partLabelService.GetQuery();
            var itemQuery = (IQueryable<Item>)_itemService.GetQuery();

            var data = from label in _partLabelService.GetQuery()
                       join item in _itemService.GetQuery()
                           on label.ItemId equals item.Id into itemGroup
                       from item in itemGroup.DefaultIfEmpty()
                       select new LabelIndexDto
                       {
                           Id = label.Id,
                           LabelDate = label.LabelDate,
                           ItemId = label.ItemId,
                           ItemCode = item.Code,
                           ItemName = item.Name,
                           Barcode = label.Barcode,
                           Quantity = label.Quantity,
                           Status = label.Status,
                       };

            var result = await data.ToList().ToDataSourceResultAsync(request);

            return Json(result, JsonRequestBehavior.AllowGet);


            /*var result = await Task.Run(() => _partLabelService.GetIndexDataSource(request)).ConfigureAwait(false);
            return Json(result, JsonRequestBehavior.AllowGet);*/
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