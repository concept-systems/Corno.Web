using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Corno.Web.Services.HealthCheck.DataIntegrity;
using Corno.Web.Windsor;
using Quartz;

namespace Corno.Web.Services.HealthCheck.DataIntegrity;

[DisallowConcurrentExecution]
public class DataIntegrityHealthCheckJob : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            var dataIntegrityService = Bootstrapper.Get<IDataIntegrityHealthCheckService>();
            var results = await dataIntegrityService.RunAllDataIntegrityChecksAsync().ConfigureAwait(false);
            
            // Auto-fix issues where possible
            foreach (var result in results)
            {
                if (result.Details != null && result.Details.ContainsKey("Issues"))
                {
                    if (result.Details["Issues"] is List<DataIntegrityIssue> issues && issues.Count > 0)
                    {
                        var fixableIssues = issues.Where(i => i.CanAutoFix).ToList();
                        if (fixableIssues.Any())
                        {
                            await dataIntegrityService.AutoFixDataIntegrityIssuesAsync(fixableIssues).ConfigureAwait(false);
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            // Log error but don't throw - Quartz will retry if needed
            Corno.Web.Logger.LogHandler.LogError(ex);
        }
    }
}

