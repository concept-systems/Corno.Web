
using System.Web.Http;

namespace Corno.Web;

public static class WebApiConfig
{
    public static void Register(HttpConfiguration config)
    {
        // Web API configuration and services

        // Web API routes
        config.MapHttpAttributeRoutes();

        // Note :
        // Change routeTemplate: "api/{controller}/{id}" to routeTemplate: "api/{controller}/{action}/{id}"
        // for Multiple post methods
        config.Routes.MapHttpRoute(
            name: "DefaultApi",
            routeTemplate: "api/{controller}/{action}/{id}",
            defaults: new { id = RouteParameter.Optional }
        );

        config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;

        /*Telerik.Reporting.Services.WebApi.ReportsControllerConfiguration.RegisterRoutes(config);*/
    }
}