using System.Web.Mvc;

namespace Corno.Web.Areas.Nilkamal;

public class NilkamalAreaRegistration : AreaRegistration
{
    public override string AreaName => "Nilkamal";

    public override void RegisterArea(AreaRegistrationContext context)
    {
        context.MapRoute(
            "Nilkamal_default",
            "Nilkamal/{controller}/{action}/{id}",
            new { action = "Index", id = UrlParameter.Optional }
        );
    }
}