using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using Corno.Web.Globals;

namespace Corno.Web.Attributes;

public class SessionExpireAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
        var context = HttpContext.Current;
        if (context.Session != null)
        {
            if (context.Session.IsNewSession)
            {
                var sessionCookie = context.Request.Headers["Cookie"];

                if (sessionCookie != null && sessionCookie.IndexOf("ASP.NET_SessionId", StringComparison.Ordinal) >= 0)
                {
                    FormsAuthentication.SignOut();
                    if (!string.IsNullOrEmpty(context.Request.RawUrl))
                    {
                        filterContext.Result =
                            new RedirectToRouteResult(
                                new RouteValueDictionary{
                                    { "controller", ApplicationGlobals.LoginController },
                                    { "action", ApplicationGlobals.LoginAction },
                                    { "area", ApplicationGlobals.LoginArea },
                                    { "namespace", ApplicationGlobals.LoginNamespace }
                                });
                        //var redirectTo =
                        //    $"~/Account/Login?ReturnUrl={HttpUtility.UrlEncode(context.Request.RawUrl)}";
                        //filterContext.Result = new RedirectResult(redirectTo);
                        return;
                    }
                }
            }
        }

        base.OnActionExecuting(filterContext);
    }
}