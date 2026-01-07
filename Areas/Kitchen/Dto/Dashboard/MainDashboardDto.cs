using System.Collections.Generic;
using Corno.Web.Areas.Kitchen.Dto.Plan;
using Corno.Web.Areas.Kitchen.Dto.Label;
using Corno.Web.Areas.Kitchen.Dto.Carton;

namespace Corno.Web.Areas.Kitchen.Dto.Dashboard;

public class MainDashboardDto
{
    // Overall Summary
    public int TotalPlans { get; set; }
    public int TotalLabels { get; set; }
    public int TotalCartons { get; set; }
    
    // Quick Stats
    public int ActivePlans { get; set; }
    public int OverduePlans { get; set; }
    public int TodayLabels { get; set; }
    public int TodayCartons { get; set; }
    
    // Progress Overview
    public double OverallPlanProgress { get; set; }
    public double OverallLabelProgress { get; set; }
    public double OverallCartonProgress { get; set; }
    
    // Recent Activity
    public List<PlanIndexDto> RecentPlans { get; set; } = new();
    public List<LabelIndexDto> RecentLabels { get; set; } = new();
    public List<CartonIndexDto> RecentCartons { get; set; } = new();
    
    // Alerts Summary
    public int HighPriorityAlerts { get; set; }
    public int MediumPriorityAlerts { get; set; }
    public int LowPriorityAlerts { get; set; }
}

