using System.Collections.Generic;
using System.Threading.Tasks;
using Corno.Web.Models.HealthCheck;

namespace Corno.Web.Services.HealthCheck.Interfaces;

public enum HealthStatus
{
    Healthy,
    Warning,
    Critical
}

public class HealthCheckResult
{
    public string CheckName { get; set; }
    public HealthStatus Status { get; set; }
    public string Message { get; set; }
    public Dictionary<string, object> Details { get; set; }
    public int ExecutionTimeMs { get; set; }
    public bool AutoFixed { get; set; }
    public string AutoFixMessage { get; set; }
}

public interface IHealthCheckService
{
    Task<List<HealthCheckResult>> RunAllChecksAsync();
    Task<HealthCheckResult> CheckDatabaseConnectivityAsync();
    Task<HealthCheckResult> CheckConnectionPoolAsync();
    Task<HealthCheckResult> CheckDiskSpaceAsync();
    Task<HealthCheckResult> CheckMemoryUsageAsync();
    Task<HealthCheckResult> CheckErrorRateAsync();
    Task<HealthCheckResult> CheckApplicationPerformanceAsync();
    Task SaveHealthReportAsync(List<HealthCheckResult> results);
    Task<Models.HealthCheck.HealthCheckSummary> GetLatestSummaryAsync();
    Task<List<Models.HealthCheck.HealthCheckReport>> GetRecentReportsAsync(int days = 7);
}

