using System;

namespace Corno.Web.Areas.Admin.Dto.HealthCheck;

public class HealthCheckSummaryDto
{
    public int Id { get; set; }
    public DateTime ReportDate { get; set; }
    public string OverallStatus { get; set; }
    public int TotalChecks { get; set; }
    public int HealthyChecks { get; set; }
    public int WarningChecks { get; set; }
    public int CriticalChecks { get; set; }
    public string SummaryDetails { get; set; }
}

