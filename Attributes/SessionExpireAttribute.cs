using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using Corno.Web.Globals;
using Microsoft.AspNet.Identity;

namespace Corno.Web.Attributes;

/// <summary>
/// Session expiration filter that checks both session and authentication cookie expiration.
/// Redirects to login page if session or authentication has expired.
/// </summary>
public class SessionExpireAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
        var context = HttpContext.Current;
        var user = filterContext.HttpContext.User;

        // Check if user is authenticated
        if (user == null || !user.Identity.IsAuthenticated)
        {
            RedirectToLogin(filterContext, context);
            return;
        }

        // Check session expiration
        if (context.Session != null)
        {
            if (context.Session.IsNewSession)
            {
                var sessionCookie = context.Request.Headers["Cookie"];

                // If we have a session cookie but it's a new session, the session expired
                if (sessionCookie != null && sessionCookie.IndexOf("ASP.NET_SessionId", StringComparison.Ordinal) >= 0)
                {
                    RedirectToLogin(filterContext, context);
                    return;
                }
            }
        }

        base.OnActionExecuting(filterContext);
    }

    private void RedirectToLogin(ActionExecutingContext filterContext, HttpContext context)
    {
        // Sign out from forms authentication
        FormsAuthentication.SignOut();

        // Clear session
        if (context.Session != null)
        {
            context.Session.Clear();
            context.Session.Abandon();
        }

        // When session expires, always redirect to login without returnUrl
        // This ensures users go to home page after login, not back to expired session page
        filterContext.Result = new RedirectToRouteResult(
            new RouteValueDictionary
            {
                { "controller", ApplicationGlobals.LoginController },
                { "action", ApplicationGlobals.LoginAction },
                { "area", ApplicationGlobals.LoginArea }
            });
    }
}