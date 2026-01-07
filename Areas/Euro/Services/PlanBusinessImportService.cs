using Corno.Web.Areas.Euro.Dto.Label;
using Corno.Web.Extensions;
using Corno.Web.Globals;
using Corno.Web.Logger;
using Corno.Web.Models.Plan;
using Corno.Web.Services.File.Interfaces;
using Corno.Web.Services.Import;
using Corno.Web.Services.Import.Interfaces;
using Corno.Web.Services.Import.Models;
using Mapster;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Corno.Web.Areas.Euro.Services.Interfaces;
using Corno.Web.Models.Packing;
using Corno.Web.Windsor;

namespace Corno.Web.Areas.Euro.Services
{
    /// <summary>
    /// Business import service for Plan imports.
    /// Contains only business logic - file reading, validation, and processing.
    /// </summary>
    public class PlanBusinessImportService : IBusinessImportService<LabelImportDto>
    {
        private readonly ICsvFileService<LabelImportDto> _csvFileService;

        public PlanBusinessImportService(ICsvFileService<LabelImportDto> csvFileService)
        {
            _csvFileService = csvFileService;
        }

        public string[] SupportedExtensions => [".csv"];

        public async Task<List<LabelImportDto>> ReadFileAsync(Stream fileStream, int startRow = 0, int headerRow = 0)
        {
            var records = _csvFileService.Read(fileStream, 0, 0).ToList().Trim();
            return await Task.FromResult(records);
        }

