using System.Web.Mvc;

namespace Corno.Web.Areas.BoardStore;

public class BoardStoreAreaRegistration : AreaRegistration
{
    public override string AreaName => "BoardStore";

    public override void RegisterArea(AreaRegistrationContext context)
    {
        context.MapRoute(
            "BoardStore_default",
            "BoardStore/{controller}/{action}/{id}",
            new { action = "Index", id = UrlParameter.Optional }
        );
    }
}