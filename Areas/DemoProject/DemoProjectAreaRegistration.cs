using System.Web.Mvc;

namespace Corno.Web.Areas.DemoProject
{
    public class DemoProjectAreaRegistration : AreaRegistration
    {
        public override string AreaName => "DemoProject";

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "DemoProject_default",
                "DemoProject/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}