using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace Corno.Concept.Portal.Windsor;

public class ReportsInstaller : IWindsorInstaller
{
    public void Install(IWindsorContainer container, IConfigurationStore store)
    {
        container.Register(Classes
            .FromThisAssembly()
            .BasedOn<Telerik.Reporting.Report>()
            .LifestyleScoped());
        //.Configure(c => c.Interceptors<ExceptionHandlingInterceptor>()));
    }
}