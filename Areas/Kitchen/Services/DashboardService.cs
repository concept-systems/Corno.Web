using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Corno.Web.Areas.Kitchen.Dto.Dashboard;
using Corno.Web.Areas.Kitchen.Dto.Plan;
using Corno.Web.Areas.Kitchen.Dto.Label;
using Corno.Web.Areas.Kitchen.Dto.Carton;
using Corno.Web.Areas.Kitchen.Services.Interfaces;
using Corno.Web.Globals;
using Corno.Web.Globals.Enums;
using Corno.Web.Windsor;

namespace Corno.Web.Areas.Kitchen.Services;

public class DashboardService : IDashboardService
{
    #region -- Constructors --
    public DashboardService(IPlanService planService, ILabelService labelService, ICartonService cartonService)
    {
        _planService = planService;
        _labelService = labelService;
        _cartonService = cartonService;
    }
    #endregion

    #region -- Data Members --
    private readonly IPlanService _planService;
    private readonly ILabelService _labelService;
    private readonly ICartonService _cartonService;
    #endregion

    #region -- Main Dashboard --
    public async Task<MainDashboardDto> GetMainDashboardAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        var today = DateTime.Today;
        startDate ??= today.AddDays(-30);
        endDate ??= today.AddDays(30);

        var dashboard = new MainDashboardDto();

        // Get plans data (optimized with projections)
        var plansQuery = await _planService.GetAsync(
            p => p.DueDate >= startDate && p.DueDate <= endDate,
            p => new PlanIndexDto
            {
                Id = p.Id,
                WarehouseOrderNo = p.WarehouseOrderNo,
                LotNo = p.LotNo,
                DueDate = p.DueDate,
                OrderQuantity = p.PlanItemDetails.Sum(d => d.OrderQuantity ?? 0),
                PackedQuantity = p.PlanItemDetails.Sum(d => d.PackQuantity ?? 0)
            }).ConfigureAwait(false);

        var plansList = plansQuery.ToList();
        dashboard.TotalPlans = plansList.Count;
        
        // Get full plan data for status check
        var planStatusQuery = await _planService.GetAsync(
            p => p.DueDate >= startDate && p.DueDate <= endDate,
            p => new { p.Status, p.DueDate }).ConfigureAwait(false);
        var planStatusList = planStatusQuery.ToList();
        dashboard.ActivePlans = planStatusList.Count(p => p.Status == StatusConstants.InProcess);
        dashboard.OverduePlans = planStatusList.Count(p => p.DueDate < today && p.Status != StatusConstants.Packed);

        // Calculate overall progress
        var totalOrderQty = plansList.Sum(p => p.OrderQuantity);
        var totalPackedQty = plansList.Sum(p => p.PackedQuantity ?? 0);
        dashboard.OverallPlanProgress = totalOrderQty > 0 ? (totalPackedQty / totalOrderQty) * 100 : 0;

        // Get labels data (optimized with projections)
        var labelsQuery = await _labelService.GetAsync(
            l => l.LabelDate >= startDate && l.LabelDate <= endDate,
            l => new LabelIndexDto
            {
                Id = l.Id,
                LabelDate = l.LabelDate,
                Status = l.Status,
                Quantity = l.Quantity
            }).ConfigureAwait(false);

        var labelsList = labelsQuery.ToList();
        dashboard.TotalLabels = labelsList.Count;
        dashboard.TodayLabels = labelsList.Count(l => l.LabelDate?.Date == today);

        var totalLabelQty = labelsList.Sum(l => l.Quantity ?? 0);
        var packedLabelQty = labelsList.Count(l => l.Status == StatusConstants.Packed);
        dashboard.OverallLabelProgress = labelsList.Count > 0 ? (packedLabelQty / (double)labelsList.Count) * 100 : 0;

        // Get cartons data (optimized with projections)
        var cartonsQuery = await _cartonService.GetAsync(
            c => c.PackingDate >= startDate && c.PackingDate <= endDate,
            c => new CartonIndexDto
            {
                Id = c.Id,
                PackingDate = c.PackingDate,
                Status = c.Status
            }).ConfigureAwait(false);

        var cartonsList = cartonsQuery.ToList();
        dashboard.TotalCartons = cartonsList.Count;
        dashboard.TodayCartons = cartonsList.Count(c => c.PackingDate?.Date == today);

