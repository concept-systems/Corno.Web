using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Corno.Web.Areas.Kitchen.Services.Interfaces;
using Corno.Web.Dtos;
using Corno.Web.Globals;
using Corno.Web.Reports;
using Telerik.Reporting;
using Telerik.Reporting.Processing;
using ReportItemBase = Telerik.Reporting.ReportItemBase;
using TextBox = Telerik.Reporting.TextBox;

namespace Corno.Web.Areas.Kitchen.Services;

public class BaseReportService : IBaseReportService
{
    /// <summary>
    /// Helper method to execute async operations in synchronous contexts (like DataObjectMethod callbacks).
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
    #region -- Save Report --
    public void SaveReport(string format, Telerik.Reporting.Report report, string fileName)
    {
        var reportProcessor = new ReportProcessor();
        var instanceReportSource = new InstanceReportSource { ReportDocument = report };
        var result = reportProcessor.RenderReport(format, instanceReportSource, null);

        using var fs = new FileStream(fileName, FileMode.Create);

        fs.Write(result.DocumentBytes, 0, result.DocumentBytes.Length);
        fs.Close();
    }
    #endregion

    #region -- Printing --
    public static void PrintDirectToPrinter(BaseReport report, int copies = 1, string printerName = null)
    {
        // Obtain the settings of the default printer
        var printerSettings = new PrinterSettings { Copies = (short)copies };
        if (null != printerName)
            printerSettings.PrinterName = printerName;

        // The standard print controller comes with no UI
        PrintController standardPrintController =
            new StandardPrintController();

        // Print the report using the custom print controller
        var reportProcessor = new ReportProcessor { PrintController = standardPrintController };

        if (report == null)
            throw new Exception("Report is null");
        var instanceReportSource = new InstanceReportSource
        {
            ReportDocument = report
        };
        reportProcessor.PrintReport(instanceReportSource, printerSettings);
    }

    public virtual void PrintDirectToPrinter(IEnumerable<BaseReport> reports, int copies = 1, string printerName = null)
    {
        // Obtain the settings of the default printer
        var printerSettings = new PrinterSettings { Copies = (short)copies, };
        if (null != printerName)
            printerSettings.PrinterName = printerName;

        var reportDocuments = reports.ToList();
        var firstReport = reportDocuments.FirstOrDefault();
        var reportPaperSize = firstReport?.PageSettings.PaperSize;
        if (null != reportPaperSize)
        {
            var printerPaperSize = new PaperSize("CUSTOM",
                (int)reportPaperSize.Value.Width.Value, (int)reportPaperSize.Value.Height.Value);
            printerSettings.PaperSizes.Add(printerPaperSize);
            printerSettings.DefaultPageSettings.PaperSize = printerPaperSize;
        }

        // The standard print controller comes with no UI
        PrintController standardPrintController = new StandardPrintController();

        // Print the report using the custom print controller
        var reportProcessor = new ReportProcessor { PrintController = standardPrintController};

        if (!reportDocuments.Any())
            throw new Exception("Report is null");
        var reportBook = new ReportBook();
        foreach (var report in reportDocuments)
        {
            reportBook.ReportSources?.Add(new InstanceReportSource { ReportDocument = report, });
        }

        var instanceReportSource = new InstanceReportSource
        {
            ReportDocument = reportBook
        };

        reportProcessor.PrintReport(instanceReportSource, printerSettings);
    }

    /*public void PrintDirectToPrinter(ReportSource reportSource, short copies = 1, string printerName = null)
    {
        // Obtain the settings of the default printer
        var printerSettings = new PrinterSettings { Copies = copies };
        if (null != printerName)
            printerSettings.PrinterName = printerName;

        // The standard print controller comes with no UI
        PrintController standardPrintController = new PrintControllerWithStatusDialog(new StandardPrintController(), "Print");

        // Print the report using the custom print controller
        var reportProcessor = new ReportProcessor { PrintController = standardPrintController };
        reportProcessor.PrintReport(reportSource, printerSettings);
    }*/

    #endregion

    #region -- General --
    public void MakeFieldsInvisible(ReportItemBase.ItemCollection itemCollection, DataTable dataTable)
    {
        // Remove all unnecessary items
        foreach (var reportItem in itemCollection)
        {
            if (reportItem.GetType() == typeof(TextBox))
            {
                if (reportItem.Name.Substring(0, 3) == "txt" || reportItem.Name.Substring(0, 3) == "lbl")
                {
                    var fieldName = reportItem.Name.Remove(0, 3);
                    var contains = dataTable.Columns.Contains(fieldName);
                    if (false == contains)
                    {
                        reportItem.Visible = false;
                        ((TextBox)reportItem).Value = string.Empty;
                    }
                }
            }

            if (reportItem.Items.Any())
                MakeFieldsInvisible(reportItem.Items, dataTable);
        }
    }

