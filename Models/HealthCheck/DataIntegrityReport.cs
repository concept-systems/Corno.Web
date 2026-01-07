using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Corno.Web.Models.Base;

namespace Corno.Web.Models.HealthCheck;

public class DataIntegrityReport : BaseModel
{
    [Required]
    public DateTime CheckDate { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string CheckType { get; set; } // PlanQuantities, CartonBarcodes, LabelSequence, etc.
    
    [Required]
    [MaxLength(20)]
    public string Status { get; set; } // Healthy, Warning, Critical
    
    [Column(TypeName = "NVARCHAR(MAX)")]
    public string Message { get; set; }
    
    [Column(TypeName = "NVARCHAR(MAX)")]
    public string Details { get; set; } // JSON format with issues
    
    public int ExecutionTimeMs { get; set; }
    
    public int RecordsChecked { get; set; }
    
    public int IssuesFound { get; set; }
    
    public int RecordsFixed { get; set; }
    
    public bool AutoFixed { get; set; }
    
    [MaxLength(100)]
    public string CreatedBy { get; set; }
}

