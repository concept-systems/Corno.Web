using Autofac;
using Corno.Web.Windsor;

namespace Corno.Web.Services.Report;

public static class ReportServiceResolver
{
    public static T Resolve<T>(string name = "reporting")
    {
        var scope = Bootstrapper.StaticContainer.BeginLifetimeScope(name);
        return scope.ResolveNamed<T>(name);
    }
}