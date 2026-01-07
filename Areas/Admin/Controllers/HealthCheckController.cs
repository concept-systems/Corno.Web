using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Corno.Web.Areas.Admin.Dto.HealthCheck;
using Corno.Web.Areas.Admin.Services.Interfaces;
using Corno.Web.Controllers;
using Corno.Web.Logger;
using Corno.Web.Services.HealthCheck.DataIntegrity;
using Corno.Web.Services.HealthCheck.Interfaces;
using Kendo.Mvc.UI;

namespace Corno.Web.Areas.Admin.Controllers;

public class HealthCheckController : SuperController
{
    #region -- Constructors --

    public HealthCheckController(IHealthCheckUIService healthCheckUIService,
        IDataIntegrityHealthCheckService dataIntegrityService)
    {
        _healthCheckUIService = healthCheckUIService;
        _dataIntegrityService = dataIntegrityService;
        _indexPath = "~/Areas/Admin/Views/HealthCheck/Index.cshtml";
        _dataIntegrityPath = "~/Areas/Admin/Views/HealthCheck/DataIntegrity.cshtml";
    }

    #endregion

    #region -- Data Members --

    private readonly IHealthCheckUIService _healthCheckUIService;
    private readonly IDataIntegrityHealthCheckService _dataIntegrityService;
    private readonly string _indexPath;
    private readonly string _dataIntegrityPath;

    #endregion

    #region -- Actions --

    [Authorize]
    public async Task<ActionResult> Index()
    {
        var dashboardData = await _healthCheckUIService.GetDashboardDataAsync().ConfigureAwait(false);
        return View(_indexPath, dashboardData);
    }

    [Authorize]
    public ActionResult DataIntegrity()
    {
        return View(_dataIntegrityPath);
    }

