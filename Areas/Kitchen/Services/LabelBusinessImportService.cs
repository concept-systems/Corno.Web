using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Corno.Web.Areas.Kitchen.Dto.Label;
using Corno.Web.Areas.Kitchen.Services.Interfaces;
using Corno.Web.Extensions;
using Corno.Web.Globals;
using Corno.Web.Logger;
using Corno.Web.Models.Packing;
using Corno.Web.Models.Plan;
using Corno.Web.Services.File.Interfaces;
using Corno.Web.Services.Import;
using Corno.Web.Services.Import.Interfaces;
using Corno.Web.Services.Import.Models;
using Corno.Web.Windsor;
using Mapster;

namespace Corno.Web.Areas.Kitchen.Services
{
    /// <summary>
    /// Business import service for Label imports (Sorting/Kitting).
    /// Contains only business logic - file reading, validation, and processing.
    /// </summary>
    public class LabelBusinessImportService : IBusinessImportService<SalvaginiExcelDto>
    {
        private readonly IExcelFileService<SalvaginiExcelDto> _excelFileService;
        private readonly ILabelService _labelService;

        public LabelBusinessImportService(
            IExcelFileService<SalvaginiExcelDto> excelFileService,
            ILabelService labelService)
        {
            _excelFileService = excelFileService;
            _labelService = labelService;
        }

        public string[] SupportedExtensions => new[] { ".xls", ".xlsx" };

        public async Task<List<SalvaginiExcelDto>> ReadFileAsync(Stream fileStream, int startRow = 0, int headerRow = 0)
        {
            var records = _excelFileService.Read(fileStream).ToList().Trim();
            return await Task.FromResult(records);
        }

