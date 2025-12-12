using System;
using Corno.Web.Reports;

namespace Corno.Web.Services.Report.Interfaces;

public interface IReportFactory
{
    BaseReport CreateReport(Type reportType);
}