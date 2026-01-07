using System;
using System.Collections.Generic;
using Corno.Web.Areas.Kitchen.Dto.Carton;

namespace Corno.Web.Areas.Kitchen.Dto.Dashboard;

public class CartonDashboardDto
{
    // Summary KPIs
    public int TotalCartons { get; set; }
    public int PackedCartons { get; set; }
    public int RackedInCartons { get; set; }
    public int DispatchedCartons { get; set; }
    public double TotalCartonQuantity { get; set; }
    public double PackedQuantity { get; set; }
    public double DispatchProgressPercentage { get; set; }
    
    // Charts Data
    public List<CartonByDateDto> CartonsByDate { get; set; } = new();
    public List<CartonByStatusDto> CartonsByStatus { get; set; } = new();
    public List<CartonByWarehouseOrderDto> CartonsByWarehouseOrder { get; set; } = new();
    public List<CartonProgressOverTimeDto> ProgressOverTime { get; set; } = new();
    
    // Recent Cartons
    public List<CartonIndexDto> RecentCartons { get; set; } = new();
}

public class CartonByDateDto
{
    public DateTime? PackingDate { get; set; }
    public string PackingDateFormatted => PackingDate.HasValue ? PackingDate.Value.ToString("dd/MM/yyyy") : "N/A";
    public int Count { get; set; }
    public double TotalQuantity { get; set; }
}

public class CartonByStatusDto
{
    public string Status { get; set; }
    public int Count { get; set; }
    public double Percentage { get; set; }
}

public class CartonByWarehouseOrderDto
{
    public string WarehouseOrderNo { get; set; }
    public int CartonCount { get; set; }
    public double TotalQuantity { get; set; }
}

public class CartonProgressOverTimeDto
{
    public DateTime Date { get; set; }
    public int PackedCount { get; set; }
    public int RackedInCount { get; set; }
    public int DispatchedCount { get; set; }
}

