using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Corno.Web.Logger;
using Corno.Web.Repository.Interfaces;
using Corno.Web.Services.HealthCheck.Interfaces;
using Corno.Web.Services.Interfaces;
using Corno.Web.Windsor;
using Corno.Web.Windsor.Context;
using Newtonsoft.Json;

namespace Corno.Web.Services.HealthCheck;

public class HealthCheckService : IHealthCheckService, IService
{
    private readonly IGenericRepository<Models.HealthCheck.HealthCheckReport> _reportRepository;
    private readonly IGenericRepository<Models.HealthCheck.HealthCheckSummary> _summaryRepository;
    private readonly BaseDbContext _dbContext;

    public HealthCheckService(
        IGenericRepository<Models.HealthCheck.HealthCheckReport> reportRepository,
        IGenericRepository<Models.HealthCheck.HealthCheckSummary> summaryRepository,
        BaseDbContext dbContext)
    {
        _reportRepository = reportRepository;
        _summaryRepository = summaryRepository;
        _dbContext = dbContext;
    }

    public async Task<List<HealthCheckResult>> RunAllChecksAsync()
    {
        var results = new List<HealthCheckResult>();
        var checks = new List<Func<Task<HealthCheckResult>>>
        {
            CheckDatabaseConnectivityAsync,
            CheckConnectionPoolAsync,
            CheckDiskSpaceAsync,
            CheckMemoryUsageAsync,
            CheckErrorRateAsync,
            CheckApplicationPerformanceAsync
        };

        foreach (var check in checks)
        {
            try
            {
                var result = await check().ConfigureAwait(false);
                results.Add(result);
            }
            catch (Exception ex)
            {
                results.Add(new HealthCheckResult
                {
                    CheckName = check.Method.Name,
                    Status = HealthStatus.Critical,
                    Message = $"Health check failed: {ex.Message}",
                    ExecutionTimeMs = 0
                });
            }
        }

        return results;
    }

    public async Task<HealthCheckResult> CheckDatabaseConnectivityAsync()
    {
        var sw = Stopwatch.StartNew();
        try
        {
            _dbContext.CheckDatabaseConnection();
            var canConnect = _dbContext.Database.Connection.State == ConnectionState.Open;
            
            sw.Stop();
            
            return new HealthCheckResult
            {
                CheckName = "Database Connectivity",
                Status = canConnect ? HealthStatus.Healthy : HealthStatus.Critical,
                Message = canConnect ? "Database connection is healthy" : "Cannot connect to database",
                Details = new Dictionary<string, object>
                {
                    ["ConnectionState"] = _dbContext.Database.Connection.State.ToString(),
                    ["Database"] = _dbContext.Database.Connection.Database
                },
                ExecutionTimeMs = (int)sw.ElapsedMilliseconds
            };
        }
        catch (Exception ex)
        {
            sw.Stop();
            return new HealthCheckResult
            {
                CheckName = "Database Connectivity",
                Status = HealthStatus.Critical,
                Message = $"Database connection failed: {ex.Message}",
                ExecutionTimeMs = (int)sw.ElapsedMilliseconds
            };
        }
    }

    public async Task<HealthCheckResult> CheckConnectionPoolAsync()
    {
        var sw = Stopwatch.StartNew();
        try
        {
            var connectionString = _dbContext.Database.Connection.ConnectionString;
            var builder = new System.Data.SqlClient.SqlConnectionStringBuilder(connectionString);
            
            var maxPoolSize = builder.MaxPoolSize > 0 ? builder.MaxPoolSize : 100;
            
            // Get approximate connection count (simplified)
            var activeConnections = 0;
            try
            {
                var query = "SELECT COUNT(*) FROM sys.dm_exec_sessions WHERE database_id = DB_ID() AND is_user_process = 1";
                var result = await _dbContext.Database.SqlQuery<int>(query).FirstOrDefaultAsync().ConfigureAwait(false);
                activeConnections = result;
            }
            catch
            {
                // If query fails, use default
            }
            
            sw.Stop();
            
            var poolUsagePercent = (activeConnections / (double)maxPoolSize) * 100;
            
            var status = poolUsagePercent > 90 ? HealthStatus.Critical 
                       : poolUsagePercent > 75 ? HealthStatus.Warning 
                       : HealthStatus.Healthy;
            
            return new HealthCheckResult
            {
                CheckName = "Connection Pool",
                Status = status,
                Message = $"Connection pool usage: {poolUsagePercent:F1}%",
                Details = new Dictionary<string, object>
                {
                    ["ActiveConnections"] = activeConnections,
                    ["MaxPoolSize"] = maxPoolSize,
                    ["UsagePercent"] = poolUsagePercent
                },
                ExecutionTimeMs = (int)sw.ElapsedMilliseconds
            };
        }
        catch (Exception ex)
        {
            sw.Stop();
            return new HealthCheckResult
            {
                CheckName = "Connection Pool",
                Status = HealthStatus.Warning,
                Message = $"Could not check connection pool: {ex.Message}",
                ExecutionTimeMs = (int)sw.ElapsedMilliseconds
            };
        }
    }

