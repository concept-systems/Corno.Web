using System;
using System.Collections.Generic;
using System.Linq;
using Corno.Web.Areas.Kitchen.Services.Interfaces;
using Corno.Web.Extensions;
using Corno.Web.Globals;
using Corno.Web.Logger;
using Corno.Web.Models.Packing;
using Corno.Web.Models.Plan;
using Corno.Web.Services.Plan;
using Corno.Web.Services.Progress.Interfaces;
using Corno.Web.Windsor;
using Corno.Web.Services.Masters.Interfaces;
using Corno.Web.Services.File.Interfaces;
using System.Collections;
using System.Data.Entity;
using System.IO;
using Corno.Web.Areas.Kitchen.Dto.Label;
using Corno.Web.Areas.Kitchen.Dto.Plan;
using Corno.Web.Areas.Kitchen.Dto.Carton;
using Corno.Web.Models.Masters;
using Corno.Web.Services.Interfaces;
using Mapster;
using Corno.Web.Repository.Interfaces;
using Corno.Web.Areas.Kitchen.Models;
using System.Threading.Tasks;
using MoreLinq;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace Corno.Web.Areas.Kitchen.Services;

public class PlanService : BasePlanService, IPlanService
{
    #region -- Constructors --
    public PlanService(IGenericRepository<Plan> genericRepository, IExcelFileService<BmImportDto> excelFileService,
        IPlanItemDetailService planItemDetailService,
        IBaseItemService itemService, IMiscMasterService miscMasterService)
    : base(genericRepository)
    {
        _excelFileService = excelFileService;
        _planItemDetailService = planItemDetailService;
        _itemService = itemService;
        _miscMasterService = miscMasterService;

        // Initialize Shirwal Families
        _shirwalFamilies =
        [
            "FGWL05", "FGWL08", "FGWL09", "FGWL11", "FGWL15", "FGWL16",
            "FGWL17", "FGWL18", "FGWL20", "FGWL21", "FGWL24", "FGWL26",
            "FGWL27", "FGWL28", "FGWL29", "FGWL30", "FGWL32", "FGWL33",
            "FGWL34", "FGWL35", "FGWL36", "FGWL37", "FGWL38", "FGWL39",
            "FGWL40", "FGWL41", "FGWL42", "FGWL43", "FGWL44", "FGWL45",
            "FGWL46", "FGWL47", "FGWL48", "FGWL49", "FGWL50", "FGWL51",
            "FGWL52", "FGWL53", "FGWL54", "FGWL55", "FGWL56", "FGWL58",
            "FGWL61", "FGWL65", "FGWL66", "FGWL67", "FGWL68",

            "FGWN34", "FGWN35", "FGWIM1", "FGWIP1"
        ];
    }

    #endregion

    #region -- Data Members --

    private readonly IExcelFileService<BmImportDto> _excelFileService;
    private readonly IPlanItemDetailService _planItemDetailService;
    private readonly IBaseItemService _itemService;
    private readonly IMiscMasterService _miscMasterService;

    private readonly string[] _shirwalFamilies;

    #endregion

    #region -- Private Methods --
    public void ConfigureMapping(MapContext context)
    {
        var userId = context.Parameters["UserId"] as string ?? "System";
        var isUpdate = context.Parameters.ContainsKey("IsUpdate") && (bool)context.Parameters["IsUpdate"];
        
        // Get pre-loaded dictionaries for batch lookups
        var warehouseDict = context.Parameters.ContainsKey("WarehouseDict") 
            ? context.Parameters["WarehouseDict"] as Dictionary<string, int>
            : new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        
        var parentItemDict = context.Parameters.ContainsKey("ParentItemDict")
            ? context.Parameters["ParentItemDict"] as Dictionary<string, int>
            : new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        
        var baanItemDict = context.Parameters.ContainsKey("BaanItemDict")
            ? context.Parameters["BaanItemDict"] as Dictionary<string, int>
            : new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        
        TypeAdapterConfig<BmImportDto, Plan>
            .NewConfig()
            .AfterMapping((src, dest) =>
            {
                dest.Code = src.CompanyCode;
                dest.PlanDate = DateTime.Now;
                dest.System = src.OneLineItemCode;
                dest.Status = StatusConstants.InProcess;
                dest.ModifiedBy = userId;
                dest.ModifiedDate = DateTime.Now;
                
                // Use pre-loaded dictionary instead of database call
                if (!string.IsNullOrWhiteSpace(src.WarehouseCode) &&
                    warehouseDict != null &&
                    warehouseDict.TryGetValue(src.WarehouseCode.Trim(), out var warehouseId))
                {
                    dest.WarehouseId = warehouseId;
                }
                
                if (isUpdate) return;
                dest.CreatedBy = userId;
                dest.CreatedDate = DateTime.Now;
            });

        TypeAdapterConfig<BmImportDto, PlanItemDetail>
            .NewConfig()
            .AfterMapping((src, dest) =>
            {
                dest.Group = src.FamilyCode;
                dest.AssemblyCode = src.SubAssemblyCode;
                dest.OrderQuantity = src.ChildQuantity;
                dest.Remark = src.OneLineItemCode;
                dest.Reserved1 = src.Color;
                dest.Description = src.ItemName;

                dest.Status = StatusConstants.Active;
                dest.ProductLine = src.FinishedGoodItem;
                
                // Use pre-loaded dictionaries instead of database calls
                if (!string.IsNullOrWhiteSpace(src.ParentItemCode) &&
                    parentItemDict != null &&
                    parentItemDict.TryGetValue(src.ParentItemCode.Trim(), out var parentItemId))
                {
                    dest.ParentItemId = parentItemId;
                }
                
                if (!string.IsNullOrWhiteSpace(src.BaanItemCode) &&
                    baanItemDict != null &&
                    baanItemDict.TryGetValue(src.BaanItemCode.Trim(), out var baanItemId))
                {
                    dest.ItemId = baanItemId;
                }
                
                dest.ModifiedBy = userId;
                dest.ModifiedDate = DateTime.Now;

                if (isUpdate) return;

                dest.CreatedBy = userId;
                dest.CreatedDate = DateTime.Now;
            });
    }

