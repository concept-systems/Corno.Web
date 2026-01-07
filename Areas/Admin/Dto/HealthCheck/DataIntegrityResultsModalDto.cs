using System.Collections.Generic;

namespace Corno.Web.Areas.Admin.Dto.HealthCheck;

public class DataIntegrityResultsModalDto
{
    public string WarehouseOrderNo { get; set; }
    public int TotalIssues { get; set; }
    public int CriticalIssues { get; set; }
    public int WarningIssues { get; set; }
    public int HealthyChecks { get; set; }
    public int FixableIssues { get; set; }
    public List<CheckResultDto> Results { get; set; }
    public List<IssuesByPositionDto> IssuesByPosition { get; set; }
    public List<IssuesByCartonDto> IssuesByCarton { get; set; }
    public List<IssuesByLabelDto> IssuesByLabel { get; set; }
    public List<PlanLevelIssueDto> PlanLevelIssues { get; set; }
}

public class CheckResultDto
{
    public string CheckName { get; set; }
    public string Status { get; set; }
    public string Message { get; set; }
    public int ExecutionTimeMs { get; set; }
}

public class IssuesByPositionDto
{
    public string Position { get; set; }
    public List<IssueDto> Issues { get; set; }
}

public class IssuesByCartonDto
{
    public string CartonBarcode { get; set; }
    public List<IssueDto> Issues { get; set; }
}

public class IssuesByLabelDto
{
    public string Barcode { get; set; }
    public List<IssueDto> Issues { get; set; }
}

public class PlanLevelIssueDto
{
    public string EntityType { get; set; }
    public string Identifier { get; set; }
    public string IssueType { get; set; }
    public string Description { get; set; }
    public string ExpectedValue { get; set; }
    public string ActualValue { get; set; }
    public bool CanAutoFix { get; set; }
}

public class IssueDto
{
    public string EntityType { get; set; }
    public string Identifier { get; set; }
    public string IssueType { get; set; }
    public string Description { get; set; }
    public string ExpectedValue { get; set; }
    public string ActualValue { get; set; }
    public bool CanAutoFix { get; set; }
}