    public async Task<HealthCheckResult> CheckDiskSpaceAsync()
    {
        var sw = Stopwatch.StartNew();
        try
        {
            var logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
            var drive = new DriveInfo(Path.GetPathRoot(logPath));
            
            var freeSpaceGb = drive.AvailableFreeSpace / (1024.0 * 1024.0 * 1024.0);
            var totalSpaceGb = drive.TotalSize / (1024.0 * 1024.0 * 1024.0);
            var usedPercent = ((totalSpaceGb - freeSpaceGb) / totalSpaceGb) * 100;
            
            sw.Stop();
            
            var status = usedPercent > 90 ? HealthStatus.Critical 
                       : usedPercent > 80 ? HealthStatus.Warning 
                       : HealthStatus.Healthy;
            
            return new HealthCheckResult
            {
                CheckName = "Disk Space",
                Status = status,
                Message = $"Disk usage: {usedPercent:F1}% ({freeSpaceGb:F2} GB free)",
                Details = new Dictionary<string, object>
                {
                    ["FreeSpaceGB"] = freeSpaceGb,
                    ["TotalSpaceGB"] = totalSpaceGb,
                    ["UsedPercent"] = usedPercent,
                    ["DriveName"] = drive.Name
                },
                ExecutionTimeMs = (int)sw.ElapsedMilliseconds
            };
        }
        catch (Exception ex)
        {
            sw.Stop();
            return new HealthCheckResult
            {
                CheckName = "Disk Space",
                Status = HealthStatus.Warning,
                Message = $"Could not check disk space: {ex.Message}",
                ExecutionTimeMs = (int)sw.ElapsedMilliseconds
            };
        }
    }

    public async Task<HealthCheckResult> CheckMemoryUsageAsync()
    {
        var sw = Stopwatch.StartNew();
        try
        {
            var process = Process.GetCurrentProcess();
            var workingSetMb = process.WorkingSet64 / (1024.0 * 1024.0);
            var privateMemoryMb = process.PrivateMemorySize64 / (1024.0 * 1024.0);
            
            sw.Stop();
            
            var status = workingSetMb > 4096 ? HealthStatus.Critical 
                       : workingSetMb > 2048 ? HealthStatus.Warning 
                       : HealthStatus.Healthy;
            
            return new HealthCheckResult
            {
                CheckName = "Memory Usage",
                Status = status,
                Message = $"Memory usage: {workingSetMb:F0} MB (Working Set)",
                Details = new Dictionary<string, object>
                {
                    ["WorkingSetMB"] = workingSetMb,
                    ["PrivateMemoryMB"] = privateMemoryMb
                },
                ExecutionTimeMs = (int)sw.ElapsedMilliseconds
            };
        }
        catch (Exception ex)
        {
            sw.Stop();
            return new HealthCheckResult
            {
                CheckName = "Memory Usage",
                Status = HealthStatus.Warning,
                Message = $"Could not check memory: {ex.Message}",
                ExecutionTimeMs = (int)sw.ElapsedMilliseconds
            };
        }
    }

    public async Task<HealthCheckResult> CheckErrorRateAsync()
    {
        var sw = Stopwatch.StartNew();
        try
        {
            var logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
            var logFile = Path.Combine(logPath, $"{DateTime.Now:yyyy-MM-dd}.log");
            
            var errorCount = 0;
            if (System.IO.File.Exists(logFile))
            {
                var lines = await Task.Run(() => System.IO.File.ReadAllLines(logFile)).ConfigureAwait(false);
                var oneHourAgo = DateTime.Now.AddHours(-1);
                errorCount = lines.Count(line => 
                    line.Contains("Error") ||
                    line.Contains("Fatal"));
            }
            
            sw.Stop();
            
            var status = errorCount > 100 ? HealthStatus.Critical 
                       : errorCount > 50 ? HealthStatus.Warning 
                       : HealthStatus.Healthy;
            
            return new HealthCheckResult
            {
                CheckName = "Error Rate",
                Status = status,
                Message = $"Error count in last hour: {errorCount}",
                Details = new Dictionary<string, object>
                {
                    ["ErrorCount"] = errorCount,
                    ["TimeWindow"] = "1 hour"
                },
                ExecutionTimeMs = (int)sw.ElapsedMilliseconds
            };
        }
        catch (Exception ex)
        {
            sw.Stop();
            return new HealthCheckResult
            {
                CheckName = "Error Rate",
                Status = HealthStatus.Warning,
                Message = $"Could not check error rate: {ex.Message}",
                ExecutionTimeMs = (int)sw.ElapsedMilliseconds
            };
        }
    }

