using System.Collections.Generic;

namespace Corno.Web.Areas.Admin.Dto;

public class DashboardDto
{
    public int TotalUsers { get; set; }
    public int TotalRoles { get; set; }
    public int ActiveUsers { get; set; }
    public int ActiveSessions { get; set; }
    public int FailedLoginAttempts24h { get; set; }
    public int PasswordResets24h { get; set; }
    
    public List<RecentActivityDto> RecentActivities { get; set; }
    public List<SystemStatisticDto> SystemStatistics { get; set; }
}

public class RecentActivityDto
{
    public string Timestamp { get; set; }
    public string UserName { get; set; }
    public string Action { get; set; }
    public string EntityType { get; set; }
    public string Details { get; set; }
    public string IpAddress { get; set; }
}

public class SystemStatisticDto
{
    public string Label { get; set; }
    public string Value { get; set; }
    public string Change { get; set; }
    public bool IsPositive { get; set; }
}

