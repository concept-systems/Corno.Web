using System;
using System.Collections.Generic;
using System.Web;
using Telerik.Reporting;
using Telerik.Reporting.Services;
using Corno.Web.Globals;

namespace Corno.Web.Services.Report;

/// <summary>
/// Custom resolver that retrieves reports from Session for HTML5 viewer
/// This resolver chains to other resolvers as fallback
/// </summary>
public class SessionReportSourceResolver : IReportSourceResolver
{
    private readonly IList<IReportSourceResolver> _fallbackResolvers;

    public SessionReportSourceResolver()
    {
        _fallbackResolvers = new List<IReportSourceResolver>();
    }

    public SessionReportSourceResolver(IReportSourceResolver fallbackResolver)
    {
        _fallbackResolvers = new List<IReportSourceResolver>();
        if (fallbackResolver != null)
        {
            _fallbackResolvers.Add(fallbackResolver);
        }
    }

    public SessionReportSourceResolver AddFallbackResolver(IReportSourceResolver fallbackResolver)
    {
        if (fallbackResolver == null)
            return this;

        var newResolver = new SessionReportSourceResolver();
        foreach (var existing in _fallbackResolvers)
        {
            newResolver._fallbackResolvers.Add(existing);
        }
        newResolver._fallbackResolvers.Add(fallbackResolver);
        return newResolver;
    }

    public ReportSource Resolve(string report, OperationOrigin operationOrigin, IDictionary<string, object> currentParameterValues)
    {
        if (string.IsNullOrEmpty(report))
            return null;

        var session = HttpContext.Current?.Session;
        
        // Check if this is a session-based report identifier
        if (session != null)
        {
            // Check if it's a session key identifier (format: "SESSION:FieldConstants.Label")
            if (report.StartsWith("SESSION:", StringComparison.OrdinalIgnoreCase))
            {
                var sessionKey = report.Substring("SESSION:".Length);
                
                if (session[sessionKey] != null)
                {
                    var reportDocument = session[sessionKey] as IReportDocument;
                    
                    if (reportDocument != null)
                    {
                        return new InstanceReportSource
                        {
                            ReportDocument = reportDocument
                        };
                    }
                }
            }
            // Also check if identifier matches a report type that's stored in Session
            else if (session[FieldConstants.Label] != null)
            {
                var reportDocument = session[FieldConstants.Label];
                var reportType = reportDocument.GetType();
                
                // If the identifier matches the report type name, return the Session report
                if (reportType.FullName == report || reportType.Name == report)
                {
                    var reportDoc = reportDocument as IReportDocument;
                    if (reportDoc != null)
                    {
                        return new InstanceReportSource
                        {
                            ReportDocument = reportDoc
                        };
                    }
                }
            }
        }

        // Try fallback resolvers in sequence
        foreach (var fallbackResolver in _fallbackResolvers)
        {
            var result = fallbackResolver?.Resolve(report, operationOrigin, currentParameterValues);
            if (result != null)
                return result;
        }

        return null;
    }
}