    [HttpPost]
    public async Task<ActionResult> GetHealthCheckReports([DataSourceRequest] DataSourceRequest request,
        DateTime? fromDate, DateTime? toDate, string status = null)
    {
        try
        {
            var reports = await _healthCheckUIService.GetHealthCheckReportsAsync(fromDate, toDate, status)
                .ConfigureAwait(false);

            var result = new DataSourceResult
            {
                Data = reports.Skip((request.Page - 1) * request.PageSize).Take(request.PageSize),
                Total = reports.Count
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            return GetGridErrorResult(exception);
        }
    }

    [HttpPost]
    public async Task<ActionResult> GetDataIntegrityIssues([DataSourceRequest] DataSourceRequest request,
        DateTime? fromDate, DateTime? toDate, string checkType = null, string status = null)
    {
        try
        {
            var issues = await _healthCheckUIService.GetDataIntegrityIssuesAsync(fromDate, toDate, checkType, status)
                .ConfigureAwait(false);

            var result = new DataSourceResult
            {
                Data = issues.Skip((request.Page - 1) * request.PageSize).Take(request.PageSize),
                Total = issues.Count
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            return GetGridErrorResult(exception);
        }
    }

    [HttpPost]
    public async Task<ActionResult> RunManualHealthCheck()
    {
        try
        {
            var success = await _healthCheckUIService.RunManualHealthCheckAsync().ConfigureAwait(false);
            return Json(
                new
                {
                    success,
                    message = success ? "Health check completed successfully" : "Health check failed"
                },
                JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            return GetJsonResult(exception);
        }
    }

    [HttpPost]
    public async Task<ActionResult> RunManualDataIntegrityCheck()
    {
        try
        {
            var success = await _healthCheckUIService.RunManualDataIntegrityCheckAsync().ConfigureAwait(false);
            return Json(
                new
                {
                    success,
                    message = success ? "Data integrity check completed successfully" : "Data integrity check failed"
                },
                JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            return GetJsonResult(exception);
        }
    }

    [HttpPost]
    public async Task<ActionResult> CheckDataIntegrityForWarehouseOrderNo(string warehouseOrderNo)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(warehouseOrderNo))
            {
                return Json(new { success = false, message = "WarehouseOrderNo is required" },
                    JsonRequestBehavior.AllowGet);
            }

            var results = await _healthCheckUIService.CheckDataIntegrityForWarehouseOrderNoAsync(warehouseOrderNo)
                .ConfigureAwait(false);

            var allIssues = new List<DataIntegrityIssue>();
            foreach (var result in results)
            {
                if (result.Details != null && result.Details.ContainsKey("Issues"))
                {
                    var issues = result.Details["Issues"] as List<DataIntegrityIssue>;
                    if (issues != null)
                    {
                        allIssues.AddRange(issues);
                    }
                }
            }

            var totalIssues = allIssues.Count;
            var criticalIssues = results.Count(r => r.Status == HealthStatus.Critical);
            var warningIssues = results.Count(r => r.Status == HealthStatus.Warning);
            var healthyChecks = results.Count(r => r.Status == HealthStatus.Healthy);

            // Organize issues by entity type and identifier for better presentation
            var issuesByPosition = allIssues
                .Where(i => i.EntityType == "PlanItemDetail" && i.Identifier.Contains("-"))
                .GroupBy(i =>
                {
                    var parts = i.Identifier.Split('-');
                    return parts.Length > 1 ? parts[parts.Length - 1] : i.Identifier;
                })
                .Select(g => new
                {
                    position = g.Key,
                    issues = g.Select(i => new
                    {
                        entityType = i.EntityType,
                        identifier = i.Identifier,
                        issueType = i.IssueType,
                        description = i.Description,
                        expectedValue = i.ExpectedValue,
                        actualValue = i.ActualValue,
                        canAutoFix = i.CanAutoFix
                    }).ToList()
                }).ToList();

            var issuesByCarton = allIssues
                .Where(i => i.EntityType == "Carton")
                .GroupBy(i => i.Identifier)
                .Select(g => new
                {
                    cartonBarcode = g.Key,
                    issues = g.Select(i => new
                    {
                        entityType = i.EntityType,
                        identifier = i.Identifier,
                        issueType = i.IssueType,
                        description = i.Description,
                        expectedValue = i.ExpectedValue,
                        actualValue = i.ActualValue,
                        canAutoFix = i.CanAutoFix
                    }).ToList()
                }).ToList();

            var issuesByLabel = allIssues
                .Where(i => i.EntityType == "Label")
                .GroupBy(i => i.Identifier)
                .Select(g => new
                {
                    barcode = g.Key,
                    issues = g.Select(i => new
                    {
                        entityType = i.EntityType,
                        identifier = i.Identifier,
                        issueType = i.IssueType,
                        description = i.Description,
                        expectedValue = i.ExpectedValue,
                        actualValue = i.ActualValue,
                        canAutoFix = i.CanAutoFix
                    }).ToList()
                }).ToList();

            var planLevelIssues = allIssues
                .Where(i => i.EntityType == "Plan" || (i.EntityType == "PlanItemDetail" && !i.Identifier.Contains("-")))
                .Select(i => new
                {
                    entityType = i.EntityType,
                    identifier = i.Identifier,
                    issueType = i.IssueType,
                    description = i.Description,
                    expectedValue = i.ExpectedValue,
                    actualValue = i.ActualValue,
                    canAutoFix = i.CanAutoFix
                }).ToList();

            var viewModel = new
            {
                warehouseOrderNo = warehouseOrderNo,
                totalIssues,
                criticalIssues,
                warningIssues,
                healthyChecks,
                fixableIssues = allIssues.Count(i => i.CanAutoFix),
                results = results.Select(r => new
                {
                    checkName = r.CheckName,
                    status = r.Status.ToString(),
                    message = r.Message,
                    executionTimeMs = r.ExecutionTimeMs
                }).ToList(),
                issuesByPosition = issuesByPosition,
                issuesByCarton = issuesByCarton,
                issuesByLabel = issuesByLabel,
                planLevelIssues = planLevelIssues,
                allIssues = allIssues.Select(i => new
                {
                    entityType = i.EntityType,
                    identifier = i.Identifier,
                    issueType = i.IssueType,
                    description = i.Description,
                    expectedValue = i.ExpectedValue,
                    actualValue = i.ActualValue,
                    canAutoFix = i.CanAutoFix
                }).ToList()
            };

            return Json(new
            {
                success = true,
                message = $"Data integrity check completed for {warehouseOrderNo}",
                warehouseOrderNo = warehouseOrderNo,
                totalIssues,
                criticalIssues,
                warningIssues,
                healthyChecks,
                fixableIssues = allIssues.Count(i => i.CanAutoFix)
            }, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            LogHandler.LogError(exception);
            throw;
        }
    }

    [HttpPost]
    public async Task<ActionResult> GetDataIntegrityResultsModal(string warehouseOrderNo)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(warehouseOrderNo))
            {
                return PartialView("_DataIntegrityResultsModal", new DataIntegrityResultsModalDto());
            }

            var results = await _healthCheckUIService.CheckDataIntegrityForWarehouseOrderNoAsync(warehouseOrderNo)
                .ConfigureAwait(false);

            var allIssues = new List<DataIntegrityIssue>();
            foreach (var result in results)
            {
                if (result.Details != null && result.Details.ContainsKey("Issues"))
                {
                    var issues = result.Details["Issues"] as List<DataIntegrityIssue>;
                    if (issues != null)
                    {
                        allIssues.AddRange(issues);
                    }
                }
            }

            var totalIssues = allIssues.Count;
            var criticalIssues = results.Count(r => r.Status == HealthStatus.Critical);
            var warningIssues = results.Count(r => r.Status == HealthStatus.Warning);
            var healthyChecks = results.Count(r => r.Status == HealthStatus.Healthy);

            // Organize issues by entity type and identifier for better presentation
            var issuesByPosition = allIssues
                .Where(i => i.EntityType == "PlanItemDetail" && i.Identifier.Contains("-"))
                .GroupBy(i =>
                {
                    var parts = i.Identifier.Split('-');
                    return parts.Length > 1 ? parts[parts.Length - 1] : i.Identifier;
                })
                .Select(g => new IssuesByPositionDto
                {
                    Position = g.Key,
                    Issues = g.Select(i => new IssueDto
                    {
                        EntityType = i.EntityType,
                        Identifier = i.Identifier,
                        IssueType = i.IssueType,
                        Description = i.Description,
                        ExpectedValue = i.ExpectedValue,
                        ActualValue = i.ActualValue,
                        CanAutoFix = i.CanAutoFix
                    }).ToList()
                }).ToList();

            var issuesByCarton = allIssues
                .Where(i => i.EntityType == "Carton")
                .GroupBy(i => i.Identifier)
                .Select(g => new IssuesByCartonDto
                {
                    CartonBarcode = g.Key,
                    Issues = g.Select(i => new IssueDto
                    {
                        EntityType = i.EntityType,
                        Identifier = i.Identifier,
                        IssueType = i.IssueType,
                        Description = i.Description,
                        ExpectedValue = i.ExpectedValue,
                        ActualValue = i.ActualValue,
                        CanAutoFix = i.CanAutoFix
                    }).ToList()
                }).ToList();

            var issuesByLabel = allIssues
                .Where(i => i.EntityType == "Label")
                .GroupBy(i => i.Identifier)
                .Select(g => new IssuesByLabelDto
                {
                    Barcode = g.Key,
                    Issues = g.Select(i => new IssueDto
                    {
                        EntityType = i.EntityType,
                        Identifier = i.Identifier,
                        IssueType = i.IssueType,
                        Description = i.Description,
                        ExpectedValue = i.ExpectedValue,
                        ActualValue = i.ActualValue,
                        CanAutoFix = i.CanAutoFix
                    }).ToList()
                }).ToList();

            var planLevelIssues = allIssues
                .Where(i => i.EntityType == "Plan" || (i.EntityType == "PlanItemDetail" && !i.Identifier.Contains("-")))
                .Select(i => new PlanLevelIssueDto
                {
                    EntityType = i.EntityType,
                    Identifier = i.Identifier,
                    IssueType = i.IssueType,
                    Description = i.Description,
                    ExpectedValue = i.ExpectedValue,
                    ActualValue = i.ActualValue,
                    CanAutoFix = i.CanAutoFix
                }).ToList();

            var viewModel = new DataIntegrityResultsModalDto
            {
                WarehouseOrderNo = warehouseOrderNo,
                TotalIssues = totalIssues,
                CriticalIssues = criticalIssues,
                WarningIssues = warningIssues,
                HealthyChecks = healthyChecks,
                FixableIssues = allIssues.Count(i => i.CanAutoFix),
                Results = results.Select(r => new CheckResultDto
                {
                    CheckName = r.CheckName,
                    Status = r.Status.ToString(),
                    Message = r.Message,
                    ExecutionTimeMs = r.ExecutionTimeMs
                }).ToList(),
                IssuesByPosition = issuesByPosition,
                IssuesByCarton = issuesByCarton,
                IssuesByLabel = issuesByLabel,
                PlanLevelIssues = planLevelIssues
            };

            return PartialView("_DataIntegrityResultsModal", viewModel);
        }
        catch (Exception exception)
        {
            LogHandler.LogError(exception);
            return PartialView("_DataIntegrityResultsModal", new DataIntegrityResultsModalDto
            {
                WarehouseOrderNo = warehouseOrderNo ?? "N/A"
            });
        }
    }

    [HttpPost]
    public async Task<ActionResult> AutoFixDataIntegrityForWarehouseOrderNo(string warehouseOrderNo)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(warehouseOrderNo))
            {
                return Json(new { success = false, message = "WarehouseOrderNo is required" },
                    JsonRequestBehavior.AllowGet);
            }

            var results = await _healthCheckUIService.CheckDataIntegrityForWarehouseOrderNoAsync(warehouseOrderNo)
                .ConfigureAwait(false);

            var allIssues = new List<DataIntegrityIssue>();
            foreach (var result in results)
            {
                if (result.Details != null && result.Details.ContainsKey("Issues"))
                {
                    var issues = result.Details["Issues"] as List<DataIntegrityIssue>;
                    if (issues != null)
                    {
                        allIssues.AddRange(issues);
                    }
                }
            }

            var fixableIssues = allIssues.Where(i => i.CanAutoFix).ToList();

            if (fixableIssues.Count == 0)
            {
                return Json(new
                {
                    success = false,
                    message = $"No auto-fixable issues found for {warehouseOrderNo}"
                }, JsonRequestBehavior.AllowGet);
            }

            // Actually fix the issues
            var actualFixResult = await _dataIntegrityService.AutoFixDataIntegrityIssuesAsync(fixableIssues)
                .ConfigureAwait(false);

            return Json(new
            {
                success = true,
                message =
                    $"Auto-fixed {actualFixResult.FixedIssues} out of {actualFixResult.TotalIssues} issues for {warehouseOrderNo}",
                details = actualFixResult.FixDetails
            }, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            return GetJsonResult(exception);
        }
    }

    [HttpPost]
    public async Task<ActionResult> AutoFixIssues(List<int> issueIds)
    {
        try
        {
            var fixResult = await _healthCheckUIService.AutoFixDataIntegrityIssuesAsync(issueIds)
                .ConfigureAwait(false);
            return Json(new
            {
                success = true,
                message = $"Fixed {fixResult.FixedIssues} out of {fixResult.TotalIssues} issues",
                details = fixResult.FixDetails
            }, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            return GetJsonResult(exception);
        }
    }

    [HttpPost]
    public async Task<ActionResult> GetLatestSummary()
    {
        try
        {
            var summary = await _healthCheckUIService.GetLatestSummaryAsync().ConfigureAwait(false);
            return Json(summary, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            return GetJsonResult(exception);
        }
    }

    #endregion
}
