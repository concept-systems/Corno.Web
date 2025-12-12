using System;
using System.Web.Mvc;
using Corno.Web.Logger;

namespace Corno.Web.Attributes
{
    /// <summary>
    /// Global exception filter that handles all unhandled exceptions in controllers
    /// </summary>
    public class GlobalExceptionFilterAttribute : FilterAttribute, IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {
            if (filterContext.ExceptionHandled)
                return;

            var exception = filterContext.Exception;
            
            // Log the exception
            LogHandler.LogError(exception);

            // Check if it's an AJAX request
            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                // For AJAX requests, return JSON error
                filterContext.Result = new JsonResult
                {
                    Data = new
                    {
                        error = true,
                        message = GetUserFriendlyMessage(exception),
                        type = exception.GetType().Name
                    },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
                filterContext.ExceptionHandled = true;
                filterContext.HttpContext.Response.StatusCode = 500;
                filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
            }
            else
            {
                // For regular requests, redirect to error page or show error view
                filterContext.Result = new ViewResult
                {
                    ViewName = "Error",
                    ViewData = new ViewDataDictionary
                    {
                        ["Exception"] = exception,
                        ["Message"] = GetUserFriendlyMessage(exception)
                    }
                };
                filterContext.ExceptionHandled = true;
                filterContext.HttpContext.Response.Clear();
                filterContext.HttpContext.Response.StatusCode = 500;
                filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
            }
        }

        private string GetUserFriendlyMessage(Exception exception)
        {
            // Return user-friendly error messages
            var detailException = LogHandler.GetDetailException(exception);
            return detailException?.Message ?? exception.Message;
        }
    }
}

