using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Corno.Web.Areas.Kitchen.Dto.Put_To_Light;
using Corno.Web.Areas.Kitchen.Models.Put_To_Light;
using Corno.Web.Areas.Kitchen.Services.Interfaces;
using Corno.Web.Controllers;
using Corno.Web.Globals;
using Corno.Web.Services.Interfaces;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Mapster;
using Microsoft.AspNet.Identity;

namespace Corno.Web.Areas.Kitchen.Controllers;

public class TrolleyConfigController : SuperController
{
    #region -- Constructors --
    public TrolleyConfigController(ITrolleyConfigService trolleyConfigService, IPlanService planService,
        IMiscMasterService miscMasterService)
    {
        _planService = planService;
        _miscMasterService = miscMasterService;
        _trolleyConfigService = trolleyConfigService;

        TypeAdapterConfig<TrolleyConfigIndexDto, TrolleyConfig>
            .NewConfig()
            .Map(dest => dest.TrolleyLightDetails, src => src.TrolleyLightDetailDtos.Adapt<List<TrolleyConfigDetail>>())
            .AfterMapping((src, dest) =>
            {
                dest.Id = src.Id;
                for (var index = 0; index < src.TrolleyLightDetailDtos.Count; index++)
                    dest.TrolleyLightDetails[index].Id = src.TrolleyLightDetailDtos[index].Id;
            });
        TypeAdapterConfig<TrolleyConfig, TrolleyConfigIndexDto>
            .NewConfig()
            .Map(dest => dest.TrolleyLightDetailDtos, src => src.TrolleyLightDetails.Adapt<List<TrolleyConfigDetailDto>>())
            .AfterMapping((src, dest) =>
            {
                dest.Id = src.Id;
                for (var index = 0; index < src.TrolleyLightDetails.Count; index++)
                    dest.TrolleyLightDetailDtos[index].Id = src.TrolleyLightDetails[index].Id;
            });
    }
    #endregion

    #region -- Data Mambers --
    private readonly IMiscMasterService _miscMasterService;
    private readonly IPlanService _planService;
    private readonly ITrolleyConfigService _trolleyConfigService;

    #endregion

    #region -- Actions --
    public ActionResult Index()
    {
        TempData["Success"] = null;
        return View();
    }

    public virtual async Task<ActionResult> Create()
    {
        try
        {
            TempData["Success"] = null;
            if (null != await _trolleyConfigService.FirstOrDefaultAsync(p => p.Status == StatusConstants.Active, p => p).ConfigureAwait(false))
                throw new Exception("There is active configuration in list. First close it and then create new.");

            return View(new TrolleyConfigIndexDto());
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
            TempData["Success"] = exception.Message;
        }
        return View(@"~\Areas\Kitchen\Views\TrolleyConfig\Index.cshtml");
    }

    [HttpPost]
    public async Task<ActionResult> Create(TrolleyConfigIndexDto dto)
    {
        if (!ModelState.IsValid)
            return View(dto);
        try
        {
            // Validate DTO
            _trolleyConfigService.ValidateDto(dto);

            var model = dto.Adapt<TrolleyConfig>();

            await _trolleyConfigService.AddAndSaveAsync(model).ConfigureAwait(false);
            return RedirectToAction("Index");
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }

        return View(dto);
    }

    public async Task<ActionResult> Edit(int id)
    {
        try
        {
            if (id <= 0)
                throw new Exception("Invalid Id");

            var model = await _trolleyConfigService.GetByIdAsync(id).ConfigureAwait(false);
            var dto = model.Adapt<TrolleyConfigIndexDto>();
            var locations = await _miscMasterService.GetViewModelListAsync(m =>
                m.MiscType == MiscMasterConstants.Location).ConfigureAwait(false);
            var colors = await _miscMasterService.GetViewModelListAsync(m =>
                m.MiscType == MiscMasterConstants.Color).ConfigureAwait(false);

            dto.TrolleyLightDetailDtos.ForEach(d =>
            {
                d.LocationName = locations.FirstOrDefault(x => x.Id == d.LocationId)?.Name;
                d.ColorName = colors.FirstOrDefault(x => x.Id == d.ColorId)?.Name;
            });
            return View(dto);
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }
        return View(new TrolleyConfigIndexDto());
    }

    [HttpPost]
    public async Task<ActionResult> Edit(TrolleyConfigIndexDto dto)
    {
        if (!ModelState.IsValid)
        {
            return View(dto);
        }
        try
        {
            // Validate DTO
            _trolleyConfigService.ValidateDto(dto);

            var model = dto.Adapt<TrolleyConfig>();

            await _trolleyConfigService.UpdateAndSaveAsync(model).ConfigureAwait(false);

            return RedirectToAction("Index");
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }

        return View(dto);
    }

