using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Corno.Web.Areas.Kitchen.Services.Interfaces;
using Corno.Web.Globals;
using Corno.Web.Models.Packing;
using Corno.Web.Repository.Interfaces;
using Corno.Web.Services.HealthCheck.Interfaces;
using Corno.Web.Services.Interfaces;
using Corno.Web.Windsor;
using Newtonsoft.Json;

namespace Corno.Web.Services.HealthCheck.DataIntegrity;

public class DataIntegrityHealthCheckService : IDataIntegrityHealthCheckService, IService
{
    private readonly IGenericRepository<Models.Plan.Plan> _planRepository;
    private readonly IGenericRepository<Models.Packing.Label> _labelRepository;
    private readonly IGenericRepository<Carton> _cartonRepository;
    private readonly IGenericRepository<Models.HealthCheck.DataIntegrityReport> _dataIntegrityRepository;
    private readonly IPlanService _planService;

    // Configuration
    private const int DaysToCheck = 90; // Only check plans from last 90 days
    private const int BatchSize = 50; // Process plans in batches
    private const string PackedStatus = "Packed";
    private const string InProgressStatus = "InProgress";

    public DataIntegrityHealthCheckService(
        IGenericRepository<Models.Plan.Plan> planRepository,
        IGenericRepository<Models.Packing.Label> labelRepository,
        IGenericRepository<Carton> cartonRepository,
        IGenericRepository<Models.HealthCheck.DataIntegrityReport> dataIntegrityRepository)
    {
        _planRepository = planRepository;
        _labelRepository = labelRepository;
        _cartonRepository = cartonRepository;
        _dataIntegrityRepository = dataIntegrityRepository;
        _planService = Bootstrapper.Get<IPlanService>();
    }

    public async Task<List<HealthCheckResult>> RunAllDataIntegrityChecksAsync()
    {
        var results = new List<HealthCheckResult>();
        var checks = new List<Func<Task<HealthCheckResult>>>
        {
            CheckPlanQuantitiesAsync,
            CheckPlanPackQuantityFromCartonsAsync,
            CheckCartonBarcodeConsistencyAsync,
            CheckLabelDetailSequenceAsync
        };

        foreach (var check in checks)
        {
            try
            {
                var result = await check().ConfigureAwait(false);
                results.Add(result);
            }
            catch (Exception ex)
            {
                results.Add(new HealthCheckResult
                {
                    CheckName = check.Method.Name,
                    Status = HealthStatus.Critical,
                    Message = $"Data integrity check failed: {ex.Message}",
                    ExecutionTimeMs = 0
                });
            }
        }

        return results;
    }

