using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Corno.Web.Areas.Nilkamal.Dto.Plan;
using Corno.Web.Areas.Nilkamal.Services.Interfaces;
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
using Corno.Web.Windsor;
using Mapster;

namespace Corno.Web.Areas.Nilkamal.Services
{
    /// <summary>
    /// Business import service for Nilkamal Plan imports.
    /// Contains only business logic - file reading, validation, and processing.
    /// </summary>
    public class PlanBusinessImportService : IBusinessImportService<PlanImportDto>
    {
        private readonly IExcelFileService<PlanImportDto> _excelFileService;
        private readonly IPlanService _planService;
        private readonly IPlanItemDetailService _planItemDetailService;
        private readonly IBaseItemService _itemService;
        private readonly IMiscMasterService _miscMasterService;
        private readonly IProductService _productService;

        public PlanBusinessImportService(
            IExcelFileService<PlanImportDto> excelFileService,
            IPlanService planService,
            IPlanItemDetailService planItemDetailService,
            IBaseItemService itemService,
            IMiscMasterService miscMasterService,
            IProductService productService)
        {
            _excelFileService = excelFileService;
            _planService = planService;
            _planItemDetailService = planItemDetailService;
            _itemService = itemService;
            _miscMasterService = miscMasterService;
            _productService = productService;
        }

        public string[] SupportedExtensions => new[] { ".xls", ".xlsx" };

        public async Task<List<PlanImportDto>> ReadFileAsync(Stream fileStream, int startRow = 0, int headerRow = 0)
        {
            var records = _excelFileService.Read(fileStream, 0, 0).ToList().Trim();
            return await Task.FromResult(records);
        }

