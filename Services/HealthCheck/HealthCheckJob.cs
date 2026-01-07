using System;
using System.Threading.Tasks;
using Corno.Web.Services.HealthCheck.Interfaces;
using Corno.Web.Windsor;
using Quartz;

namespace Corno.Web.Services.HealthCheck;

[DisallowConcurrentExecution]
public class HealthCheckJob : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            var healthCheckService = Bootstrapper.Get<IHealthCheckService>();
            var results = await healthCheckService.RunAllChecksAsync().ConfigureAwait(false);
            await healthCheckService.SaveHealthReportAsync(results).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            // Log error but don't throw - Quartz will retry if needed
            Corno.Web.Logger.LogHandler.LogError(ex);
        }
    }
}

