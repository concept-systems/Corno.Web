using System;
using System.Collections.Generic;
using Corno.Web.Areas.Kitchen.Dto.Label;
using Corno.Web.Globals.Enums;

namespace Corno.Web.Areas.Kitchen.Dto.Dashboard;

public class LabelDashboardDto
{
    // Overall Summary KPIs
    public int TotalLabels { get; set; }
    public int ActiveLabels { get; set; }
    public int PrintedLabels { get; set; }
    public int PackedLabels { get; set; }
    
    // Label Categories
    public LabelCategoryStatsDto PartLabels { get; set; } = new() { Category = "Part Labels" };
    public LabelCategoryStatsDto StiffenerLabels { get; set; } = new() { Category = "Stiffener Labels" };
    public LabelCategoryStatsDto StoreShirwalLabels { get; set; } = new() { Category = "Store Shirwal Labels" };
    public LabelCategoryStatsDto StoreKhalapurLabels { get; set; } = new() { Category = "Store Khalapur Labels" };
    public LabelCategoryStatsDto SubAssemblyLabels { get; set; } = new() { Category = "SubAssembly Labels" };
    public LabelCategoryStatsDto TrolleyLabels { get; set; } = new() { Category = "Trolley Labels" };
    
    // Charts Data
    public List<LabelByDateDto> LabelsByDate { get; set; } = new();
    public List<LabelByStatusDto> LabelsByStatus { get; set; } = new();
    public List<LabelByTypeDto> LabelsByType { get; set; } = new();
    public List<LabelByWarehouseOrderDto> LabelsByWarehouseOrder { get; set; } = new();
    public List<LabelProgressOverTimeDto> ProgressOverTime { get; set; } = new();
    
    // Recent Labels
    public List<LabelIndexDto> RecentLabels { get; set; } = new();
}

public class LabelCategoryStatsDto
{
    public string Category { get; set; }
    public int TotalCount { get; set; }
    public int ActiveCount { get; set; }
    public int PrintedCount { get; set; }
    public int PackedCount { get; set; }
    public double TotalQuantity { get; set; }
    public double PackedQuantity { get; set; }
    public double ProgressPercentage { get; set; }
}

public class LabelByDateDto
{
    public DateTime? LabelDate { get; set; }
    public string LabelDateFormatted => LabelDate.HasValue ? LabelDate.Value.ToString("dd/MM/yyyy") : "N/A";
    public int Count { get; set; }
    public double TotalQuantity { get; set; }
}

public class LabelByStatusDto
{
    public string Status { get; set; }
    public int Count { get; set; }
    public double Percentage { get; set; }
}

public class LabelByTypeDto
{
    public string LabelType { get; set; }
    public int Count { get; set; }
    public double Percentage { get; set; }
}

public class LabelByWarehouseOrderDto
{
    public string WarehouseOrderNo { get; set; }
    public int LabelCount { get; set; }
    public double TotalQuantity { get; set; }
}

public class LabelProgressOverTimeDto
{
    public DateTime Date { get; set; }
    public int PrintedCount { get; set; }
    public int BentCount { get; set; }
    public int SortedCount { get; set; }
    public int PackedCount { get; set; }
}