    private async Task<string> GetNextLotNoAsync(string fileName)
    {
        var lotNo = await FirstOrDefaultAsync(p => p.Reserved1 == fileName,
            p => p.LotNo).ConfigureAwait(false);
        if (!string.IsNullOrEmpty(lotNo))
            return lotNo;

        var lotNos = await GetAsync(p =>
                DbFunctions.TruncateTime(p.CreatedDate) == DbFunctions.TruncateTime(DateTime.Now),
            p => p.LotNo).ConfigureAwait(false);
        var nextLotNo = lotNos.Max();
        if (string.IsNullOrEmpty(nextLotNo))
            return DateTime.Now.ToString("ddMMyyyy001");
        var lotNoSerialNo = nextLotNo.Substring(nextLotNo.Length - 3).ToInt() + 1;
        nextLotNo = DateTime.Now.ToString($"ddMMyyyy{lotNoSerialNo.ToString().PadLeft(3, '0')}");
        return nextLotNo;
    }
    
    #endregion

    #region -- Public Methods --

    public async Task UpdatePackQuantitiesAsync(Plan plan)
    {
        if (null == plan)
            return;

        var planItemDetails = plan.PlanItemDetails.ToList();
        //var planItemDetails = plan.PlanItemDetails
        //    .Where(d => (d.OrderQuantity ?? 0) > (d.PackQuantity ?? 0)).ToList();
        if (planItemDetails.Count <= 0 && plan.Status == StatusConstants.Packed) return;

        if (planItemDetails.Any())
        {
            var cartonService = Bootstrapper.Get<ICartonService>();
            var cartons = await cartonService.GetAsync(c => c.WarehouseOrderNo == plan.WarehouseOrderNo, c =>
                new { c.Id, c.CartonDetails }).ConfigureAwait(false);

            planItemDetails.ForEach(d => d.PackQuantity = cartons.SelectMany(c =>
                    c.CartonDetails.Where(x => x.Position == d.Position),
                (_, detail) => detail.Quantity).Sum());
        }

        var allPacked = plan.PlanItemDetails.All(d => (d.PackQuantity ?? 0) >= (d.OrderQuantity ?? 0));
        plan.PackQuantity = plan.OrderQuantity;
        plan.Status = allPacked ? StatusConstants.Packed : StatusConstants.InProcess;

        await UpdateAndSaveAsync(plan).ConfigureAwait(false);
    }

    public async Task UpdateQuantitiesAsync(Plan plan)
    {
        if (null == plan)
            throw new Exception("Invalid Plan");

        var labelService = Bootstrapper.Get<ILabelService>();
        var cartonService = Bootstrapper.Get<ICartonService>();

        // Load data
        var labels = await labelService.GetAsync(l => l.WarehouseOrderNo == plan.WarehouseOrderNo, l => l).ConfigureAwait(false);
        var cartons = await cartonService.GetAsync(c => c.WarehouseOrderNo == plan.WarehouseOrderNo, c => new { c.Id, c.CartonDetails }).ConfigureAwait(false);
        
        // Get packed barcodes for quick lookup (inline to work with anonymous types)
        var packedBarcodes = new HashSet<string>(
            cartons.SelectMany(c => c.CartonDetails.Select(cd => cd.Barcode))
                .Where(b => !string.IsNullOrEmpty(b)),
            StringComparer.OrdinalIgnoreCase
        );
        
        // Update label statuses based on packedBarcodes
        var modifiedLabels = UpdateLabelStatuses(labels, packedBarcodes);
        
        // Calculate quantities based on updated label statuses
        CalculateQuantities(plan, labels);

        if (modifiedLabels.Any())
        {
            var labelServiceTemp = Bootstrapper.Get<ILabelService>();
            await labelServiceTemp.UpdateRangeAndSaveAsync(modifiedLabels).ConfigureAwait(false);
        }

        // Save changes
        await UpdateAndSaveAsync(plan).ConfigureAwait(false);
    }

    public async Task IncreasePlanQuantityAsync(Plan plan, PlanItemDetail planItemDetail, double quantity, string newStatus)
    {
        var planDetail = plan?.PlanItemDetails.FirstOrDefault(d =>
            d.Position == planItemDetail.Position);
        if (null == planDetail)
            throw new Exception($"No plan detail available warehouse order no {plan?.WarehouseOrderNo} & " +
                                $"Item Id {planItemDetail.ItemId} & Position {planItemDetail.Position}");

        await UpdateAsync(plan).ConfigureAwait(false);
    }
    

    public async Task<IEnumerable<string>> GetFamiliesAsync(int locationId, List<PlanItemDetail> planItemDetails = null)
    {
        switch (locationId)
        {
            case 1: // Shirwal
                return await Task.FromResult(_shirwalFamilies).ConfigureAwait(false);
            case 2: // Khalapur
                if (null == planItemDetails)
                    return (await _planItemDetailService.GetAsync<string>(d => !_shirwalFamilies.Contains(d.Group),
                        d => d.Group).ConfigureAwait(false)).Distinct();
                return planItemDetails.Where(d => !_shirwalFamilies.Contains(d.Group)).Select(d => d.Group).Distinct();
            default:
                if (null == planItemDetails)
                    return (await _planItemDetailService.GetAsync<string>(null, d => d.Group).ConfigureAwait(false))
                        .Distinct();
                return planItemDetails.Select(d => d.Group).Distinct();
        }
    }

