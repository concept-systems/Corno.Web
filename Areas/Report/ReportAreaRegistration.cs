using System.Web.Mvc;

namespace Corno.Web.Areas.Report;

public class ReportAreaRegistration : AreaRegistration
{
    public override string AreaName => "Report";

    public override void RegisterArea(AreaRegistrationContext context)
    {
        context.MapRoute(
            "Report_default",
            "Report/{controller}/{action}/{id}",
            new { controller = "Home", action = "Index", id = UrlParameter.Optional },
            ["Corno.Web.Areas.Report.Controllers"]
        );
    }
}