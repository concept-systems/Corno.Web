using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Corno.Web.Areas.Admin.Dto.HealthCheck;
using Corno.Web.Services.HealthCheck.DataIntegrity;
using Corno.Web.Services.HealthCheck.Interfaces;
using Corno.Web.Services.Interfaces;

namespace Corno.Web.Areas.Admin.Services.Interfaces;

public interface IHealthCheckUIService : IService
{
    Task<HealthCheckDashboardDto> GetDashboardDataAsync();
    Task<List<HealthCheckReportDto>> GetHealthCheckReportsAsync(DateTime? fromDate, DateTime? toDate, string status = null);
    Task<List<DataIntegrityIssueDto>> GetDataIntegrityIssuesAsync(DateTime? fromDate, DateTime? toDate, string checkType = null, string status = null);
    Task<HealthCheckSummaryDto> GetLatestSummaryAsync();
    Task<bool> RunManualHealthCheckAsync();
    Task<bool> RunManualDataIntegrityCheckAsync();
    Task<List<HealthCheckResult>> CheckDataIntegrityForWarehouseOrderNoAsync(string warehouseOrderNo);
    Task<DataIntegrityFixResult> AutoFixDataIntegrityIssuesAsync(List<int> issueIds);
}

