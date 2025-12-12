using Autofac;
using Corno.Web.Services.Interfaces;

namespace Corno.Web.Windsor;

public class ServicesModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterAssemblyTypes(ThisAssembly)
            .Where(t => typeof(IService).IsAssignableFrom(t) && t.IsClass)
            .AsImplementedInterfaces()
            .InstancePerLifetimeScope();
    }
}