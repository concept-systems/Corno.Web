using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Corno.Web.Models.Base;

namespace Corno.Web.Models.HealthCheck;

public class HealthCheckSummary : BaseModel
{
    [Required]
    public DateTime ReportDate { get; set; }
    
    [Required]
    [MaxLength(20)]
    public string OverallStatus { get; set; } // Healthy, Warning, Critical
    
    public int TotalChecks { get; set; }
    
    public int HealthyChecks { get; set; }
    
    public int WarningChecks { get; set; }
    
    public int CriticalChecks { get; set; }
    
    [Column(TypeName = "NVARCHAR(MAX)")]
    public string SummaryDetails { get; set; } // JSON
    
    [MaxLength(100)]
    public string CreatedBy { get; set; }
}

