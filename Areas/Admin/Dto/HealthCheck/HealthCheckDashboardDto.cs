namespace Corno.Web.Areas.Admin.Dto.HealthCheck;

public class HealthCheckDashboardDto
{
    public HealthCheckSummaryDto LatestSummary { get; set; }
    public int TotalIssues { get; set; }
    public int CriticalIssues { get; set; }
    public int WarningIssues { get; set; }
    public int AutoFixedCount { get; set; }
    public System.DateTime LastCheckDate { get; set; }
}

