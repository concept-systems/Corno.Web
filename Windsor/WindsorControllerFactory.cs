using System;
using System.Web.Mvc;
using System.Web.Routing;
using Castle.MicroKernel.Lifestyle;
using Castle.Windsor;

namespace Corno.Concept.Portal.Windsor;

public class WindsorControllerFactory : DefaultControllerFactory
{
    private readonly IWindsorContainer _container;

    public WindsorControllerFactory(IWindsorContainer container)
    {
        _container = container;
    }

    protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType) 
    {
        using var scope = Bootstrapper.StaticContainer.BeginScope();
        if (controllerType != null && _container.Kernel.HasComponent(controllerType))
            return (IController)_container.Resolve(controllerType);

        return base.GetControllerInstance(requestContext, controllerType);
    }

    public override void ReleaseController(IController controller)
    {
        //_container?.Release(controller);
    }
}