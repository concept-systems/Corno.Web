using System;
using System.Threading.Tasks;
using Corno.Web.Logger;
using Corno.Web.Services.Report;
using Telerik.Reporting;
using Telerik.ReportViewer.Common;

namespace Corno.Web.Reports;

public class BaseReport : Report
{
    #region -- Constructors --

    public BaseReport()
    {
        Error += BaseReport_Error;

        Style.Font.Name = "Segoe UI";
        Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8.25D);
    }
    #endregion

    #region -- Properties --
    public object Attributes { get; set; }
    #endregion

    #region -- Methods --
    // Export To CSV
    public virtual void ExportToCsv(string filePath)
    {

    }

    public virtual object GetDataSource()
    {
        return DataSource;
    }


    protected T ResolveService<T>()
    {
        return ReportServiceResolver.Resolve<T>();
    }

    /// <summary>
    /// Helper method to execute async operations in synchronous contexts (like NeedDataSource event handlers).
    /// This prevents deadlocks by running async code on a thread pool thread, avoiding synchronization context capture.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="asyncFunc">The async function to execute.</param>
    /// <returns>The result of the async operation.</returns>
    protected static TResult RunAsync<TResult>(Func<Task<TResult>> asyncFunc)
    {
        return Task.Run(async () => await asyncFunc().ConfigureAwait(false))
            .GetAwaiter().GetResult();
    }

    #endregion

    #region -- Events --
    public virtual void OnActionExecuting(object sender, InteractiveActionCancelEventArgs args)
    {

    }

    private void BaseReport_Error(object sender, ErrorEventArgs eventArgs)
    {
        var exception = LogHandler.GetDetailException(eventArgs.Exception);
        LogHandler.LogInfo(exception);
        LogHandler.LogError(exception);
    }
    #endregion
}