using System.Web.Mvc;
using Autofac;

namespace Corno.Web.Windsor;

public class ControllersModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterAssemblyTypes(ThisAssembly)
            .Where(t => typeof(Controller).IsAssignableFrom(t))
            .InstancePerLifetimeScope();
    }
}