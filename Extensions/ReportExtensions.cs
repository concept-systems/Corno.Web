using System;
using Telerik.Reporting;

namespace Corno.Web.Extensions;

public static class ReportExtensions
{
    /// <summary>
    /// Converts a Telerik Report to a Base64 string in PDF format.
    /// </summary>
    /// <param name="report">The Telerik report instance.</param>
    /// <returns>Base64 encoded PDF string.</returns>
    public static string ToBase64(this Report report)
    {
        if (report == null)
            throw new ArgumentNullException(nameof(report));

        var reportProcessor = new Telerik.Reporting.Processing.ReportProcessor();
        var result = reportProcessor.RenderReport("PDF", report, null);
        return Convert.ToBase64String(result.DocumentBytes);
    }

    /// <summary>
    /// Converts a Telerik Report to a byte array in PDF format.
    /// </summary>
    /// <param name="report">The Telerik report instance.</param>
    /// <returns>Byte array of the PDF document.</returns>
    public static byte[] ToDocumentBytes(this Report report)
    {
        if (report == null)
            throw new ArgumentNullException(nameof(report));

        var reportProcessor = new Telerik.Reporting.Processing.ReportProcessor();
        var result = reportProcessor.RenderReport("PDF", report, null);
        return result.DocumentBytes;
    }

    public static string ToBase64(this ReportBook reportBook)
    {
        if (reportBook == null)
            throw new ArgumentNullException(nameof(reportBook));

        var reportProcessor = new Telerik.Reporting.Processing.ReportProcessor();
        var result = reportProcessor.RenderReport("PDF", reportBook, null);
        return Convert.ToBase64String(result.DocumentBytes);
    }
    public static byte[] ToDocumentBytes(this ReportBook reportBook)
    {
        if (reportBook == null)
            throw new ArgumentNullException(nameof(reportBook));

        var reportProcessor = new Telerik.Reporting.Processing.ReportProcessor();
        var result = reportProcessor.RenderReport("PDF", reportBook, null);
        return result.DocumentBytes;
    }
}