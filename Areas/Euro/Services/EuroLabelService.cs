using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Corno.Web.Areas.Euro.Dto.Label;
using Corno.Web.Areas.Euro.Services.Interfaces;
using Corno.Web.Globals;
using Corno.Web.Models;
using Corno.Web.Repository.Interfaces;
using Corno.Web.Services;
using Corno.Web.Services.File.Interfaces;
using Corno.Web.Services.Progress.Interfaces;
using Corno.Web.Areas.Kitchen.Services;
using Corno.Web.Areas.Kitchen.Models;
using Corno.Web.Models.Packing;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Mapster;
using Corno.Web.Services.Import.Interfaces;

namespace Corno.Web.Areas.Euro.Services;

public class EuroLabelService : BaseService<Label>, IEuroLabelService, IFileImportService<LabelImportDto>
{
    private readonly ICsvFileService<LabelImportDto> _csvFileService;

    public EuroLabelService(IGenericRepository<Label> genericRepository,
        ICsvFileService<LabelImportDto> csvFileService)
        : base(genericRepository)
    {
        _csvFileService = csvFileService;
    }

    private string GetProductionOrderNo(string barcode)
    {
        var data = barcode.Split('_');
        return data.Length > 1 ? data[1] : string.Empty;
    }

    public void ConfigureMapping(MapContext context)
    {
        var userId = context.Parameters["UserId"] as string ?? "System";
        var isUpdate = context.Parameters.ContainsKey("IsUpdate") && (bool)context.Parameters["IsUpdate"];
        TypeAdapterConfig<LabelImportDto, Label>
            .NewConfig()
            .AfterMapping((src, dest) =>
            {
                dest.CompanyId = 19;
                // Use RunAsync helper to avoid deadlock in Mapster callback
                dest.SerialNo = RunAsync(() => GetNextSequenceAsync(FieldConstants.BarcodeLabelSerialNo));
                dest.LabelDate = DateTime.Now;
                dest.ProductionOrderNo = GetProductionOrderNo(src.Barcode);
                dest.Quantity = 1;
                dest.Reserved1 = src.MaterialDescription;
                dest.Reserved2 = src.Grains;
                dest.Links = src.CustomerReference;
                dest.Position = src.BusinessUnit;

                dest.CreatedBy = userId;
                dest.CreatedDate = DateTime.Now;
                dest.Status = StatusConstants.Active;
            });
    }

    public DataSourceResult GetIndexDataSource(DataSourceRequest request)
    {
        try
        {
            /*var config = new TypeAdapterConfig();
                config.NewConfig<Label, LabelIndexDto>()
                .AfterMapping((src, dest) =>
                {
                    dest.OcNo = src.ProductionOrderNo;
                });

            var query = GetQuery();
            var data = from label in query
                select new LabelIndexDto
                {
                    Id = label.Id,
                    OcNo = label.ProductionOrderNo,
                    Description = label.Description,
                    LabelDate = label.LabelDate,
                    Barcode = label.Barcode,
                    Status = label.Status
                };*/

            var result = GetQuery().ProjectToType<LabelIndexDto>().ToDataSourceResult(request);
            return result;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error getting index data: {ex.Message}", ex);
        }
    }

    #region -- IFileImportService Implementation --
    
    public string[] SupportedExtensions => new[] { ".csv" };

    public async Task<List<LabelImportDto>> ImportAsync(Stream fileStream, string filePath, IBaseProgressService progressService,
        string userId, string sessionId, ImportSessionService sessionService)
    {
        // Convert IBaseProgressService to IWebProgressService
        var webProgressService = progressService as IWebProgressService;
        if (webProgressService == null)
        {
            throw new Exception("Progress service must be IWebProgressService for EuroLabel import");
        }

        // Call the underlying service and convert List<object> to List<LabelImportDto>
        var results = await ImportAsync(fileStream, filePath, webProgressService, userId, sessionId, sessionService).ConfigureAwait(false);
        
        // Convert results to LabelImportDto list
        var importDtos = new List<LabelImportDto>();
        foreach (var result in results)
        {
            if (result is LabelImportDto dto)
            {
                importDtos.Add(dto);
            }
        }

        return importDtos;
    }

    public async Task ValidateImportDataAsync(List<LabelImportDto> records, ImportSession session, ImportSessionService sessionService)
    {
        // Validation is done in the ImportAsync method itself
        await Task.CompletedTask;
    }

    #endregion

