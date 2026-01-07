using System.Web.Mvc;

namespace Corno.Web.Areas.Euro;

public class EuroAreaRegistration : AreaRegistration
{
    public override string AreaName => "Euro";

    public override void RegisterArea(AreaRegistrationContext context)
    {
        context.MapRoute(
            "Euro_default",
            "Euro/{controller}/{action}/{id}",
            new { action = "Index", id = UrlParameter.Optional }
        );
    }
}

