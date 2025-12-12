using System;
using System.Collections;
using Telerik.Reporting;

namespace Corno.Web.Areas.Kitchen.Services.Interfaces;

public interface IReportService : IBaseReportService
{
    void FillUsersParameter(ReportParameterCollection parameters,
        string methodName, Type reportType);
    //void FillUsersParameter(ReportParameter parameter);
    void FillLocationParameter(ReportParameter parameter);
    IEnumerable UpdateLotNos(DateTime dueDate); 
}