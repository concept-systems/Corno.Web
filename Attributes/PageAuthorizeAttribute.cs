using System;
using System.Web;
using System.Web.Mvc;
using Corno.Web.Areas.Admin.Services.Interfaces;
using Corno.Web.Windsor;
using Microsoft.AspNet.Identity;

namespace Corno.Web.Attributes;

/// <summary>
/// Authorization attribute for page-level access control
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class PageAuthorizeAttribute : AuthorizeAttribute
{
    public string Controller { get; set; }
    public string Action { get; set; }
    public string Area { get; set; }

    protected override bool AuthorizeCore(HttpContextBase httpContext)
    {
        if (!httpContext.User.Identity.IsAuthenticated)
            return false;

        try
        {
            var userId = httpContext.User.Identity.GetUserId();
            if (string.IsNullOrEmpty(userId))
                return false;

            var permissionService = Bootstrapper.Get<IPermissionService>();

            var controller = Controller ?? httpContext.Request.RequestContext.RouteData.Values["controller"]?.ToString();
            var action = Action ?? httpContext.Request.RequestContext.RouteData.Values["action"]?.ToString();
            var area = Area ?? httpContext.Request.RequestContext.RouteData.Values["area"]?.ToString();

            if (string.IsNullOrEmpty(controller) || string.IsNullOrEmpty(action))
                return true; // Allow if no controller/action specified

            return permissionService.HasPageAccessAsync(userId, controller, action, area).Result;
        }
        catch
        {
            return false;
        }
    }

    protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
    {
        filterContext.Result = new HttpStatusCodeResult(System.Net.HttpStatusCode.Forbidden, "You do not have permission to access this page.");
    }
}

