using System;
using System.Data.Entity.Validation;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Corno.Web.Attributes;
using Corno.Web.Globals;
using Corno.Web.Logger;
using Corno.Web.Models;
using Kendo.Mvc.UI;

namespace Corno.Web.Controllers;

[CustomAuthorize]
[Compress]
[SessionExpire]
public class SuperController : Controller
{
    public void HandleControllerException(Exception exception)
    {
        // Clear the ModelState
        ModelState.Clear();

        if (exception.GetType() == typeof(DbEntityValidationException))
        {
            if (exception is DbEntityValidationException dbEx)
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                        ModelState.AddModelError(validationError.PropertyName, validationError.ErrorMessage);
                }
        }

        var errors = ModelState.Values.SelectMany(v => v.Errors);
        foreach (var error in errors)
            ModelState.AddModelError(error.ErrorMessage, error.Exception);

        ModelState.AddModelError(string.Empty, exception.Message);

        if (exception.InnerException == null) return;

        ModelState.AddModelError(string.Empty, exception.InnerException.Message);
        if (exception.InnerException.InnerException != null)
            ModelState.AddModelError(string.Empty, exception.InnerException.InnerException.Message);

        //var stackInfo = LogHandler.GetCallStackInfo();
        //var message = $"Call stack:\n{stackInfo}\nOriginal message: {exception.Message}";

        LogHandler.LogError(exception);
    }

    protected JsonResult GetJsonResult(Exception exception)
    {
        return Json(  JsonResponse.GetResponse(MessageType.Error, 
            LogHandler.GetDetailException(exception)?.Message), JsonRequestBehavior.AllowGet);
    }

    protected JsonResult GetJsonResult(MessageType messageType, string message)
    {
        return Json(JsonResponse.GetResponse(messageType, message), 
            JsonRequestBehavior.AllowGet);
    }

    // Action to load the partial view
    public async Task<ActionResult> LoadPartialView(string partialViewName)
    {
        // Redirect old WebForms ReportViewer to HTML5 viewer
        if (partialViewName is "Partials/ReportViewer_New" or "Partials/ReportViewer")
        {
            partialViewName = "Partials/ReportViewer_Html5";
        }
        
        return await Task.FromResult(PartialView(partialViewName)).ConfigureAwait(false);
    }

    /// <summary>
    /// Returns a DataSourceResult with errors for grid error handling
    /// </summary>
    protected JsonResult GetGridErrorResult(Exception exception)
    {
        HandleControllerException(exception);
        ModelState.AddModelError("Error", exception.Message);
        return Json(new DataSourceResult { Errors = ModelState }, JsonRequestBehavior.AllowGet);
    }

    /// <summary>
    /// Returns a JsonResult with error information for combo box error handling
    /// Returns empty array to prevent Kendo errors, error will be shown via DataSource error event
    /// </summary>
    protected JsonResult GetComboBoxErrorResult(Exception exception)
    {
        LogHandler.LogError(exception);
        var errorMessage = LogHandler.GetDetailException(exception)?.Message ?? exception.Message;
        Response.StatusCode = 500;
        Response.TrySkipIisCustomErrors = true;
        // Include error message in response header so error handler can access it
        Response.Headers.Add("X-Error-Message", errorMessage);
        // Return empty array so Kendo doesn't throw .slice() error
        // The actual error will be caught by the DataSource error event handler
        return Json(new object[0], JsonRequestBehavior.AllowGet);
    }

    /// <summary>
    /// Returns a JsonResult with error information for combo box error handling (with custom message)
    /// Returns empty array to prevent Kendo errors, error will be shown via DataSource error event
    /// </summary>
    protected JsonResult GetComboBoxErrorResult(string errorMessage)
    {
        Response.StatusCode = 500;
        Response.TrySkipIisCustomErrors = true;
        // Include error message in response header so error handler can access it
        Response.Headers.Add("X-Error-Message", errorMessage);
        // Return empty array so Kendo doesn't throw .slice() error
        // The actual error will be caught by the DataSource error event handler
        return Json(new object[0], JsonRequestBehavior.AllowGet);
    }
}

public class CustomAuthorizeAttribute : AuthorizeAttribute
{
    protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
    {
        // Redirect to the root URL
        filterContext.Result = new RedirectResult("~/");
    }
}