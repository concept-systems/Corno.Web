using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Corno.Web.Areas.Kitchen.Dto.Label;
using Corno.Web.Areas.Kitchen.Services.Interfaces;
using Corno.Web.Extensions;
using Corno.Web.Globals;
using Corno.Web.Logger;
using Corno.Web.Models.Masters;
using Corno.Web.Models.Plan;
using Corno.Web.Services.File.Interfaces;
using Corno.Web.Services.Import;
using Corno.Web.Services.Import.Interfaces;
using Corno.Web.Services.Import.Models;
using Corno.Web.Services.Interfaces;
using Corno.Web.Services.Masters.Interfaces;
using Mapster;

namespace Corno.Web.Areas.Kitchen.Services
{
    /// <summary>
    /// Business import service for Plan imports.
    /// Contains only business logic - file reading, validation, and processing.
    /// </summary>
    public class PlanBusinessImportService : IBusinessImportService<BmImportDto>
    {
        private readonly IExcelFileService<BmImportDto> _excelFileService;
        private readonly IPlanService _planService;
        private readonly IPlanItemDetailService _planItemDetailService;
        private readonly IBaseItemService _itemService;
        private readonly IMiscMasterService _miscMasterService;

        public PlanBusinessImportService(
            IExcelFileService<BmImportDto> excelFileService,
            IPlanService planService,
            IPlanItemDetailService planItemDetailService,
            IBaseItemService itemService,
            IMiscMasterService miscMasterService)
        {
            _excelFileService = excelFileService;
            _planService = planService;
            _planItemDetailService = planItemDetailService;
            _itemService = itemService;
            _miscMasterService = miscMasterService;
        }

        public string[] SupportedExtensions => new[] { ".xls", ".xlsx" };

        public async Task<List<BmImportDto>> ReadFileAsync(Stream fileStream, int startRow = 0, int headerRow = 0)
        {
            var records = _excelFileService.Read(fileStream, 0, 6).ToList().Trim();
            return await Task.FromResult(records);
        }

        public async Task ValidateImportDataAsync(List<BmImportDto> records, ImportSession session, ImportSessionService sessionService)
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
                sessionService.UpdateSession(session.SessionId, s =>
                {
                    s.ErrorMessages.AddRange(errors);
                });
            }

