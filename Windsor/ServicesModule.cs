using Autofac;
using Corno.Web.Services.Import;
using Corno.Web.Services.Import.Interfaces;
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

        // Register generic FileImportService for IFileImportService
        builder.RegisterGeneric(typeof(FileImportService<>))
            .As(typeof(IFileImportService<>))
            .InstancePerLifetimeScope();

        // Register ImportSessionService as singleton (shared across requests)
        builder.RegisterType<ImportSessionService>()
            .AsSelf()
            .SingleInstance();
    }
}