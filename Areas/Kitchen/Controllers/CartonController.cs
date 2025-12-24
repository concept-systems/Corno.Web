using Corno.Web.Areas.Kitchen.Dto.Carcass;
using Corno.Web.Areas.Kitchen.Dto.Carton;
using Corno.Web.Areas.Kitchen.Services.Interfaces;
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
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Corno.Web.Extensions;

namespace Corno.Web.Areas.Kitchen.Controllers;

[Authorize]
public class CartonController : SuperController
{
    #region -- Constructors --
    public CartonController(
        ICarcassPackingService carcassPackingService,
        INonWeighingPackingService nonWeighingPackingService,
        IPlanService planService,
        ILabelService labelService)
    {
        _carcassPackingService = carcassPackingService;
        _nonWeighingPackingService = nonWeighingPackingService;
        _planService = planService;
        _labelService = labelService;
    }
    #endregion

    #region -- Data Members --
    private readonly ICarcassPackingService _carcassPackingService;
    private readonly INonWeighingPackingService _nonWeighingPackingService;
    private readonly IPlanService _planService;
    private readonly ILabelService _labelService;
    #endregion

    #region -- Private Methods --
    private void LoadCarcassDataFromSession(CarcassCrudDto dto)
    {
        dto.Plan = Session[FieldConstants.Plan] as Plan;
        dto.Labels = Session[FieldConstants.Labels] as List<Label>;
    }

    private void SaveCarcassDataToSession(CarcassCrudDto dto)
    {
        Session[FieldConstants.Plan] = dto.Plan;
        Session[FieldConstants.Labels] = dto.Labels;
    }

    private void ClearDataFromSession()
    {
        Session[FieldConstants.Plan] = null;
        Session[FieldConstants.Labels] = null;
    }

    private void ClearCarcassDataFromDto(CarcassCrudDto dto)
    {
        dto.Plan = null;
        dto.Labels = null;
    }

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
            // Get data from both services and combine
            var carcassQuery = _carcassPackingService.GetQuery();
            var nonWeighingQuery = _nonWeighingPackingService.GetQuery();

            var carcassData = from carton in carcassQuery
                select new CartonIndexDto
                {
                    Id = carton.Id,
                    CartonNo = carton.CartonNo.ToString(),
                    PackingDate = carton.PackingDate,
                    SoNo = carton.SoNo,
                    WarehouseOrderNo = carton.WarehouseOrderNo,
                    CartonBarcode = carton.CartonBarcode,
                    Status = carton.Status,
                    /*LotNo = carton.LotNo,
                    DueDate = carton.DueDate,
                    OneLineItemCode = carton.OneLineItemCode*/
                };

            var nonWeighingData = from carton in nonWeighingQuery
                select new CartonIndexDto
                {
                    Id = carton.Id,
                    CartonNo = carton.CartonNo.ToString(),
                    PackingDate = carton.PackingDate,
                    SoNo = carton.SoNo,
                    WarehouseOrderNo = carton.WarehouseOrderNo,
                    CartonBarcode = carton.CartonBarcode,
                    Status = carton.Status,
                    /*LotNo = carton.LotNo,
                    DueDate = carton.DueDate,
                    OneLineItemCode = carton.OneLineItemCode*/
                };

            var combinedData = carcassData.Union(nonWeighingData);
            var result = await combinedData.ToDataSourceResultAsync(request).ConfigureAwait(false);
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

            CartonViewDto dto;
            if (cartonType == "Carcass" || string.IsNullOrEmpty(cartonType))
            {
                // Try Carcass first if type is Carcass or not specified
                try
                {
                    var carcassDto = await _carcassPackingService.View(id).ConfigureAwait(false);
                    if (carcassDto != null)
                    {
                        Session[FieldConstants.Label] = carcassDto.ReportBook;
                        return View(carcassDto);
                    }
                }
                catch
                {
                    // If not found, fall back to NonWeighingPacking
                }
            }

            // Try NonWeighingPacking
            dto = await _nonWeighingPackingService.View(id).ConfigureAwait(false);
            if (dto != null)
            {
                Session[FieldConstants.Label] = dto.ReportBook;
                return View(dto);
            }

            throw new Exception("Carton not found");
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

            if (cartonType == "Carcass")
            {
                var carton = await _carcassPackingService.FirstOrDefaultAsync(p => p.Id == id, p => p).ConfigureAwait(false);
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

                var cartonToDelete = await _carcassPackingService.FirstOrDefaultAsync(p => p.Id == id, p => p).ConfigureAwait(false);
                await _carcassPackingService.DeleteAsync(cartonToDelete).ConfigureAwait(false);
                await _carcassPackingService.SaveAsync().ConfigureAwait(false);
            }
            else
            {
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
            }

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

    #region -- Carcass Actions --
    public virtual async Task<ActionResult> CreateCarcass()
    {
        Session[FieldConstants.Label] = null;
        ModelState.Clear();
        return await Task.FromResult(View("CreateCarcass", new CarcassCrudDto())).ConfigureAwait(false);
    }

    [HttpPost]
    public async Task<JsonResult> ValidateCarcassBarcodeAsync(CarcassCrudDto dto)
    {
        try
        {
            dto.CarcassDetailsDtos = JsonConvert.DeserializeObject<List<CarcassDetailsDto>>(dto.CarcassDetailsDtosJson);
            LoadCarcassDataFromSession(dto);
            await _carcassPackingService.ValidateBarcodeAsync(dto).ConfigureAwait(false);
            SaveCarcassDataToSession(dto);
            ClearCarcassDataFromDto(dto);

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
    public async Task<ActionResult> PreviewCarcassAsync(CarcassCrudDto dto)
    {
        try
        {
            dto.CarcassDetailsDtos = JsonConvert.DeserializeObject<List<CarcassDetailsDto>>(dto.CarcassDetailsDtosJson);
            LoadCarcassDataFromSession(dto);
            var reportBook = await _carcassPackingService.Preview(dto).ConfigureAwait(false);
            dto.PrintToPrinter = false;
            SaveCarcassDataToSession(dto);
            ClearCarcassDataFromDto(dto);

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
    public async Task<ActionResult> PrintCarcassAsync(CarcassCrudDto dto)
    {
        try
        {
            dto.CarcassDetailsDtos = JsonConvert.DeserializeObject<List<CarcassDetailsDto>>(dto.CarcassDetailsDtosJson);
            LoadCarcassDataFromSession(dto);
            var reportBook = await _carcassPackingService.Print(dto, User.Identity.GetUserId()).ConfigureAwait(false);
            ClearCarcassDataFromDto(dto);
            ClearDataFromSession();
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
    public async Task<JsonResult> CancelCarcass(CarcassCrudDto dto)
    {
        try
        {
            ClearCarcassDataFromDto(dto);
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

