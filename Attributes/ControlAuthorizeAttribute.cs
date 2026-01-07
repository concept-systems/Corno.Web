using System;
using System.Web.Mvc;
using Corno.Web.Areas.Admin.Services.Interfaces;
using Corno.Web.Windsor;
using Microsoft.AspNet.Identity;

namespace Corno.Web.Attributes;

/// <summary>
/// Action filter for control-level access authorization
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class ControlAuthorizeAttribute : ActionFilterAttribute
{
    public string ControlId { get; set; }

    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
        if (string.IsNullOrEmpty(ControlId))
        {
            base.OnActionExecuting(filterContext);
            return;
        }

        var userId = filterContext.HttpContext.User?.Identity?.GetUserId();
        if (string.IsNullOrEmpty(userId))
        {
            filterContext.Result = new HttpStatusCodeResult(System.Net.HttpStatusCode.Unauthorized, "User not authenticated");
            return;
        }

        try
        {
            var permissionService = Bootstrapper.Get<IPermissionService>();

            var controller = filterContext.RouteData.Values["controller"]?.ToString();
            var action = filterContext.RouteData.Values["action"]?.ToString();

            if (!permissionService.HasControlAccessAsync(userId, ControlId, controller, action).Result)
            {
                filterContext.Result = new HttpStatusCodeResult(System.Net.HttpStatusCode.Forbidden, 
                    $"Access denied to control: {ControlId}");
                return;
            }
        }
        catch (Exception)
        {
            filterContext.Result = new HttpStatusCodeResult(System.Net.HttpStatusCode.InternalServerError, 
                "Error checking control permissions");
            return;
        }

        base.OnActionExecuting(filterContext);
    }
}

