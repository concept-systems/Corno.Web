using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Corno.Web.Areas.Admin.Dto.HealthCheck;
using Corno.Web.Areas.Admin.Services.Interfaces;
using Corno.Web.Models.HealthCheck;
using Corno.Web.Repository.Interfaces;
using Corno.Web.Services.HealthCheck.DataIntegrity;
using Corno.Web.Services.HealthCheck.Interfaces;
using Corno.Web.Windsor;
using Newtonsoft.Json;
using HealthCheckResult = Corno.Web.Services.HealthCheck.Interfaces.HealthCheckResult;

namespace Corno.Web.Areas.Admin.Services;

public class HealthCheckUIService : IHealthCheckUIService
{
    private readonly IGenericRepository<HealthCheckReport> _reportRepository;
    private readonly IGenericRepository<HealthCheckSummary> _summaryRepository;
    private readonly IGenericRepository<DataIntegrityReport> _dataIntegrityRepository;
    private readonly IHealthCheckService _healthCheckService;
    private readonly IDataIntegrityHealthCheckService _dataIntegrityService;

    public HealthCheckUIService(
        IGenericRepository<HealthCheckReport> reportRepository,
        IGenericRepository<HealthCheckSummary> summaryRepository,
        IGenericRepository<DataIntegrityReport> dataIntegrityRepository)
    {
        _reportRepository = reportRepository;
        _summaryRepository = summaryRepository;
        _dataIntegrityRepository = dataIntegrityRepository;
        _healthCheckService = Bootstrapper.Get<IHealthCheckService>();
        _dataIntegrityService = Bootstrapper.Get<IDataIntegrityHealthCheckService>();
    }

    public async Task<HealthCheckDashboardDto> GetDashboardDataAsync()
    {
        var latestSummary = await GetLatestSummaryAsync().ConfigureAwait(false);
        var cutoffDate = DateTime.Now.AddDays(-7);
        var recentReports = await _reportRepository.GetAsync(r => r.CheckDate >= cutoffDate, 
            r => r).ConfigureAwait(false);

        return new HealthCheckDashboardDto
        {
            LatestSummary = latestSummary,
            TotalIssues = recentReports.Count(r => r.Status != "Healthy"),
            CriticalIssues = recentReports.Count(r => r.Status == "Critical"),
            WarningIssues = recentReports.Count(r => r.Status == "Warning"),
            AutoFixedCount = recentReports.Count(r => r.AutoFixed),
            LastCheckDate = latestSummary?.ReportDate ?? DateTime.MinValue
        };
    }

    public async Task<List<HealthCheckReportDto>> GetHealthCheckReportsAsync(DateTime? fromDate, DateTime? toDate, string status = null)
    {
        System.Linq.Expressions.Expression<Func<HealthCheckReport, bool>> filter = r => true;
        
        if (fromDate.HasValue && toDate.HasValue)
            filter = r => r.CheckDate >= fromDate.Value && r.CheckDate <= toDate.Value;
        else if (fromDate.HasValue)
            filter = r => r.CheckDate >= fromDate.Value;
        else if (toDate.HasValue)
            filter = r => r.CheckDate <= toDate.Value;
        
        if (!string.IsNullOrEmpty(status))
        {
            var fromDateVal = fromDate;
            var toDateVal = toDate;
            if (fromDateVal.HasValue && toDateVal.HasValue)
                filter = r => r.CheckDate >= fromDateVal.Value && r.CheckDate <= toDateVal.Value && r.Status == status;
            else if (fromDateVal.HasValue)
                filter = r => r.CheckDate >= fromDateVal.Value && r.Status == status;
            else if (toDateVal.HasValue)
                filter = r => r.CheckDate <= toDateVal.Value && r.Status == status;
            else
                filter = r => r.Status == status;
        }

        var reports = await _reportRepository.GetAsync(
            filter,
            r => r,
            q => q.OrderByDescending(x => x.CheckDate)
        ).ConfigureAwait(false);

        return reports.Select(r => new HealthCheckReportDto
        {
            Id = r.Id,
            CheckDate = r.CheckDate,
            CheckType = r.CheckType,
            Status = r.Status,
            Message = r.Message,
            Details = r.Details,
            ExecutionTimeMs = r.ExecutionTimeMs,
            AutoFixed = r.AutoFixed,
            CreatedBy = r.CreatedBy
        }).ToList();
    }