        var dispatchedCartons = cartonsList.Count(c => c.Status == StatusConstants.Dispatched);
        dashboard.OverallCartonProgress = cartonsList.Count > 0 ? (dispatchedCartons / (double)cartonsList.Count) * 100 : 0;

        // Get recent items (limit to 10 for performance)
        var recentPlans = await _planService.GetAsync(
            p => true,
            p => new PlanIndexDto
            {
                Id = p.Id,
                WarehouseOrderNo = p.WarehouseOrderNo,
                LotNo = p.LotNo,
                DueDate = p.DueDate,
                OrderQuantity = p.PlanItemDetails.Sum(d => d.OrderQuantity ?? 0),
                PackedQuantity = p.PlanItemDetails.Sum(d => d.PackQuantity ?? 0)
            },
            orderBy: q => q.OrderByDescending(x => x.CreatedDate)).ConfigureAwait(false);

        dashboard.RecentPlans = recentPlans.Take(10).ToList();

        var recentLabels = await _labelService.GetAsync(
            l => true,
            l => new LabelIndexDto
            {
                Id = l.Id,
                LabelDate = l.LabelDate,
                WarehouseOrderNo = l.WarehouseOrderNo,
                Barcode = l.Barcode,
                Status = l.Status,
                Quantity = l.Quantity
            },
            orderBy: q => q.OrderByDescending(x => x.LabelDate)).ConfigureAwait(false);

        dashboard.RecentLabels = recentLabels.Take(10).ToList();

        var recentCartons = await _cartonService.GetAsync(
            c => true,
            c => new CartonIndexDto
            {
                Id = c.Id,
                PackingDate = c.PackingDate,
                WarehouseOrderNo = c.WarehouseOrderNo,
                CartonBarcode = c.CartonBarcode,
                Status = c.Status
            },
            orderBy: q => q.OrderByDescending(x => x.PackingDate)).ConfigureAwait(false);

        dashboard.RecentCartons = recentCartons.Take(10).ToList();

