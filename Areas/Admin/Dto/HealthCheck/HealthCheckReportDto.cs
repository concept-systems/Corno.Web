using System;

namespace Corno.Web.Areas.Admin.Dto.HealthCheck;

public class HealthCheckReportDto
{
    public int Id { get; set; }
    public DateTime CheckDate { get; set; }
    public string CheckType { get; set; }
    public string Status { get; set; } // Healthy, Warning, Critical
    public string Message { get; set; }
    public string Details { get; set; } // JSON string
    public int ExecutionTimeMs { get; set; }
    public bool AutoFixed { get; set; }
    public string CreatedBy { get; set; }
}

