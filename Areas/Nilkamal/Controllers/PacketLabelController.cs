using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;
using Corno.Web.Areas.DemoProject.Dtos;
using Corno.Web.Areas.Nilkamal.Dto.PacketLabel;
using Corno.Web.Areas.Nilkamal.Labels;
using Corno.Web.Areas.Nilkamal.Services.Interfaces;
using Corno.Web.Controllers;
using Corno.Web.Extensions;
using Corno.Web.Globals;
using Corno.Web.Logger;
using Corno.Web.Models.Masters;
using Corno.Web.Models.Packing;
using Corno.Web.Models.Plan;
using Corno.Web.Services.Interfaces;
using Corno.Web.Services.Masters.Interfaces;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Mapster;

namespace Corno.Web.Areas.Nilkamal.Controllers;

public class PacketLabelController : SuperController
{
    #region -- Constructors --
    public PacketLabelController(IPartLabelService partLabelService, IPlanService planService, IBaseItemService itemService, ICartonService cartonService,
        IProductService productService, IMiscMasterService miscMasterService)
    {
        _partLabelService = partLabelService;
        _planService = planService;
        _itemService = itemService;
        _cartonService = cartonService;
        _productService = productService;
        _miscMasterService = miscMasterService;

        const string viewPath = "~/Areas/Nilkamal/Views/PacketLabel";
        _createPath = $"{viewPath}/Create.cshtml";

        TypeAdapterConfig<PacketViewDto, Carton>
            .NewConfig()
            .Map(dest => dest.CartonDetails, src => src.PacketViewDetailDto.Adapt<List<LabelDetail>>())
            .AfterMapping((src, dest) =>
            {
                dest.Id = src.Id;
                for (var index = 0; index < src.PacketViewDetailDto.Count; index++)
                    dest.CartonDetails[index].Id = src.PacketViewDetailDto[index].Id;
            });
        TypeAdapterConfig<Carton, PacketViewDto>
            .NewConfig()
            .Map(dest => dest.PacketViewDetailDto, src => src.CartonDetails.Adapt<List<PacketViewDetailDto>>())
            .AfterMapping((src, dest) =>
            {
                dest.Id = src.Id;
                for (var index = 0; index < src.CartonDetails.Count; index++)
                    dest.PacketViewDetailDto[index].Id = src.CartonDetails[index].Id;
            });
    }
    #endregion

