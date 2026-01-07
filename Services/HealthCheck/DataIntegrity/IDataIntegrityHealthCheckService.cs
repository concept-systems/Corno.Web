using System.Collections.Generic;
using System.Threading.Tasks;
using Corno.Web.Services.HealthCheck.Interfaces;

namespace Corno.Web.Services.HealthCheck.DataIntegrity;

public class DataIntegrityIssue
{
    public string EntityType { get; set; } // Plan, Label, Carton
    public int? EntityId { get; set; }
    public string Identifier { get; set; } // WarehouseOrderNo, Barcode, etc.
    public string IssueType { get; set; }
    public string Description { get; set; }
    public string ExpectedValue { get; set; }
    public string ActualValue { get; set; }
    public bool CanAutoFix { get; set; }
}

public class DataIntegrityFixResult
{
    public int TotalIssues { get; set; }
    public int FixedIssues { get; set; }
    public int FailedFixes { get; set; }
    public List<string> FixDetails { get; set; } = new List<string>();
}

public interface IDataIntegrityHealthCheckService
{
    Task<List<HealthCheckResult>> RunAllDataIntegrityChecksAsync();
    Task<HealthCheckResult> CheckPlanQuantitiesAsync();
    Task<HealthCheckResult> CheckCartonBarcodeConsistencyAsync();
    Task<HealthCheckResult> CheckLabelDetailSequenceAsync();
    Task<HealthCheckResult> CheckPlanPackQuantityFromCartonsAsync();
    Task<List<HealthCheckResult>> CheckDataIntegrityForWarehouseOrderNoAsync(string warehouseOrderNo);
    Task<DataIntegrityFixResult> AutoFixDataIntegrityIssuesAsync(List<DataIntegrityIssue> issues);
}