        public async Task ValidateImportDataAsync(List<PlanImportDto> records, ImportSession session, ImportSessionService sessionService)
        {
            var errors = new List<string>();
            var rowNumber = 1; // Starting from row 1 (header is row 0)
            var hasValidationErrors = false;

            foreach (var record in records)
            {
                rowNumber++;
                var rowErrors = new List<string>();

                if (string.IsNullOrWhiteSpace(record.ProductionOrderNo))
                    rowErrors.Add("Production Order No. is required");

                if (string.IsNullOrWhiteSpace(record.ProductCode))
                    rowErrors.Add("Product Code is required");

                if (record.PlanQty == null || record.PlanQty <= 0)
                    rowErrors.Add("Plan QTY must be greater than 0");

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

        public async Task<List<PlanImportDto>> ProcessImportAsync(List<PlanImportDto> records, ImportSession session, ImportSessionService sessionService, string userId)
        {
            var validationErrorCount = records.Count(r => !string.IsNullOrEmpty(r.Status) && r.Status == "Error");

            var fileName = session.FileName;
            var groups = records.GroupBy(p => p.ProductionOrderNo).ToList();

            // Optimize: Get all existing plans in one query
            var productionOrderNos = groups.Select(g => g.Key).Where(wn => !string.IsNullOrWhiteSpace(wn)).ToList();
            var existingPlansList = await _planService.GetAsync(p => productionOrderNos.Contains(p.ProductionOrderNo), p => p).ConfigureAwait(false);
            var existingPlansDict = existingPlansList.ToDictionary(p => p.ProductionOrderNo, p => p, StringComparer.OrdinalIgnoreCase);

            // Batch load all required lookups
            var uniqueProductCodes = records.Where(r => !string.IsNullOrWhiteSpace(r.ProductCode))
                .Select(r => r.ProductCode.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            // Batch load existing products
            var existingProducts = await _productService.GetAsync(
                p => uniqueProductCodes.Contains(p.Code),
                p => new { p.Id, p.Code, p.Name, p.ProductItemDetails }
            ).ConfigureAwait(false);
            var productDict = existingProducts.ToDictionary(p => p.Code, p => p, StringComparer.OrdinalIgnoreCase);

            sessionService.UpdateSession(session.SessionId, s =>
            {
                s.Status = ImportStatus.Processing;
                s.CurrentStep = "Processing records";
                s.CurrentMessage = $"Processing {groups.Count} production orders...";
                s.TotalWarehouseOrders = groups.Count;
                s.ProgressDetails["TotalWarehouseOrders"] = groups.Count;
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
                            if (string.IsNullOrEmpty(item.Status) || item.Status != "Error")
                            {
                                item.Status = "Error";
                                item.Remark = "Validation error in group";
                            }
                        }
                        processedCount += group.Count();
                        continue;
                    }

                    // Check if product exists
                    if (!productDict.TryGetValue(first.ProductCode?.Trim() ?? "", out var productInfo))
                    {
                        foreach (var item in group)
                        {
                            item.Status = "Error";
                            item.Remark = $"Product with code {first.ProductCode} not available in system.";
                            errorCount++;
                        }
                        processedCount += group.Count();
                        continue;
                    }

                    if (existingPlansDict.TryGetValue(productionOrderNo, out var existingPlan))
                    {
                        // Update existing plan
                        existingPlan.SoNo = first.SoNo;
                        existingPlan.DueDate = first.DueDate ?? DateTime.Now.AddDays(7);
                        existingPlan.OrderQuantity = first.PlanQty;
                        existingPlan.ProductId = productInfo.Id;
                        existingPlan.ModifiedBy = userId;
                        existingPlan.ModifiedDate = DateTime.Now;

                        // Update plan item details from product
                        existingPlan.PlanItemDetails.Clear();
                        foreach (var productDetail in productInfo.ProductItemDetails)
                        {
                            var planItemDetail = new PlanItemDetail
                            {
                                ProductId = productInfo.Id,
                                ItemId = productDetail.ItemId,
                                PackingTypeId = productDetail.PackingTypeId,
                                BomQuantity = productDetail.Quantity.ToInt(),
                                OrderQuantity = first.PlanQty.ToInt() * productDetail.Quantity.ToInt(),
                                Status = StatusConstants.Active,
                                CreatedBy = userId,
                                CreatedDate = DateTime.Now,
                                ModifiedBy = userId,
                                ModifiedDate = DateTime.Now
                            };
                            existingPlan.PlanItemDetails.Add(planItemDetail);
                        }

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
                        // Create new plan
                        var newPlan = new Plan
                        {
                            PlanDate = DateTime.Now,
                            ProductionOrderNo = productionOrderNo,
                            SoNo = first.SoNo,
                            ProductId = productInfo.Id,
                            DueDate = first.DueDate ?? DateTime.Now.AddDays(7),
                            OrderQuantity = first.PlanQty,
                            Status = StatusConstants.InProcess,
                            CreatedBy = userId,
                            CreatedDate = DateTime.Now,
                            ModifiedBy = userId,
                            ModifiedDate = DateTime.Now
                        };

                        // Add plan item details from product
                        foreach (var productDetail in productInfo.ProductItemDetails)
                        {
                            var planItemDetail = new PlanItemDetail
                            {
                                ProductId = productInfo.Id,
                                ItemId = productDetail.ItemId,
                                PackingTypeId = productDetail.PackingTypeId,
                                BomQuantity = productDetail.Quantity.ToInt(),
                                OrderQuantity = first.PlanQty.ToInt() * productDetail.Quantity.ToInt(),
                                Status = StatusConstants.Active,
                                CreatedBy = userId,
                                CreatedDate = DateTime.Now,
                                ModifiedBy = userId,
                                ModifiedDate = DateTime.Now
                            };
                            newPlan.PlanItemDetails.Add(planItemDetail);
                        }

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

            TypeAdapterConfig<PlanImportDto, Plan>
                .NewConfig()
                .AfterMapping((src, dest) =>
                {
                    dest.PlanDate = DateTime.Now;
                    dest.DueDate = src.DueDate ?? DateTime.Now.AddDays(7);
                    dest.SoNo = src.SoNo;
                    dest.OrderQuantity = src.PlanQty;
                    dest.Status = StatusConstants.InProcess;
                    dest.ModifiedBy = userId;
                    dest.ModifiedDate = DateTime.Now;

                    if (isUpdate) return;
                    dest.CreatedBy = userId;
                    dest.CreatedDate = DateTime.Now;
                });
        }
    }
}

