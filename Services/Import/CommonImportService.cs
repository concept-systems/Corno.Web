using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Corno.Web.Logger;
using Corno.Web.Services.File.Interfaces;
using Corno.Web.Services.Import.Interfaces;
using Corno.Web.Services.Import.Models;

namespace Corno.Web.Services.Import
{
    /// <summary>
    /// Common, reusable import service that handles file reading, validation, progress tracking, and results.
    /// Business logic is delegated to IBusinessImportService implementations.
    /// </summary>
    /// <typeparam name="TDto">The DTO type that represents a single import record</typeparam>
    public class CommonImportService<TDto> where TDto : class
    {
        private readonly IBusinessImportService<TDto> _businessImportService;
        private readonly ImportSessionService _sessionService;

        public CommonImportService(IBusinessImportService<TDto> businessImportService, ImportSessionService sessionService)
        {
            _businessImportService = businessImportService;
            _sessionService = sessionService;
        }

        /// <summary>
        /// Main import method that orchestrates the entire import process
        /// </summary>
        public async Task<List<TDto>> ImportAsync(Stream fileStream, string filePath, string userId, string sessionId)
        {
            var session = _sessionService.GetSession(sessionId);
            if (session == null)
                throw new Exception("Import session not found");

            try
            {
                // Check for cancellation
                if (session.IsCancelled)
                    throw new OperationCanceledException("Import was cancelled");

                // Step 1: Read file
                var records = await ReadFileAsync(fileStream, session, filePath);

                if (!records.Any())
                    throw new Exception("No entries found in file to import. Please ensure the file contains data.");

                // Step 2: Validate data
                await ValidateDataAsync(records, session);

                // Check for cancellation after validation
                if (session.IsCancelled)
                    throw new OperationCanceledException("Import was cancelled");

                // Step 3: Process import (business logic)
                var processedRecords = await ProcessImportAsync(records, session, userId);

                // Step 4: Complete session
                _sessionService.UpdateSession(sessionId, s =>
                {
                    s.ImportedCount = processedRecords.Count(r => GetRecordStatus(r) == "Imported");
                    s.UpdatedCount = processedRecords.Count(r => GetRecordStatus(r) == "Updated");
                    s.ErrorCount = processedRecords.Count(r => GetRecordStatus(r) == "Error");
                    s.SkippedCount = processedRecords.Count(r => GetRecordStatus(r) == "Skipped");
                    s.PercentComplete = 100;
                    s.CurrentMessage = "Import completed successfully";
                });

                return processedRecords;
            }
            catch (OperationCanceledException)
            {
                _sessionService.UpdateSession(sessionId, s =>
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
                _sessionService.FailSession(sessionId, exception?.Message ?? "Import failed");
                throw;
            }
        }

        private async Task<List<TDto>> ReadFileAsync(Stream fileStream, ImportSession session, string filePath)
        {
            var readStartTime = DateTime.Now;
            _sessionService.UpdateSession(session.SessionId, s =>
            {
                s.Status = ImportStatus.Reading;
                s.CurrentStep = "Reading file";
                s.CurrentMessage = "Reading file...";
                s.ProcessingSteps.Add("Reading file started");
                s.ProgressDetails["FileType"] = Path.GetExtension(filePath)?.ToUpper();
                s.ProgressDetails["FileReading"] = "In progress";
            });

            var records = await _businessImportService.ReadFileAsync(fileStream, 0, 0);

            var readDuration = DateTime.Now - readStartTime;

            _sessionService.UpdateSession(session.SessionId, s =>
            {
                s.TotalRecords = records.Count;
                s.ProcessingSteps.Add($"File read completed: {records.Count} records in {readDuration.TotalSeconds:F2}s");
                s.ProgressDetails["RecordsRead"] = records.Count;
                s.ProgressDetails["ReadDuration"] = readDuration.TotalSeconds;
            });

            return records;
        }

        private async Task ValidateDataAsync(List<TDto> records, ImportSession session)
        {
            _sessionService.UpdateSession(session.SessionId, s =>
            {
                s.Status = ImportStatus.Validating;
                s.CurrentStep = "Validating data";
                s.CurrentMessage = $"Validating {records.Count} records...";
            });

            var validateStartTime = DateTime.Now;
            await _businessImportService.ValidateImportDataAsync(records, session, _sessionService);
            var validateDuration = DateTime.Now - validateStartTime;

            _sessionService.UpdateSession(session.SessionId, s =>
            {
                s.ProcessingSteps.Add($"Validation completed: {validateDuration.TotalSeconds:F2}s");
                s.ProgressDetails["ValidationDuration"] = validateDuration.TotalSeconds;
            });
        }

        private async Task<List<TDto>> ProcessImportAsync(List<TDto> records, ImportSession session, string userId)
        {
            _sessionService.UpdateSession(session.SessionId, s =>
            {
                s.Status = ImportStatus.Processing;
                s.CurrentStep = "Processing records";
                s.CurrentMessage = $"Processing {records.Count} records...";
            });

            var processedRecords = await _businessImportService.ProcessImportAsync(records, session, _sessionService, userId);

            return processedRecords;
        }

        /// <summary>
        /// Gets the status of a record. This uses reflection to find a Status property.
        /// If the DTO doesn't have a Status property, returns null.
        /// </summary>
        private string GetRecordStatus(TDto record)
        {
            var statusProperty = typeof(TDto).GetProperty("Status");
            if (statusProperty != null)
            {
                return statusProperty.GetValue(record)?.ToString() ?? "";
            }
            return "";
        }
    }
}

