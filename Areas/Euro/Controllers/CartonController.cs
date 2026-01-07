using Corno.Web.Areas.Euro.Dto.Carton;
using Corno.Web.Areas.Euro.Services.Interfaces;
using Corno.Web.Controllers;
using Corno.Web.Globals;
using Corno.Web.Logger;
using Corno.Web.Models.Packing;
using Corno.Web.Models.Plan;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Corno.Web.Extensions;

namespace Corno.Web.Areas.Euro.Controllers;

[Authorize]
public class CartonController : SuperController
{
    #region -- Constructors --
    public CartonController(
        INonWeighingPackingService nonWeighingPackingService,
        IPlanService planService,
        ILabelService labelService)
    {
        _nonWeighingPackingService = nonWeighingPackingService;
        _planService = planService;
        _labelService = labelService;
    }
    #endregion

    #region -- Data Members --
    private readonly INonWeighingPackingService _nonWeighingPackingService;
    private readonly IPlanService _planService;
    private readonly ILabelService _labelService;
    #endregion

    #region -- Private Methods --
    private void LoadCartonDataFromSession(CartonCrudDto dto)
    {
        dto.Plan = Session[FieldConstants.Plan] as Plan;
        dto.Labels = Session[FieldConstants.Labels] as List<Label>;
    }

    private void SaveCartonDataToSession(CartonCrudDto dto)
    {
        Session[FieldConstants.Plan] = dto.Plan;
        Session[FieldConstants.Labels] = dto.Labels;
    }

    private void ClearDataFromSession()
    {
        Session[FieldConstants.Plan] = null;
        Session[FieldConstants.Labels] = null;
    }

    private void ClearCartonDataFromDto(CartonCrudDto dto)
    {
        dto.Plan = null;
        dto.Labels = null;
    }
    #endregion

    #region -- Common Actions --
    public async Task<ActionResult> Index()
    {
        return await Task.FromResult(View(new CartonIndexDto())).ConfigureAwait(false);
    }

    public async Task<ActionResult> GetAllCartonsIndexViewDto([DataSourceRequest] DataSourceRequest request)
    {
        try
        {
            var result = await _nonWeighingPackingService.GetIndexDataSourceAsync(request).ConfigureAwait(false);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
            ModelState.AddModelError("Error", exception.Message);
            return Json(new DataSourceResult { Errors = ModelState });
        }
    }

    public async Task<ActionResult> View(int? id, string cartonType = null)
    {
        try
        {
            if (id == null)
                throw new Exception("Invalid Id");

            var dto = await _nonWeighingPackingService.ViewAsync(id).ConfigureAwait(false);
            return View(dto);
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }
        return View(new CartonViewDto());
    }

