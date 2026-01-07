using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Corno.Web.Areas.Admin.Dto;
using Corno.Web.Areas.Admin.Models;
using Corno.Web.Areas.Admin.Services.Interfaces;
using Corno.Web.Repository.Interfaces;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Corno.Web.Areas.Admin.Services;

public class DashboardService : IDashboardService
{
    private readonly IGenericRepository<AspNetUser> _userRepository;
    private readonly IGenericRepository<AspNetRole> _roleRepository;
    private readonly IGenericRepository<AspNetLoginHistory> _loginHistoryRepository;
    private readonly IGenericRepository<AuditLog> _auditLogRepository;

    public DashboardService(
        IGenericRepository<AspNetUser> userRepository,
        IGenericRepository<AspNetRole> roleRepository,
        IGenericRepository<AspNetLoginHistory> loginHistoryRepository,
        IGenericRepository<AuditLog> auditLogRepository)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _loginHistoryRepository = loginHistoryRepository;
        _auditLogRepository = auditLogRepository;
    }

    public async Task<DashboardDto> GetDashboardDataAsync()
    {
        var dashboard = new DashboardDto
        {
            RecentActivities = new List<RecentActivityDto>(),
            SystemStatistics = new List<SystemStatisticDto>()
        };

        // Get total users
        var allUsers = await _userRepository.ListAsync().ConfigureAwait(false);
        dashboard.TotalUsers = allUsers.Count();

        // Get total roles
        var allRoles = await _roleRepository.ListAsync().ConfigureAwait(false);
        dashboard.TotalRoles = allRoles.Count();

        // Get active users (users with active sessions)
        // Active session criteria: 
        // 1. Successful login (LoginResult == Success)
        // 2. Not logged out (LogoutTime == null)
        // 3. Either IsActive = true OR LoginTime within last 24 hours (to catch records where IsActive wasn't set)
        var yesterday = DateTime.Now.AddDays(-1);
        var activeSessions = await _loginHistoryRepository.GetAsync<AspNetLoginHistory>(
            h => h.LoginResult == LoginResult.Success 
                && h.LogoutTime == null 
                && (h.IsActive || (h.LoginTime.HasValue && h.LoginTime.Value >= yesterday)),
            h => h
        ).ConfigureAwait(false);
        dashboard.ActiveUsers = activeSessions.Select(h => h.AspNetUserId).Distinct().Count();
        dashboard.ActiveSessions = activeSessions.Count();

        // Get failed login attempts in last 24 hours
        var failedLogins = await _loginHistoryRepository.GetAsync<AspNetLoginHistory>(
            h => h.LoginTime >= yesterday && h.LoginResult == LoginResult.Failure,
            h => h
        ).ConfigureAwait(false);
        dashboard.FailedLoginAttempts24h = failedLogins.Count();

        // Get password resets in last 24 hours (if PasswordResetTokens table exists)
        // This would require PasswordResetToken repository

        // Get recent activities
        var recentLogs = await _auditLogRepository.GetAsync<AuditLog>(
            null,
            a => a,
            q => (IOrderedQueryable<AuditLog>)q.OrderByDescending(x => x.Timestamp).Take(10)
        ).ConfigureAwait(false);

        foreach (var log in recentLogs)
        {
            dashboard.RecentActivities.Add(new RecentActivityDto
            {
                Timestamp = log.Timestamp.ToString("MM/dd/yyyy HH:mm"),
                UserName = log.UserName,
                Action = log.Action,
                EntityType = log.EntityType,
                Details = log.Details,
                IpAddress = log.IpAddress
            });
        }

        // System statistics
        dashboard.SystemStatistics.Add(new SystemStatisticDto
        {
            Label = "Active Sessions",
            Value = dashboard.ActiveSessions.ToString(),
            Change = "+0",
            IsPositive = true
        });

        dashboard.SystemStatistics.Add(new SystemStatisticDto
        {
            Label = "Failed Login Attempts (24h)",
            Value = dashboard.FailedLoginAttempts24h.ToString(),
            Change = "+0",
            IsPositive = false
        });

        return dashboard;
    }
}

