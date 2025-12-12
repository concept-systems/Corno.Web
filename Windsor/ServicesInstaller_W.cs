using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Corno.Concept.Portal.Services.Interfaces;

namespace Corno.Concept.Portal.Windsor;

public class ServicesInstaller : IWindsorInstaller
{
    public void Install(IWindsorContainer container, Castle.MicroKernel.SubSystems.Configuration.IConfigurationStore store)
    {
        container.Register(Classes.FromThisAssembly()
            .BasedOn(typeof(IService))
            .WithServiceAllInterfaces()
            .LifestyleScoped()
            .Configure(c => c.LifestyleScoped()));
        //    /*.Interceptors<ExceptionHandlingInterceptor>()*/));
    }
}