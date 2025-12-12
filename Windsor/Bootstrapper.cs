using System;
using System.Configuration;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using Corno.Web.Dtos;
using Corno.Web.Repository.Interfaces;
using Corno.Web.Repository;
using Corno.Web.Services.File;
using Corno.Web.Services.File.Interfaces;
using Corno.Web.Services.Report;
using Corno.Web.Services.Report.Interfaces;
using Corno.Web.Windsor.Context;

namespace Corno.Web.Windsor
{
    public class Bootstrapper : IDisposable
    {
        private static IContainer _container;
        public static IContainer StaticContainer => _container;

        public static void Initialize()
        {
            var builder = new ContainerBuilder();

            var connectionString = ConfigurationManager.ConnectionStrings["CornoContext"].ConnectionString;
            // Register DbContext
            builder.Register(c => new ErpDbContext(connectionString))
                .As<BaseDbContext>()
                .InstancePerLifetimeScope();

            builder.RegisterGeneric(typeof(GenericRepository<>))
                .As(typeof(IGenericRepository<>))
                .InstancePerLifetimeScope();
            builder.RegisterGeneric(typeof(ExcelFileService<>))
                .As(typeof(IExcelFileService<>))
                .InstancePerLifetimeScope();
            builder.RegisterGeneric(typeof(CsvFileService<>))
                .As(typeof(ICsvFileService<>))
                .InstancePerLifetimeScope();

            builder.RegisterType<ReportFactory>()
                .As<IReportFactory>()
                .InstancePerLifetimeScope(); // Important to match the report's lifetime

            // Register services, repositories, etc. via assembly scanning
            // Register MVC controllers
            builder.RegisterControllers(Assembly.GetExecutingAssembly());
            builder.RegisterAssemblyModules(
                //Assembly.GetExecutingAssembly(),
                Assembly.GetAssembly(typeof(ServicesModule)),
                //Assembly.GetAssembly(typeof(ControllersModule)),
                Assembly.GetAssembly(typeof(ReportsModule))
                //Assembly.GetAssembly(typeof(ImportModule))
            );

            //builder.RegisterFilterProvider(); // Important for MVC filters

            _container = builder.Build();

            // Set Autofac as the MVC Dependency Resolver
            DependencyResolver.SetResolver(new AutofacDependencyResolver(_container));
        }

        public static void RegisterType<TInterface, TImplementation>() where TImplementation : TInterface
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<TImplementation>().As<TInterface>().InstancePerLifetimeScope();
        }

        public static TEntity Get<TEntity>()
        {
            // Try to get the request lifetime scope if HTTP context is available
            // This handles the normal case where we're in an HTTP request
            ILifetimeScope requestScope = null;
            try
            {
                // Check if HTTP context exists and Autofac resolver is available
                if (HttpContext.Current != null)
                {
                    var resolver = AutofacDependencyResolver.Current;
                    if (resolver != null)
                    {
                        requestScope = resolver.RequestLifetimeScope;
                        if (requestScope != null)
                        {
                            return requestScope.Resolve<TEntity>();
                        }
                    }
                }
            }
            catch
            {
                // If accessing RequestLifetimeScope fails (e.g., HTTP context not properly initialized,
                // or we're on a thread pool thread where HttpContext.Current is null),
                // fall through to fallback
            }

            // Fallback: Create a new lifetime scope when HTTP context is not available
            // This is needed for scenarios like:
            // 1. Report generation outside of HTTP requests
            // 2. When RunAsync executes on thread pool threads (where HttpContext.Current is null)
            // 3. When the request lifetime scope is not available
            // 
            // Note: The scope is not disposed here because the service and its dependencies
            // (like DbContext) need to remain alive for the duration of async operations.
            // The scope will be disposed when the service is garbage collected or when
            // the application shuts down. This is acceptable for services used immediately
            // in constructors where RunAsync blocks until async operations complete.
            var scope = _container.BeginLifetimeScope();
            return scope.Resolve<TEntity>();
        }

        public static TEntity Get<TEntity>(BaseDto dto)
        {
            return _container.Resolve<TEntity>(new NamedParameter("dto", dto));
        }

        public static TEntity Get<TEntity>(Type tEntity, BaseDto dto = default)
        {
            if (null == dto)
                return (TEntity)_container?.Resolve(tEntity);
            return (TEntity)_container?.Resolve(tEntity, new NamedParameter("dto", dto));
        }

        public static Telerik.Reporting.Report GetLabel(string labelFormat)
        {
            return GetLabel(labelFormat, Assembly.GetExecutingAssembly());
        }

        public static Telerik.Reporting.Report GetLabel(string labelFormat, Assembly assembly)
        {
            foreach (var type in assembly.GetTypes())
            {
                if (!string.Equals(type.Name, labelFormat, StringComparison.CurrentCultureIgnoreCase)) continue;
                return (Telerik.Reporting.Report)Activator.CreateInstance(type);
            }

            return null;
        }

        public void Dispose()
        {
            _container?.Dispose();
        }
    }
}
