namespace Corno.Web.Controllers;

using System.IO;
using System.Web;
using Telerik.Reporting.Cache.File;
using Telerik.Reporting.Services;
using Telerik.Reporting.Services.WebApi;

//The class name determines the service URL. 
//ReportsController class name defines /api/reports/ service URL.
public class ReportsController : ReportsControllerBase
{
    static ReportServiceConfiguration configurationInstance;

    static ReportsController()
    {
        //This is the folder that contains the report definitions
        //In this case this is the Reports folder
        var appPath = HttpContext.Current.Server.MapPath("~/");
        var reportsPath = Path.Combine(appPath, "Reports");

        //Standard practice: Use TypeReportSourceResolver for class-based reports
        //TypeReportSourceResolver will create report instances from type names (e.g., "Corno.Web.Areas.Report.Reports.MyReport")
        //No Session required - reports are created on-demand by the resolver
        var resolver = new TypeReportSourceResolver();
        
        // If you also need to support .trdx/.trdp files, you can add UriReportSourceResolver as fallback
        // For now, using only TypeReportSourceResolver for class-based reports (standard approach)

        //Setup the ReportServiceConfiguration
        configurationInstance = new ReportServiceConfiguration
        {
            HostAppId = "Html5App",
            Storage = new FileStorage(),
            ReportSourceResolver = resolver,
            // ReportSharingTimeout = 0,
            // ClientSessionTimeout = 15,
        };
    }

    public ReportsController()
    {
        //Initialize the service configuration
        ReportServiceConfiguration = configurationInstance;
    }
}