        return dashboard;
    }
    #endregion

    #region -- Plan Dashboard --
    public async Task<PlanDashboardDto> GetPlanDashboardAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        var today = DateTime.Today;
        startDate ??= today.AddDays(-30);
        endDate ??= today.AddDays(30);

        var dashboard = new PlanDashboardDto();

        // Get all plans with optimized projection
        var plansQuery = await _planService.GetAsync(
            p => p.DueDate >= startDate && p.DueDate <= endDate,
            p => new PlanIndexDto
            {
                Id = p.Id,
                WarehouseOrderNo = p.WarehouseOrderNo,
                LotNo = p.LotNo,
                DueDate = p.DueDate,
                OrderQuantity = p.PlanItemDetails.Sum(d => d.OrderQuantity ?? 0),
                PrintQuantity = p.PlanItemDetails.Sum(d => d.PrintQuantity ?? 0),
                BendQuantity = p.PlanItemDetails.Sum(d => d.BendQuantity ?? 0),
                SortQuantity = p.PlanItemDetails.Sum(d => d.SortQuantity ?? 0),
                SubAssemblyQuantity = p.PlanItemDetails.Sum(d => d.SubAssemblyQuantity ?? 0),
                PackedQuantity = p.PlanItemDetails.Sum(d => d.PackQuantity ?? 0)
            }).ConfigureAwait(false);

        var plansList = plansQuery.ToList();
        
        // Get plan status and created date separately for efficiency
        var planStatusQuery = await _planService.GetAsync(
            p => p.DueDate >= startDate && p.DueDate <= endDate,
            p => new { p.Id, p.Status, p.CreatedDate }).ConfigureAwait(false);
        var planStatusDict = planStatusQuery.ToDictionary(p => p.Id);

        // Calculate KPIs
        dashboard.TotalPlans = plansList.Count;
        dashboard.ActivePlans = plansList.Count(p => planStatusDict.ContainsKey(p.Id) && planStatusDict[p.Id].Status == StatusConstants.InProcess);
        dashboard.CompletedPlans = plansList.Count(p => planStatusDict.ContainsKey(p.Id) && planStatusDict[p.Id].Status == StatusConstants.Packed);
        dashboard.PlansDueToday = plansList.Count(p => p.DueDate?.Date == today);
        dashboard.OverduePlans = plansList.Count(p => p.DueDate < today && (!planStatusDict.ContainsKey(p.Id) || planStatusDict[p.Id].Status != StatusConstants.Packed));

        // Calculate quantities
        dashboard.TotalOrderQuantity = plansList.Sum(p => p.OrderQuantity);
        dashboard.TotalPrintQuantity = plansList.Sum(p => p.PrintQuantity ?? 0);
        dashboard.TotalBendQuantity = plansList.Sum(p => p.BendQuantity ?? 0);
        dashboard.TotalSortQuantity = plansList.Sum(p => p.SortQuantity ?? 0);
        dashboard.TotalSubAssemblyQuantity = plansList.Sum(p => p.SubAssemblyQuantity ?? 0);
        dashboard.TotalPackedQuantity = plansList.Sum(p => p.PackedQuantity ?? 0);

        // Calculate progress percentages
        dashboard.OverallProgressPercentage = dashboard.TotalOrderQuantity > 0
            ? (dashboard.TotalPackedQuantity / dashboard.TotalOrderQuantity) * 100
            : 0;

        dashboard.PrintProgressPercentage = dashboard.TotalOrderQuantity > 0
            ? (dashboard.TotalPrintQuantity / dashboard.TotalOrderQuantity) * 100
            : 0;

        dashboard.BendProgressPercentage = dashboard.TotalOrderQuantity > 0
            ? (dashboard.TotalBendQuantity / dashboard.TotalOrderQuantity) * 100
            : 0;

        dashboard.SortProgressPercentage = dashboard.TotalOrderQuantity > 0
            ? (dashboard.TotalSortQuantity / dashboard.TotalOrderQuantity) * 100
            : 0;

        dashboard.SubAssemblyProgressPercentage = dashboard.TotalOrderQuantity > 0
            ? (dashboard.TotalSubAssemblyQuantity / dashboard.TotalOrderQuantity) * 100
            : 0;

        dashboard.PackProgressPercentage = dashboard.OverallProgressPercentage;

        // Charts data - Plans by Due Date
        dashboard.PlansByDueDate = plansList
            .GroupBy(p => p.DueDate?.Date)
            .Where(g => g.Key.HasValue)
            .Select(g => new PlanByDueDateDto
            {
                DueDate = g.Key,
                PlanCount = g.Count(),
                TotalOrderQuantity = g.Sum(p => p.OrderQuantity)
            })
            .OrderBy(d => d.DueDate)
            .ToList();

        // Plans by Lot No
        dashboard.PlansByLotNo = plansList
            .GroupBy(p => p.LotNo)
            .Select(g => new PlanByLotNoDto
            {
                LotNo = g.Key,
                PlanCount = g.Count(),
                TotalOrderQuantity = g.Sum(p => p.OrderQuantity),
                TotalPackedQuantity = g.Sum(p => p.PackedQuantity ?? 0)
            })
            .OrderByDescending(d => d.TotalOrderQuantity)
            .Take(20)
            .ToList();

        // Status Distribution
        var statusGroups = planStatusDict.Values.GroupBy(p => p.Status ?? "Unknown");
        var totalPlans = plansList.Count;
        dashboard.StatusDistribution = statusGroups.Select(g => new PlanStatusDistributionDto
        {
            Status = g.Key,
            Count = g.Count(),
            Percentage = totalPlans > 0 ? (g.Count() / (double)totalPlans) * 100 : 0
        }).ToList();

        // Stage Completion
        dashboard.StageCompletion = new List<PlanStageCompletionDto>
        {
            new() { Stage = "Print", CompletedQuantity = dashboard.TotalPrintQuantity, TotalQuantity = dashboard.TotalOrderQuantity },
            new() { Stage = "Bend", CompletedQuantity = dashboard.TotalBendQuantity, TotalQuantity = dashboard.TotalOrderQuantity },
            new() { Stage = "Sort", CompletedQuantity = dashboard.TotalSortQuantity, TotalQuantity = dashboard.TotalOrderQuantity },
            new() { Stage = "SubAssembly", CompletedQuantity = dashboard.TotalSubAssemblyQuantity, TotalQuantity = dashboard.TotalOrderQuantity },
            new() { Stage = "Pack", CompletedQuantity = dashboard.TotalPackedQuantity, TotalQuantity = dashboard.TotalOrderQuantity }
        };

        foreach (var stage in dashboard.StageCompletion)
        {
            stage.CompletionPercentage = stage.TotalQuantity > 0
                ? (stage.CompletedQuantity / stage.TotalQuantity) * 100
                : 0;
        }

        // Recent Plans
        var recentPlans = plansList
            .OrderByDescending(p => planStatusDict.ContainsKey(p.Id) ? planStatusDict[p.Id].CreatedDate : (DateTime?)null)
            .Take(10)
            .ToList();

        dashboard.RecentPlans = recentPlans;

        // Alerts
        dashboard.Alerts = new List<PlanAlertDto>();
        foreach (var plan in plansList.Where(p => p.DueDate < today && (!planStatusDict.ContainsKey(p.Id) || planStatusDict[p.Id].Status != StatusConstants.Packed)))
        {
            var packedQty = plan.PackedQuantity ?? 0;
            var orderQty = plan.OrderQuantity;
            var progress = orderQty > 0 ? (packedQty / orderQty) * 100 : 0;

            dashboard.Alerts.Add(new PlanAlertDto
            {
                WarehouseOrderNo = plan.WarehouseOrderNo,
                LotNo = plan.LotNo,
                DueDate = plan.DueDate,
                AlertType = "Overdue",
                Message = $"Plan {plan.WarehouseOrderNo} is overdue with {progress:F1}% completion",
                Severity = progress < 50 ? "High" : progress < 80 ? "Medium" : "Low"
            });
        }

        return dashboard;
    }

    public async Task<PlanDashboardDto> GetPlanDashboardDataAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        return await GetPlanDashboardAsync(startDate, endDate).ConfigureAwait(false);
    }
    #endregion

    #region -- Label Dashboard --
    public async Task<LabelDashboardDto> GetLabelDashboardAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        var today = DateTime.Today;
        startDate ??= today.AddDays(-30);
        endDate ??= today.AddDays(30);

        var dashboard = new LabelDashboardDto();

        // Get all labels with optimized projection
        var labelsQuery = await _labelService.GetAsync(
            l => l.LabelDate >= startDate && l.LabelDate <= endDate,
            l => new LabelIndexDto
            {
                Id = l.Id,
                LabelDate = l.LabelDate,
                LabelType = l.LabelType,
                Status = l.Status,
                Quantity = l.Quantity,
                WarehouseOrderNo = l.WarehouseOrderNo
            }).ConfigureAwait(false);

        var labelsList = labelsQuery.ToList();

        // Overall KPIs
        dashboard.TotalLabels = labelsList.Count;
        dashboard.ActiveLabels = labelsList.Count(l => l.Status == StatusConstants.Active || l.Status == StatusConstants.Printed);
        dashboard.PrintedLabels = labelsList.Count(l => l.Status == StatusConstants.Printed);
        dashboard.PackedLabels = labelsList.Count(l => l.Status == StatusConstants.Packed);

        // Label Categories
        dashboard.PartLabels = GetLabelCategoryStats(labelsList, LabelType.Part);
        dashboard.StiffenerLabels = GetLabelCategoryStats(labelsList, LabelType.Stiffener);
        dashboard.StoreShirwalLabels = GetLabelCategoryStats(labelsList, LabelType.StoreShirwal);
        dashboard.StoreKhalapurLabels = GetLabelCategoryStats(labelsList, LabelType.StoreKhalapur);
        dashboard.SubAssemblyLabels = GetLabelCategoryStats(labelsList, LabelType.SubAssembly);
        dashboard.TrolleyLabels = GetLabelCategoryStats(labelsList, LabelType.Trolley);

        // Charts data - Labels by Date
        dashboard.LabelsByDate = labelsList
            .GroupBy(l => l.LabelDate?.Date)
            .Where(g => g.Key.HasValue)
            .Select(g => new LabelByDateDto
            {
                LabelDate = g.Key,
                Count = g.Count(),
                TotalQuantity = g.Sum(l => l.Quantity ?? 0)
            })
            .OrderBy(d => d.LabelDate)
            .ToList();

        // Labels by Status
        var statusGroups = labelsList.GroupBy(l => l.Status ?? "Unknown");
        var totalLabels = labelsList.Count;
        dashboard.LabelsByStatus = statusGroups.Select(g => new LabelByStatusDto
        {
            Status = g.Key,
            Count = g.Count(),
            Percentage = totalLabels > 0 ? (g.Count() / (double)totalLabels) * 100 : 0
        }).ToList();

        // Labels by Type
        var typeGroups = labelsList.GroupBy(l => l.LabelType ?? "Unknown");
        dashboard.LabelsByType = typeGroups.Select(g => new LabelByTypeDto
        {
            LabelType = g.Key,
            Count = g.Count(),
            Percentage = totalLabels > 0 ? (g.Count() / (double)totalLabels) * 100 : 0
        }).ToList();

        // Labels by Warehouse Order (Top 20)
        dashboard.LabelsByWarehouseOrder = labelsList
            .GroupBy(l => l.WarehouseOrderNo)
            .Select(g => new LabelByWarehouseOrderDto
            {
                WarehouseOrderNo = g.Key,
                LabelCount = g.Count(),
                TotalQuantity = g.Sum(l => l.Quantity ?? 0)
            })
            .OrderByDescending(d => d.LabelCount)
            .Take(20)
            .ToList();

        // Recent Labels
        var recentLabels = labelsList
            .OrderByDescending(l => l.LabelDate)
            .Take(10)
            .Select(l => new LabelIndexDto
            {
                Id = l.Id,
                LabelDate = l.LabelDate,
                WarehouseOrderNo = l.WarehouseOrderNo,
                Status = l.Status,
                Quantity = l.Quantity,
                LabelType = l.LabelType
            })
            .ToList();

        dashboard.RecentLabels = recentLabels;

        return dashboard;
    }

    private LabelCategoryStatsDto GetLabelCategoryStats(List<LabelIndexDto> labelsList, LabelType labelType)
    {
        var typeString = labelType.ToString();
        var categoryLabels = labelsList.Where(l => 
            l.LabelType == typeString || 
            (labelType == LabelType.Part && (l.LabelType == "Part" || string.IsNullOrEmpty(l.LabelType))) ||
            (labelType == LabelType.Store && l.LabelType == "Store")).ToList();

        var totalQty = categoryLabels.Sum(l => l.Quantity ?? 0);
        var packedQty = categoryLabels.Where(l => l.Status == StatusConstants.Packed).Sum(l => l.Quantity ?? 0);

        return new LabelCategoryStatsDto
        {
            Category = GetCategoryName(labelType),
            TotalCount = categoryLabels.Count,
            ActiveCount = categoryLabels.Count(l => l.Status == StatusConstants.Active || l.Status == StatusConstants.Printed),
            PrintedCount = categoryLabels.Count(l => l.Status == StatusConstants.Printed),
            PackedCount = categoryLabels.Count(l => l.Status == StatusConstants.Packed),
            TotalQuantity = totalQty,
            PackedQuantity = packedQty,
            ProgressPercentage = totalQty > 0 ? (packedQty / totalQty) * 100 : 0
        };
    }

    private string GetCategoryName(LabelType labelType)
    {
        return labelType switch
        {
            LabelType.Part => "Part Labels",
            LabelType.Stiffener => "Stiffener Labels",
            LabelType.StoreShirwal => "Store Shirwal Labels",
            LabelType.StoreKhalapur => "Store Khalapur Labels",
            LabelType.SubAssembly => "SubAssembly Labels",
            LabelType.Trolley => "Trolley Labels",
            _ => "Other Labels"
        };
    }

    public async Task<LabelDashboardDto> GetLabelDashboardDataAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        return await GetLabelDashboardAsync(startDate, endDate).ConfigureAwait(false);
    }
    #endregion

    #region -- Carton Dashboard --
    public async Task<CartonDashboardDto> GetCartonDashboardAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        var today = DateTime.Today;
        startDate ??= today.AddDays(-30);
        endDate ??= today.AddDays(30);

        var dashboard = new CartonDashboardDto();

        // Get all cartons with optimized projection
        // Note: We need to get carton details separately for quantity calculation
        var cartonsQuery = await _cartonService.GetAsync(
            c => c.PackingDate >= startDate && c.PackingDate <= endDate,
            c => new CartonIndexDto
            {
                Id = c.Id,
                PackingDate = c.PackingDate,
                Status = c.Status,
                WarehouseOrderNo = c.WarehouseOrderNo,
                CartonBarcode = c.CartonBarcode
            }).ConfigureAwait(false);

        var cartonsList = cartonsQuery.ToList();
        
        // Get carton quantities separately (optimized)
        var cartonIds = cartonsList.Select(c => c.Id).ToList();
        var cartonQuantities = await _cartonService.GetAsync(
            c => cartonIds.Contains(c.Id),
            c => new { c.Id, Quantity = c.CartonDetails.Sum(d => d.Quantity ?? 0) }).ConfigureAwait(false);
        var quantityDict = cartonQuantities.ToDictionary(c => c.Id);

        // Calculate KPIs
        dashboard.TotalCartons = cartonsList.Count;
        dashboard.PackedCartons = cartonsList.Count(c => c.Status == StatusConstants.Packed);
        dashboard.RackedInCartons = cartonsList.Count(c => c.Status == StatusConstants.RackIn);
        dashboard.DispatchedCartons = cartonsList.Count(c => c.Status == StatusConstants.Dispatched);

        // Calculate quantities
        dashboard.TotalCartonQuantity = cartonsList.Sum(c => quantityDict.ContainsKey(c.Id ?? 0) ? quantityDict[c.Id ?? 0].Quantity : 0);
        dashboard.PackedQuantity = cartonsList
            .Where(c => c.Status == StatusConstants.Packed || c.Status == StatusConstants.RackIn || c.Status == StatusConstants.Dispatched)
            .Sum(c => quantityDict.ContainsKey(c.Id ?? 0) ? quantityDict[c.Id ?? 0].Quantity : 0);

        dashboard.DispatchProgressPercentage = dashboard.TotalCartons > 0
            ? (dashboard.DispatchedCartons / (double)dashboard.TotalCartons) * 100
            : 0;

        // Charts data - Cartons by Date
        dashboard.CartonsByDate = cartonsList
            .GroupBy(c => c.PackingDate?.Date)
            .Where(g => g.Key.HasValue)
            .Select(g => new CartonByDateDto
            {
                PackingDate = g.Key,
                Count = g.Count(),
                TotalQuantity = g.Sum(c => quantityDict.ContainsKey(c.Id ?? 0) ? quantityDict[c.Id ?? 0].Quantity : 0)
            })
            .OrderBy(d => d.PackingDate)
            .ToList();

        // Cartons by Status
        var statusGroups = cartonsList.GroupBy(c => c.Status ?? "Unknown");
        var totalCartons = cartonsList.Count;
        dashboard.CartonsByStatus = statusGroups.Select(g => new CartonByStatusDto
        {
            Status = g.Key,
            Count = g.Count(),
            Percentage = totalCartons > 0 ? (g.Count() / (double)totalCartons) * 100 : 0
        }).ToList();

        // Cartons by Warehouse Order (Top 20)
        dashboard.CartonsByWarehouseOrder = cartonsList
            .GroupBy(c => c.WarehouseOrderNo)
            .Select(g => new CartonByWarehouseOrderDto
            {
                WarehouseOrderNo = g.Key,
                CartonCount = g.Count(),
                TotalQuantity = g.Sum(c => quantityDict.ContainsKey(c.Id ?? 0) ? quantityDict[c.Id ?? 0].Quantity : 0)
            })
            .OrderByDescending(d => d.CartonCount)
            .Take(20)
            .ToList();

        // Recent Cartons
        var recentCartons = cartonsList
            .OrderByDescending(c => c.PackingDate)
            .Take(10)
            .Select(c => new CartonIndexDto
            {
                Id = c.Id,
                PackingDate = c.PackingDate,
                WarehouseOrderNo = c.WarehouseOrderNo,
                CartonBarcode = c.CartonBarcode,
                Status = c.Status
            })
            .ToList();

        dashboard.RecentCartons = recentCartons;

        return dashboard;
    }

    public async Task<CartonDashboardDto> GetCartonDashboardDataAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        return await GetCartonDashboardAsync(startDate, endDate).ConfigureAwait(false);
    }
    #endregion
}