    public async Task<ActionResult> DeleteCarton(int? id, string cartonType = null)
    {
        try
        {
            if (null == id)
                throw new Exception("Invalid Carton Id");

            var carton = await _nonWeighingPackingService.FirstOrDefaultAsync(p => p.Id == id, p => p).ConfigureAwait(false);
            if (carton == null)
                throw new Exception("Carton not found");

            var barcodeList = carton.CartonDetails.Select(d => d.Barcode).Distinct();
            var labels = (await _labelService.GetAsync<Label>(l =>
                l.WarehouseOrderNo == carton.WarehouseOrderNo && barcodeList.Contains(l.Barcode), l => l).ConfigureAwait(false)).ToList();
            var plan = await _planService.GetByWarehouseOrderNoAsync(carton.WarehouseOrderNo).ConfigureAwait(false);
            foreach (var label in labels)
            {
                label.Status = StatusConstants.Sorted;
                label.LabelDetails.RemoveAll(l => l.Status == StatusConstants.Packed);
                var planItemDetail = plan?.PlanItemDetails.FirstOrDefault(d => d.Position == label.Position);
                if (null == planItemDetail) continue;
                if (planItemDetail.PackQuantity > 0)
                    planItemDetail.PackQuantity -= label.Quantity;
            }
            await _labelService.UpdateRangeAsync(labels).ConfigureAwait(false);
            await _planService.UpdateAsync(plan).ConfigureAwait(false);

            var cartonToDelete = await _nonWeighingPackingService.FirstOrDefaultAsync(p => p.Id == id, p => p).ConfigureAwait(false);
            await _nonWeighingPackingService.DeleteAsync(cartonToDelete).ConfigureAwait(false);
            await _nonWeighingPackingService.SaveAsync().ConfigureAwait(false);

            return Json(new { success = true, message = "Carton Deleted Successfully" }, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
            var message = LogHandler.GetDetailException(exception)?.Message;
            return Json(new { Success = false, Message = (object)message },
                JsonRequestBehavior.AllowGet);
        }
    }
    #endregion

    #region -- NonWeighingPacking Actions --
    public virtual async Task<ActionResult> CreateNonWeighingPacking()
    {
        Session[FieldConstants.Label] = null;
        ModelState.Clear();
        return await Task.FromResult(View("CreateNonWeighingPacking", new CartonCrudDto())).ConfigureAwait(false);
    }

    public async Task<JsonResult> ValidateNonWeighingBarcodeAsync(CartonCrudDto dto, string userId)
    {
        try
        {
            dto.CartonDetailsDtos = JsonConvert.DeserializeObject<List<CartonDetailsDto>>(dto.CartonDetailsDtosJson);
            LoadCartonDataFromSession(dto);
            await _nonWeighingPackingService.ValidateBarcodeAsync(dto).ConfigureAwait(false);
            SaveCartonDataToSession(dto);
            ClearCartonDataFromDto(dto);

            return Json(new { Success = true, dto, Message = string.Empty },
                JsonRequestBehavior.AllowGet);
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
    public async Task<ActionResult> PreviewNonWeighingAsync(CartonCrudDto dto)
    {
        try
        {
            dto.CartonDetailsDtos = JsonConvert.DeserializeObject<List<CartonDetailsDto>>(dto.CartonDetailsDtosJson);
            LoadCartonDataFromSession(dto);
            var reportBook = await _nonWeighingPackingService.Preview(dto).ConfigureAwait(false);
            dto.PrintToPrinter = false;
            SaveCartonDataToSession(dto);
            ClearCartonDataFromDto(dto);

            return File(reportBook.ToDocumentBytes(), "application/pdf");
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
    public async Task<ActionResult> PrintNonWeighingAsync(CartonCrudDto dto)
    {
        try
        {
            dto.CartonDetailsDtos = JsonConvert.DeserializeObject<List<CartonDetailsDto>>(dto.CartonDetailsDtosJson);
            LoadCartonDataFromSession(dto);
            var reportBook = await _nonWeighingPackingService.Print(dto, User.Identity.GetUserId()).ConfigureAwait(false);
            ClearCartonDataFromDto(dto);
            Session[FieldConstants.Labels] = null;
            return File(reportBook.ToDocumentBytes(), "application/pdf");
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
    public async Task<JsonResult> CancelNonWeighing(CartonCrudDto dto)
    {
        try
        {
            ClearCartonDataFromDto(dto);
            ClearDataFromSession();

            return Json(new { Success = true, dto, Message = string.Empty },
                JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
            var message = LogHandler.GetDetailException(exception)?.Message;
            return Json(new { Success = false, Message = message },
                JsonRequestBehavior.AllowGet);
        }
    }
    #endregion

    #region -- Helper Actions --
    [AcceptVerbs(HttpVerbs.Post)]
    public async Task<ActionResult> Inline_Create_Update_Destroy([DataSourceRequest] DataSourceRequest request, CartonDetailsDto model)
    {
        try
        {
            return await Task.FromResult(Json(new[] { model }.ToDataSourceResult(request, ModelState))).ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            return GetGridErrorResult(exception);
        }
    }
    #endregion
}

