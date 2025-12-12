using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Corno.Web.Areas.BoardStore.Models;
using Corno.Web.Areas.BoardStore.Services.Interfaces;
using Corno.Web.Controllers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace Corno.Web.Areas.BoardStore.Controllers;

public class RequestController : SuperController
{
    #region -- Constructors --
    public RequestController(IRequestService requestService) 
    {
        _requestService = requestService;
    }
    #endregion

    #region -- Data Mambers --
    private readonly IRequestService _requestService;
    #endregion

    #region -- Actions --
    public ActionResult Index()
    {
        return View(new Request());
    }

    public async Task<ActionResult> View(int? id)
    {
        try
        {
            var request = await _requestService.GetByIdAsync(id ?? 0).ConfigureAwait(false);
            if (null == request)
                throw new Exception("Plan with Id not found");

            var viewModel = new Request
            {
                RequestNo = request.RequestNo,
            };

            foreach (var stack in request.Stacks)
            {
                viewModel.Stacks.Add(new Stack
                {
                    Id = stack.Id,
                    RequestId = stack.RequestId,
                    RequestNo = stack.RequestNo,
                    StackNo = stack.StackNo,
                    BayNo = stack.BayNo
                });
            }

            return View(viewModel);
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }

        return View();
    }

    //public ActionResult DeleteRequest(int? id)
    //{
    //    try
    //    {
    //        var request = _requestService.FirstOrDefault(p => p.Id == id,
    //            p => p);
    //        if (null == request)
    //            throw new Exception($"Plan not found for warehouse order {id}");
         
    //        _requestService.DeleteAsync(request);

    //        return View(request);
    //    }
    //    catch (Exception exception)
    //    {
    //        HandleControllerException(exception);
    //        return Json(new { success = false, message = exception.Message }, JsonRequestBehavior.AllowGet);
    //    }

    //    return View();
    //}

    //public ActionResult DeleteRequest(int? id)
    //{
    //    try
    //    {
    //        var request = _requestService.GetById(id);
    //        if (request == null)
    //        {
    //            throw new Exception($"Request with ID {id} was not found.");
    //        }

    //        _requestService.Delete(request);
    //        _requestService.Save();

    //        return Json(new { success = true, message = "Record deleted successfully." }, JsonRequestBehavior.AllowGet);
    //    }
    //    catch (Exception ex)
    //    {
    //        // Log the exception for debugging
    //        Console.WriteLine($"Error: {ex.Message}");
    //        return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
    //    }

    //}
    public async Task<ActionResult> DeleteRequest(int? id)
    {
        try
        {
            if (null == id)
                return Json(new { success = false, message = "ID cannot be null." });

            var request = await _requestService.FirstOrDefaultAsync(p => p.Id == id, p => p).ConfigureAwait(false);
            if (request == null)
                return Json(new { success = false, message = "request with the specified ID not found." });

            // Perform the deletion
            await _requestService.DeleteAsync(request).ConfigureAwait(false);
            await _requestService.SaveAsync().ConfigureAwait(false);

            return Json(new { success = true, message = "request deleted successfully." });
        }
        catch (Exception exception)
        {
            // Log the exception and return an error message
            HandleControllerException(exception);
        }
        return Json(new { success = false, message = "An error occurred while deleting the request." });
    }
    public async Task<ActionResult> GetIndexViewModels([DataSourceRequest] DataSourceRequest request)
    {
        try
        {
            var query = _requestService.GetQuery();
            var result = await query.ToDataSourceResultAsync(request).ConfigureAwait(false);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            return GetGridErrorResult(exception);
        }
    }

    #endregion
}