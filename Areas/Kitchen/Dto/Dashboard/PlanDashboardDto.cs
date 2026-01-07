using System;
using System.Collections.Generic;
using Corno.Web.Areas.Kitchen.Dto.Plan;

namespace Corno.Web.Areas.Kitchen.Dto.Dashboard;

public class PlanDashboardDto
{
    // Summary KPIs
    public int TotalPlans { get; set; }
    public int ActivePlans { get; set; }
    public int CompletedPlans { get; set; }
    public double TotalOrderQuantity { get; set; }
    public double TotalPackedQuantity { get; set; }
    public double OverallProgressPercentage { get; set; }
    public int PlansDueToday { get; set; }
    public int OverduePlans { get; set; }
    
    // Stage Quantities
    public double TotalPrintQuantity { get; set; }
    public double TotalBendQuantity { get; set; }
    public double TotalSortQuantity { get; set; }
    public double TotalSubAssemblyQuantity { get; set; }
    
    // Progress Metrics
    public double PrintProgressPercentage { get; set; }
    public double BendProgressPercentage { get; set; }
    public double SortProgressPercentage { get; set; }
    public double SubAssemblyProgressPercentage { get; set; }
    public double PackProgressPercentage { get; set; }
    
    // Charts Data
    public List<PlanByDueDateDto> PlansByDueDate { get; set; } = new();
    public List<PlanByLotNoDto> PlansByLotNo { get; set; } = new();
    public List<PlanProgressOverTimeDto> ProgressOverTime { get; set; } = new();
    public List<PlanStatusDistributionDto> StatusDistribution { get; set; } = new();
    public List<PlanStageCompletionDto> StageCompletion { get; set; } = new();
    
    // Recent Plans
    public List<PlanIndexDto> RecentPlans { get; set; } = new();
    
    // Alerts
    public List<PlanAlertDto> Alerts { get; set; } = new();
}

public class PlanByDueDateDto
{
    public DateTime? DueDate { get; set; }
    public string DueDateFormatted => DueDate.HasValue ? DueDate.Value.ToString("dd/MM/yyyy") : "N/A";
    public int PlanCount { get; set; }
    public double TotalOrderQuantity { get; set; }
}

public class PlanByLotNoDto
{
    public string LotNo { get; set; }
    public int PlanCount { get; set; }
    public double TotalOrderQuantity { get; set; }
    public double TotalPackedQuantity { get; set; }
}

public class PlanProgressOverTimeDto
{
    public DateTime Date { get; set; }
    public double OrderQuantity { get; set; }
    public double PrintQuantity { get; set; }
    public double BendQuantity { get; set; }
    public double SortQuantity { get; set; }
    public double PackQuantity { get; set; }
}

public class PlanStatusDistributionDto
{
    public string Status { get; set; }
    public int Count { get; set; }
    public double Percentage { get; set; }
}

public class PlanStageCompletionDto
{
    public string Stage { get; set; }
    public double CompletedQuantity { get; set; }
    public double TotalQuantity { get; set; }
    public double CompletionPercentage { get; set; }
}

public class PlanAlertDto
{
    public string WarehouseOrderNo { get; set; }
    public string LotNo { get; set; }
    public DateTime? DueDate { get; set; }
    public string AlertType { get; set; }
    public string Message { get; set; }
    public string Severity { get; set; } // "High", "Medium", "Low"
}