        public async Task ValidateImportDataAsync(List<LabelImportDto> records, ImportSession session, ImportSessionService sessionService)
        {
            var errors = new List<string>();
            var rowNumber = 7; // Starting from row 7 (header is row 6)
            var hasValidationErrors = false;

            foreach (var record in records)
            {
                rowNumber++;
                var rowErrors = new List<string>();

                if (string.IsNullOrWhiteSpace(record.Barcode))
                    rowErrors.Add("Barcode is required");

                if (string.IsNullOrWhiteSpace(record.Description))
                    rowErrors.Add("Part Name is required");

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

        public async Task<List<LabelImportDto>> ProcessImportAsync(List<LabelImportDto> records, ImportSession session, ImportSessionService sessionService, string userId)
        {
            var validationErrorCount = records.Count(r => !string.IsNullOrEmpty(r.Status) && r.Status == "Error");

            var fileName = session.FileName;
            
            var planService = Bootstrapper.Get<IPlanService>();
            var labelService = Bootstrapper.Get<ILabelService>();

            // Optimize: Get all existing plans in one query
            records.ForEach(d => d.ProductionOrderNo = GetProductionOrderNo(d.Barcode));
            var groups = records.GroupBy(p => p.ProductionOrderNo).ToList();
            var productionOrderNos = groups.Select(g => g.Key).Where(wn => !string.IsNullOrWhiteSpace(wn)).ToList();
            var existingPlansList = await planService.GetAsync(p => productionOrderNos.Contains(p.ProductionOrderNo), p => p).ConfigureAwait(false);
            var existingPlansDict = existingPlansList.ToDictionary(p => p.ProductionOrderNo, p => p, StringComparer.OrdinalIgnoreCase);
            // Get existing labels by barcode for update check
            var barcodes = records.Select(r => r.Barcode).Where(b => !string.IsNullOrEmpty(b)).ToList();
            var existingList = await labelService.GetAsync(l => barcodes.Contains(l.Barcode), l => l).ConfigureAwait(false);
            var existingLabels = existingList.ToDictionary(l => l.Barcode, l => l);

            sessionService.UpdateSession(session.SessionId, s =>
            {
                s.Status = ImportStatus.Processing;
                s.CurrentStep = "Processing records";
                s.CurrentMessage = $"Processing {groups.Count} production orders...";
                s.TotalProductionOrders = groups.Count;
                s.ProgressDetails["TotalProductionOrders"] = groups.Count;
                s.ProgressDetails["ExistingPlansFound"] = existingPlansDict.Count;
                s.ProcessingSteps.Add($"Starting to process {groups.Count} production orders");
            });

            var context = new MapContext
            {
                Parameters =
                {
                    ["UserId"] = userId,
                    ["IsUpdate"] = false,
                }
            };
            ConfigureMapping(context);

            var plansToAdd = new List<Plan>();
            var plansToUpdate = new List<Plan>();
            var labelsToAdd = new List<Label>();
            var labelsToUpdate = new List<Label>();
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

                    var productionOrderNo = first.ProductionOrderNo;
                    if (string.IsNullOrWhiteSpace(productionOrderNo))
                    {
                        foreach (var item in group)
                        {
                            item.Status = "Error";
                            item.Remark = "Missing Production Order No.";
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
                            if (!string.IsNullOrEmpty(item.Status) && item.Status == "Error") continue;
                            item.Status = "Error";
                            item.Remark = "Validation error in group";
                        }
                        processedCount += group.Count();
                        continue;
                    }

                    if (existingPlansDict.TryGetValue(productionOrderNo, out var existingPlan))
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

                    // Update Labels 
                    foreach (var record in group)
                    {
                        // Check if label exists
                        if (existingLabels.TryGetValue(record.Barcode ?? "", out var existingLabel))
                        {
                            // Update existing label
                            record.Adapt(existingLabel);
                            labelsToUpdate.Add(existingLabel);
                        }
                        else
                        {
                            // Create new label
                            var newLabel = record.Adapt<Label>();

                            labelsToAdd.Add(newLabel);
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
                await planService.AddRangeAsync(plansToAdd).ConfigureAwait(false);
            }

            if (plansToUpdate.Any())
            {
                sessionService.UpdateSession(session.SessionId, s =>
                {
                    s.CurrentMessage = $"Updating {plansToUpdate.Count} existing plans...";
                });
                await planService.UpdateRangeAsync(plansToUpdate).ConfigureAwait(false);
            }

            // Labels 
            if (labelsToAdd.Any())
            {
                sessionService.UpdateSession(session.SessionId, s =>
                {
                    s.CurrentMessage = $"Adding {labelsToAdd.Count} new labels...";
                });
                await labelService.AddRangeAsync(labelsToAdd).ConfigureAwait(false);
            }

            if (labelsToUpdate.Any())
            {
                sessionService.UpdateSession(session.SessionId, s =>
                {
                    s.CurrentMessage = $"Updating {labelsToUpdate.Count} existing labels...";
                });
                await labelService.UpdateRangeAsync(labelsToUpdate).ConfigureAwait(false);
            }

            await labelService.SaveAsync().ConfigureAwait(false);
            await planService.SaveAsync().ConfigureAwait(false);

            return records;
        }

        private string GetProductionOrderNo(string barcode)
        {
            var data = barcode.Split('_');
            return data.Length > 1 ? data[1] : string.Empty;
        }

        private void ConfigureMapping(MapContext context)
        {
            var userId = context.Parameters["UserId"] as string ?? "System";
            var isUpdate = context.Parameters.ContainsKey("IsUpdate") && (bool)context.Parameters["IsUpdate"];

            TypeAdapterConfig<LabelImportDto, Plan>
                .NewConfig()
                .AfterMapping((src, dest) =>
                {
                    dest.Code = src.Project;
                    dest.PlanDate = DateTime.Now;
                    dest.Status = StatusConstants.Active;
                    dest.ModifiedBy = userId;
                    dest.ModifiedDate = DateTime.Now;

                    if (isUpdate) return;
                    dest.CreatedBy = userId;
                    dest.CreatedDate = DateTime.Now;
                });

            TypeAdapterConfig<LabelImportDto, PlanItemDetail>
                .NewConfig()
                .AfterMapping((src, dest) =>
                {
                    dest.ItemCode = src.MaterialDescription;
                    dest.Description = src.Description;
                    dest.OrderQuantity = src.Quantity ?? 0;

                    dest.Status = StatusConstants.Active;

                    dest.ModifiedBy = userId;
                    dest.ModifiedDate = DateTime.Now;

                    if (isUpdate) return;

                    dest.CreatedBy = userId;
                    dest.CreatedDate = DateTime.Now;
                });

            var serialNo = 1;
            TypeAdapterConfig<LabelImportDto, Label>
                .NewConfig()
                .AfterMapping((src, dest) =>
                {
                    dest.CompanyId = 19;
                    dest.Code = src.Project;
                    // Use RunAsync helper to avoid deadlock in Mapster callback
                    dest.SerialNo = serialNo++;
                    dest.LabelDate = DateTime.Now;
                    dest.ProductionOrderNo = GetProductionOrderNo(src.Barcode);
                    dest.ArticleNo = src.ArticleNo;
                    dest.Quantity = src.Quantity ?? 1;
                    dest.Reserved1 = src.MaterialDescription;
                    dest.Reserved2 = src.Grains;
                    dest.Links = src.CustomerReference;
                    dest.Position = src.BusinessUnit;

                    dest.CreatedBy = userId;
                    dest.CreatedDate = DateTime.Now;
                    dest.Status = StatusConstants.Active;
                });
        }

        private async Task<string> GetNextLotNoAsync(string fileName)
        {
            var planService = Bootstrapper.Get<IPlanService>();
            var lotNo = await planService.FirstOrDefaultAsync(p => p.Reserved1 == fileName,
                p => p.LotNo).ConfigureAwait(false);
            if (!string.IsNullOrEmpty(lotNo))
                return lotNo;

            var lotNos = await planService.GetAsync(p =>
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

