using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Corno.Web.Reports;
using Corno.Web.Services.Interfaces;
using Telerik.Reporting;

namespace Corno.Web.Areas.Kitchen.Services.Interfaces;

public interface IBaseReportService : IService
{
    #region -- Methods --

    void MakeFieldsInvisible(ReportItemBase.ItemCollection itemCollection, DataTable dataTable);
    Telerik.Reporting.Report GetTelerikReport(string reportName);
    //void SaveReport(string format, Telerik.Reporting.Report report, string fileName);
    void SaveReport(Telerik.Reporting.Report report, string fileName);
    //void PrintReportDirectToPrinter(Telerik.Reporting.Report report, short copies = 1, 
    //    string printerName = null);
    void PrintDirectToPrinter(IEnumerable<BaseReport> reports,
        int copies = 1, string printerName = null);
    /*void PrintDirectToPrinter(ReportSource reportSource, short copies = 1,
        string printerName = null);*/

    #region -- Parameters --
    ReportParameter CreateParameter(string name, IEnumerable dataSource, bool multiValue = false);
    ReportParameter CreateParameter(string name, string valueMember, string displayMember,
        IEnumerable dataSource, bool multiValue = false);
    void FillStatusParameter(ReportParameter parameter);

    void FillOneFilterParameter(ReportParameterCollection parameters,
        string methodName, Type reportType, string filterName, Type filterType, string fieldName);
    void FillTwoDateFilterParameter(ReportParameterCollection parameters, string methodName, 
        Type reportType, string fieldName);

    void AddIsPreviewParameter(ReportParameterCollection parameters);

    #endregion

    #endregion
}