        public async Task ValidateImportDataAsync(List<SalvaginiExcelDto> records, ImportSession session, ImportSessionService sessionService)
        {
            var errors = new List<string>();
            var rowNumber = 0;
            var hasValidationErrors = false;

            foreach (var record in records)
            {
                rowNumber++;
                var rowErrors = new List<string>();

                if (string.IsNullOrWhiteSpace(record.Barcode))
                    rowErrors.Add("Barcode is required");

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

        public async Task<List<SalvaginiExcelDto>> ProcessImportAsync(List<SalvaginiExcelDto> records, ImportSession session, ImportSessionService sessionService, string userId)
        {
            // Get newStatus from session context (set by controller)
            var importType = session.ProgressDetails.TryGetValue("NewStatus", out var detail) 
                ? detail?.ToString() 
                : null;

            if (string.IsNullOrWhiteSpace(importType))
                throw new Exception("NewStatus is required for label import. Must be 'Kitting' or 'Sorting'.");

            // Map import type to actual status
            string newStatus;
            List<string> allowedOldStatuses;
            
            if (importType.Equals("Kitting", StringComparison.OrdinalIgnoreCase))
            {
                newStatus = StatusConstants.Bent;
                allowedOldStatuses = [StatusConstants.Active, StatusConstants.Printed];
            }
            else if (importType.Equals("Sorting", StringComparison.OrdinalIgnoreCase))
            {
                newStatus = StatusConstants.Sorted;
                allowedOldStatuses = [StatusConstants.Bent];
            }
            else
            {
                throw new Exception($"Invalid import type '{importType}'. Only 'Kitting' and 'Sorting' are supported.");
            }

            var validationErrorCount = records.Count(r => !string.IsNullOrEmpty(r.Status) && r.Status == "Error");

            sessionService.UpdateSession(session.SessionId, s =>
            {
                s.Status = ImportStatus.Processing;
                s.CurrentStep = "Processing records";
                s.CurrentMessage = $"Processing {records.Count} records for {importType}...";
                s.ProgressDetails["NewStatus"] = newStatus;
                s.ProgressDetails["ImportType"] = importType;
            });

            var barcodes = records.Select(r => r.Barcode).Where(d => !string.IsNullOrEmpty(d)).Distinct().ToList();
            if (!barcodes.Any())
                throw new Exception("All barcodes are null. Check excel file.");

            // Get all labels for the barcodes, ordered by LabelDate descending to get the most recent
            var labels = await _labelService.GetAsync(l => barcodes.Contains(l.Barcode), l => l,
                q => q.OrderByDescending(x => x.LabelDate ?? x.CreatedDate)).ConfigureAwait(false);
            
            // Group by barcode and get the most recent label (first after ordering by LabelDate)
            labels = labels.GroupBy(l => l.Barcode)
                .Select(g => g.FirstOrDefault()) // First is the most recent due to OrderByDescending
                .Where(l => l != null)
                .ToList();

            var warehouseOrderNos = labels.Select(l => l.WarehouseOrderNo).Distinct().ToList();
            var planService = Bootstrapper.Get<IPlanService>();
            var plans = (await planService.GetAsync(p => warehouseOrderNos.Contains(p.WarehouseOrderNo), p => p).ConfigureAwait(false)).ToList();

            var updatedLabels = new List<Label>();
            var updatedPlans = new List<Plan>();
            var processedCount = 0;
            var importedCount = 0;
            var errorCount = validationErrorCount;
            var skippedCount = 0;

            foreach (var record in records)
            {
                if (session.IsCancelled)
                    throw new OperationCanceledException("Import was cancelled");

                try
                {
                    // Get the most recent label for this barcode (already ordered by LabelDate in query)
                    var label = labels.FirstOrDefault(l => l.Barcode == record.Barcode);
                    if (null == label)
                    {
                        record.Status = StatusConstants.Exists;
                        record.Remark = $"Label not found for barcode {record.Barcode}.";
                        skippedCount++;
                        processedCount++;
                        continue;
                    }

                    // Validate old status based on import type
                    if (!allowedOldStatuses.Contains(label.Status))
                    {
                        record.Status = "Error";
                        record.Remark = $"Invalid label status '{label.Status}' for order {label.WarehouseOrderNo}. Expected one of: {string.Join(", ", allowedOldStatuses)}.";
                        errorCount++;
                        processedCount++;
                        continue;
                    }

                    var plan = plans.FirstOrDefault(p => p.WarehouseOrderNo == label.WarehouseOrderNo);
                    if (null == plan)
                    {
                        record.Status = StatusConstants.Ignore;
                        record.Remark = $"Plan with warehouse order {label.WarehouseOrderNo} not found.";
                        skippedCount++;
                        processedCount++;
                        continue;
                    }

                    if (label.Status == newStatus)
                    {
                        record.Status = StatusConstants.Exists;
                        record.Remark = $"Label already has status '{newStatus}'.";
                        skippedCount++;
                        processedCount++;
                        continue;
                    }

                    var planDetail = plan.PlanItemDetails.FirstOrDefault(d => d.Position == label.Position);
                    if (null == planDetail)
                    {
                        record.Status = "Error";
                        record.Remark = $"No plan detail available warehouse order no {plan?.WarehouseOrderNo} & Item Id {label.ItemId} & Position {label.Position}";
                        errorCount++;
                        processedCount++;
                        continue;
                    }

                    // Update plan quantity based on import type (Kitting or Sorting)
                    if (importType.Equals("Kitting", StringComparison.OrdinalIgnoreCase))
                    {
                        // For Kitting: Update BendQuantity
                        planDetail.BendQuantity ??= 0;
                        planDetail.BendQuantity += label.Quantity ?? 0;
                    }
                    else if (importType.Equals("Sorting", StringComparison.OrdinalIgnoreCase))
                    {
                        // For Sorting: Update SortQuantity
                        planDetail.SortQuantity ??= 0;
                        planDetail.SortQuantity += label.Quantity ?? 0;
                    }

                    if (!updatedPlans.Contains(plan))
                    {
                        updatedPlans.Add(plan);
                    }

                    // Update label status and add detail
                    label.Status = newStatus;
                    label.AddDetail(null, null, null, null, newStatus, null);
                    updatedLabels.Add(label);

                    record.Status = StatusConstants.Imported;
                    importedCount++;
                    processedCount++;
                }
                catch (Exception exception)
                {
                    errorCount++;
                    LogHandler.LogError(exception);
                    record.Status = StatusConstants.Ignore;
                    record.Remark = LogHandler.GetDetailException(exception)?.Message;
                    processedCount++;
                }

                sessionService.UpdateSession(session.SessionId, s =>
                {
                    s.ProcessedRecords = processedCount;
                    s.PercentComplete = (double)processedCount / s.TotalRecords * 100;
                    s.CurrentMessage = $"Processed {processedCount} of {s.TotalRecords} records ({s.PercentComplete:F1}%)";
                    s.ImportedCount = importedCount;
                    s.ErrorCount = errorCount;
                    s.SkippedCount = skippedCount;
                });
            }

            if (updatedPlans.Any())
            {
                sessionService.UpdateSession(session.SessionId, s =>
                {
                    s.CurrentMessage = $"Saving {updatedPlans.Count} plans and {updatedLabels.Count} labels...";
                });
                await planService.UpdateRangeAndSaveAsync(updatedPlans).ConfigureAwait(false);
            }

            if (updatedLabels.Any())
            {
                await _labelService.UpdateRangeAndSaveAsync(updatedLabels).ConfigureAwait(false);
            }

            return records;
        }
    }
}

