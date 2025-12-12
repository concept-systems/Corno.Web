using System.Web.Mvc;

namespace Corno.Web.Areas.Masters;

public class MastersAreaRegistration : AreaRegistration 
{
    public override string AreaName => "Masters";

    public override void RegisterArea(AreaRegistrationContext context) 
    {
        context.MapRoute(
            "Masters_default",
            "Masters/{controller}/{action}/{id}",
            new { action = "Index", id = UrlParameter.Optional }
        );
    }
}