    public async Task<List<DataIntegrityIssueDto>> GetDataIntegrityIssuesAsync(DateTime? fromDate, DateTime? toDate, string checkType = null, string status = null)
    {
        System.Linq.Expressions.Expression<Func<DataIntegrityReport, bool>> filter = r => true;
        
        var fromDateVal = fromDate;
        var toDateVal = toDate;
        var checkTypeVal = checkType;
        var statusVal = status;
        
        if (fromDateVal.HasValue && toDateVal.HasValue)
        {
            if (!string.IsNullOrEmpty(checkTypeVal) && !string.IsNullOrEmpty(statusVal))
                filter = r => r.CheckDate >= fromDateVal.Value && r.CheckDate <= toDateVal.Value && r.CheckType == checkTypeVal && r.Status == statusVal;
            else if (!string.IsNullOrEmpty(checkTypeVal))
                filter = r => r.CheckDate >= fromDateVal.Value && r.CheckDate <= toDateVal.Value && r.CheckType == checkTypeVal;
            else if (!string.IsNullOrEmpty(statusVal))
                filter = r => r.CheckDate >= fromDateVal.Value && r.CheckDate <= toDateVal.Value && r.Status == statusVal;
            else
                filter = r => r.CheckDate >= fromDateVal.Value && r.CheckDate <= toDateVal.Value;
        }
        else if (fromDateVal.HasValue)
        {
            if (!string.IsNullOrEmpty(checkTypeVal) && !string.IsNullOrEmpty(statusVal))
                filter = r => r.CheckDate >= fromDateVal.Value && r.CheckType == checkTypeVal && r.Status == statusVal;
            else if (!string.IsNullOrEmpty(checkTypeVal))
                filter = r => r.CheckDate >= fromDateVal.Value && r.CheckType == checkTypeVal;
            else if (!string.IsNullOrEmpty(statusVal))
                filter = r => r.CheckDate >= fromDateVal.Value && r.Status == statusVal;
            else
                filter = r => r.CheckDate >= fromDateVal.Value;
        }
        else if (toDateVal.HasValue)
        {
            if (!string.IsNullOrEmpty(checkTypeVal) && !string.IsNullOrEmpty(statusVal))
                filter = r => r.CheckDate <= toDateVal.Value && r.CheckType == checkTypeVal && r.Status == statusVal;
            else if (!string.IsNullOrEmpty(checkTypeVal))
                filter = r => r.CheckDate <= toDateVal.Value && r.CheckType == checkTypeVal;
            else if (!string.IsNullOrEmpty(statusVal))
                filter = r => r.CheckDate <= toDateVal.Value && r.Status == statusVal;
            else
                filter = r => r.CheckDate <= toDateVal.Value;
        }
        else
        {
            if (!string.IsNullOrEmpty(checkTypeVal) && !string.IsNullOrEmpty(statusVal))
                filter = r => r.CheckType == checkTypeVal && r.Status == statusVal;
            else if (!string.IsNullOrEmpty(checkTypeVal))
                filter = r => r.CheckType == checkTypeVal;
            else if (!string.IsNullOrEmpty(statusVal))
                filter = r => r.Status == statusVal;
        }

        var reports = await _dataIntegrityRepository.GetAsync(
            filter,
            r => r,
            q => q.OrderByDescending(x => x.CheckDate)
        ).ConfigureAwait(false);

        var issues = new List<DataIntegrityIssueDto>();
        foreach (var report in reports)
        {
            if (!string.IsNullOrEmpty(report.Details))
            {
                try
                {
                    var details = JsonConvert.DeserializeObject<Dictionary<string, object>>(report.Details);
                    if (details != null && details.ContainsKey("Issues"))
                    {
                        var issueList = JsonConvert.DeserializeObject<List<DataIntegrityIssue>>(details["Issues"].ToString());
                        if (issueList != null)
                        {
                            issues.AddRange(issueList.Select(i => new DataIntegrityIssueDto
                            {
                                Id = report.Id,
                                CheckDate = report.CheckDate,
                                CheckType = report.CheckType,
                                EntityType = i.EntityType,
                                EntityId = i.EntityId,
                                Identifier = i.Identifier,
                                IssueType = i.IssueType,
                                Description = i.Description,
                                ExpectedValue = i.ExpectedValue,
                                ActualValue = i.ActualValue,
                                CanAutoFix = i.CanAutoFix,
                                Status = report.Status,
                                IsFixed = report.AutoFixed
                            }));
                        }
                    }
                }
                catch
                {
                    // Skip invalid JSON
                }
            }
        }

        return issues;
    }