    public async Task<List<object>> ImportAsync(Stream fileStream, string fileName, IWebProgressService progressService,
        string userId, string sessionId, ImportSessionService sessionService)
    {
        var session = sessionService.GetSession(sessionId);
        if (session == null)
            throw new Exception("Import session not found");

        try
        {
            // Check for cancellation
            if (session.IsCancelled || progressService.IsCancelled())
                throw new OperationCanceledException("Import was cancelled");

            var readStartTime = DateTime.Now;
            sessionService.UpdateSession(session.SessionId, s =>
            {
                s.Status = ImportStatus.Reading;
                s.CurrentStep = "Reading CSV file";
                s.CurrentMessage = "Reading CSV file...";
                s.ProcessingSteps.Add("Reading file started");
                s.ProgressDetails["FileType"] = "CSV";
                s.ProgressDetails["FileReading"] = "In progress";
            });
            progressService.Report("Reading CSV file...", MessageType.Progress);

            // Read CSV file
            var records = _csvFileService.Read(fileStream, 0)
                .ToList();

            var readDuration = DateTime.Now - readStartTime;

            if (!records.Any())
                throw new Exception("No entries found in CSV file to import. Please ensure the file contains data.");

            sessionService.UpdateSession(session.SessionId, s =>
            {
                s.TotalRecords = records.Count;
                s.Status = ImportStatus.Processing;
                s.CurrentStep = "Processing records";
                s.CurrentMessage = $"Processing {records.Count} records...";
                s.ProcessingSteps.Add($"File read completed: {records.Count} records in {readDuration.TotalSeconds:F2}s");
                s.ProgressDetails["RecordsRead"] = records.Count;
                s.ProgressDetails["ReadDuration"] = readDuration.TotalSeconds;
            });

            // Initialize progress
            progressService.Initialize(fileName, 0, records.Count, 1);

            var labelsToAdd = new List<Label>();
            var labelsToUpdate = new List<Label>();
            var processedCount = 0;
            var importedCount = 0;
            var updatedCount = 0;
            var errorCount = 0;
            var skippedCount = 0;
            var importResults = new List<object>();

            // Get existing labels by barcode for update check
            var barcodes = records.Select(r => r.Barcode).Where(b => !string.IsNullOrEmpty(b)).ToList();
            var existingList = await GetAsync(l => barcodes.Contains(l.Barcode), l => l).ConfigureAwait(false);
            var existingLabels = existingList.ToDictionary(l => l.Barcode, l => l);

            var context = new MapContext
            {
                Parameters = { ["UserId"] = userId, ["IsUpdate"] = false }
            };
            ConfigureMapping(context);

            foreach (var record in records)
            {
                // Check for cancellation
                if (session.IsCancelled || progressService.IsCancelled())
                    throw new OperationCanceledException("Import was cancelled");

                try
                {
                    processedCount++;

                    // Validate required fields
                    if (string.IsNullOrWhiteSpace(record.Barcode))
                    {
                        errorCount++;
                        record.Status = "Error";
                        record.Remark = "Barcode. is required";
                        importResults.Add(record);
                        continue;
                    }

                    // Check if label exists
                    if (existingLabels.TryGetValue(record.Barcode ?? "", out var existingLabel))
                    {
                        // Update existing label
                        record.Adapt(existingLabel);

                        labelsToUpdate.Add(existingLabel);
                        updatedCount++;
                        record.Status = "Updated";
                    }
                    else
                    {
                        // Create new label
                        var newLabel = record.Adapt<Label>();

                        labelsToAdd.Add(newLabel);
                        importedCount++;
                        record.Status = "Imported";
                    }

                    importResults.Add(record);

                    // Update progress
                    var percentComplete = (processedCount * 100) / records.Count;
                    sessionService.UpdateSession(session.SessionId, s =>
                    {
                        s.ProcessedRecords = processedCount;
                        s.ImportedCount = importedCount;
                        s.UpdatedCount = updatedCount;
                        s.ErrorCount = errorCount;
                        s.SkippedCount = skippedCount;
                        s.PercentComplete = percentComplete;
                        s.CurrentMessage = $"Processed {processedCount} of {records.Count} records";
                    });

                    //progressService.Report(processedCount, $"Processing record {processedCount} of {records.Count}");
                }
                catch (Exception ex)
                {
                    errorCount++;
                    record.Status = "Error";
                    record.Remark = ex.Message;
                    importResults.Add(record);
                    sessionService.UpdateSession(session.SessionId, s =>
                    {
                        s.ErrorMessages.Add($"Row {processedCount}: {ex.Message}");
                    });
                }
            }

            // Save to database
            if (labelsToAdd.Any())
            {
                await AddRangeAsync(labelsToAdd).ConfigureAwait(false);
            }

            if (labelsToUpdate.Any())
            {
                await UpdateRangeAsync(labelsToUpdate).ConfigureAwait(false);
            }

            await SaveAsync().ConfigureAwait(false);

            // Update final session status
            sessionService.UpdateSession(session.SessionId, s =>
            {
                s.Status = ImportStatus.Completed;
                s.ProcessedRecords = processedCount;
                s.ImportedCount = importedCount;
                s.UpdatedCount = updatedCount;
                s.ErrorCount = errorCount;
                s.SkippedCount = skippedCount;
                s.PercentComplete = 100;
                s.EndTime = DateTime.Now;
                s.CurrentMessage = "Import completed successfully";
                s.ProcessingSteps.Add($"Import completed: {importedCount} imported, {updatedCount} updated, {errorCount} errors");
            });

            progressService.Report("Import completed", MessageType.Progress);

            return importResults.Cast<object>().ToList();
        }
        catch (OperationCanceledException)
        {
            sessionService.UpdateSession(session.SessionId, s =>
            {
                s.Status = ImportStatus.Cancelled;
                s.CurrentMessage = "Import was cancelled";
                s.EndTime = DateTime.Now;
            });
            throw;
        }
        catch (Exception ex)
        {
            sessionService.UpdateSession(session.SessionId, s =>
            {
                s.Status = ImportStatus.Failed;
                s.CurrentMessage = $"Import failed: {ex.Message}";
                s.EndTime = DateTime.Now;
                s.ErrorMessages.Add(ex.Message);
            });
            throw;
        }
    }
}