    public async Task<HealthCheckResult> CheckApplicationPerformanceAsync()
    {
        var sw = Stopwatch.StartNew();
        try
        {
            var testSw = Stopwatch.StartNew();
            await _dbContext.Database.SqlQuery<int>("SELECT 1").FirstOrDefaultAsync().ConfigureAwait(false);
            testSw.Stop();
            
            sw.Stop();
            
            var queryTimeMs = testSw.ElapsedMilliseconds;
            var status = queryTimeMs > 5000 ? HealthStatus.Critical 
                       : queryTimeMs > 2000 ? HealthStatus.Warning 
                       : HealthStatus.Healthy;
            
            return new HealthCheckResult
            {
                CheckName = "Application Performance",
                Status = status,
                Message = $"Database query response time: {queryTimeMs} ms",
                Details = new Dictionary<string, object>
                {
                    ["QueryResponseTimeMs"] = queryTimeMs
                },
                ExecutionTimeMs = (int)sw.ElapsedMilliseconds
            };
        }
        catch (Exception ex)
        {
            sw.Stop();
            return new HealthCheckResult
            {
                CheckName = "Application Performance",
                Status = HealthStatus.Critical,
                Message = $"Performance check failed: {ex.Message}",
                ExecutionTimeMs = (int)sw.ElapsedMilliseconds
            };
        }
    }

    public async Task SaveHealthReportAsync(List<HealthCheckResult> results)
    {
        foreach (var result in results)
        {
            var report = new Models.HealthCheck.HealthCheckReport
            {
                CheckDate = DateTime.Now,
                CheckType = result.CheckName,
                Status = result.Status.ToString(),
                Message = result.Message,
                Details = JsonConvert.SerializeObject(result.Details),
                ExecutionTimeMs = result.ExecutionTimeMs,
                AutoFixed = result.AutoFixed,
                CreatedBy = "System",
                CreatedDate = DateTime.Now
            };
            
            await _reportRepository.AddAsync(report).ConfigureAwait(false);
        }
        await _reportRepository.SaveAsync().ConfigureAwait(false);

        // Create summary
        var summary = new Models.HealthCheck.HealthCheckSummary
        {
            ReportDate = DateTime.Now,
            OverallStatus = results.Any(r => r.Status == HealthStatus.Critical) ? "Critical"
                          : results.Any(r => r.Status == HealthStatus.Warning) ? "Warning"
                          : "Healthy",
            TotalChecks = results.Count,
            HealthyChecks = results.Count(r => r.Status == HealthStatus.Healthy),
            WarningChecks = results.Count(r => r.Status == HealthStatus.Warning),
            CriticalChecks = results.Count(r => r.Status == HealthStatus.Critical),
            SummaryDetails = JsonConvert.SerializeObject(results),
            CreatedBy = "System",
            CreatedDate = DateTime.Now
        };
        
        await _summaryRepository.AddAsync(summary).ConfigureAwait(false);
        await _summaryRepository.SaveAsync().ConfigureAwait(false);
    }

    public async Task<Models.HealthCheck.HealthCheckSummary> GetLatestSummaryAsync()
    {
        var summary = await _summaryRepository
            .FirstOrDefaultAsync<Models.HealthCheck.HealthCheckSummary>(
                s => true, 
                s => s, 
                q => q.OrderByDescending(x => x.ReportDate))
            .ConfigureAwait(false);
        
        return summary;
    }

    public async Task<List<Models.HealthCheck.HealthCheckReport>> GetRecentReportsAsync(int days = 7)
    {
        var cutoffDate = DateTime.Now.AddDays(-days);
        var reports = await _reportRepository
            .GetAsync<Models.HealthCheck.HealthCheckReport>(
                r => r.CheckDate >= cutoffDate, 
                r => r, 
                q => q.OrderByDescending(x => x.CheckDate))
            .ConfigureAwait(false);
        
        return reports;
    }
}

