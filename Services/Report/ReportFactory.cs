using System;
using Autofac;
using Corno.Web.Reports;
using Corno.Web.Services.Report.Interfaces;

namespace Corno.Web.Services.Report;

public class ReportFactory : IReportFactory
{
    #region -- Constructors --
    public ReportFactory(ILifetimeScope lifetimeScope)
    {
        _lifetimeScope = lifetimeScope;
    }
    #endregion

    #region -- Data Members --
    private readonly ILifetimeScope _lifetimeScope;
    #endregion

    #region -- Public Methods --
    public BaseReport CreateReport(Type reportType)
    {
        //return new HandoverRpt();

        if (!typeof(BaseReport).IsAssignableFrom(reportType))
            throw new ArgumentException(@"Type must be a subclass of BaseReport", nameof(reportType));

        // Fallback: create manually
        return (BaseReport)Activator.CreateInstance(reportType);

        // Using Autofac to resolve dependencies
        //return (BaseReport)_lifetimeScope.Resolve(reportType);

        /*Bootstrapper.StaticContainer.BeginLifetimeScope();
        var report = Bootstrapper.Get<BaseReport>(reportType);
        return report;*/
    }
    #endregion
}