            await Task.CompletedTask;
        }

        public async Task<List<BmImportDto>> ProcessImportAsync(List<BmImportDto> records, ImportSession session, ImportSessionService sessionService, string userId)
        {
            var validationErrorCount = records.Count(r => !string.IsNullOrEmpty(r.Status) && r.Status == "Error");

            var fileName = session.FileName;
            var lotNo = await GetNextLotNoAsync(fileName);
            var groups = records.GroupBy(p => p.WarehouseOrderNo).ToList();

            // Optimize: Get all existing plans in one query
            var warehouseOrderNos = groups.Select(g => g.Key).Where(wn => !string.IsNullOrWhiteSpace(wn)).ToList();
            var existingPlansList = await _planService.GetAsync(p => warehouseOrderNos.Contains(p.WarehouseOrderNo), p => p).ConfigureAwait(false);
            var existingPlansDict = existingPlansList.ToDictionary(p => p.WarehouseOrderNo, p => p, StringComparer.OrdinalIgnoreCase);

            // Batch load all required lookups
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

            // Create missing items
            var missingParentItems = uniqueParentItemCodes
                .Where(code => !parentItemDict.ContainsKey(code))
                .Select(code => new { Code = code, Name = records.FirstOrDefault(r => r.ParentItemCode?.Trim() == code)?.ParentItemName ?? code })
                .ToList();

            var missingBaanItems = uniqueBaanItemCodes
                .Where(code => !baanItemDict.ContainsKey(code))
                .Select(code => new { Code = code, Name = records.FirstOrDefault(r => r.BaanItemCode?.Trim() == code)?.BaanItemName ?? code })
                .ToList();

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
                    foreach (var item in parentItemsToAdd)
                    {
                        parentItemDict[item.Code] = item.Id;
                    }
                }
            }

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
                    foreach (var item in baanItemsToAdd)
                    {
                        baanItemDict[item.Code] = item.Id;
                    }
                }
            }

            sessionService.UpdateSession(session.SessionId, s =>
            {
                s.Status = ImportStatus.Processing;
                s.CurrentStep = "Processing records";
                s.CurrentMessage = $"Processing {groups.Count} warehouse orders...";
                s.TotalWarehouseOrders = groups.Count;
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
            var errorCount = validationErrorCount;

            foreach (var group in groups)
            {
                if (session.IsCancelled)
                    throw new OperationCanceledException("Import was cancelled");

                try
                {
                    var first = group.FirstOrDefault();
                    if (null == first)
                    {
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

                    var groupErrorRecords = group.Where(item => !string.IsNullOrEmpty(item.Status) && item.Status == "Error").ToList();
                    if (groupErrorRecords.Any())
                    {
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

                    if (existingPlansDict.TryGetValue(warehouseOrderNo, out var existingPlan))
                    {
                        first.Adapt(existingPlan);
                        existingPlan.PlanItemDetails = group.Adapt<List<PlanItemDetail>>();
                        plansToUpdate.Add(existingPlan);
                        updatedCount++;

                        foreach (var item in group)
                        {
                            item.Status = "Updated";
                            item.Remark = "Plan updated successfully";
                        }
                    }
                    else
                    {
                        var newPlan = first.Adapt<Plan>();
                        newPlan.LotNo = lotNo;
                        newPlan.Reserved1 = fileName;
                        newPlan.PlanItemDetails = group.Adapt<List<PlanItemDetail>>();
                        plansToAdd.Add(newPlan);
                        importedCount++;

                        foreach (var item in group)
                        {
                            item.Status = "Imported";
                            item.Remark = "Plan imported successfully";
                        }
                    }

                    processedCount += group.Count();

                    sessionService.UpdateSession(session.SessionId, s =>
                    {
                        s.ProcessedRecords = processedCount;
                        s.PercentComplete = (double)processedCount / s.TotalRecords * 100;
                        s.CurrentMessage = $"Processed {processedCount} of {s.TotalRecords} records ({s.PercentComplete:F1}%)";
                        s.ImportedCount = importedCount;
                        s.UpdatedCount = updatedCount;
                        s.ErrorCount = errorCount;
                    });
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

            if (plansToAdd.Any())
            {
                sessionService.UpdateSession(session.SessionId, s =>
                {
                    s.CurrentMessage = $"Adding {plansToAdd.Count} new plans...";
                });
                await _planService.AddRangeAsync(plansToAdd).ConfigureAwait(false);
            }

            if (plansToUpdate.Any())
            {
                sessionService.UpdateSession(session.SessionId, s =>
                {
                    s.CurrentMessage = $"Updating {plansToUpdate.Count} existing plans...";
                });
                await _planService.UpdateRangeAsync(plansToUpdate).ConfigureAwait(false);
            }

            await _planService.SaveAsync().ConfigureAwait(false);

            return records;
        }

        private void ConfigureMapping(MapContext context)
        {
            var userId = context.Parameters["UserId"] as string ?? "System";
            var isUpdate = context.Parameters.ContainsKey("IsUpdate") && (bool)context.Parameters["IsUpdate"];

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
            var lotNo = await _planService.FirstOrDefaultAsync(p => p.Reserved1 == fileName,
                p => p.LotNo).ConfigureAwait(false);
            if (!string.IsNullOrEmpty(lotNo))
                return lotNo;

            var lotNos = await _planService.GetAsync(p =>
                    DbFunctions.TruncateTime(p.CreatedDate) == DbFunctions.TruncateTime(DateTime.Now),
                p => p.LotNo).ConfigureAwait(false);
            var nextLotNo = lotNos.Max();
            if (string.IsNullOrEmpty(nextLotNo))
                return DateTime.Now.ToString("ddMMyyyy001");
            var lotNoSerialNo = nextLotNo.Substring(nextLotNo.Length - 3).ToInt() + 1;
            nextLotNo = DateTime.Now.ToString($"ddMMyyyy{lotNoSerialNo.ToString().PadLeft(3, '0')}");
            return nextLotNo;
        }
    }
}