    public async Task<HealthCheckResult> CheckPlanQuantitiesAsync()
    {
        var sw = Stopwatch.StartNew();
        var issues = new List<DataIntegrityIssue>();
        var plansChecked = 0;
        var plansSkipped = 0;
        
        try
        {
            var cutoffDate = DateTime.Now.AddDays(-DaysToCheck);
            
            var totalPlans = await _planRepository.CountAsync(p => (p.Status == null || p.Status == InProgressStatus || p.Status != PackedStatus)
                                                                   && (p.DueDate >= cutoffDate || p.PlanDate >= cutoffDate || p.DueDate == null)
            ).ConfigureAwait(false);

            var totalBatches = (int)Math.Ceiling(totalPlans / (double)BatchSize);
            
            for (int batch = 0; batch < totalBatches; batch++)
            {
                var batchNum = batch; // Capture for closure
                var plans = await _planRepository.GetAsync(
                    p => (p.Status == null || p.Status == InProgressStatus || p.Status != PackedStatus)
                      && (p.DueDate >= cutoffDate || p.PlanDate >= cutoffDate || p.DueDate == null)
                      && p.WarehouseOrderNo != null && !string.IsNullOrEmpty(p.WarehouseOrderNo),
                    p => p,
                    q => (IOrderedQueryable<Models.Plan.Plan>)q.OrderBy(x => x.DueDate).ThenBy(x => x.WarehouseOrderNo)
                          .Skip(batchNum * BatchSize)
                          .Take(BatchSize)
                ).ConfigureAwait(false);

                foreach (var plan in plans)
                {
                    var allPacked = await CheckAndMarkPlanAsPackedAsync(plan).ConfigureAwait(false);
                    if (allPacked)
                    {
                        plansSkipped++;
                        continue;
                    }

                    plansChecked++;
                    
                    var labels = await _labelRepository.GetAsync(
                        l => l.WarehouseOrderNo == plan.WarehouseOrderNo,
                        l => l
                    ).ConfigureAwait(false);

                    var labelsByPosition = labels.GroupBy(l => l.Position ?? string.Empty)
                        .ToDictionary(g => g.Key, g => g.ToList(), StringComparer.OrdinalIgnoreCase);

                    // Get cartons for this plan to check if labels are packed
                    var cartons = await _cartonRepository.GetAsync(
                        c => c.WarehouseOrderNo == plan.WarehouseOrderNo,
                        c => c
                    ).ConfigureAwait(false);
                    
            var cartonBarcodesByPosition = cartons
                .SelectMany(c => c.CartonDetails ?? [])
                .Where(cd => !string.IsNullOrEmpty(cd.Barcode) && !string.IsNullOrEmpty(cd.Position))
                .GroupBy(cd => cd.Position ?? string.Empty)
                .ToDictionary(
                    g => g.Key, 
                    g => new HashSet<string>(g.Select(cd => cd.Barcode), StringComparer.OrdinalIgnoreCase),
                    StringComparer.OrdinalIgnoreCase
                );

                    foreach (var planItemDetail in plan.PlanItemDetails ?? [])
                    {
                        var position = planItemDetail.Position ?? string.Empty;
                        
                        if (!labelsByPosition.TryGetValue(position, out var positionLabels))
                        {
                            if ((planItemDetail.PrintQuantity ?? 0) > 0 ||
                                (planItemDetail.BendQuantity ?? 0) > 0 ||
                                (planItemDetail.SortQuantity ?? 0) > 0)
                            {
                                issues.Add(new DataIntegrityIssue
                                {
                                    EntityType = "PlanItemDetail",
                                    EntityId = planItemDetail.Id,
                                    Identifier = $"{plan.WarehouseOrderNo}-{position}",
                                    IssueType = "NonZeroQuantityWithoutLabels",
                                    Description = "PlanItemDetail has quantities but no labels exist",
                                    ExpectedValue = "0",
                                    ActualValue = $"Print:{planItemDetail.PrintQuantity}, Bend:{planItemDetail.BendQuantity}, Sort:{planItemDetail.SortQuantity}",
                                    CanAutoFix = true
                                });
                            }
                            continue;
                        }

                        // Check if all labels for this position are packed and in cartons
                        var allLabelsPackedAndInCartons = true;
                        if (cartonBarcodesByPosition.TryGetValue(position, out var cartonBarcodes))
                        {
                            foreach (var label in positionLabels)
                            {
                                var isPacked = IsLabelPacked(label);
                                var isInCarton = !string.IsNullOrEmpty(label.Barcode) && 
                                                cartonBarcodes.Contains(label.Barcode);
                                
                                if (!isPacked || !isInCarton)
                                {
                                    allLabelsPackedAndInCartons = false;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            allLabelsPackedAndInCartons = false;
                        }

                        // If all labels are packed and in cartons, and PackQuantity exists, skip quantity checks
                        // because it means everything is correct
                        if (allLabelsPackedAndInCartons && (planItemDetail.PackQuantity ?? 0) > 0)
                        {
                            continue; // Skip quantity mismatch checks for packed items
                        }

                        var expectedPrintQty = CalculateQuantityFromLabelDetails(positionLabels, StatusConstants.Printed);
                        var expectedBendQty = CalculateQuantityFromLabelDetails(positionLabels, StatusConstants.Bent);
                        var expectedSortQty = CalculateQuantityFromLabelDetails(positionLabels, StatusConstants.Sorted);

                        var actualPrintQty = planItemDetail.PrintQuantity ?? 0;
                        if (Math.Abs(actualPrintQty - expectedPrintQty) > 0.01)
                        {
                            issues.Add(new DataIntegrityIssue
                            {
                                EntityType = "PlanItemDetail",
                                EntityId = planItemDetail.Id,
                                Identifier = $"{plan.WarehouseOrderNo}-{position}",
                                IssueType = "PrintQuantityMismatch",
                                Description = "PrintQuantity doesn't match LabelDetails with Printed status",
                                ExpectedValue = expectedPrintQty.ToString(CultureInfo.InvariantCulture),
                                ActualValue = actualPrintQty.ToString(CultureInfo.InvariantCulture),
                                CanAutoFix = true
                            });
                        }

                        var actualBendQty = planItemDetail.BendQuantity ?? 0;
                        if (Math.Abs(actualBendQty - expectedBendQty) > 0.01)
                        {
                            issues.Add(new DataIntegrityIssue
                            {
                                EntityType = "PlanItemDetail",
                                EntityId = planItemDetail.Id,
                                Identifier = $"{plan.WarehouseOrderNo}-{position}",
                                IssueType = "BendQuantityMismatch",
                                Description = "BendQuantity doesn't match LabelDetails with Bent status",
                                ExpectedValue = expectedBendQty.ToString(CultureInfo.InvariantCulture),
                                ActualValue = actualBendQty.ToString(CultureInfo.InvariantCulture),
                                CanAutoFix = true
                            });
                        }

                        var actualSortQty = planItemDetail.SortQuantity ?? 0;
                        if (Math.Abs(actualSortQty - expectedSortQty) > 0.01)
                        {
                            issues.Add(new DataIntegrityIssue
                            {
                                EntityType = "PlanItemDetail",
                                EntityId = planItemDetail.Id,
                                Identifier = $"{plan.WarehouseOrderNo}-{position}",
                                IssueType = "SortQuantityMismatch",
                                Description = "SortQuantity doesn't match LabelDetails with Sorted status",
                                ExpectedValue = expectedSortQty.ToString(CultureInfo.InvariantCulture),
                                ActualValue = actualSortQty.ToString(CultureInfo.InvariantCulture),
                                CanAutoFix = true
                            });
                        }
                    }
                }
            }

            sw.Stop();
            var status = issues.Count == 0 ? HealthStatus.Healthy 
                       : issues.Count > 50 ? HealthStatus.Critical 
                       : HealthStatus.Warning;

            // Save report
            await SaveDataIntegrityReportAsync("Plan Quantities Integrity", status, issues, plansChecked, plansSkipped, sw.ElapsedMilliseconds).ConfigureAwait(false);

            return new HealthCheckResult
            {
                CheckName = "Plan Quantities Integrity",
                Status = status,
                Message = issues.Count == 0 
                    ? $"All Plan quantities are correct (Checked: {plansChecked}, Skipped: {plansSkipped})" 
                    : $"Found {issues.Count} quantity mismatches (Checked: {plansChecked}, Skipped: {plansSkipped})",
                Details = new Dictionary<string, object>
                {
                    ["IssuesFound"] = issues.Count,
                    ["PlansChecked"] = plansChecked,
                    ["PlansSkipped"] = plansSkipped,
                    ["TotalPlans"] = totalPlans,
                    ["Issues"] = issues.Take(10).Select(i => new { i.Identifier, i.IssueType, i.Description }).ToList()
                },
                ExecutionTimeMs = (int)sw.ElapsedMilliseconds
            };
        }
        catch (Exception ex)
        {
            sw.Stop();
            return new HealthCheckResult
            {
                CheckName = "Plan Quantities Integrity",
                Status = HealthStatus.Critical,
                Message = $"Error checking plan quantities: {ex.Message}",
                ExecutionTimeMs = (int)sw.ElapsedMilliseconds
            };
        }
    }

    public async Task<HealthCheckResult> CheckPlanPackQuantityFromCartonsAsync()
    {
        var sw = Stopwatch.StartNew();
        var issues = new List<DataIntegrityIssue>();
        var plansChecked = 0;
        var plansSkipped = 0;

        try
        {
            var cutoffDate = DateTime.Now.AddDays(-DaysToCheck);
            
            var totalPlans = await _planRepository.CountAsync(
                p => (p.Status == null || p.Status == InProgressStatus || p.Status != PackedStatus)
                  && (p.DueDate >= cutoffDate || p.PlanDate >= cutoffDate || p.DueDate == null)
            ).ConfigureAwait(false);

            var totalBatches = (int)Math.Ceiling(totalPlans / (double)BatchSize);
            
            for (int batch = 0; batch < totalBatches; batch++)
            {
                var batchNum = batch; // Capture for closure
                var plans = await _planRepository.GetAsync(
                    p => (p.Status == null || p.Status == InProgressStatus || p.Status != PackedStatus)
                      && (p.DueDate >= cutoffDate || p.PlanDate >= cutoffDate || p.DueDate == null)
                      && p.WarehouseOrderNo != null && !string.IsNullOrEmpty(p.WarehouseOrderNo),
                    p => p,
                    q => (IOrderedQueryable<Models.Plan.Plan>)q.OrderBy(x => x.DueDate).ThenBy(x => x.WarehouseOrderNo)
                          .Skip(batchNum * BatchSize)
                          .Take(BatchSize)
                ).ConfigureAwait(false);

                foreach (var plan in plans)
                {
                    var allPacked = await CheckAndMarkPlanAsPackedAsync(plan).ConfigureAwait(false);
                    if (allPacked)
                    {
                        plansSkipped++;
                        continue;
                    }

                    plansChecked++;

                    var cartons = await _cartonRepository.GetAsync(
                        c => c.WarehouseOrderNo == plan.WarehouseOrderNo,
                        c => c
                    ).ConfigureAwait(false);

                    var expectedPackQty = cartons
                        .SelectMany(c => c.CartonDetails ?? [])
                        .Sum(cd => cd.Quantity ?? 0);

                    var actualPlanPackQty = plan.PackQuantity ?? 0;
                    if (Math.Abs(actualPlanPackQty - expectedPackQty) > 0.01)
                    {
                        issues.Add(new DataIntegrityIssue
                        {
                            EntityType = "Plan",
                            EntityId = plan.Id,
                            Identifier = plan.WarehouseOrderNo,
                            IssueType = "PlanPackQuantityMismatch",
                            Description = "Plan PackQuantity doesn't match total from Cartons",
                            ExpectedValue = expectedPackQty.ToString(CultureInfo.InvariantCulture),
                            ActualValue = actualPlanPackQty.ToString(CultureInfo.InvariantCulture),
                            CanAutoFix = true
                        });
                    }

                    // Get labels for this plan to verify barcode matching
                    var labels = await _labelRepository.GetAsync(
                        l => l.WarehouseOrderNo == plan.WarehouseOrderNo,
                        l => l
                    ).ConfigureAwait(false);
                    
                    var labelsByBarcode = labels
                        .Where(l => !string.IsNullOrEmpty(l.Barcode))
                        .ToDictionary(l => l.Barcode, l => l, StringComparer.OrdinalIgnoreCase);

                    foreach (var planItemDetail in plan.PlanItemDetails ?? [])
                    {
                        var position = planItemDetail.Position ?? string.Empty;
                        
                        // Get carton details for this position
                        var positionCartonDetails = cartons
                            .SelectMany(c => c.CartonDetails ?? [])
                            .Where(cd => (cd.Position ?? string.Empty).Equals(position, StringComparison.OrdinalIgnoreCase))
                            .ToList();
                        
                        // Verify that each barcode in carton exists in labels and matches WarehouseOrderNo and Position
                        var validCartonQty = 0.0;
                        foreach (var cartonDetail in positionCartonDetails)
                        {
                            if (string.IsNullOrEmpty(cartonDetail.Barcode)) continue;
                            
                            if (labelsByBarcode.TryGetValue(cartonDetail.Barcode, out var label))
                            {
                                // Verify WarehouseOrderNo and Position match
                                var warehouseOrderMatch = (label.WarehouseOrderNo ?? string.Empty)
                                    .Equals(plan.WarehouseOrderNo, StringComparison.OrdinalIgnoreCase);
                                var positionMatch = (label.Position ?? string.Empty)
                                    .Equals(position, StringComparison.OrdinalIgnoreCase);
                                
                                if (warehouseOrderMatch && positionMatch)
                                {
                                    validCartonQty += cartonDetail.Quantity ?? 0;
                                }
                            }
                        }

                        var actualPackQty = planItemDetail.PackQuantity ?? 0;
                        if (Math.Abs(actualPackQty - validCartonQty) > 0.01)
                        {
                            issues.Add(new DataIntegrityIssue
                            {
                                EntityType = "PlanItemDetail",
                                EntityId = planItemDetail.Id,
                                Identifier = $"{plan.WarehouseOrderNo}-{position}",
                                IssueType = "PlanItemDetailPackQuantityMismatch",
                                Description = "PlanItemDetail PackQuantity doesn't match CartonDetails for position (verified by WarehouseOrderNo, Position, and Barcode)",
                                ExpectedValue = validCartonQty.ToString(CultureInfo.InvariantCulture),
                                ActualValue = actualPackQty.ToString(CultureInfo.InvariantCulture),
                                CanAutoFix = true
                            });
                        }
                    }
                }
            }

            sw.Stop();
            var status = issues.Count == 0 ? HealthStatus.Healthy 
                       : issues.Count > 30 ? HealthStatus.Critical 
                       : HealthStatus.Warning;

            await SaveDataIntegrityReportAsync("Plan PackQuantity from Cartons", status, issues, plansChecked, plansSkipped, sw.ElapsedMilliseconds).ConfigureAwait(false);

            return new HealthCheckResult
            {
                CheckName = "Plan PackQuantity from Cartons",
                Status = status,
                Message = issues.Count == 0 
                    ? $"All Plan PackQuantities match Cartons (Checked: {plansChecked}, Skipped: {plansSkipped})" 
                    : $"Found {issues.Count} PackQuantity mismatches (Checked: {plansChecked}, Skipped: {plansSkipped})",
                Details = new Dictionary<string, object>
                {
                    ["IssuesFound"] = issues.Count,
                    ["PlansChecked"] = plansChecked,
                    ["PlansSkipped"] = plansSkipped,
                    ["Issues"] = issues.Take(10).Select(i => new { i.Identifier, i.IssueType }).ToList()
                },
                ExecutionTimeMs = (int)sw.ElapsedMilliseconds
            };
        }
        catch (Exception ex)
        {
            sw.Stop();
            return new HealthCheckResult
            {
                CheckName = "Plan PackQuantity from Cartons",
                Status = HealthStatus.Critical,
                Message = $"Error checking PackQuantity: {ex.Message}",
                ExecutionTimeMs = (int)sw.ElapsedMilliseconds
            };
        }
    }

    public async Task<HealthCheckResult> CheckCartonBarcodeConsistencyAsync()
    {
        var sw = Stopwatch.StartNew();
        var issues = new List<DataIntegrityIssue>();

        try
        {
            var cutoffDate = DateTime.Now.AddDays(-DaysToCheck);
            
            var cartons = await _cartonRepository.GetAsync(
                c => (c.PackingDate >= cutoffDate || c.PackingDate == null)
                  && (c.WarehouseOrderNo != null || c.ProductionOrderNo != null),
                c => c,
                q => q.OrderBy(x => x.PackingDate)
            ).ConfigureAwait(false);

            foreach (var carton in cartons)
            {
                var cartonDetails = carton.CartonDetails ?? [];
                if (!cartonDetails.Any()) continue;

                var barcodes = cartonDetails
                    .Where(cd => !string.IsNullOrEmpty(cd.Barcode))
                    .Select(cd => cd.Barcode)
                    .Distinct()
                    .ToList();

                if (!barcodes.Any()) continue;

                var labels = await _labelRepository.GetAsync(
                    l => barcodes.Contains(l.Barcode),
                    l => l
                ).ConfigureAwait(false);

                var cartonWarehouseOrderNo = carton.WarehouseOrderNo ?? string.Empty;
                var cartonProductionOrderNo = carton.ProductionOrderNo ?? string.Empty;

                foreach (var label in labels)
                {
                    var labelWarehouseOrderNo = label.WarehouseOrderNo ?? string.Empty;
                    var labelProductionOrderNo = label.ProductionOrderNo ?? string.Empty;

                    var warehouseOrderMatch = !string.IsNullOrEmpty(cartonWarehouseOrderNo) && 
                                            !string.IsNullOrEmpty(labelWarehouseOrderNo) &&
                                            cartonWarehouseOrderNo.Equals(labelWarehouseOrderNo, StringComparison.OrdinalIgnoreCase);
                    
                    var productionOrderMatch = !string.IsNullOrEmpty(cartonProductionOrderNo) && 
                                             !string.IsNullOrEmpty(labelProductionOrderNo) &&
                                             cartonProductionOrderNo.Equals(labelProductionOrderNo, StringComparison.OrdinalIgnoreCase);

                    if (!warehouseOrderMatch && !productionOrderMatch)
                    {
                        issues.Add(new DataIntegrityIssue
                        {
                            EntityType = "Carton",
                            EntityId = carton.Id,
                            Identifier = carton.CartonBarcode ?? carton.Id.ToString(),
                            IssueType = "BarcodeOrderNoMismatch",
                            Description = "Carton contains barcode from different WarehouseOrderNo/ProductionOrderNo",
                            ExpectedValue = $"WarehouseOrderNo: {cartonWarehouseOrderNo}, ProductionOrderNo: {cartonProductionOrderNo}",
                            ActualValue = $"Barcode: {label.Barcode}, WarehouseOrderNo: {labelWarehouseOrderNo}, ProductionOrderNo: {labelProductionOrderNo}",
                            CanAutoFix = false
                        });
                    }
                }
            }

            sw.Stop();
            var status = issues.Count == 0 ? HealthStatus.Healthy 
                       : issues.Count > 10 ? HealthStatus.Critical 
                       : HealthStatus.Warning;

            await SaveDataIntegrityReportAsync("Carton Barcode Consistency", status, issues, cartons.Count, 0, sw.ElapsedMilliseconds).ConfigureAwait(false);

            return new HealthCheckResult
            {
                CheckName = "Carton Barcode Consistency",
                Status = status,
                Message = issues.Count == 0 
                    ? $"All Cartons contain barcodes from same order (Checked: {cartons.Count})" 
                    : $"Found {issues.Count} Cartons with inconsistent barcodes",
                Details = new Dictionary<string, object>
                {
                    ["IssuesFound"] = issues.Count,
                    ["CartonsChecked"] = cartons.Count,
                    ["Issues"] = issues.Take(10).Select(i => new { i.Identifier, i.Description }).ToList()
                },
                ExecutionTimeMs = (int)sw.ElapsedMilliseconds
            };
        }
        catch (Exception ex)
        {
            sw.Stop();
            return new HealthCheckResult
            {
                CheckName = "Carton Barcode Consistency",
                Status = HealthStatus.Critical,
                Message = $"Error checking carton barcodes: {ex.Message}",
                ExecutionTimeMs = (int)sw.ElapsedMilliseconds
            };
        }
    }

    public async Task<HealthCheckResult> CheckLabelDetailSequenceAsync()
    {
        var sw = Stopwatch.StartNew();
        var issues = new List<DataIntegrityIssue>();

        try
        {
            var cutoffDate = DateTime.Now.AddDays(-DaysToCheck);
            
            // Set includes to load LabelDetails
            //_labelRepository.SetIncludes(nameof(LabelDetails));
            
            // Load labels first, then filter in memory to avoid LINQ to Entities translation issues
            var allLabels = await _labelRepository.GetAsync(
                l => (l.LabelDate >= cutoffDate || l.LabelDate == null),
                l => l,
                q => q.OrderBy(x => x.LabelDate)
            ).ConfigureAwait(false);
            
            // Filter in memory to only include labels with LabelDetails
            var labels = allLabels.Where(l => l.LabelDetails != null && l.LabelDetails.Any()).ToList();

            var expectedSequence = new[] 
            { 
                StatusConstants.Active, 
                StatusConstants.Printed, 
                StatusConstants.Bent, 
                StatusConstants.Sorted, 
                StatusConstants.Packed 
            };

            foreach (var label in labels)
            {
                var labelDetails = (label.LabelDetails ?? [])
                    .OrderBy(ld => ld.ScanDate ?? DateTime.MinValue)
                    .ToList();

                if (!labelDetails.Any()) continue;

                var statusSequence = labelDetails
                    .Where(ld => !string.IsNullOrEmpty(ld.Status))
                    .Select(ld => ld.Status)
                    .ToList();

                var lastValidIndex = -1;
                for (int i = 0; i < expectedSequence.Length; i++)
                {
                    var expectedStatus = expectedSequence[i];
                    var statusIndex = statusSequence.IndexOf(expectedStatus);
                    
                    if (statusIndex == -1)
                    {
                        var hasLaterStatus = expectedSequence.Skip(i + 1)
                            .Any(s => statusSequence.Contains(s));
                        
                        if (hasLaterStatus)
                        {
                            issues.Add(new DataIntegrityIssue
                            {
                                EntityType = "Label",
                                EntityId = label.Id,
                                Identifier = label.Barcode ?? label.Id.ToString(),
                                IssueType = "MissingStatusInSequence",
                                Description = $"Label is missing '{expectedStatus}' status but has later statuses",
                                ExpectedValue = string.Join(" -> ", expectedSequence),
                                ActualValue = string.Join(" -> ", statusSequence),
                                CanAutoFix = false
                            });
                        }
                    }
                    else
                    {
                        if (statusIndex < lastValidIndex)
                        {
                            issues.Add(new DataIntegrityIssue
                            {
                                EntityType = "Label",
                                EntityId = label.Id,
                                Identifier = label.Barcode ?? label.Id.ToString(),
                                IssueType = "StatusOutOfSequence",
                                Description = $"Status '{expectedStatus}' appears out of sequence",
                                ExpectedValue = string.Join(" -> ", expectedSequence),
                                ActualValue = string.Join(" -> ", statusSequence),
                                CanAutoFix = false
                            });
                        }
                        lastValidIndex = statusIndex;
                    }
                }
            }

            sw.Stop();
            var status = issues.Count == 0 ? HealthStatus.Healthy 
                       : issues.Count > 100 ? HealthStatus.Critical 
                       : HealthStatus.Warning;

            await SaveDataIntegrityReportAsync("Label Detail Sequence", status, issues, labels.Count, 0, sw.ElapsedMilliseconds).ConfigureAwait(false);

            return new HealthCheckResult
            {
                CheckName = "Label Detail Sequence",
                Status = status,
                Message = issues.Count == 0 
                    ? $"All Labels have correct status sequence (Checked: {labels.Count})" 
                    : $"Found {issues.Count} Labels with incorrect status sequence",
                Details = new Dictionary<string, object>
                {
                    ["IssuesFound"] = issues.Count,
                    ["LabelsChecked"] = labels.Count,
                    ["Issues"] = issues.Take(10).Select(i => new { i.Identifier, i.IssueType }).ToList()
                },
                ExecutionTimeMs = (int)sw.ElapsedMilliseconds
            };
        }
        catch (Exception ex)
        {
            sw.Stop();
            return new HealthCheckResult
            {
                CheckName = "Label Detail Sequence",
                Status = HealthStatus.Critical,
                Message = $"Error checking label sequence: {ex.Message}",
                ExecutionTimeMs = (int)sw.ElapsedMilliseconds
            };
        }
    }

    public async Task<List<HealthCheckResult>> CheckDataIntegrityForWarehouseOrderNoAsync(string warehouseOrderNo)
    {
        var results = new List<HealthCheckResult>();
        
        if (string.IsNullOrWhiteSpace(warehouseOrderNo))
        {
            results.Add(new HealthCheckResult
            {
                CheckName = "WarehouseOrderNo Validation",
                Status = HealthStatus.Critical,
                Message = "WarehouseOrderNo cannot be empty",
                ExecutionTimeMs = 0
            });
            return results;
        }

        try
        {
            // Get the plan
            var plan = await _planRepository.FirstOrDefaultAsync(
                p => p.WarehouseOrderNo == warehouseOrderNo,
                p => p
            ).ConfigureAwait(false);

            if (plan == null)
            {
                results.Add(new HealthCheckResult
                {
                    CheckName = "Plan Lookup",
                    Status = HealthStatus.Warning,
                    Message = $"Plan with WarehouseOrderNo '{warehouseOrderNo}' not found",
                    ExecutionTimeMs = 0
                });
                return results;
            }

            // Check Plan Quantities
            var planQuantitiesResult = await CheckPlanQuantitiesForSinglePlanAsync(plan).ConfigureAwait(false);
            results.Add(planQuantitiesResult);

            // Check PackQuantity from Cartons
            var packQuantityResult = await CheckPlanPackQuantityFromCartonsForSinglePlanAsync(plan).ConfigureAwait(false);
            results.Add(packQuantityResult);

            // Check Carton Barcode Consistency
            var cartonBarcodeResult = await CheckCartonBarcodeConsistencyForSinglePlanAsync(plan).ConfigureAwait(false);
            results.Add(cartonBarcodeResult);

            // Check Label Sequence
            var labelSequenceResult = await CheckLabelDetailSequenceForSinglePlanAsync(plan).ConfigureAwait(false);
            results.Add(labelSequenceResult);
        }
        catch (Exception ex)
        {
            results.Add(new HealthCheckResult
            {
                CheckName = "Data Integrity Check",
                Status = HealthStatus.Critical,
                Message = $"Error checking data integrity for WarehouseOrderNo '{warehouseOrderNo}': {ex.Message}",
                ExecutionTimeMs = 0
            });
        }

        return results;
    }

    private async Task<HealthCheckResult> CheckPlanQuantitiesForSinglePlanAsync(Models.Plan.Plan plan)
    {
        var sw = Stopwatch.StartNew();
        var issues = new List<DataIntegrityIssue>();

        try
        {
            var labels = await _labelRepository.GetAsync(l => l.WarehouseOrderNo == plan.WarehouseOrderNo,
                l => l).ConfigureAwait(false);

            var labelsByPosition = labels.GroupBy(l => l.Position ?? string.Empty)
                .ToDictionary(g => g.Key, g => g.ToList(), StringComparer.OrdinalIgnoreCase);

            // Get cartons for this plan to check if labels are packed
            var cartons = await _cartonRepository.GetAsync(c => c.WarehouseOrderNo == plan.WarehouseOrderNo,
                c => c).ConfigureAwait(false);
            
            var cartonBarcodesByPosition = cartons
                .SelectMany(c => c.CartonDetails ?? [])
                .Where(cd => !string.IsNullOrEmpty(cd.Barcode) && !string.IsNullOrEmpty(cd.Position))
                .GroupBy(cd => cd.Position ?? string.Empty)
                .ToDictionary(
                    g => g.Key, 
                    g => new HashSet<string>(g.Select(cd => cd.Barcode), StringComparer.OrdinalIgnoreCase),
                    StringComparer.OrdinalIgnoreCase
                );

            foreach (var planItemDetail in plan.PlanItemDetails ?? [])
            {
                var position = planItemDetail.Position ?? string.Empty;

                if (!labelsByPosition.TryGetValue(position, out var positionLabels))
                {
                    if ((planItemDetail.PrintQuantity ?? 0) > 0 ||
                        (planItemDetail.BendQuantity ?? 0) > 0 ||
                        (planItemDetail.SortQuantity ?? 0) > 0)
                    {
                        issues.Add(new DataIntegrityIssue
                        {
                            EntityType = "PlanItemDetail",
                            EntityId = planItemDetail.Id,
                            Identifier = $"{plan.WarehouseOrderNo}-{position}",
                            IssueType = "NonZeroQuantityWithoutLabels",
                            Description = "PlanItemDetail has quantities but no labels exist",
                            ExpectedValue = "0",
                            ActualValue = $"Print:{planItemDetail.PrintQuantity}, Bend:{planItemDetail.BendQuantity}, Sort:{planItemDetail.SortQuantity}",
                            CanAutoFix = true
                        });
                    }
                    continue;
                }

                // Check if all labels for this position are packed and in cartons
                var allLabelsPackedAndInCartons = true;
                if (cartonBarcodesByPosition.TryGetValue(position, out var cartonBarcodes))
                {
                    foreach (var label in positionLabels)
                    {
                        var isPacked = IsLabelPacked(label);
                        var isInCarton = !string.IsNullOrEmpty(label.Barcode) && 
                                        cartonBarcodes.Contains(label.Barcode);
                        
                        if (!isPacked || !isInCarton)
                        {
                            allLabelsPackedAndInCartons = false;
                            break;
                        }
                    }
                }
                else
                {
                    allLabelsPackedAndInCartons = false;
                }

                // If all labels are packed and in cartons, and PackQuantity exists, skip quantity checks
                // because it means everything is correct
                if (allLabelsPackedAndInCartons && (planItemDetail.PackQuantity ?? 0) > 0)
                {
                    continue; // Skip quantity mismatch checks for packed items
                }

                var expectedPrintQty = CalculateQuantityFromLabelDetails(positionLabels, StatusConstants.Printed);
                var expectedBendQty = CalculateQuantityFromLabelDetails(positionLabels, StatusConstants.Bent);
                var expectedSortQty = CalculateQuantityFromLabelDetails(positionLabels, StatusConstants.Sorted);

                var actualPrintQty = planItemDetail.PrintQuantity ?? 0;
                if (Math.Abs(actualPrintQty - expectedPrintQty) > 0.01)
                {
                    issues.Add(new DataIntegrityIssue
                    {
                        EntityType = "PlanItemDetail",
                        EntityId = planItemDetail.Id,
                        Identifier = $"{plan.WarehouseOrderNo}-{position}",
                        IssueType = "PrintQuantityMismatch",
                        Description = "PrintQuantity doesn't match LabelDetails with Printed status",
                        ExpectedValue = expectedPrintQty.ToString(CultureInfo.InvariantCulture),
                        ActualValue = actualPrintQty.ToString(CultureInfo.InvariantCulture),
                        CanAutoFix = true
                    });
                }

                var actualBendQty = planItemDetail.BendQuantity ?? 0;
                if (Math.Abs(actualBendQty - expectedBendQty) > 0.01)
                {
                    issues.Add(new DataIntegrityIssue
                    {
                        EntityType = "PlanItemDetail",
                        EntityId = planItemDetail.Id,
                        Identifier = $"{plan.WarehouseOrderNo}-{position}",
                        IssueType = "BendQuantityMismatch",
                        Description = "BendQuantity doesn't match LabelDetails with Bent status",
                        ExpectedValue = expectedBendQty.ToString(CultureInfo.InvariantCulture),
                        ActualValue = actualBendQty.ToString(CultureInfo.InvariantCulture),
                        CanAutoFix = true
                    });
                }

                var actualSortQty = planItemDetail.SortQuantity ?? 0;
                if (Math.Abs(actualSortQty - expectedSortQty) > 0.01)
                {
                    issues.Add(new DataIntegrityIssue
                    {
                        EntityType = "PlanItemDetail",
                        EntityId = planItemDetail.Id,
                        Identifier = $"{plan.WarehouseOrderNo}-{position}",
                        IssueType = "SortQuantityMismatch",
                        Description = "SortQuantity doesn't match LabelDetails with Sorted status",
                        ExpectedValue = expectedSortQty.ToString(CultureInfo.InvariantCulture),
                        ActualValue = actualSortQty.ToString(CultureInfo.InvariantCulture),
                        CanAutoFix = true
                    });
                }
            }

            sw.Stop();
            var status = issues.Count == 0 ? HealthStatus.Healthy 
                       : issues.Count > 10 ? HealthStatus.Critical 
                       : HealthStatus.Warning;

            return new HealthCheckResult
            {
                CheckName = "Plan Quantities Integrity",
                Status = status,
                Message = issues.Count == 0 
                    ? $"All Plan quantities are correct for {plan.WarehouseOrderNo}" 
                    : $"Found {issues.Count} quantity mismatches for {plan.WarehouseOrderNo}",
                Details = new Dictionary<string, object>
                {
                    ["IssuesFound"] = issues.Count,
                    ["WarehouseOrderNo"] = plan.WarehouseOrderNo,
                    ["Issues"] = issues
                },
                ExecutionTimeMs = (int)sw.ElapsedMilliseconds
            };
        }
        catch (Exception ex)
        {
            sw.Stop();
            return new HealthCheckResult
            {
                CheckName = "Plan Quantities Integrity",
                Status = HealthStatus.Critical,
                Message = $"Error checking plan quantities: {ex.Message}",
                ExecutionTimeMs = (int)sw.ElapsedMilliseconds
            };
        }
    }

    private async Task<HealthCheckResult> CheckPlanPackQuantityFromCartonsForSinglePlanAsync(Models.Plan.Plan plan)
    {
        var sw = Stopwatch.StartNew();
        var issues = new List<DataIntegrityIssue>();

        try
        {
            var cartons = await _cartonRepository.GetAsync(
                c => c.WarehouseOrderNo == plan.WarehouseOrderNo,
                c => c
            ).ConfigureAwait(false);

            // Get labels for this plan to verify barcode matching
            var labels = await _labelRepository.GetAsync(
                l => l.WarehouseOrderNo == plan.WarehouseOrderNo,
                l => l
            ).ConfigureAwait(false);
            
            var labelsByBarcode = labels
                .Where(l => !string.IsNullOrEmpty(l.Barcode))
                .ToDictionary(l => l.Barcode, l => l, StringComparer.OrdinalIgnoreCase);

            // Calculate valid pack quantity by verifying barcodes match labels
            var validPlanPackQty = 0.0;
            foreach (var carton in cartons)
            {
                foreach (var cartonDetail in carton.CartonDetails ?? [])
                {
                    if (string.IsNullOrEmpty(cartonDetail.Barcode)) continue;
                    
                    if (labelsByBarcode.TryGetValue(cartonDetail.Barcode, out var label))
                    {
                        // Verify WarehouseOrderNo matches
                        var warehouseOrderMatch = (label.WarehouseOrderNo ?? string.Empty)
                            .Equals(plan.WarehouseOrderNo, StringComparison.OrdinalIgnoreCase);
                        
                        if (warehouseOrderMatch)
                        {
                            validPlanPackQty += cartonDetail.Quantity ?? 0;
                        }
                    }
                }
            }

            var actualPlanPackQty = plan.PackQuantity ?? 0;
            if (Math.Abs(actualPlanPackQty - validPlanPackQty) > 0.01)
            {
                issues.Add(new DataIntegrityIssue
                {
                    EntityType = "Plan",
                    EntityId = plan.Id,
                    Identifier = plan.WarehouseOrderNo,
                    IssueType = "PlanPackQuantityMismatch",
                    Description = "Plan PackQuantity doesn't match total from Cartons (verified by WarehouseOrderNo and Barcode)",
                    ExpectedValue = validPlanPackQty.ToString(CultureInfo.InvariantCulture),
                    ActualValue = actualPlanPackQty.ToString(CultureInfo.InvariantCulture),
                    CanAutoFix = true
                });
            }

            foreach (var planItemDetail in plan.PlanItemDetails ?? [])
            {
                var position = planItemDetail.Position ?? string.Empty;
                
                // Get carton details for this position
                var positionCartonDetails = cartons
                    .SelectMany(c => c.CartonDetails ?? [])
                    .Where(cd => (cd.Position ?? string.Empty).Equals(position, StringComparison.OrdinalIgnoreCase))
                    .ToList();
                
                // Verify that each barcode in carton exists in labels and matches WarehouseOrderNo and Position
                var validCartonQty = 0.0;
                foreach (var cartonDetail in positionCartonDetails)
                {
                    if (string.IsNullOrEmpty(cartonDetail.Barcode)) continue;
                    
                    if (labelsByBarcode.TryGetValue(cartonDetail.Barcode, out var label))
                    {
                        // Verify WarehouseOrderNo and Position match
                        var warehouseOrderMatch = (label.WarehouseOrderNo ?? string.Empty)
                            .Equals(plan.WarehouseOrderNo, StringComparison.OrdinalIgnoreCase);
                        var positionMatch = (label.Position ?? string.Empty)
                            .Equals(position, StringComparison.OrdinalIgnoreCase);
                        
                        if (warehouseOrderMatch && positionMatch)
                        {
                            validCartonQty += cartonDetail.Quantity ?? 0;
                        }
                    }
                }

                var actualPackQty = planItemDetail.PackQuantity ?? 0;
                if (Math.Abs(actualPackQty - validCartonQty) > 0.01)
                {
                    issues.Add(new DataIntegrityIssue
                    {
                        EntityType = "PlanItemDetail",
                        EntityId = planItemDetail.Id,
                        Identifier = $"{plan.WarehouseOrderNo}-{position}",
                        IssueType = "PlanItemDetailPackQuantityMismatch",
                        Description = "PlanItemDetail PackQuantity doesn't match CartonDetails for position (verified by WarehouseOrderNo, Position, and Barcode)",
                        ExpectedValue = validCartonQty.ToString(CultureInfo.InvariantCulture),
                        ActualValue = actualPackQty.ToString(CultureInfo.InvariantCulture),
                        CanAutoFix = true
                    });
                }
            }

            sw.Stop();
            var status = issues.Count == 0 ? HealthStatus.Healthy 
                       : issues.Count > 5 ? HealthStatus.Critical 
                       : HealthStatus.Warning;

            return new HealthCheckResult
            {
                CheckName = "Plan PackQuantity from Cartons",
                Status = status,
                Message = issues.Count == 0 
                    ? $"All Plan PackQuantities match Cartons for {plan.WarehouseOrderNo}" 
                    : $"Found {issues.Count} PackQuantity mismatches for {plan.WarehouseOrderNo}",
                Details = new Dictionary<string, object>
                {
                    ["IssuesFound"] = issues.Count,
                    ["WarehouseOrderNo"] = plan.WarehouseOrderNo,
                    ["Issues"] = issues
                },
                ExecutionTimeMs = (int)sw.ElapsedMilliseconds
            };
        }
        catch (Exception ex)
        {
            sw.Stop();
            return new HealthCheckResult
            {
                CheckName = "Plan PackQuantity from Cartons",
                Status = HealthStatus.Critical,
                Message = $"Error checking PackQuantity: {ex.Message}",
                ExecutionTimeMs = (int)sw.ElapsedMilliseconds
            };
        }
    }

    private async Task<HealthCheckResult> CheckCartonBarcodeConsistencyForSinglePlanAsync(Models.Plan.Plan plan)
    {
        var sw = Stopwatch.StartNew();
        var issues = new List<DataIntegrityIssue>();

        try
        {
            var cartons = await _cartonRepository.GetAsync(
                c => c.WarehouseOrderNo == plan.WarehouseOrderNo,
                c => c
            ).ConfigureAwait(false);

            foreach (var carton in cartons)
            {
                var cartonDetails = carton.CartonDetails ?? [];
                if (!cartonDetails.Any()) continue;

                var barcodes = cartonDetails
                    .Where(cd => !string.IsNullOrEmpty(cd.Barcode))
                    .Select(cd => cd.Barcode)
                    .Distinct()
                    .ToList();

                if (!barcodes.Any()) continue;

                var labels = await _labelRepository.GetAsync(
                    l => barcodes.Contains(l.Barcode),
                    l => l
                ).ConfigureAwait(false);

                var cartonWarehouseOrderNo = carton.WarehouseOrderNo ?? string.Empty;
                var cartonProductionOrderNo = carton.ProductionOrderNo ?? string.Empty;

                foreach (var label in labels)
                {
                    var labelWarehouseOrderNo = label.WarehouseOrderNo ?? string.Empty;
                    var labelProductionOrderNo = label.ProductionOrderNo ?? string.Empty;

                    var warehouseOrderMatch = !string.IsNullOrEmpty(cartonWarehouseOrderNo) && 
                                            !string.IsNullOrEmpty(labelWarehouseOrderNo) &&
                                            cartonWarehouseOrderNo.Equals(labelWarehouseOrderNo, StringComparison.OrdinalIgnoreCase);

                    var productionOrderMatch = !string.IsNullOrEmpty(cartonProductionOrderNo) && 
                                             !string.IsNullOrEmpty(labelProductionOrderNo) &&
                                             cartonProductionOrderNo.Equals(labelProductionOrderNo, StringComparison.OrdinalIgnoreCase);

                    if (!warehouseOrderMatch && !productionOrderMatch)
                    {
                        issues.Add(new DataIntegrityIssue
                        {
                            EntityType = "Carton",
                            EntityId = carton.Id,
                            Identifier = carton.CartonBarcode ?? carton.Id.ToString(),
                            IssueType = "BarcodeOrderNoMismatch",
                            Description = "Carton contains barcode from different WarehouseOrderNo/ProductionOrderNo",
                            ExpectedValue = $"WarehouseOrderNo: {cartonWarehouseOrderNo}, ProductionOrderNo: {cartonProductionOrderNo}",
                            ActualValue = $"Barcode: {label.Barcode}, WarehouseOrderNo: {labelWarehouseOrderNo}, ProductionOrderNo: {labelProductionOrderNo}",
                            CanAutoFix = false
                        });
                    }
                }
            }

            sw.Stop();
            var status = issues.Count == 0 ? HealthStatus.Healthy 
                       : issues.Count > 5 ? HealthStatus.Critical 
                       : HealthStatus.Warning;

            return new HealthCheckResult
            {
                CheckName = "Carton Barcode Consistency",
                Status = status,
                Message = issues.Count == 0 
                    ? $"All Cartons contain barcodes from same order for {plan.WarehouseOrderNo}" 
                    : $"Found {issues.Count} Cartons with inconsistent barcodes for {plan.WarehouseOrderNo}",
                Details = new Dictionary<string, object>
                {
                    ["IssuesFound"] = issues.Count,
                    ["WarehouseOrderNo"] = plan.WarehouseOrderNo,
                    ["Issues"] = issues
                },
                ExecutionTimeMs = (int)sw.ElapsedMilliseconds
            };
        }
        catch (Exception ex)
        {
            sw.Stop();
            return new HealthCheckResult
            {
                CheckName = "Carton Barcode Consistency",
                Status = HealthStatus.Critical,
                Message = $"Error checking carton barcodes: {ex.Message}",
                ExecutionTimeMs = (int)sw.ElapsedMilliseconds
            };
        }
    }

    private async Task<HealthCheckResult> CheckLabelDetailSequenceForSinglePlanAsync(Models.Plan.Plan plan)
    {
        var sw = Stopwatch.StartNew();
        var issues = new List<DataIntegrityIssue>();

        try
        {
            // Set includes to load LabelDetails
            //_labelRepository.SetIncludes(nameof(Label.LabelDetails));
            
            // Load labels first, then filter in memory to avoid LINQ to Entities translation issues
            var allLabels = await _labelRepository.GetAsync(
                l => l.WarehouseOrderNo == plan.WarehouseOrderNo,
                l => l
            ).ConfigureAwait(false);
            
            // Filter in memory to only include labels with LabelDetails
            var labels = allLabels.Where(l => l.LabelDetails != null && l.LabelDetails.Any()).ToList();

            var expectedSequence = new[] 
            { 
                StatusConstants.Active, 
                StatusConstants.Printed, 
                StatusConstants.Bent, 
                StatusConstants.Sorted, 
                StatusConstants.Packed 
            };

            foreach (var label in labels)
            {
                var labelDetails = (label.LabelDetails ?? [])
                    .OrderBy(ld => ld.ScanDate ?? DateTime.MinValue)
                    .ToList();

                if (!labelDetails.Any()) continue;

                var statusSequence = labelDetails
                    .Where(ld => !string.IsNullOrEmpty(ld.Status))
                    .Select(ld => ld.Status)
                    .ToList();

                var lastValidIndex = -1;
                for (int i = 0; i < expectedSequence.Length; i++)
                {
                    var expectedStatus = expectedSequence[i];
                    var statusIndex = statusSequence.IndexOf(expectedStatus);

                    if (statusIndex == -1)
                    {
                        var hasLaterStatus = expectedSequence.Skip(i + 1)
                            .Any(s => statusSequence.Contains(s));

                        if (hasLaterStatus)
                        {
                            issues.Add(new DataIntegrityIssue
                            {
                                EntityType = "Label",
                                EntityId = label.Id,
                                Identifier = label.Barcode ?? label.Id.ToString(),
                                IssueType = "MissingStatusInSequence",
                                Description = $"Label is missing '{expectedStatus}' status but has later statuses",
                                ExpectedValue = string.Join(" -> ", expectedSequence),
                                ActualValue = string.Join(" -> ", statusSequence),
                                CanAutoFix = false
                            });
                        }
                    }
                    else
                    {
                        if (statusIndex < lastValidIndex)
                        {
                            issues.Add(new DataIntegrityIssue
                            {
                                EntityType = "Label",
                                EntityId = label.Id,
                                Identifier = label.Barcode ?? label.Id.ToString(),
                                IssueType = "StatusOutOfSequence",
                                Description = $"Status '{expectedStatus}' appears out of sequence",
                                ExpectedValue = string.Join(" -> ", expectedSequence),
                                ActualValue = string.Join(" -> ", statusSequence),
                                CanAutoFix = false
                            });
                        }
                        lastValidIndex = statusIndex;
                    }
                }
            }

            sw.Stop();
            var status = issues.Count == 0 ? HealthStatus.Healthy 
                       : issues.Count > 20 ? HealthStatus.Critical 
                       : HealthStatus.Warning;

            return new HealthCheckResult
            {
                CheckName = "Label Detail Sequence",
                Status = status,
                Message = issues.Count == 0 
                    ? $"All Labels have correct status sequence for {plan.WarehouseOrderNo}" 
                    : $"Found {issues.Count} Labels with incorrect status sequence for {plan.WarehouseOrderNo}",
                Details = new Dictionary<string, object>
                {
                    ["IssuesFound"] = issues.Count,
                    ["WarehouseOrderNo"] = plan.WarehouseOrderNo,
                    ["Issues"] = issues
                },
                ExecutionTimeMs = (int)sw.ElapsedMilliseconds
            };
        }
        catch (Exception ex)
        {
            sw.Stop();
            return new HealthCheckResult
            {
                CheckName = "Label Detail Sequence",
                Status = HealthStatus.Critical,
                Message = $"Error checking label sequence: {ex.Message}",
                ExecutionTimeMs = (int)sw.ElapsedMilliseconds
            };
        }
    }

    public async Task<DataIntegrityFixResult> AutoFixDataIntegrityIssuesAsync(List<DataIntegrityIssue> issues)
    {
        var fixResult = new DataIntegrityFixResult
        {
            TotalIssues = issues.Count,
            FixedIssues = 0,
            FailedFixes = 0,
            FixDetails = []
        };

        var fixableIssues = issues.Where(i => i.CanAutoFix).ToList();

        foreach (var issue in fixableIssues)
        {
            try
            {
                switch (issue.IssueType)
                {
                    case "PrintQuantityMismatch":
                    case "BendQuantityMismatch":
                    case "SortQuantityMismatch":
                    case "PlanPrintQuantityMismatch":
                    case "PlanPackQuantityMismatch":
                    case "PlanItemDetailPackQuantityMismatch":
                    case "NonZeroQuantityWithoutLabels":
                        await FixPlanQuantityAsync(issue).ConfigureAwait(false);
                        fixResult.FixedIssues++;
                        fixResult.FixDetails.Add($"Fixed {issue.IssueType} for {issue.Identifier}");
                        break;
                    
                    default:
                        fixResult.FailedFixes++;
                        fixResult.FixDetails.Add($"Cannot auto-fix {issue.IssueType} for {issue.Identifier}");
                        break;
                }
            }
            catch (Exception ex)
            {
                fixResult.FailedFixes++;
                fixResult.FixDetails.Add($"Error fixing {issue.IssueType} for {issue.Identifier}: {ex.Message}");
            }
        }

        return fixResult;
    }

    private async Task<bool> CheckAndMarkPlanAsPackedAsync(Models.Plan.Plan plan)
    {
        if (plan.PlanItemDetails == null || !plan.PlanItemDetails.Any())
            return false;

        var allPacked = plan.PlanItemDetails.All(pid => 
            (pid.PackQuantity ?? 0) >= (pid.OrderQuantity ?? 0) && 
            (pid.OrderQuantity ?? 0) > 0);

        if (allPacked && plan.Status != PackedStatus)
        {
            plan.Status = PackedStatus;
            plan.ModifiedDate = DateTime.Now;
            plan.ModifiedBy = "System";
            await _planRepository.UpdateAsync(plan).ConfigureAwait(false);
            await _planRepository.SaveAsync().ConfigureAwait(false);
            return true;
        }

        if (!allPacked && (plan.Status == null || plan.Status == PackedStatus))
        {
            plan.Status = InProgressStatus;
            plan.ModifiedDate = DateTime.Now;
            plan.ModifiedBy = "System";
            await _planRepository.UpdateAsync(plan).ConfigureAwait(false);
            await _planRepository.SaveAsync().ConfigureAwait(false);
        }

        return allPacked;
    }

    private double CalculateQuantityFromLabelDetails(List<Models.Packing.Label> labels, string status)
    {
        return labels
            .Where(l => 
            {
                // If label is Packed, it has gone through all previous statuses
                var isPacked = l.LabelDetails?.Any(d => d.Status == StatusConstants.Packed) ?? false;
                if (isPacked)
                {
                    // If checking for Printed, Bent, or Sorted, and label is Packed, count it
                    // because Packed means it went through all previous statuses
                    if (status == StatusConstants.Printed || 
                        status == StatusConstants.Bent || 
                        status == StatusConstants.Sorted)
                    {
                        return true;
                    }
                }
                // Otherwise, check if label has the specific status
                return l.LabelDetails?.Any(d => d.Status == status) ?? false;
            })
            .Sum(l => l.Quantity ?? 0);
    }
    
    private bool IsLabelPacked(Models.Packing.Label label)
    {
        return label.LabelDetails?.Any(d => d.Status == StatusConstants.Packed) ?? false;
    }
    
    private async Task<bool> IsLabelInCartonAsync(string warehouseOrderNo, string barcode, string position)
    {
        var cartons = await _cartonRepository.GetAsync(
            c => c.WarehouseOrderNo == warehouseOrderNo,
            c => c
        ).ConfigureAwait(false);
        
        return cartons
            .SelectMany(c => c.CartonDetails ?? [])
            .Any(cd => (cd.Barcode ?? string.Empty).Equals(barcode, StringComparison.OrdinalIgnoreCase) &&
                      (cd.Position ?? string.Empty).Equals(position, StringComparison.OrdinalIgnoreCase));
    }

    private async Task FixPlanQuantityAsync(DataIntegrityIssue issue)
    {
        var parts = issue.Identifier.Split('-');
        if (parts.Length < 1) return;

        var warehouseOrderNo = parts[0];
        var plan = await _planRepository.FirstOrDefaultAsync(
            p => p.WarehouseOrderNo == warehouseOrderNo,
            p => p
        ).ConfigureAwait(false);

        if (plan == null) return;

        // Recalculate quantities using PlanService
        await _planService.UpdateQuantitiesAsync(plan).ConfigureAwait(false);
    }

    private async Task SaveDataIntegrityReportAsync(string checkType, HealthStatus status, List<DataIntegrityIssue> issues, int recordsChecked, int recordsSkipped, long elapsedMs)
    {
        var report = new Models.HealthCheck.DataIntegrityReport
        {
            CheckDate = DateTime.Now,
            CheckType = checkType,
            Status = status.ToString(),
            Message = issues.Count == 0 
                ? $"All checks passed (Checked: {recordsChecked}, Skipped: {recordsSkipped})" 
                : $"Found {issues.Count} issues (Checked: {recordsChecked}, Skipped: {recordsSkipped})",
            Details = JsonConvert.SerializeObject(new { Issues = issues }),
            ExecutionTimeMs = (int)elapsedMs,
            RecordsChecked = recordsChecked,
            IssuesFound = issues.Count,
            RecordsFixed = 0,
            AutoFixed = false,
            CreatedBy = "System",
            CreatedDate = DateTime.Now
        };
        
        await _dataIntegrityRepository.AddAsync(report).ConfigureAwait(false);
        await _dataIntegrityRepository.SaveAsync().ConfigureAwait(false);
    }
}