    public async Task<ActionResult> Close(int? id)
    {
        try
        {
            if (null == id)
                return Json(new { success = false, message = "ID cannot be null." });

            var model = await _trolleyConfigService.FirstOrDefaultAsync(p => p.Id == id,
                p => p).ConfigureAwait(false);
            if (model == null)
                return Json(new { success = false, message = "Trolley with the specified ID not found." });

            model.Status = StatusConstants.Closed;
            model.DeletedBy = User.Identity.GetUserId();
            model.DeletedDate = DateTime.Now;

            // Save updates
            await _trolleyConfigService.UpdateAndSaveAsync(model).ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }
        return RedirectToAction("Index");
    }
    public async Task<ActionResult> GetLotNos([DataSourceRequest] DataSourceRequest request, DateTime dueDate)
    {
        var dataSource = await _planService.GetLotNosAsync(dueDate).ConfigureAwait(false);
        return Json(dataSource, JsonRequestBehavior.AllowGet);
    }

    //public ActionResult GetFamilies([DataSourceRequest] DataSourceRequest request, string lotNo)
    //{
    //    try
    //    {
    //        var plans = _planService.Get(p => p.LotNo == lotNo, p => new { p.PlanItemDetails });

    //        var groupsList = new List<string> { "FGWN22", "FGWN23" };
    //        var groups = plans.SelectMany(d => 
    //                d.PlanItemDetails, (_, d) => new { d.Group, Color = d.Reserved1 })
    //            .Where(d => groupsList.Contains(d.Group)) // Dynamic filtering
    //            .DistinctBy(d => d.Group)
    //            .OrderBy(d => d.Group)
    //            .ToList();
    //        return Json(groups, JsonRequestBehavior.AllowGet);
    //    }
    //    catch (Exception ex)
    //    {
    //        return Json(new { Errors = new { Error = ex.Message } }, JsonRequestBehavior.AllowGet);
    //    }
    //}

    //public ActionResult GetColors([DataSourceRequest] DataSourceRequest request, string lotNo, string family)
    //{
    //    try
    //    {
    //        var plans = _planService.Get(p => p.LotNo == lotNo && 
    //               p.PlanItemDetails.Any(d => d.Group == family) , p => new {p.WarehouseOrderNo, p.PlanItemDetails })
    //            .ToList();

    //        var colorCodes = plans.SelectMany(p => p.PlanItemDetails
    //                .Where(d => d.Group == family),
    //            (p, d) => d.Reserved1 )
    //            .ToList()                .Distinct();

    //        var colors = _miscMasterService.GetViewModelList(p => p.MiscType == MiscMasterConstants.Color &&
    //                                                              colorCodes.Contains(p.Code));
    //        return Json(colors, JsonRequestBehavior.AllowGet);
    //    }
    //    catch (Exception ex)
    //    {
    //        return Json(new { Errors = new { Error = ex.Message } }, JsonRequestBehavior.AllowGet);
    //    }
    //}

    public async Task<ActionResult> GetColors([DataSourceRequest] DataSourceRequest request, string lotNo)
    {
        try
        {
            // Get all plans for the given lot number
            var plans = await _planService.GetAsync(
                p => p.LotNo == lotNo,
                p => new { p.WarehouseOrderNo, p.PlanItemDetails }
            ).ConfigureAwait(false);

            // Extract all unique Reserved1 values (assumed to represent color codes)
            var colorCodes = plans
                .SelectMany(p => p.PlanItemDetails)
                .Select(d => d.Reserved1)
                .Where(code => !string.IsNullOrEmpty(code))
                .Distinct()
                .ToList();

            // Fetch color details from MiscMaster using the extracted codes
            var colors = await _miscMasterService.GetViewModelListAsync(p =>
                p.MiscType == MiscMasterConstants.Color &&
                colorCodes.Contains(p.Code)
            ).ConfigureAwait(false);

            return Json(colors, JsonRequestBehavior.AllowGet);
        }
        catch (Exception ex)
        {
            return Json(new { Errors = new { Error = ex.Message } }, JsonRequestBehavior.AllowGet);
        }
    }


    public async Task<ActionResult> GetIndexViewDto([DataSourceRequest] DataSourceRequest request)
    {
        try
        {
            var query = _trolleyConfigService.GetQuery();
            var data = from label in query
                select new TrolleyConfigIndexDto
                {
                    Id = label.Id,
                    DueDate = label.DueDate,
                    LotNo = label.LotNo,
                    Family = label.Family,
                    Status = label.Status
                };
            var result = await data.ToDataSourceResultAsync(request).ConfigureAwait(false);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
            //ModelState.AddModelError("Error", exception.Message);
            return Json(new DataSourceResult { Errors = ModelState });
        }
    }

    [AcceptVerbs(HttpVerbs.Post)]
    public async Task<ActionResult> Inline_Create_Update_Destroy_Config([DataSourceRequest] DataSourceRequest request, TrolleyConfigDetailDto dto)
    {
        dto.LocationName = await _miscMasterService.GetNameAsync(dto.LocationId).ConfigureAwait(false);
        dto.ColorName = await _miscMasterService.GetNameAsync(dto.ColorId).ConfigureAwait(false);
        return Json(new[] { dto }.ToDataSourceResult(request, ModelState));
    }

    #endregion
}