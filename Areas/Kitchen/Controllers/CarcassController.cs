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
using System.Threading.Tasks;
using System.Web.Mvc;
using Corno.Web.Extensions;


namespace Corno.Web.Areas.Kitchen.Controllers;

public class CarcassController : SuperController
{
    #region -- Constructors --
    public CarcassController(ICarcassPackingService carcassPackingService)
           
    {
        _carcassPackingService = carcassPackingService;
    }

    private readonly ICarcassPackingService _carcassPackingService;

    #endregion

    #region -- Private Methods --
    private void LoadDataFromSession(CarcassCrudDto dto)
    {
        // Process Plan - Check if warehouse order number is already present in dto
        dto.Plan = Session[FieldConstants.Plan] as Plan;
        dto.Labels = Session[FieldConstants.Labels] as List<Label>;
    }

    private void SaveDataToSession(CarcassCrudDto dto)
    {
        Session[FieldConstants.Plan] = dto.Plan;
        Session[FieldConstants.Labels] = dto.Labels;
    }

    private void ClearDataFromSession()
    {
        Session[FieldConstants.Plan] = null;
        Session[FieldConstants.Labels] = null;
    }

    private void ClearDataFromDto(CarcassCrudDto dto)
    {
        dto.Plan = null;
        dto.Labels = null;
    }
    #endregion

    #region -- Actions --
    public async Task<ActionResult> Index()
    {
        return await Task.FromResult(View(new CarcassIndexDto())).ConfigureAwait(false);
    }

    public virtual async Task<ActionResult> Create()
    {
        Session[FieldConstants.Label] = null;
        ModelState.Clear();
        return await Task.FromResult(View(new CarcassCrudDto())).ConfigureAwait(false);
    }

    [HttpPost]
    public async Task<JsonResult> ValidateBarcodeAsync(CarcassCrudDto dto)
    {
        try
        {
            // Deserialize CartonDetailsDtosJson into actual DTO list
            dto.CarcassDetailsDtos = JsonConvert.DeserializeObject<List<CarcassDetailsDto>>(dto.CarcassDetailsDtosJson);

            // Process Plan - Check if warehouse order number is already present in dto
            LoadDataFromSession(dto);
            // validate barcode
            await _carcassPackingService.ValidateBarcodeAsync(dto).ConfigureAwait(false);
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
    }

    [HttpPost]
    public async Task<ActionResult> PreviewAsync(CarcassCrudDto dto)
    {
        try
        {
            // Deserialize CartonDetailsDtosJson into actual DTO list
            dto.CarcassDetailsDtos = JsonConvert.DeserializeObject<List<CarcassDetailsDto>>(dto.CarcassDetailsDtosJson);

            // Load Data from session
            LoadDataFromSession(dto);

            // Now call your service method properly
            var reportBook = await _carcassPackingService.Preview(dto).ConfigureAwait(false);
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
    public async Task<ActionResult> PrintAsync(CarcassCrudDto dto)
    {
        try
        {
            // Deserialize CartonDetailsDtosJson into actual DTO list
            dto.CarcassDetailsDtos = JsonConvert.DeserializeObject<List<CarcassDetailsDto>>(dto.CarcassDetailsDtosJson);

            // Process Plan - Check if warehouse order number is already present in dto
            LoadDataFromSession(dto);

            // Now call your service method properly
            var reportBook = await _carcassPackingService.Print(dto, User.Identity.GetUserId()).ConfigureAwait(false);
            /*var reportProcessor = new Telerik.Reporting.Processing.ReportProcessor();
            var result = reportProcessor.RenderReport("PDF", reportBook, null);*/

            ClearDataFromDto(dto);
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
    public async Task<JsonResult> Cancel(CarcassCrudDto dto)
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

    //public ActionResult View(int? id)
    //{
    //    try
    //    {
    //        var dto = _carcassPackingService.View(id);
    //        Session[FieldConstants.Label] = dto.ReportBook;
    //        return View(dto);
    //    }
    //    catch (Exception exception)
    //    {
    //        HandleControllerException(exception);
    //    }
    //    return View(new CartonViewDto());
    //}


    public async Task<ActionResult> GetIndexViewDto([DataSourceRequest] DataSourceRequest request)
    {
        /*var query = from carton in _carcassPackingService.GetQuery()
            select new 
            {
                carton.Id,
                CartonNo = "C" + carton.CartonNo.ToString().PadLeft(3, '0'),
                carton.PackingDate,
                carton.SoNo,
                carton.WarehouseOrderNo,
                carton.CartonBarcode,
                carton.Status
            };
        var result = await query.ToDataSourceResultAsync(request);*/
        var result = _carcassPackingService.GetIndexDataSource(request);
        return Json(result, JsonRequestBehavior.AllowGet);
    }


    [AcceptVerbs(HttpVerbs.Post)]
    public async Task<ActionResult> Inline_Create_Update_Destroy([DataSourceRequest] DataSourceRequest request, CartonDetailsDto model)
    {
        return await Task.FromResult(Json(new[] { model }.ToDataSourceResult(request, ModelState))).ConfigureAwait(false);
    }
    #endregion
}