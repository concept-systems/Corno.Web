using System.Web.Mvc;
using System.Web.Routing;
using Corno.Web.Globals;

//using Corno.Web.AreaLib;

namespace Corno.Web;

public class RouteConfig
{
    public static void RegisterRoutes(RouteCollection routes)
    {
        routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

        // Regular Login
        routes.MapRoute(name: "Default",
            url: "{controller}/{action}/{id}",
            defaults: new
            {
                controller = ApplicationGlobals.LoginController,
                action = ApplicationGlobals.LoginAction,
                area = ApplicationGlobals.LoginArea,
                id = UrlParameter.Optional
            },
            new[] { ApplicationGlobals.LoginNamespace });


        //// Regular Login
        //if (string.IsNullOrEmpty(ApplicationGlobals.LoginArea))
        //{
        //    routes.MapRoute(name: "Default",
        //        url: "{controller}/{action}/{id}",
        //        defaults: new
        //        {
        //            controller = ApplicationGlobals.LoginController,
        //            action = ApplicationGlobals.LoginAction,
        //            id = UrlParameter.Optional
        //        },
        //        new[] {ApplicationGlobals.LoginNamespace});
        //}
        //else
        //{
        //    routes.MapRoute(name: "Default",
        //        url: "{controller}/{action}/{id}",
        //        defaults: new
        //        {
        //            controller = ApplicationGlobals.LoginController,
        //            action = ApplicationGlobals.LoginAction,
        //            area = ApplicationGlobals.LoginArea,
        //            id = UrlParameter.Optional
        //        },
        //        new[] { ApplicationGlobals.LoginNamespace });
        //}


        //// Regular Login
        //routes.MapRoute(name: "Default",
        //        url: "{controller}/{action}/{id}",
        //        defaults: new { controller = "Account", action = "Login", id = UrlParameter.Optional },
        //        new[] { "Corno.Web.Controllers" });

        ////// Raychem
        ////routes.MapRoute(name: "Default",
        ////    url: "{controller}/{action}/{id}",
        ////    defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
        ////    new[] { "Corno.Web.Areas.Raychem.Controllers" });

        //// Concept Systems
        ////routes.MapRoute(
        ////    name: "Default",
        ////    url: "{controller}/{action}/{id}",
        ////    defaults: new {controller = "Account", action = "Login", area = "Concept", id = UrlParameter.Optional},
        ////    new[] {"Corno.Web.Areas.Concept.Controllers"});

        ////// Bharati Vidyapeeth
        ////routes.MapRoute(
        ////    name: "Default",
        ////    url: "{controller}/{action}/{id}",
        ////    defaults: new { controller = "Account", action = "OtpLogin", area = "Bvdu", id = UrlParameter.Optional },
        ////    new[] { "Corno.Web.Areas.Bvdu.Controllers" }
        ////);
    }
}