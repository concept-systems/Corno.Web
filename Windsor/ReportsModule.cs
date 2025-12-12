using Autofac;
using Corno.Web.Reports;

namespace Corno.Web.Windsor;

public class ReportsModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterAssemblyTypes(ThisAssembly)
            .Where(t => typeof(BaseReport).IsAssignableFrom(t) && t.IsClass)
            .AsSelf()
            .InstancePerLifetimeScope();
    }
}