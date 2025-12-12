using System;
using System.Configuration;
using System.Web;
using Castle.MicroKernel.Context;
using Castle.MicroKernel;
using Castle.MicroKernel.Lifestyle.Scoped;
using Castle.MicroKernel.Registration;
using Castle.Windsor.Installer;
using Corno.Concept.Portal.Repository;
using Corno.Concept.Portal.Repository.Interfaces;
using Corno.Concept.Portal.Windsor.Context;

namespace Corno.Concept.Portal.Windsor;

public static class BootstrapperPortal
{
    #region -- Data Members --
    private static Bootstrapper _bootstrapper;
    #endregion

    #region -- Methods --
    public static void Initialize()
    {
        // Initialize
        _bootstrapper = Bootstrapper.Bootstrap();
        
        var connectionString = ConfigurationManager.ConnectionStrings["CornoContext"].ConnectionString;
        
        _bootstrapper.Container?.Register(Component.For<BaseDbContext>()
            .ImplementedBy<ErpDbContext>()
            .DependsOn(Dependency.OnValue("connectionString", connectionString))
            .LifestyleScoped()
        );
        
        _bootstrapper.Container?.Register(Component.For(typeof(IGenericRepository<>))
            .ImplementedBy(typeof(GenericRepository<>))
            .LifestyleScoped());

        _bootstrapper.Container?.Install( /*FromAssembly.This(),*/
            FromAssembly.Containing<ServicesInstaller>(),
            FromAssembly.Containing<ControllersInstaller>(),
            FromAssembly.Containing<ReportsInstaller>());
    }
    #endregion
}