    public async Task<HealthCheckSummaryDto> GetLatestSummaryAsync()
    {
        var summary = await _summaryRepository.FirstOrDefaultAsync(s => true,
            s => s,
            q => q.OrderByDescending(x => x.ReportDate)
        ).ConfigureAwait(false);

        if (summary == null) return null;

        return new HealthCheckSummaryDto
        {
            Id = summary.Id,
            ReportDate = summary.ReportDate,
            OverallStatus = summary.OverallStatus,
            TotalChecks = summary.TotalChecks,
            HealthyChecks = summary.HealthyChecks,
            WarningChecks = summary.WarningChecks,
            CriticalChecks = summary.CriticalChecks,
            SummaryDetails = summary.SummaryDetails
        };
    }

    public async Task<bool> RunManualHealthCheckAsync()
    {
        try
        {
            var results = await _healthCheckService.RunAllChecksAsync().ConfigureAwait(false);
            await _healthCheckService.SaveHealthReportAsync(results).ConfigureAwait(false);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> RunManualDataIntegrityCheckAsync()
    {
        try
        {
            await _dataIntegrityService.RunAllDataIntegrityChecksAsync().ConfigureAwait(false);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<List<HealthCheckResult>> CheckDataIntegrityForWarehouseOrderNoAsync(string warehouseOrderNo)
    {
        try
        {
            var results = await _dataIntegrityService.CheckDataIntegrityForWarehouseOrderNoAsync(warehouseOrderNo).ConfigureAwait(false);
            return results;
        }
        catch (Exception ex)
        {
            return new List<HealthCheckResult>
            {
                new HealthCheckResult
                {
                    CheckName = "Error",
                    Status = HealthStatus.Critical,
                    Message = $"Error checking data integrity: {ex.Message}",
                    ExecutionTimeMs = 0
                }
            };
        }
    }

    public async Task<DataIntegrityFixResult> AutoFixDataIntegrityIssuesAsync(List<int> issueIds)
    {
        // Get issues from latest reports
        var reports = await _dataIntegrityRepository.GetAsync(
            r => issueIds.Contains(r.Id),
            r => r
        ).ConfigureAwait(false);

        var allIssues = new List<DataIntegrityIssue>();
        foreach (var report in reports)
        {
            if (!string.IsNullOrEmpty(report.Details))
            {
                try
                {
                    var details = JsonConvert.DeserializeObject<Dictionary<string, object>>(report.Details);
                    if (details != null && details.TryGetValue("Issues", out var detail))
                    {
                        var issueList = JsonConvert.DeserializeObject<List<DataIntegrityIssue>>(detail.ToString());
                        if (issueList != null)
                        {
                            allIssues.AddRange(issueList);
                        }
                    }
                }
                catch
                {
                    // Skip invalid JSON
                }
            }
        }

        var fixResult = await _dataIntegrityService.AutoFixDataIntegrityIssuesAsync(allIssues).ConfigureAwait(false);
        
        // Update reports to mark as fixed
        foreach (var report in reports)
        {
            report.AutoFixed = true;
            report.ModifiedDate = DateTime.Now;
            report.ModifiedBy = "System";
            await _dataIntegrityRepository.UpdateAsync(report).ConfigureAwait(false);
            await _dataIntegrityRepository.SaveAsync().ConfigureAwait(false);
        }

        return fixResult;
    }
}