    public async Task<IEnumerable<string>> GetFamiliesAsync(int locationId, Plan plan)
    {
        switch (locationId)
        {
            case 1: // Shirwal
                return await Task.FromResult(_shirwalFamilies).ConfigureAwait(false);
            case 2: // Khalapur
                return plan.PlanItemDetails.Where(d => !_shirwalFamilies.Contains(d.Group))
                    .Select(d => d.Group).Distinct();
            default:
                return plan.PlanItemDetails.Select(d => d.Group).Distinct();
        }
    }

    public async Task<IEnumerable> GetLotNosAsync(DateTime dueDate)
    {
        var lotNos = await GetAsync(p => DbFunctions.TruncateTime(p.DueDate) == DbFunctions.TruncateTime(dueDate),
            p => new {  p.LotNo }).ConfigureAwait(false);
        return lotNos.DistinctBy(p => p.LotNo);
    }

    #region -- IFileImportService Implementation --
    
    public string[] SupportedExtensions => new[] { ".xls", ".xlsx" };

    public async Task ValidateImportDataAsync(List<BmImportDto> records, ImportSession session, ImportSessionService sessionService)
    {
        // Validation is done in the ImportAsync method itself
        await Task.CompletedTask;
    }

    #endregion

    public async Task<List<BmImportDto>> ImportAsync(Stream fileStream, string filePath, IBaseProgressService progressService,
        string userId, string sessionId, ImportSessionService sessionService)
    {
        var session = sessionService.GetSession(sessionId);
        if (session == null)
            throw new Exception("Import session not found");

        try
        {
            // Check for cancellation
            if (session.IsCancelled)
                throw new OperationCanceledException("Import was cancelled");

            var readStartTime = DateTime.Now;
            sessionService.UpdateSession(session.SessionId, s =>
            {
                s.Status = ImportStatus.Reading;
                s.CurrentStep = "Reading Excel file";
                s.CurrentMessage = "Reading Excel file...";
                s.ProcessingSteps.Add("Reading file started");
                s.ProgressDetails["FileType"] = "Excel";
                s.ProgressDetails["FileReading"] = "In progress";
            });

            // Optimized Excel reading with better error handling
            var records = _excelFileService.Read(fileStream, 0, 6)
                .ToList().Trim();
            
            var readDuration = DateTime.Now - readStartTime;
            
            if (!records.Any())
                throw new Exception("No entries found in excel file to import. Please ensure the file contains data.");

            sessionService.UpdateSession(session.SessionId, s =>
            {
                s.TotalRecords = records.Count;
                s.Status = ImportStatus.Validating;
                s.CurrentStep = "Validating data";
                s.CurrentMessage = $"Validating {records.Count} records...";
                s.ProcessingSteps.Add($"File read completed: {records.Count} records in {readDuration.TotalSeconds:F2}s");
                s.ProgressDetails["RecordsRead"] = records.Count;
                s.ProgressDetails["ReadDuration"] = readDuration.TotalSeconds;
            });

            // Validate data
            ValidateImportData(records, session, sessionService);
            
            // Count validation errors
            var validationErrorCount = records.Count(r => !string.IsNullOrEmpty(r.Status) && r.Status == "Error");

            // Check for cancellation after validation
            if (session.IsCancelled)
                throw new OperationCanceledException("Import was cancelled");

            var validateStartTime = DateTime.Now;
            var fileName = Path.GetFileName(filePath);
            var lotNo = await GetNextLotNoAsync(fileName).ConfigureAwait(false);
            var groups = records.GroupBy(p => p.WarehouseOrderNo)
                .ToList();

            var validateDuration = DateTime.Now - validateStartTime;
            sessionService.UpdateSession(session.SessionId, s =>
            {
                s.ProcessingSteps.Add($"Validation completed: {validateDuration.TotalSeconds:F2}s");
                s.ProgressDetails["ValidationDuration"] = validateDuration.TotalSeconds;
            });

            // Optimize: Get all existing plans in one query with full details
            var warehouseOrderNos = groups.Select(g => g.Key).Where(wn => !string.IsNullOrWhiteSpace(wn)).ToList();
            var existingPlansList = await GetAsync(p => warehouseOrderNos.Contains(p.WarehouseOrderNo), p => p).ConfigureAwait(false);
            var existingPlansDict = existingPlansList.ToDictionary(p => p.WarehouseOrderNo, p => p, StringComparer.OrdinalIgnoreCase);

            // Batch load all required lookups to avoid 9000+ database calls
            // Collect unique codes from all records
            var uniqueWarehouseCodes = records.Where(r => !string.IsNullOrWhiteSpace(r.WarehouseCode))
                .Select(r => r.WarehouseCode.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
            
            var uniqueParentItemCodes = records.Where(r => !string.IsNullOrWhiteSpace(r.ParentItemCode))
                .Select(r => r.ParentItemCode.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
            
            var uniqueBaanItemCodes = records.Where(r => !string.IsNullOrWhiteSpace(r.BaanItemCode))
                .Select(r => r.BaanItemCode.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            // Batch load warehouses
            var warehouses = await _miscMasterService.GetAsync(
                m => m.MiscType == MiscMasterConstants.Warehouse && uniqueWarehouseCodes.Contains(m.Code),
                m => new { m.Id, m.Code }
            ).ConfigureAwait(false);
            // Handle potential duplicates by taking the first occurrence
            var warehouseDict = warehouses
                .GroupBy(w => w.Code, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.First().Id, StringComparer.OrdinalIgnoreCase);
            
            // Batch load existing parent items
            var existingParentItems = await _itemService.GetAsync(
                i => uniqueParentItemCodes.Contains(i.Code),
                i => new { i.Id, i.Code, i.Name }
            ).ConfigureAwait(false);
            var parentItemDict = existingParentItems.ToDictionary(i => i.Code, i => i.Id, StringComparer.OrdinalIgnoreCase);

            // Batch load existing baan items
            var existingBaanItems = await _itemService.GetAsync(
                i => uniqueBaanItemCodes.Contains(i.Code),
                i => new { i.Id, i.Code, i.Name }
            ).ConfigureAwait(false);
            var baanItemDict = existingBaanItems.ToDictionary(i => i.Code, i => i.Id, StringComparer.OrdinalIgnoreCase);

            // Identify missing items that need to be created
            var missingParentItems = uniqueParentItemCodes
                .Where(code => !parentItemDict.ContainsKey(code))
                .Select(code => new { Code = code, Name = records.FirstOrDefault(r => r.ParentItemCode?.Trim() == code)?.ParentItemName ?? code })
                .ToList();
            
            var missingBaanItems = uniqueBaanItemCodes
                .Where(code => !baanItemDict.ContainsKey(code))
                .Select(code => new { Code = code, Name = records.FirstOrDefault(r => r.BaanItemCode?.Trim() == code)?.BaanItemName ?? code })
                .ToList();

            // Batch create missing parent items
            if (missingParentItems.Any())
            {
                var parentItemsToAdd = new List<Item>();
                foreach (var item in missingParentItems)
                {
                    var newItem = await _itemService.CreateObjectAsync(item.Code, item.Name).ConfigureAwait(false);
                    parentItemsToAdd.Add(newItem);
                }
                if (parentItemsToAdd.Any())
                {
                    await _itemService.AddRangeAsync(parentItemsToAdd).ConfigureAwait(false);
                    await _itemService.SaveAsync().ConfigureAwait(false);
                    
                    // Update dictionaries with newly created items
                    foreach (var item in parentItemsToAdd)
                    {
                        parentItemDict[item.Code] = item.Id;
                        //parentItemNameDict[item.Code] = item.Name;
                    }
                }
            }

            // Batch create missing baan items
            if (missingBaanItems.Any())
            {
                var baanItemsToAdd = new List<Item>();
                foreach (var item in missingBaanItems)
                {
                    var newItem = await _itemService.CreateObjectAsync(item.Code, item.Name).ConfigureAwait(false);
                    baanItemsToAdd.Add(newItem);
                }
                if (baanItemsToAdd.Any())
                {
                    await _itemService.AddRangeAsync(baanItemsToAdd).ConfigureAwait(false);
                    await _itemService.SaveAsync().ConfigureAwait(false);
                    
                    // Update dictionaries with newly created items
                    foreach (var item in baanItemsToAdd)
                    {
                        baanItemDict[item.Code] = item.Id;
                        //baanItemNameDict[item.Code] = item.Name;
                    }
                }
            }

            // Update session with processing status
            sessionService.UpdateSession(session.SessionId, s =>
            {
                s.Status = ImportStatus.Processing;
                s.CurrentStep = "Processing records";
                s.CurrentMessage = $"Processing {groups.Count} warehouse orders...";
                s.TotalWarehouseOrders = groups.Count;
                s.CurrentWarehouseOrderIndex = 0;
                s.ProgressDetails["TotalWarehouseOrders"] = groups.Count;
                s.ProgressDetails["ExistingPlansFound"] = existingPlansDict.Count;
                s.ProcessingSteps.Add($"Starting to process {groups.Count} warehouse orders");
            });

            var context = new MapContext
            {
                Parameters = 
                { 
                    ["UserId"] = userId, 
                    ["IsUpdate"] = false,
                    ["WarehouseDict"] = warehouseDict,
                    ["ParentItemDict"] = parentItemDict,
                    ["BaanItemDict"] = baanItemDict
                }
            };
            ConfigureMapping(context);

            var plansToAdd = new List<Plan>();
            var plansToUpdate = new List<Plan>();
            var processedCount = 0;
            var importedCount = 0;
            var updatedCount = 0;
            var errorCount = validationErrorCount; // Start with validation errors

            foreach (var group in groups)
            {
                // Check for cancellation
                if (session.IsCancelled)
                    throw new OperationCanceledException("Import was cancelled");

                try
                {
                    var first = group.FirstOrDefault();
                    if (null == first)
                    {
                        // Mark all items in group as skipped
                        foreach (var item in group)
                        {
                            item.Status = "Skipped";
                            item.Remark = "No valid record found in group";
                        }
                        processedCount += group.Count();
                        continue;
                    }

                    var warehouseOrderNo = first.WarehouseOrderNo;
                    if (string.IsNullOrWhiteSpace(warehouseOrderNo))
                    {
                        foreach (var item in group)
                        {
                            item.Status = "Error";
                            item.Remark = "Missing Warehouse Order No.";
                            errorCount++;
                        }
                        processedCount += group.Count();
                        continue;
                    }

                    // Check if any record in the group already has validation errors
                    var groupErrorRecords = group.Where(item => !string.IsNullOrEmpty(item.Status) && item.Status == "Error").ToList();
                    if (groupErrorRecords.Any())
                    {
                        // Skip processing this group - records already marked as errors from validation
                        // Count errors that weren't already counted (validation errors are already in errorCount via session)
                        // Just mark any records without error status
                        foreach (var item in group)
                        {
                            if (string.IsNullOrEmpty(item.Status) || item.Status != "Error")
                            {
                                item.Status = "Error";
                                item.Remark = "Validation error in group";
                            }
                        }
                        processedCount += group.Count();
                        continue;
                    }

                    // Check if plan exists - use the plan we already loaded
                    if (existingPlansDict.TryGetValue(warehouseOrderNo, out var existingPlan))
                    {
                        // Update plan properties - no need to load again, we already have it
                        first.Adapt(existingPlan);
                        existingPlan.PlanItemDetails = group.Adapt<List<PlanItemDetail>>();
                        plansToUpdate.Add(existingPlan);
                        updatedCount++;
                        
                        // Set Status and Remark for all records in the group
                        foreach (var item in group)
                        {
                            item.Status = "Updated";
                            item.Remark = "Plan updated successfully";
                        }
                    }
                    else
                    {
                        // Create new plan
                        var newPlan = first.Adapt<Plan>();
                        newPlan.LotNo = lotNo;
                        newPlan.Reserved1 = fileName;
                        newPlan.PlanItemDetails = group.Adapt<List<PlanItemDetail>>();
                        plansToAdd.Add(newPlan);
                        importedCount++;
                        
                        // Set Status and Remark for all records in the group
                        foreach (var item in group)
                        {
                            item.Status = "Imported";
                            item.Remark = "Plan imported successfully";
                        }
                    }

                    processedCount += group.Count();
                    
                    // Update session
                    sessionService.UpdateSession(session.SessionId, s =>
                    {
                        s.ProcessedRecords = processedCount;
                        s.PercentComplete = (double)processedCount / s.TotalRecords * 100;
                        s.CurrentMessage = $"Processed {processedCount} of {s.TotalRecords} records ({s.PercentComplete:F1}%)";
                        s.ImportedCount = importedCount;
                        s.UpdatedCount = updatedCount;
                        s.ErrorCount = errorCount;
                    });
                    
                    // Progress is tracked via sessionService, no need for progressService
                }
                catch (Exception exception)
                {
                    errorCount++;
                    LogHandler.LogError(exception);
                    foreach (var item in group)
                    {
                        item.Status = "Error";
                        item.Remark = $"Import failed: {exception.Message}";
                    }
                    processedCount += group.Count();
                }
            }

            // Bulk operations for better performance
            if (plansToAdd.Any())
            {
                sessionService.UpdateSession(session.SessionId, s =>
                {
                    s.CurrentMessage = $"Adding {plansToAdd.Count} new plans...";
                });
                await AddRangeAsync(plansToAdd).ConfigureAwait(false);
            }

            if (plansToUpdate.Any())
            {
                sessionService.UpdateSession(session.SessionId, s =>
                {
                    s.CurrentMessage = $"Updating {plansToUpdate.Count} existing plans...";
                });
                await UpdateRangeAsync(plansToUpdate).ConfigureAwait(false);
            }

            // Single save operation for all changes
            await SaveAsync().ConfigureAwait(false);

            sessionService.UpdateSession(session.SessionId, s =>
            {
                s.ImportedCount = importedCount;
                s.UpdatedCount = updatedCount;
                s.ErrorCount = errorCount;
                s.PercentComplete = 100;
                s.CurrentMessage = "Import completed successfully";
            });

            return records;
        }
        catch (OperationCanceledException)
        {
            sessionService.UpdateSession(sessionId, s =>
            {
                s.Status = ImportStatus.Cancelled;
                s.CurrentMessage = "Import was cancelled";
                s.EndTime = DateTime.Now;
            });
            throw;
        }
        catch (Exception exception)
        {
            exception = LogHandler.GetDetailException(exception);
            LogHandler.LogError(exception);
            sessionService.FailSession(sessionId, LogHandler.GetDetailException(exception)?.Message);
            throw;
        }
    }

    private void ValidateImportData(List<BmImportDto> records, ImportSession session, ImportSessionService sessionService)
    {
        var errors = new List<string>();
        var rowNumber = 7; // Starting from row 7 (header is row 6)
        var hasValidationErrors = false;

        foreach (var record in records)
        {
            rowNumber++;
            var rowErrors = new List<string>();

            if (string.IsNullOrWhiteSpace(record.WarehouseOrderNo))
                rowErrors.Add("Warehouse Order No. is required");

            if (string.IsNullOrWhiteSpace(record.Position))
                rowErrors.Add("Position is required");

            if (string.IsNullOrWhiteSpace(record.ItemCode))
                rowErrors.Add("Item Code is required");

            if (record.ChildQuantity == null || record.ChildQuantity <= 0)
                rowErrors.Add("Child Quantity must be greater than 0");

            if (rowErrors.Any())
            {
                hasValidationErrors = true;
                record.Status = "Error";
                record.Remark = string.Join(", ", rowErrors);
                errors.Add($"Row {rowNumber}: {string.Join(", ", rowErrors)}");
            }
        }

        if (hasValidationErrors)
        {
            // Update session with errors
            sessionService.UpdateSession(session.SessionId, s =>
            {
                s.ErrorMessages.AddRange(errors);
            });

            // Don't throw exception - allow import to continue with error records marked
            // This way users can see which records have errors in the results grid
        }
    }

    #region -- Private Helper Methods for UpdateQuantities --

    private HashSet<Label> UpdateLabelStatuses(List<Label> labels, HashSet<string> packedBarcodes)
    {
        var modifiedLabels = new HashSet<Label>();
        var currentTime = DateTime.Now;

        foreach (var label in labels)
        {
            if (string.IsNullOrEmpty(label.Barcode))
                continue;

            // Check if label has Packed status but is NOT in packedBarcodes - need to revert
            if (label.Status == StatusConstants.Packed && !packedBarcodes.Contains(label.Barcode))
            {
                RevertPackedStatus(label, currentTime);
                modifiedLabels.Add(label);
            }
            // Check if this label's barcode is in the packed barcodes set
            else if (packedBarcodes.Contains(label.Barcode))
            {
                // Only update if not already packed
                if (label.Status != StatusConstants.Packed)
                {
                    UpdateToPackedStatus(label, currentTime);
                    modifiedLabels.Add(label);
                }
                else
                {
                    // Check if Packed entry exists in LabelDetails, if not add it
                    var hasPackedDetail = label.LabelDetails?.Any(d => d.Status == StatusConstants.Packed) ?? false;
                    if (!hasPackedDetail)
                    {
                        UpdateToPackedStatus(label, currentTime);
                        modifiedLabels.Add(label);
                    }
                }
            }
        }

        return modifiedLabels;
    }

    private void RevertPackedStatus(Label label, DateTime currentTime)
    {
        // Find the previous status from LabelDetails (excluding Packed entries)
        var previousDetail = label.LabelDetails?
            .Where(d => d.Status != StatusConstants.Packed)
            .OrderByDescending(d => d.ScanDate)
            .FirstOrDefault();

        // Revert to previous status or default to Sorted
        var newStatus = previousDetail?.Status ?? StatusConstants.Sorted;
        label.Status = newStatus;
        label.ModifiedDate = currentTime;

        // Delete all Packed entries from LabelDetails
        if (label.LabelDetails != null)
        {
            label.LabelDetails.RemoveAll(d => d.Status == StatusConstants.Packed);
        }
    }

    private void UpdateToPackedStatus(Label label, DateTime currentTime)
    {
        label.Status = StatusConstants.Packed;
        label.ModifiedDate = currentTime;

        // Check if Packed entry already exists in LabelDetails
        var hasPackedDetail = label.LabelDetails?.Any(d => d.Status == StatusConstants.Packed) ?? false;
        if (!hasPackedDetail)
        {
            // Add Packed entry in LabelDetails
            var labelDetail = new LabelDetail
            {
                ScanDate = currentTime,
                Status = StatusConstants.Packed
            };
            labelDetail.UpdateCreated("System");
            labelDetail.UpdateModified("System");
            if (label.LabelDetails == null)
                label.LabelDetails = new List<LabelDetail>();
            label.LabelDetails.Add(labelDetail);
        }
    }

    private void CalculateQuantities(Plan plan, List<Label> labels)
    {
        // Group labels by position for efficient lookup
        var labelsByPosition = labels.GroupBy(l => l.Position)
            .ToDictionary(g => g.Key, g => g.ToList(), StringComparer.OrdinalIgnoreCase);

        foreach (var planItemDetail in plan.PlanItemDetails)
        {
            if (!labelsByPosition.TryGetValue(planItemDetail.Position, out var positionLabels))
            {
                ResetQuantities(planItemDetail);
                continue;
            }

            // Calculate quantities based on updated label statuses
            planItemDetail.BendQuantity = SumQuantitiesByDetailStatus(positionLabels, StatusConstants.Bent);
            planItemDetail.SortQuantity = SumQuantitiesByDetailStatus(positionLabels, StatusConstants.Sorted);
            planItemDetail.SubAssemblyQuantity = SumQuantitiesByDetailStatus(positionLabels, StatusConstants.SubAssembled);
            planItemDetail.PackQuantity = SumQuantitiesByDetailStatus(positionLabels, StatusConstants.Packed);
        }
    }

    private int SumQuantitiesByDetailStatus(List<Label> labels, string status)
    {
        return labels.Where(l => l.LabelDetails?.Any(d => d.Status == status) ?? false).Sum(l => l.Quantity ?? 0).ToInt();
    }

    private void ResetQuantities(PlanItemDetail planItemDetail)
    {
        planItemDetail.BendQuantity = 0;
        planItemDetail.SortQuantity = 0;
        planItemDetail.SubAssemblyQuantity = 0;
        planItemDetail.PackQuantity = 0;
    }

    #endregion

    #region -- View Model Methods --

    public async Task<PlanViewDto> GetViewModelAsync(Plan plan)
    {
        TypeAdapterConfig<Plan, PlanViewDto>
            .NewConfig()
            .Map(dest => dest.OneLineItemCode, src => src.System)
            .Map(dest => dest.PlanViewItemDtos, src => src.PlanItemDetails);

        TypeAdapterConfig<PlanItemDetail, PlanViewItemDto>
            .NewConfig();

        // Map Plan to DTO using Mapster
        var dto = plan.Adapt<PlanViewDto>();

        // Populate Warehouse Name from MiscMaster (optimized - only select Name)
        if (plan.WarehouseId.HasValue)
        {
            var warehouseName = await _miscMasterService.FirstOrDefaultAsync(m => m.Id == plan.WarehouseId.Value && 
                    m.MiscType == MiscMasterConstants.Warehouse, 
                m => m.Name).ConfigureAwait(false);
            dto.WarehouseName = warehouseName;
        }

        // Populate Import File Name from Reserved1
        dto.ImportFileName = plan.Reserved1;

        // Fetch label dates projection asynchronously
        var labelService = Bootstrapper.Get<ILabelService>();
        var labelDates = await labelService.GetAsync(
                p => p.WarehouseOrderNo == plan.WarehouseOrderNo && p.LabelDate.HasValue,
                p => new { p.LabelDate }).ConfigureAwait(false);
        var labelChart = labelDates
            .GroupBy(p => p.LabelDate?.Date)
            .Select(g => new PlanViewLabelChartDto
            {
                LabelDate = g.Key,
                Count = g.Count()
            })
            .ToList();

        // Fetch carton dates projection asynchronously
        var cartonService = Bootstrapper.Get<ICartonService>();
        var cartonDates = await cartonService.GetAsync(
                p => p.WarehouseOrderNo == plan.WarehouseOrderNo && p.PackingDate.HasValue,
                p => new { p.PackingDate }).ConfigureAwait(false);
        var cartonChart = cartonDates
            .GroupBy(p => p.PackingDate?.Date)
            .Select(g => new CartonViewChartDto
            {
                PackingDate = g.Key,
                Count = g.Count()
            })
            .ToList();

        dto.PlanViewChartDtos = labelChart;
        dto.CartonViewChartDtos = cartonChart;

        return dto;
    }

    public async Task<PositionDetailDto> FillLabelInformationAsync(PositionDetailDto positionView)
    {
        var labelService = Bootstrapper.Get<ILabelService>();
        var labels = await labelService.GetAsync(p => p.WarehouseOrderNo == positionView.WarehouseOrderNo &&
                                           p.Position == positionView.Position, 
                p => new
                {
                    p.Id, p.AssemblyCode, p.CarcassCode, p.LabelDate, 
                    p.Barcode, p.Status

                }).ConfigureAwait(false);

        var firstLabel = labels.FirstOrDefault();

        positionView.AssemblyCode = firstLabel?.AssemblyCode;
        positionView.CarcassCode = firstLabel?.CarcassCode;

        var cartonService = Bootstrapper.Get<ICartonService>();
        var cartons = await cartonService.GetAsync(p => p.WarehouseOrderNo == positionView.WarehouseOrderNo &&
                                             p.CartonDetails.Any(d => d.Position == positionView.Position),
                p => new { p.Id, p.PackingDate, p.CartonNo, p.CartonBarcode, p.Status }).ConfigureAwait(false);

        var positionLabels = labels.Select(x =>
                new PositionDetailDto.PositionLabelDto
                {
                Id = x.Id,
                LabelDate = x.LabelDate,
                Barcode = x.Barcode,
                Status = x.Status,
            }).ToList();
        var positionCartons = cartons.Select(x => new PositionDetailDto.PositionLabelDto
        {
            CartonNo = x.CartonNo,
        }).ToList();
        if (positionView.PositionLabelDtos == null)
        {
            positionView.PositionLabelDtos = new List<PositionDetailDto.PositionLabelDto>();
        }

        positionLabels.AddRange(positionCartons); // Merging labels and cartons
        positionView.PositionLabelDtos.AddRange(positionLabels); // Adding to final list

        return positionView;
    }

    public async Task FillCartonInformationAsync(PositionDetailDto positionView)
    {
        var cartonService = Bootstrapper.Get<ICartonService>();
        var cartons = await cartonService.GetAsync(p => p.WarehouseOrderNo == positionView.WarehouseOrderNo &&
                                             p.CartonDetails.Any(d => d.Position == positionView.Position), 
                p => new { p.Id, p.PackingDate, p.CartonNo, p.CartonBarcode, p.Status }).ConfigureAwait(false);

        var positionLabels = cartons.Select(x =>
            new PositionDetailDto.PositionCartonDto
            {
                Id = x.Id,
                PackingDate = x.PackingDate,
                CartonBarcode = x.CartonBarcode,
                CartonNo = x.CartonNo,
                Status = x.Status
            }).ToList();
        positionView.PositionCartonDtos.AddRange(positionLabels);
    }

    #endregion

    #region -- Plan Operations --

    public async Task<PlanViewDto> DeletePositionAsync(string warehouseOrderNo, string position)
    {
        if (null == position)
            throw new Exception("Position cannot be null.");

        var plan = await GetByWarehouseOrderNoAsync(warehouseOrderNo).ConfigureAwait(false);
        if (plan == null)
            throw new Exception("Warehouse Order with the specified Position not found.");

        plan.PlanItemDetails.RemoveAll(d =>
            d.Position == position);

        await UpdateAndSaveAsync(plan).ConfigureAwait(false);

        var viewModel = await GetViewModelAsync(plan).ConfigureAwait(false);

        return viewModel;
    }

    public async Task DeletePlanAsync(string warehouseOrderNo)
    {
        var labelService = Bootstrapper.Get<ILabelService>();
        var packedLabel = await labelService.FirstOrDefaultAsync(p => p.WarehouseOrderNo == warehouseOrderNo && 
                                                               p.Status == StatusConstants.Packed, p => p).ConfigureAwait(false);
        if (null != packedLabel)
            throw new Exception("Cannot delete plan. There are labels with Packed status for this warehouse order. Please unpack the labels first.");

        var deleteStatus = new List<string> { StatusConstants.Active, StatusConstants.Printed };
        var label = await labelService.FirstOrDefaultAsync(p => p.WarehouseOrderNo == warehouseOrderNo && 
                                                          !deleteStatus.Contains(p.Status), p => p).ConfigureAwait(false);
        if (null != label)
            throw new Exception("There are labels which are scanned for operations.");

        var plan = await GetByWarehouseOrderNoAsync(warehouseOrderNo).ConfigureAwait(false);
        await DeleteAsync(plan).ConfigureAwait(false);

        var labels = await labelService.GetAsync(p => p.WarehouseOrderNo == warehouseOrderNo, p => p).ConfigureAwait(false);
        await labelService.DeleteRangeAsync(labels).ConfigureAwait(false);

        await SaveAsync().ConfigureAwait(false);
    }

    public async Task UpdateDueDateAsync(string warehouseOrderNo, DateTime dueDate)
    {
        if (string.IsNullOrEmpty(warehouseOrderNo))
            throw new Exception("Warehouse Order No is required");

        var plan = await FirstOrDefaultAsync(p => p.WarehouseOrderNo == warehouseOrderNo,
            p => p).ConfigureAwait(false);
        if (null == plan)
            throw new Exception($"Plan not found for warehouse order {warehouseOrderNo}");

        plan.DueDate = dueDate;
        await UpdateAndSaveAsync(plan).ConfigureAwait(false);
    }

    public async Task UpdateLotNoAsync(string warehouseOrderNo, string lotNo)
    {
        if (string.IsNullOrEmpty(warehouseOrderNo))
            throw new Exception("Warehouse Order No is required");

        if (string.IsNullOrEmpty(lotNo))
            throw new Exception("Lot No is required");

        var plan = await GetByWarehouseOrderNoAsync(warehouseOrderNo).ConfigureAwait(false);
        if (plan == null)
            throw new Exception($"Plan not found for warehouse order {warehouseOrderNo}");

        plan.LotNo = lotNo;
        await UpdateAndSaveAsync(plan).ConfigureAwait(false);
    }

    #endregion

    #region -- Data Retrieval Methods --

    public async Task<DataSourceResult> GetIndexViewDtoAsync(DataSourceRequest request)
    {
        // Step 1: Get paged Plans (only IDs and basic fields)
        var baseQuery = GetQuery();
        var pagedPlans = await baseQuery.ToDataSourceResultAsync(request).ConfigureAwait(false);

        // Step 2: Get aggregates for those IDs
        var planIds = (pagedPlans.Data as List<Plan>)?.Select(x => x.Id).ToList();
        var result = await GetAsync(p => planIds.Contains(p.Id), p => new PlanIndexDto
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

        pagedPlans.Data = result;
        return pagedPlans;
    }

    public async Task<DataSourceResult> GetLabelsForPlanAsync(DataSourceRequest request, string warehouseOrderNo)
    {
        if (string.IsNullOrEmpty(warehouseOrderNo))
            return new DataSourceResult { Errors = new Dictionary<string, object>() };

        var labelService = Bootstrapper.Get<ILabelService>();
        var query = await labelService.GetAsync(c => c.WarehouseOrderNo == warehouseOrderNo, p => p,
            null, true);

        var data = from label in query
            select new LabelIndexDto
            {
                Id = label.Id,
                LabelDate = label.LabelDate,
                WarehouseOrderNo = label.WarehouseOrderNo,
                CarcassCode = label.CarcassCode,
                AssemblyCode = label.AssemblyCode,
                Position = label.Position,
                Barcode = label.Barcode,
                LotNo = label.LotNo,
                Family = label.Reserved1,
                OrderQuantity = label.OrderQuantity,
                Quantity = label.Quantity,
                Status = label.Status
            };

        var result = await data.ToDataSourceResultAsync(request).ConfigureAwait(false);
        return result;
    }

    public async Task<DataSourceResult> GetCartonsForPlanAsync(DataSourceRequest request, string warehouseOrderNo)
    {
        if (string.IsNullOrEmpty(warehouseOrderNo))
            return new DataSourceResult { Errors = new Dictionary<string, object>() };

        var cartonService = Bootstrapper.Get<ICartonService>();
        var query = await cartonService.GetAsync(c => c.WarehouseOrderNo == warehouseOrderNo, p => p,
            null, true).ConfigureAwait(false);

        var data = from carton in query
            select new CartonIndexDto
            {
                Id = carton.Id,
                PackingDate = carton.PackingDate,
                SoNo = carton.SoNo,
                WarehouseOrderNo = carton.WarehouseOrderNo,
                CartonNo = carton.CartonNo.ToString(),
                CartonBarcode = carton.CartonBarcode,
                Status = carton.Status
            };

        var result = await data.ToDataSourceResultAsync(request).ConfigureAwait(false);
        return result;
    }

    #endregion

    #region -- Pending Plan Methods --

    public async Task<IEnumerable> GetPendingLotNosAsync(DateTime dueDate)
    {
        var lotNos = await GetAsync<object>(p => DbFunctions.TruncateTime(p.DueDate) ==
                                          DbFunctions.TruncateTime(dueDate) &&
                                          p.PlanItemDetails.Any(d => d.ItemType.Equals(FieldConstants.Bo) && 
                                                                     (d.OrderQuantity ?? 0) > (d.PrintQuantity ?? 0)),
                p => new { p.LotNo }).ConfigureAwait(false);
        return lotNos.Distinct().ToList();
    }

    public async Task<IEnumerable> GetPendingWarehouseOrdersAsync(string lotNo)
    {
        var warehouseOrderNos = await GetAsync<object>(p => p.LotNo.Equals(lotNo) &&
                                           p.PlanItemDetails.Any(d => d.ItemType.Equals(FieldConstants.Bo) && 
                                                                      (d.OrderQuantity ?? 0) > (d.PrintQuantity ?? 0)),
                p => new { p.WarehouseOrderNo }).ConfigureAwait(false);
        return warehouseOrderNos.Distinct().ToList();
    }

    public async Task<IEnumerable> GetPendingFamiliesAsync(Plan plan, List<string> selectedGroups = null)
    {
        var query = plan.PlanItemDetails.Where(d => d.ItemType.Equals(FieldConstants.Bo) && 
                                                       (d.OrderQuantity ?? 0) > (d.PrintQuantity ?? 0));
        
        // Apply selectedGroups filter if provided (for trolley labels)
        if (selectedGroups != null && selectedGroups.Any())
        {
            query = query.Where(d => selectedGroups.Contains(d.Group));
        }
        
        var families = query
            .Select(p => new { Family = p.Group })
            .Distinct()
            .ToList();
        
        return await Task.FromResult(families).ConfigureAwait(false);
    }

    #endregion
}


#endregion






