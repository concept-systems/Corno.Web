using System;

namespace Corno.Web.Areas.Admin.Dto.HealthCheck;

public class DataIntegrityIssueDto
{
    public int Id { get; set; }
    public DateTime CheckDate { get; set; }
    public string CheckType { get; set; }
    public string EntityType { get; set; }
    public int? EntityId { get; set; }
    public string Identifier { get; set; }
    public string IssueType { get; set; }
    public string Description { get; set; }
    public string ExpectedValue { get; set; }
    public string ActualValue { get; set; }
    public bool CanAutoFix { get; set; }
    public bool IsFixed { get; set; }
    public DateTime? FixedDate { get; set; }
    public string Status { get; set; } // Open, Fixed, CannotFix
}

