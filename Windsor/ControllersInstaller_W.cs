using System.Web.Mvc;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace Corno.Concept.Portal.Windsor;

public class ControllersInstaller : IWindsorInstaller
{
    public void Install(IWindsorContainer container, IConfigurationStore store)
    {
        container.Register(
            Classes.FromThisAssembly()
                .BasedOn<IController>()
                .If(c => c.Name.EndsWith("Controller"))
                .LifestyleScoped());
                //.Configure(c => c.LifestyleScoped()
                //    /*.Interceptors<ExceptionHandlingInterceptor>()*/));

        //ControllerBuilder.Current.SetControllerFactory(new WindsorControllerFactory(container));
    }
}