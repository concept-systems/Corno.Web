using System;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Corno.Web.Globals;
using Corno.Web.Logger;
using Corno.Web.Windsor;

namespace Corno.Web;

public class MvcApplication : System.Web.HttpApplication
{
    #region -- Private Methods --

    private void InitializeGlobals()
    {
        ApplicationGlobals.LoginNamespace = "Corno.Web.Controllers";
        ApplicationGlobals.LoginArea = string.Empty;
        ApplicationGlobals.LoginController = "Account";
        ApplicationGlobals.LoginAction = "Login";
    }

    /*private void InitializeEneter()
    {
        //var assemblyService = Bootstrapper.Get<IAssemblyService>();
        //assemblyService.GetType(ApplicationGlobals.OperationNamespace)

        // For Nilkamal
        var operationService = Bootstrapper.Get<IOperationService>();
        var androidService = Bootstrapper.Get<IAndroidService>();
        androidService.Start(operationService);
    }*/
    #endregion

    protected void Application_Start(object sender, EventArgs e)
    {
        Telerik.Reporting.Services.WebApi.ReportsControllerConfiguration.RegisterRoutes(GlobalConfiguration.Configuration);
        // Initialize global objects
        InitializeGlobals();

        AreaRegistration.RegisterAllAreas();

        GlobalConfiguration.Configure(WebApiConfig.Register);
        FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
        RouteConfig.RegisterRoutes(RouteTable.Routes);
        BundleConfig.RegisterBundles(BundleTable.Bundles);
        //ReportsControllerConfiguration.RegisterRoutes(GlobalConfiguration.Configuration);

        // Register
        Bootstrapper.Initialize();
        
        // Mapster
        MapsterConfig.RegisterMappings();
    }

    protected void Application_BeginRequest(object sender, EventArgs e)
    {
        // Removed debug logging for performance optimization
        // Debug logging adds overhead to every request
    }

    protected void Application_EndRequest(object sender, EventArgs e)
    {
        // Removed debug logging for performance optimization
        // Debug logging adds overhead to every request
    }


    protected void Application_End(object sender, EventArgs e)
    {
        /*var androidService = Bootstrapper.Get<IAndroidService>();
        androidService.Stop();*/
    }

    void Application_Error(object sender, EventArgs e)
    {
        // Code that runs when an unhandled error occurs
        var exception = Server.GetLastError();
        if (null == exception) return;

        LogHandler.LogError(exception);
    }

    void Session_Start(object sender, EventArgs e)
    {
        // Code that runs when a new session is started  
    }

    void Session_End(object sender, EventArgs e)
    {
        // Code that runs when a session ends.  
        // Note: The Session_End event is raised only when the sessionstate mode  
        // is set to InProc in the Web.config file. If session mode is set to StateServer  
        // or SQLServer, the event is not raised.  
    }

}