    public ArrayList GetSelectedValues(string columnName, DataTable dataTable)
    {
        var selectedValues = new ArrayList();
        foreach (DataRow filterRow in dataTable.Rows)
        {
            if (string.Empty == filterRow[columnName].ToString() ||
                "" == filterRow[columnName].ToString())
                continue;

            if (false == selectedValues.Contains(filterRow[columnName]))
            {
                selectedValues.Add(filterRow[columnName]);
            }
        }

        return selectedValues;
    }

    public Telerik.Reporting.Report GetTelerikReport(string reportName)
    {
        Type type;
        //if (type != null)
        //    return (Report) assembly.CreateInstance(type.FullName);

        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        foreach (var assembly in assemblies)
        {
            type = assembly.GetType(reportName);
            if (type != null)
                return (Telerik.Reporting.Report)assembly.CreateInstance(type.FullName ?? throw new InvalidOperationException());
        }

        // Get Assembly name from its type name
        var nameStrings = reportName.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
        var assemblyName = nameStrings[0];
        for (var index = 1; index < nameStrings.Length - 1; index++)
        {
            assemblyName += "." + nameStrings[index];
            try
            {
                var assembly = Assembly.Load(assemblyName);
                if (null == assembly)
                    continue;
                type = assembly.GetType(reportName);
                if (type != null)
                    return (Telerik.Reporting.Report)assembly.CreateInstance(type.FullName ?? throw new InvalidOperationException());
            }
            catch
            {
                //Ignore
            }
        }

        return null;
    }

    public void SaveReport(Telerik.Reporting.Report report, string fileName)
    {
        var reportProcessor = new ReportProcessor();
        var instanceReportSource = new InstanceReportSource { ReportDocument = report };
        var result = reportProcessor.RenderReport("PDF", instanceReportSource, null);

        using var fs = new FileStream(fileName, FileMode.Create);

        fs.Write(result.DocumentBytes, 0, result.DocumentBytes.Length);
        fs.Close();
    }
    #endregion

    #region -- Parameters --
    public ReportParameter CreateParameter(string name, IEnumerable dataSource, bool multiValue = false)
    {
        return CreateParameter(name, FieldConstants.Id, FieldConstants.Name,
            dataSource, multiValue);
    }

    public ReportParameter CreateParameter(string name, string valueMember, string displayMember,
        IEnumerable dataSource, bool multiValue = false)
    {
        var parameter = new ReportParameter
        {
            Name = name, 
            Text = name.ToCamelCase(),
            MultiValue = multiValue, 
            Value = $"= Fields.{valueMember}", 
            Visible = true,
            AvailableValues =
            {
                ValueMember = $"= Fields.{valueMember}",
                DisplayMember = $"= Fields.{displayMember}",
                DataSource = dataSource
            }
        };

        return parameter;
    }

    public void FillStatusParameter(ReportParameter parameter)
    {
        if (parameter == null)
            return;
        parameter.AvailableValues.ValueMember = "=Fields.Id";
        parameter.AvailableValues.DisplayMember = "=Fields.Name";
        parameter.AvailableValues.DataSource = new List<MasterDto>
        {
            new() {Id = 0, Name = "All"},
            new() {Id = 1, Name = "Completed"},
            new() {Id = 2, Name = "Pending"},
        };
        parameter.Value = "=Fields.Id";
    }

    /*public void FillStatusParameter(ReportParameter parameter, IEnumerable dataSource)
    {
        if (parameter == null)
            return;
        parameter.AvailableValues.ValueMember = "=Fields.Id";
        parameter.AvailableValues.DisplayMember = "=Fields.Name";
        parameter.AvailableValues.DataSource = dataSource;
        parameter.Value = "=Fields.Id";
    }*/

    public virtual void FillOneFilterParameter(ReportParameterCollection parameters,
        string methodName, Type reportType, string filterName, Type filterType, string fieldName)
    {
        var objectDataSource = new ObjectDataSource
        {
            DataMember = methodName,
            DataSource = reportType
        };

        var dateFieldName = filterName.Substring(0, 1).ToLower() + filterName.Substring(1);
        objectDataSource.Parameters.Add(new ObjectDataSourceParameter(dateFieldName, filterType, $"=Parameters.{filterName}"));
        parameters[fieldName].AvailableValues.DataSource = objectDataSource;
    }

    public virtual void FillTwoDateFilterParameter(ReportParameterCollection parameters,
        string methodName, Type reportType, string fieldName)
    {
        var objectDataSource = new ObjectDataSource
        {
            DataMember = methodName,
            DataSource = reportType
        };
        objectDataSource.Parameters.Add(new ObjectDataSourceParameter("fromDate", typeof(DateTime), "=Parameters.FromDate"));
        objectDataSource.Parameters.Add(new ObjectDataSourceParameter("toDate", typeof(DateTime), "=Parameters.ToDate"));
        parameters[fieldName].AvailableValues.DataSource = objectDataSource;
    }

    public virtual void AddIsPreviewParameter(ReportParameterCollection parameters)
    {
        parameters.Add("isPreview", ReportParameterType.Boolean, true);
        parameters["isPreview"].Visible = true;
        parameters["isPreview"].Value = true;  // Default to false (export)
    }

    #endregion
}