    #region -- Data Mambers --
    private readonly string _createPath;
    private readonly IPartLabelService _partLabelService;
    private readonly IPlanService _planService;
    private readonly IBaseItemService _itemService;
    private readonly ICartonService _cartonService;
    private readonly IProductService _productService;
    private readonly IMiscMasterService _miscMasterService;

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
        return View(new PacketViewDto());
    }

    public Task<ActionResult> Create(string productionOrderNo)
    {
        if (string.IsNullOrEmpty(productionOrderNo))
            Session[FieldConstants.Label] = null;

        return Task.FromResult<ActionResult>(View(new PacketLabelCrudDto()));
    }

    [System.Web.Http.HttpPost]
    //[MultipleButton(Name = "action", Argument = "Print")]
    public async Task<ActionResult> Print(PacketLabelCrudDto dto)
    {
        if (!ModelState.IsValid)
            return View(_createPath, dto);
        try
        {
            // Get Plan
            var plan = await GetPlanAsync(dto.ProductionOrderNo).ConfigureAwait(false);
            // Create Labels
            var report = await _cartonService.CreateLabelsAsync(dto, plan, true).ConfigureAwait(false);

            dto.Clear();
            ModelState.Clear();

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
    public async Task<ActionResult> Preview(PacketLabelCrudDto dto)
    {
        if (!ModelState.IsValid)
            return View(_createPath, dto);
        try
        {
            // Get Plan
            var plan = await GetPlanAsync(dto.ProductionOrderNo).ConfigureAwait(false);
            // Create Labels
            var report = await _cartonService.CreateLabelsAsync(dto, plan).ConfigureAwait(false);
            // Create Label Reports
            //Session[FieldConstants.Label] = _partLabelService.CreateLabelReport(labels, false);
            //var report = await _cartonService.CreateLabelReportAsync(cartons, false).ConfigureAwait(false);
            return File(report.ToDocumentBytes(), "application/pdf");
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
            var message = LogHandler.GetDetailException(exception)?.Message;
            return Json(new { Success = false, Message = message }, JsonRequestBehavior.AllowGet);
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
            var dto = await _cartonService.CreateViewDtoAsync(id);

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
            var allCartons = await Task.Run(() => _cartonService.GetListAsync());

            var cartons = allCartons.Where(x => ids.Contains(x.Id))
                .ToList();

            var report = new PacketLabelRpt(cartons, false);

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

    public async Task<ActionResult> GetProducts([DataSourceRequest] DataSourceRequest request, string productionOrderNo)
    {
        try
        {
            if (string.IsNullOrEmpty(productionOrderNo))
                return Json(null, JsonRequestBehavior.AllowGet);

            var plan = await GetPlanAsync(productionOrderNo).ConfigureAwait(false);
            var pendingItems = await _planService.GetPendingProductsAsync(plan).ConfigureAwait(false);

            // Convert to list, then to queryable for proper filtering
            var itemsList = pendingItems.Cast<object>().ToList();
            var queryableItems = itemsList.AsQueryable();
            
            // Apply filter, sorting, and paging from DataSourceRequest
            // MultiColumnComboBox expects just the Data array, not the full DataSourceResult
            var result = await queryableItems.ToDataSourceResultAsync(request);
            return Json(result.Data, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            return GetComboBoxErrorResult(exception);
        }
    }

    public async Task<ActionResult> GetPackets([DataSourceRequest] DataSourceRequest request, string productionOrderNo, int productId)
    {
        try
        {
            if (productId <= 0) 
                return Json(null, JsonRequestBehavior.AllowGet);

            var pendingProducts = await _planService.GetPendingPacketsAsync(productionOrderNo, productId).ConfigureAwait(false);

            // Convert to list, then to queryable for proper filtering
            var productsList = pendingProducts.Cast<object>().ToList();
            var queryableItems = productsList.AsQueryable();
            
            // Apply filter, sorting, and paging from DataSourceRequest
            // MultiColumnComboBox expects just the Data array, not the full DataSourceResult
            var result = await queryableItems.ToDataSourceResultAsync(request);
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
            var query = (IQueryable<Carton>)_cartonService.GetQuery();
            var productQuery = (IQueryable<Product>)_productService.GetQuery();

            var data = from carton in _cartonService.GetQuery()
                       join product in _productService.GetQuery()
                           on carton.ProductId equals product.Id into productGroup
                       from product in productGroup.DefaultIfEmpty()
                       select new PacketIndexDto
                       {
                           Id = carton.Id,
                           PackingDate = carton.PackingDate,
                           ProductionOrderNo = carton.ProductionOrderNo,
                           ProductId = carton.ProductId,
                           ProductCode = product.Code,
                           ProductName = product.Name,
                           CartonBarcode = carton.CartonBarcode,
                           Status = carton.Status,
                       };

            var result = await data.ToList().ToDataSourceResultAsync(request);

            return Json(result, JsonRequestBehavior.AllowGet);
            
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
            ModelState.AddModelError("Error", exception.Message);
            return Json(new DataSourceResult { Errors = ModelState });
        }
    }

    [System.Web.Http.HttpPost]
    public async Task<JsonResult> GetProductDetails(PacketLabelCrudDto dto)
    {
        try
        {
            var plan = await _planService.FirstOrDefaultAsync(
                p => p.ProductionOrderNo == dto.ProductionOrderNo && p.ProductId == dto.ProductId,
                p => p);

            if (plan == null)
                throw new Exception("Invalid plan.");

            /*var planItemDetail = plan.PlanItemDetails
                .FirstOrDefault(d => d.ProductionOrderNo == dto.ProductionOrderNo);*/

            var planItemDetail = plan.PlanItemDetails.FirstOrDefault(d =>
                    d.ProductionOrderNo == dto.ProductionOrderNo &&
                    d.Plan.ProductId == dto.ProductId
                );

            if (planItemDetail == null)
                throw new Exception("Invalid product detail in plan.");

            var product = await _productService.GetByIdAsync(dto.ProductId);
            if (product == null)
                throw new Exception("Product not found.");

            var packet = await _miscMasterService.GetByIdAsync(planItemDetail?.PackingTypeId);
            if (packet == null)
                throw new Exception("Packing type not found.");

            dto.ProductCode = product.Code;
            dto.ProductName = product.Name;
            dto.PackingTypeName = packet?.Code;
            dto.OrderQuantity = planItemDetail?.OrderQuantity;
            dto.PrintQuantity = planItemDetail?.PrintQuantity;

            return Json(dto, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            LogHandler.LogError(exception);
        }

        return Json(dto, JsonRequestBehavior.AllowGet);
    }

    #endregion
}