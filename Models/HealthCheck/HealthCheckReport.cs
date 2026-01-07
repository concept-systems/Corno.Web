using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Corno.Web.Models.Base;

namespace Corno.Web.Models.HealthCheck;

public class HealthCheckReport : BaseModel
{
    [Required]
    public DateTime CheckDate { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string CheckType { get; set; }
    
    [Required]
    [MaxLength(20)]
    public string Status { get; set; } // Healthy, Warning, Critical
    
    [Column(TypeName = "NVARCHAR(MAX)")]
    public string Message { get; set; }
    
    [Column(TypeName = "NVARCHAR(MAX)")]
    public string Details { get; set; } // JSON format
    
    public int ExecutionTimeMs { get; set; }
    
    public bool AutoFixed { get; set; }
    
    [MaxLength(100)]
    public string CreatedBy { get; set; }
}

