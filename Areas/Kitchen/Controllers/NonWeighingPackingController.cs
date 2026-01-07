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
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Corno.Web.Extensions;

namespace Corno.Web.Areas.Kitchen.Controllers;
public class NonWeighingPackingController : SuperController
{
    #region -- Constructors --
    public NonWeighingPackingController(INonWeighingPackingService nonWeighingPackingService,
        IPlanService planService, ILabelService labelService)
    {
        _nonWeighingPackingService = nonWeighingPackingService;
        _planService = planService;
        _labelService = labelService;
    }

    private readonly ILabelService _labelService;
    private readonly INonWeighingPackingService _nonWeighingPackingService;
    private readonly IPlanService _planService;
    #endregion

    #region -- Private Methods --
    private Plan GetPlan(string warehouseOrderNo)
    {
        if (Session[FieldConstants.Plan] is Plan plan &&
            plan.WarehouseOrderNo.Equals(warehouseOrderNo))
            return plan;

        //plan = _planService.GetByWarehouseOrderNo(warehouseOrderNo);
        //Session[FieldConstants.Plan] = plan ?? throw new Exception($"Warehouse order {warehouseOrderNo} not found");

        return null;
    }

    private void LoadDataFromSession(CartonCrudDto dto)
    {
        // Process Plan - Check if warehouse order number is already present in dto
        dto.Plan = Session[FieldConstants.Plan] as Plan;
        dto.Labels = Session[FieldConstants.Labels] as List<Label>;
    }

    private void SaveDataToSession(CartonCrudDto dto)
    {
        Session[FieldConstants.Plan] = dto.Plan;
        Session[FieldConstants.Labels] = dto.Labels;
    }

    private void ClearDataFromSession()
    {
        Session[FieldConstants.Plan] = null;
        Session[FieldConstants.Labels] = null;
    }
    private void ClearDataFromDto(CartonCrudDto dto)
    {
        dto.Plan = null;
        dto.Labels = null;
    }

    #endregion

    #region -- Actions --
    public async Task<ActionResult> Index()
    {
        return await Task.FromResult(View(new CartonIndexDto())).ConfigureAwait(false);
    }

    public virtual async Task<ActionResult> Create()
    {
        Session[FieldConstants.Label] = null;
        ModelState.Clear();

        return await Task.FromResult(View(new CartonCrudDto())).ConfigureAwait(false);
    }

    [HttpPost]
    /*public JsonResult ValidateBarcode(CartonCrudDto dto, string userId)
    {
        try
        {
            // Deserialize CartonDetailsDtosJson into actual DTO list
            dto.CartonDetailsDtos = JsonConvert.DeserializeObject<List<CartonDetailsDto>>(dto.CartonDetailsDtosJson);

            // Process Plan - Check if warehouse order number is already present in dto
            LoadDataFromSession(dto);

            // validate barcode
            _nonWeighingPackingService.ValidateBarcode(dto);

            // Save data to session
            SaveDataToSession(dto);

            // Clear Data from Dto
            ClearDataFromDto(dto);

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
    }*/

