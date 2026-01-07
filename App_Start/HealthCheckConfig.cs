using System;
using Quartz;
using Quartz.Impl;

namespace Corno.Web;

public class HealthCheckConfig
{
    private static IScheduler _scheduler;

    public static void Start()
    {
        try
        {
            var factory = new StdSchedulerFactory();
            _scheduler = factory.GetScheduler().Result;
            _scheduler.Start();

            // Schedule health check job to run every hour
            var healthCheckJob = JobBuilder.Create<Services.HealthCheck.HealthCheckJob>()
                .WithIdentity("HealthCheckJob", "HealthCheckGroup")
                .Build();

            var healthCheckTrigger = TriggerBuilder.Create()
                .WithIdentity("HealthCheckTrigger", "HealthCheckGroup")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInHours(1)
                    .RepeatForever())
                .Build();

            _scheduler.ScheduleJob(healthCheckJob, healthCheckTrigger);

            // Schedule data integrity check job to run every 6 hours
            var dataIntegrityJob = JobBuilder.Create<Services.HealthCheck.DataIntegrity.DataIntegrityHealthCheckJob>()
                .WithIdentity("DataIntegrityHealthCheckJob", "HealthCheckGroup")
                .Build();

            var dataIntegrityTrigger = TriggerBuilder.Create()
                .WithIdentity("DataIntegrityTrigger", "HealthCheckGroup")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInHours(6)
                    .RepeatForever())
                .Build();

            _scheduler.ScheduleJob(dataIntegrityJob, dataIntegrityTrigger);
        }
        catch (Exception ex)
        {
            Logger.LogHandler.LogError(ex);
        }
    }

    public static void Stop()
    {
        try
        {
            _scheduler?.Shutdown(true);
        }
        catch (Exception ex)
        {
            Logger.LogHandler.LogError(ex);
        }
    }
}

