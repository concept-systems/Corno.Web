using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Corno.Web.Areas.Masters.Dtos.Product;
using Corno.Web.Extensions;
using Corno.Web.Globals;
using Corno.Web.Logger;
using Corno.Web.Models.Masters;
using Corno.Web.Services.File.Interfaces;
using Corno.Web.Services.Import;
using Corno.Web.Services.Import.Interfaces;
using Corno.Web.Services.Import.Models;
using Corno.Web.Services.Interfaces;
using Corno.Web.Services.Masters.Interfaces;
using Mapster;

namespace Corno.Web.Areas.Masters.Services
{
    /// <summary>
    /// Business import service for Product (BOM) imports.
    /// Contains only business logic - file reading, validation, and processing.
    /// </summary>
    public class ProductBusinessImportService : IBusinessImportService<BomImportDto>
    {
        private readonly IExcelFileService<BomImportDto> _excelFileService;
        private readonly IProductService _productService;
        private readonly IBaseItemService _itemService;
        private readonly IMiscMasterService _miscMasterService;

        public ProductBusinessImportService(
            IExcelFileService<BomImportDto> excelFileService,
            IProductService productService,
            IBaseItemService itemService,
            IMiscMasterService miscMasterService)
        {
            _excelFileService = excelFileService;
            _productService = productService;
            _itemService = itemService;
            _miscMasterService = miscMasterService;
        }

        public string[] SupportedExtensions => new[] { ".xls", ".xlsx" };

        public async Task<List<BomImportDto>> ReadFileAsync(Stream fileStream, int startRow = 0, int headerRow = 0)
        {
            var records = _excelFileService.Read(fileStream, 0, 0).ToList().Trim();
            return await Task.FromResult(records);
        }