    public async Task<JsonResult> ValidateBarcodeAsync(CartonCrudDto dto, string userId)
    {
        try
        {
            // Deserialize CartonDetailsDtosJson into actual DTO list
            dto.CartonDetailsDtos = JsonConvert.DeserializeObject<List<CartonDetailsDto>>(dto.CartonDetailsDtosJson);

            // Process Plan - Check if warehouse order number is already present in dto
            LoadDataFromSession(dto); // If this involves DB, make it async too

            // Validate barcode asynchronously
            await _nonWeighingPackingService.ValidateBarcodeAsync(dto).ConfigureAwait(false);

            // Save data to session
            SaveDataToSession(dto); // If DB or heavy I/O, convert to async

            // Clear Data from Dto
            ClearDataFromDto(dto);

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
    public async Task<ActionResult> PreviewAsync(CartonCrudDto dto)
    {
        try
        {
            // Deserialize CartonDetailsDtosJson into actual DTO list
            dto.CartonDetailsDtos = JsonConvert.DeserializeObject<List<CartonDetailsDto>>(dto.CartonDetailsDtosJson);

            // Load Data from session
            LoadDataFromSession(dto);

            // Now call your service method properly
            var reportBook = await _nonWeighingPackingService.Preview(dto).ConfigureAwait(false);
            //Session[FieldConstants.Label] = reportBook;
            dto.PrintToPrinter = false;

            // Save data to session
            SaveDataToSession(dto);

            // Clear Data from Dto
            ClearDataFromDto(dto);

            return File(reportBook.ToDocumentBytes(), "application/pdf");
            /*return Json(new { Success = true, dto, Message = string.Empty },
                JsonRequestBehavior.AllowGet);*/
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
    public async Task<ActionResult> PrintAsync(CartonCrudDto dto)
    {
        try
        {
            // Deserialize CartonDetailsDtosJson into actual DTO list
            dto.CartonDetailsDtos = JsonConvert.DeserializeObject<List<CartonDetailsDto>>(dto.CartonDetailsDtosJson);

            // Process Plan - Check if warehouse order number is already present in dto
            LoadDataFromSession(dto);

            // Now call your service method properly
            var reportBook = await _nonWeighingPackingService.Print(dto, User.Identity.GetUserId()).ConfigureAwait(false);
            //var reportProcessor = new Telerik.Reporting.Processing.ReportProcessor();
            //var result = reportProcessor.RenderReport("PDF", reportBook, null);
            // Clear Data from Dto

            ClearDataFromDto(dto);
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
    public async Task<JsonResult> Cancel(CartonCrudDto dto)
    {
        try
        {
            // Clear Data from Dto
            ClearDataFromDto(dto);
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

    public async Task<ActionResult> View(int? id)
    {
        try
        {
            var dto = await _nonWeighingPackingService.ViewAsync(id);
            //Session[FieldConstants.Label] = dto.ReportBook;
            return View(dto);
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }
        return View(new CartonViewDto());
    }

    public async Task<ActionResult> DeleteCarton(int? id)
    {
        try
        {
            if (null == id)
                throw new Exception("Invalid Carton Id");

            var carton = await _nonWeighingPackingService.FirstOrDefaultAsync(p => p.Id == id, p => p).ConfigureAwait(false);
            if (carton == null)
                throw new Exception("Carton not found");

            var barcodeList = carton.CartonDetails.Select(d => d.Barcode).Distinct();
            // Delete labels
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

            // Perform the deletion
            var cartonToDelete = await _nonWeighingPackingService.FirstOrDefaultAsync(p => p.Id == id, p => p).ConfigureAwait(false);
            await _nonWeighingPackingService.DeleteAsync(cartonToDelete).ConfigureAwait(false);
            await _nonWeighingPackingService.SaveAsync().ConfigureAwait(false);
            return Json(new { success = true, message = "Carton Deleted Successfully" }, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            // Log the exception and return an error message
            HandleControllerException(exception);
            var message = LogHandler.GetDetailException(exception)?.Message;
            return Json(new { Success = false, Message = (object)message },
                JsonRequestBehavior.AllowGet);
        }
    }

    public async Task<ActionResult> GetIndexViewDto([DataSourceRequest] DataSourceRequest request)
    {
        try
        {
            /*var query = from carton in _nonWeighingPackingService.GetQuery()
                select new
                {
                    carton.Id,
                    carton.CartonNo,
                    carton.PackingDate,
                    carton.SoNo,
                    carton.WarehouseOrderNo,
                    carton.CartonBarcode,
                    carton.Status
                };
            var result = await query.ToDataSourceResultAsync(request);*/
            var result = await _nonWeighingPackingService.GetIndexDataSourceAsync(request).ConfigureAwait(false);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            return GetGridErrorResult(exception);
        }
    }

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