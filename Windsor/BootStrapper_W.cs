using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Reflection;
using System.Web.Mvc;
using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.Installer;
using Corno.Concept.Portal.Dtos;
using Corno.Concept.Portal.Globals;

namespace Corno.Concept.Portal.Windsor;

public class Bootstrapper : IContainerAccessor, IDisposable
{
    #region -- Constructors --
    private Bootstrapper(IWindsorContainer container)
    {
        _container = container;
        Container = container;
    }
    #endregion

    #region -- Data Members --
    static IWindsorContainer _container;
    public static IWindsorContainer StaticContainer => _container;
    #endregion

    #region -- Properties --
    public IWindsorContainer Container { get; }
    #endregion

    #region -- Methods --
    public static Bootstrapper Bootstrap()
    {
        var container = new WindsorContainer().
            Install(FromAssembly.This());
        ControllerBuilder.Current.SetControllerFactory(new WindsorControllerFactory(container));
        return new Bootstrapper(container);
    }

    public static void RegisterDbContext(DbContext dbContext)
    {
        _container?.Register(Component.For(dbContext.GetType())
            .DependsOn(Property.ForKey<string>()
                .Eq("Name = " + FieldConstants.CornoContext)));
    }

    public static void RegisterType(IRegistration[] registrations)
    {
        _container?.Register(registrations);
    }

    public static void RegisterType(Type interfaceType, Type serviceType, Dependency dependency = null)
    {
        _container?.Register(Component.For(interfaceType)
            .ImplementedBy(serviceType)
            .DependsOn(dependency)
            .LifestylePerThread());
    }

    public static void RegisterType(Type interfaceType, Type serviceType,
        List<Dependency> dependencies = null)
    {
        _container?.Register(Component.For(interfaceType)
            .ImplementedBy(serviceType)
            .DependsOn(dependencies)
            .LifestylePerThread());
    }

    public static TEntity Get<TEntity>(Type tEntity, BaseDto dto = default)
    {
        if (null == dto)
            return (TEntity)_container?.Resolve(tEntity);
        return (TEntity)_container?.Resolve(tEntity, new Arguments { { "dto", dto } });
    }

    public static TEntity Get<TEntity>()
    {
        return (TEntity)_container?.Resolve(typeof(TEntity));
    }

    public static TEntity Get<TEntity>(BaseDto dto)
    {
        return _container.Resolve<TEntity>(new Arguments { { "dto", dto } });
    }

    public static Telerik.Reporting.Report GetLabel(string labelFormat)
    {
        var assembly = Assembly.GetExecutingAssembly();

        return GetLabel(labelFormat, assembly);
    }

    public static Telerik.Reporting.Report GetLabel(string labelFormat, Assembly assembly)
    {
        Telerik.Reporting.Report labelRpt = null;
        foreach (var type in assembly.GetTypes())
        {
            if (string.IsNullOrEmpty(labelFormat) || !string.Equals(type.Name, labelFormat, StringComparison.CurrentCultureIgnoreCase)) continue;

            if (type.FullName != null) labelRpt = (Telerik.Reporting.Report)assembly.CreateInstance(type.FullName);
            break;
        }

        return labelRpt;
    }

    public void Dispose()
    {
        _container.Dispose();
    }
    #endregion
}