        public async Task ValidateImportDataAsync(List<BomImportDto> records, ImportSession session, ImportSessionService sessionService)
        {
            var errors = new List<string>();
            var rowNumber = 1; // Starting from row 1 (header is row 0)
            var hasValidationErrors = false;

            foreach (var record in records)
            {
                rowNumber++;
                var rowErrors = new List<string>();

                if (string.IsNullOrWhiteSpace(record.ProductCode))
                    rowErrors.Add("Product Code is required");

                if (string.IsNullOrWhiteSpace(record.ItemCode))
                    rowErrors.Add("Item Code is required");

                if (record.Quantity == null || record.Quantity <= 0)
                    rowErrors.Add("Quantity must be greater than 0");

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

        public async Task<List<BomImportDto>> ProcessImportAsync(List<BomImportDto> records, ImportSession session, ImportSessionService sessionService, string userId)
        {
            var validationErrorCount = records.Count(r => !string.IsNullOrEmpty(r.Status) && r.Status == "Error");

            var groups = records.GroupBy(p => p.ProductCode).ToList();

            // Optimize: Get all existing products in one query
            var productCodes = groups.Select(g => g.Key).Where(pc => !string.IsNullOrWhiteSpace(pc)).ToList();
            var existingProductsList = await _productService.GetAsync(p => productCodes.Contains(p.Code), p => p).ConfigureAwait(false);
            var existingProductsDict = existingProductsList.ToDictionary(p => p.Code, p => p, StringComparer.OrdinalIgnoreCase);

            // Batch load all required lookups
            var uniqueItemCodes = records.Where(r => !string.IsNullOrWhiteSpace(r.ItemCode))
                .Select(r => r.ItemCode.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            var uniquePackingTypeCodes = records.Where(r => !string.IsNullOrWhiteSpace(r.PackingTypeCode))
                .Select(r => r.PackingTypeCode.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            // Batch load existing items
            var existingItems = await _itemService.GetAsync(
                i => uniqueItemCodes.Contains(i.Code),
                i => new { i.Id, i.Code, i.Name }
            ).ConfigureAwait(false);
            var itemDict = existingItems.ToDictionary(i => i.Code, i => i.Id, StringComparer.OrdinalIgnoreCase);

            // Batch load packing types
            var packingTypes = await _miscMasterService.GetAsync(
                m => m.MiscType == MiscMasterConstants.PackingType && uniquePackingTypeCodes.Contains(m.Code),
                m => new { m.Id, m.Code }
            ).ConfigureAwait(false);
            var packingTypeDict = packingTypes
                .GroupBy(p => p.Code, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.First().Id, StringComparer.OrdinalIgnoreCase);

            // Create missing items
            var missingItems = uniqueItemCodes
                .Where(code => !itemDict.ContainsKey(code))
                .Select(code => new { Code = code, Name = records.FirstOrDefault(r => r.ItemCode?.Trim() == code)?.ItemName ?? code })
                .ToList();

            if (missingItems.Any())
            {
                var itemsToAdd = new List<Item>();
                foreach (var item in missingItems)
                {
                    var newItem = await _itemService.CreateObjectAsync(item.Code, item.Name).ConfigureAwait(false);
                    itemsToAdd.Add(newItem);
                }
                if (itemsToAdd.Any())
                {
                    await _itemService.AddRangeAsync(itemsToAdd).ConfigureAwait(false);
                    await _itemService.SaveAsync().ConfigureAwait(false);
                    foreach (var item in itemsToAdd)
                    {
                        itemDict[item.Code] = item.Id;
                    }
                }
            }

            sessionService.UpdateSession(session.SessionId, s =>
            {
                s.Status = ImportStatus.Processing;
                s.CurrentStep = "Processing records";
                s.CurrentMessage = $"Processing {groups.Count} products...";
                s.TotalWarehouseOrders = groups.Count;
                s.ProgressDetails["TotalWarehouseOrders"] = groups.Count;
                s.ProgressDetails["ExistingProductsFound"] = existingProductsDict.Count;
                s.ProcessingSteps.Add($"Starting to process {groups.Count} products");
            });

            var productsToAdd = new List<Product>();
            var productsToUpdate = new List<Product>();
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

                    var productCode = first.ProductCode;
                    if (string.IsNullOrWhiteSpace(productCode))
                    {
                        foreach (var item in group)
                        {
                            item.Status = "Error";
                            item.Remark = "Missing Product Code.";
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

                    Product product;
                    bool isNewProduct = false;

                    if (existingProductsDict.TryGetValue(productCode, out var existingProduct))
                    {
                        product = existingProduct;
                        product.Name = first.ProductName ?? product.Name;
                        product.ModifiedBy = userId;
                        product.ModifiedDate = DateTime.Now;
                        productsToUpdate.Add(product);
                        updatedCount++;
                    }
                    else
                    {
                        product = new Product
                        {
                            Code = productCode,
                            Name = first.ProductName ?? productCode,
                            Status = StatusConstants.Active,
                            CreatedBy = userId,
                            CreatedDate = DateTime.Now,
                            ModifiedBy = userId,
                            ModifiedDate = DateTime.Now
                        };
                        productsToAdd.Add(product);
                        importedCount++;
                        isNewProduct = true;
                    }

                    // Process ProductItemDetails
                    var productItemDetailsToAdd = new List<ProductItemDetail>();
                    var productItemDetailsToUpdate = new List<ProductItemDetail>();

                    foreach (var record in group)
                    {
                        if (!string.IsNullOrEmpty(record.Status) && record.Status == "Error")
                            continue;

                        if (!itemDict.TryGetValue(record.ItemCode?.Trim() ?? "", out var itemId))
                        {
                            record.Status = "Error";
                            record.Remark = $"Item with code {record.ItemCode} not found";
                            errorCount++;
                            continue;
                        }

                        int? packingTypeId = null;
                        if (!string.IsNullOrWhiteSpace(record.PackingTypeCode))
                        {
                            if (!packingTypeDict.TryGetValue(record.PackingTypeCode.Trim(), out var ptId))
                            {
                                record.Status = "Error";
                                record.Remark = $"Packing Type with code {record.PackingTypeCode} not found";
                                errorCount++;
                                continue;
                            }
                            packingTypeId = ptId;
                        }

                        // Check if ProductItemDetail already exists for this product/item/packing type combination
                        var existingDetail = product.ProductItemDetails?.FirstOrDefault(d =>
                            d.ItemId == itemId &&
                            d.PackingTypeId == packingTypeId);

                        if (existingDetail != null)
                        {
                            // Update existing detail
                            existingDetail.Quantity = record.Quantity;
                            existingDetail.ModifiedBy = userId;
                            existingDetail.ModifiedDate = DateTime.Now;
                            productItemDetailsToUpdate.Add(existingDetail);
                        }
                        else
                        {
                            // Create new detail
                            var newDetail = new ProductItemDetail
                            {
                                ProductId = product.Id,
                                ItemId = itemId,
                                Quantity = record.Quantity,
                                PackingTypeId = packingTypeId ?? 0,
                                Status = StatusConstants.Active,
                                CreatedBy = userId,
                                CreatedDate = DateTime.Now,
                                ModifiedBy = userId,
                                ModifiedDate = DateTime.Now
                            };
                            productItemDetailsToAdd.Add(newDetail);
                        }

                        record.Status = isNewProduct ? "Imported" : "Updated";
                        record.Remark = isNewProduct ? "Product imported successfully" : "Product updated successfully";
                    }

                    // Add new ProductItemDetails to product
                    if (product.ProductItemDetails == null)
                        product.ProductItemDetails = new List<ProductItemDetail>();

                    foreach (var detail in productItemDetailsToAdd)
                    {
                        detail.ProductId = product.Id;
                        product.ProductItemDetails.Add(detail);
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

            if (productsToAdd.Any())
            {
                sessionService.UpdateSession(session.SessionId, s =>
                {
                    s.CurrentMessage = $"Adding {productsToAdd.Count} new products...";
                });
                await _productService.AddRangeAsync(productsToAdd).ConfigureAwait(false);
            }

            if (productsToUpdate.Any())
            {
                sessionService.UpdateSession(session.SessionId, s =>
                {
                    s.CurrentMessage = $"Updating {productsToUpdate.Count} existing products...";
                });
                await _productService.UpdateRangeAsync(productsToUpdate).ConfigureAwait(false);
            }

            await _productService.SaveAsync().ConfigureAwait(false);

            return records;
        }
    }
}

