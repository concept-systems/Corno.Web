using System.Web.Mvc;

namespace Corno.Web.Areas.Kitchen;

public class KitchenAreaRegistration : AreaRegistration
{
    public override string AreaName => "Kitchen";

    public override void RegisterArea(AreaRegistrationContext context)
    {
        context.MapRoute(
            "Kitchen_default",
            "Kitchen/{controller}/{action}/{id}",
            new { action = "Index", id = UrlParameter.Optional }
        );
    }
}