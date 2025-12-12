using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Autofac;
using Corno.Web.Areas.Kitchen.Models;
using Corno.Web.Areas.Kitchen.Services;
using Corno.Web.Controllers;
using Corno.Web.Services.Import.Interfaces;
using Corno.Web.Services.Progress.Interfaces;
using Corno.Web.Windsor;
using Microsoft.AspNet.Identity;

namespace Corno.Web.Services.Import
{
    /// <summary>
    /// Base controller for import functionality that can be reused across different areas/controllers
    /// </summary>
    /// <typeparam name="TDto">The DTO type for import records</typeparam>
    public abstract class BaseImportController<TDto> : SuperController where TDto : class
    {
        protected readonly IFileImportService<TDto> ImportService;
        protected readonly IWebProgressService ProgressService;
        protected readonly ImportSessionService SessionService;

        protected BaseImportController(IFileImportService<TDto> importService, IWebProgressService progressService)
        {
            ImportService = importService;
            ProgressService = progressService;
            SessionService = new ImportSessionService();
            ProgressService.SetWebProgress();
        }

        /// <summary>
        /// Gets the controller name for URL generation (override in derived classes)
        /// </summary>
        protected abstract string ControllerName { get; }

        /// <summary>
        /// Gets the action name for import (override in derived classes)
        /// </summary>
        protected virtual string ImportActionName => "Import";

        /// <summary>
        /// Gets the action name for getting progress (override in derived classes)
        /// </summary>
        protected virtual string GetProgressActionName => "GetImportProgress";

        /// <summary>
        /// Gets the action name for canceling import (override in derived classes)
        /// </summary>
        protected virtual string CancelImportActionName => "CancelImport";

        /// <summary>
        /// Gets the action name for getting import results (override in derived classes)
        /// </summary>
        protected virtual string GetResultsActionName => "GetImportResults";

        /// <summary>
        /// Gets the action name for checking active import (override in derived classes)
        /// </summary>
        protected virtual string CheckActiveImportActionName => "CheckActiveImport";

        /// <summary>
        /// Validates the uploaded file
        /// </summary>
        protected virtual ValidationResult ValidateFile(HttpPostedFileBase file)
        {
            if (file == null)
                return new ValidationResult { IsValid = false, ErrorMessage = "No file selected for import." };

            var allowedExtensions = ImportService.SupportedExtensions;
            var fileExtension = Path.GetExtension(file.FileName)?.ToLower();
            
            if (string.IsNullOrEmpty(fileExtension) || !allowedExtensions.Contains(fileExtension))
            {
                return new ValidationResult
                {
                    IsValid = false,
                    ErrorMessage = $"Invalid file type. Only {string.Join(", ", allowedExtensions)} files are allowed."
                };
            }

            const long maxFileSize = 10 * 1024 * 1024; // 10 MB
            if (file.ContentLength > maxFileSize)
            {
                return new ValidationResult
                {
                    IsValid = false,
                    ErrorMessage = $"File size exceeds the maximum allowed size of 10 MB."
                };
            }

            return new ValidationResult { IsValid = true };
        }

        [HttpGet]
        public ActionResult Import()
        {
            // Just render the import view; POST is handled by BaseImportController
            return View();
        }


        [HttpPost]
        public virtual Task<ActionResult> Import(IEnumerable<HttpPostedFileBase> files)
        {
            try
            {
                var userId = User.Identity.GetUserId();
                if (string.IsNullOrEmpty(userId))
                    return Task.FromResult<ActionResult>(Json(new { success = false, message = "User not authenticated." }, JsonRequestBehavior.AllowGet));

                // Check if user already has an active import
                if (SessionService.HasActiveImport(userId))
                {
                    var activeSession = SessionService.GetActiveSessionByUser(userId);
                    return Task.FromResult<ActionResult>(Json(new
                    {
                        success = false,
                        message = "You already have an import in progress. Please wait for it to complete or cancel it first.",
                        sessionId = activeSession?.SessionId
                    }, JsonRequestBehavior.AllowGet));
                }

                var file = files?.FirstOrDefault();
                var validation = ValidateFile(file);
                if (!validation.IsValid)
                {
                    return Task.FromResult<ActionResult>(Json(new { success = false, message = validation.ErrorMessage }, JsonRequestBehavior.AllowGet));
                }

                // Create import session
                var session = SessionService.CreateSession(userId, file.FileName);

                // Store file stream for background processing
                var fileBytes = new byte[file.InputStream.Length];
                file.InputStream.Read(fileBytes, 0, (int)file.InputStream.Length);
                var fileName = file.FileName;

                // Start import in background task (fire and forget)
                // Use Task.Run instead of QueueUserWorkItem to properly handle async operations
                _ = Task.Run(async () =>
                {
                    // Create a new lifetime scope for this background operation
                    await using var scope = Bootstrapper.StaticContainer.BeginLifetimeScope();
                    try
                    {
                        var importService = scope.Resolve<IFileImportService<TDto>>();
                        var progressService = scope.Resolve<IWebProgressService>();
                        var sessionService = new ImportSessionService();

                        using var stream = new MemoryStream(fileBytes);
                        progressService.SetWebProgress();
                        var importModels = await importService.ImportAsync(stream, fileName, progressService,
                            userId, session.SessionId, sessionService).ConfigureAwait(false);

                        // Store results in session
                        sessionService.UpdateSession(session.SessionId, s =>
                        {
                            s.ImportResults = importModels.Count > 1000
                                ? importModels.Take(1000).ToList()
                                : importModels;
                        });

                        // Get updated session for summary
                        var updatedSession = sessionService.GetSession(session.SessionId);

                        // Create summary
                        var summary = CreateImportSummary(updatedSession, fileName);
                        sessionService.CompleteSession(session.SessionId, summary);
                    }
                    catch (OperationCanceledException)
                    {
                        var sessionService = new ImportSessionService();
                        sessionService.UpdateSession(session.SessionId, s =>
                        {
                            s.Status = ImportStatus.Cancelled;
                            s.CurrentMessage = "Import was cancelled";
                            s.EndTime = DateTime.Now;
                        });
                    }
                    catch (Exception ex)
                    {
                        var sessionService = new ImportSessionService();
                        var errorMessage = ex.Message;
                        if (ex.InnerException != null)
                        {
                            errorMessage += " " + ex.InnerException.Message;
                        }
                        sessionService.FailSession(session.SessionId, errorMessage);
                    }
                    // Scope will be disposed here after all async operations complete
                });

                return Task.FromResult<ActionResult>(Json(new { success = true, sessionId = session.SessionId }, JsonRequestBehavior.AllowGet));
            }
            catch (InvalidOperationException ex)
            {
                return Task.FromResult<ActionResult>(Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet));
            }
            catch (Exception exception)
            {
                HandleControllerException(exception);
                return Task.FromResult<ActionResult>(Json(new { success = false, message = exception.Message }, JsonRequestBehavior.AllowGet));
            }
        }

        /// <summary>
        /// Creates import summary from session (can be overridden in derived classes)
        /// </summary>
        protected virtual ImportSummary CreateImportSummary(ImportSession session, string fileName)
        {
            var summary = new ImportSummary
            {
                TotalRecords = session.TotalRecords,
                SuccessfullyImported = session.ImportedCount,
                Updated = session.UpdatedCount,
                Errors = session.ErrorCount,
                Skipped = session.SkippedCount,
                StartTime = session.StartTime,
                EndTime = session.EndTime ?? DateTime.Now,
                Duration = (session.EndTime ?? DateTime.Now) - session.StartTime,
                ErrorDetails = session.ErrorMessages.Select((msg, idx) => new ImportErrorDetail
                {
                    RowNumber = idx + 1,
                    ErrorMessage = msg
                }).ToList(),
                ProcessingSteps = session.ProcessingSteps ?? new List<string>(),
                SummaryDetails = session.ProgressDetails ?? new Dictionary<string, object>(),
                TotalWarehouseOrders = session.TotalWarehouseOrders,
                FileName = session.FileName,
                FileType = Path.GetExtension(fileName)?.ToUpper()
            };
            summary.RecordsPerSecond = summary.Duration.TotalSeconds > 0
                ? summary.TotalRecords / summary.Duration.TotalSeconds
                : 0;

            if (summary.SummaryDetails != null)
            {
                summary.SummaryDetails["TotalRecords"] = summary.TotalRecords;
                summary.SummaryDetails["SuccessfullyImported"] = summary.SuccessfullyImported;
                summary.SummaryDetails["Updated"] = summary.Updated;
                summary.SummaryDetails["Errors"] = summary.Errors;
                summary.SummaryDetails["DurationSeconds"] = summary.Duration.TotalSeconds;
                summary.SummaryDetails["RecordsPerSecond"] = summary.RecordsPerSecond;
            }

            return summary;
        }

        [HttpGet]
        public virtual ActionResult GetImportProgress(string sessionId)
        {
            try
            {
                var userId = User.Identity.GetUserId();
                var session = SessionService.GetSession(sessionId);

                if (session == null)
                    return Json(new { success = false, message = "Session not found" }, JsonRequestBehavior.AllowGet);

                if (session.UserId != userId)
                    return Json(new { success = false, message = "Unauthorized access to session" }, JsonRequestBehavior.AllowGet);

                // Calculate estimated time remaining
                if (session.ProcessedRecords > 0 && session.PercentComplete > 0 && session.PercentComplete < 100)
                {
                    var elapsed = DateTime.Now - session.StartTime;
                    var estimatedTotal = TimeSpan.FromMilliseconds(elapsed.TotalMilliseconds / (session.PercentComplete / 100.0));
                    session.EstimatedTimeRemaining = estimatedTotal - elapsed;
                }

                return Json(new
                {
                    success = true,
                    session = new
                    {
                        sessionId = session.SessionId,
                        status = session.Status.ToString(),
                        totalRecords = session.TotalRecords,
                        processedRecords = session.ProcessedRecords,
                        importedCount = session.ImportedCount,
                        updatedCount = session.UpdatedCount,
                        errorCount = session.ErrorCount,
                        skippedCount = session.SkippedCount,
                        percentComplete = session.PercentComplete,
                        currentMessage = session.CurrentMessage,
                        currentStep = session.CurrentStep,
                        isCancelled = session.IsCancelled,
                        estimatedTimeRemaining = session.EstimatedTimeRemaining?.ToString(@"mm\:ss"),
                        hasResults = session.ImportResults != null &&
                            (session.ImportResults is System.Collections.IEnumerable enumerable && enumerable.Cast<object>().Any()),
                        currentWarehouseOrderNo = session.CurrentWarehouseOrderNo,
                        currentWarehouseOrderIndex = session.CurrentWarehouseOrderIndex,
                        totalWarehouseOrders = session.TotalWarehouseOrders,
                        progressDetails = session.ProgressDetails,
                        summary = session.Summary != null ? new
                        {
                            totalRecords = session.Summary.TotalRecords,
                            successfullyImported = session.Summary.SuccessfullyImported,
                            updated = session.Summary.Updated,
                            errors = session.Summary.Errors,
                            skipped = session.Summary.Skipped,
                            duration = session.Summary.Duration.ToString(@"hh\:mm\:ss"),
                            recordsPerSecond = session.Summary.RecordsPerSecond,
                            fileName = session.Summary.FileName,
                            fileType = session.Summary.FileType,
                            totalWarehouseOrders = session.Summary.TotalWarehouseOrders,
                            processingSteps = session.Summary.ProcessingSteps,
                            summaryDetails = session.Summary.SummaryDetails
                        } : null
                    }
                }, JsonRequestBehavior.AllowGet);
            }
            catch (System.ObjectDisposedException ex)
            {
                // Handle disposed DbContext - session is no longer valid
                return Json(new { success = false, message = "Session has expired. Please refresh the page." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                HandleControllerException(exception);
                var errorMessage = exception.Message;
                if (errorMessage.Contains("disposed") || errorMessage.Contains("DbContext"))
                {
                    errorMessage = "Session has expired. Please refresh the page.";
                }
                return Json(new { success = false, message = errorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public virtual ActionResult CancelImport(string sessionId)
        {
            try
            {
                var userId = User.Identity.GetUserId();
                if (SessionService.CancelSession(sessionId, userId))
                {
                    ProgressService.CancelRequested();
                    return Json(new { success = true, message = "Import cancelled successfully" }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { success = false, message = "Failed to cancel import or session not found" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                HandleControllerException(exception);
                return Json(new { success = false, message = exception.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public virtual ActionResult GetImportResults(string sessionId)
        {
            try
            {
                var userId = User.Identity.GetUserId();
                var session = SessionService.GetSession(sessionId);

                if (session == null)
                    return Json(new { success = false, message = "Session not found" }, JsonRequestBehavior.AllowGet);

                if (session.UserId != userId)
                    return Json(new { success = false, message = "Unauthorized access" }, JsonRequestBehavior.AllowGet);

                var results = session.ImportResults;
                if (results == null)
                {
                    results = new List<object>();
                }
                else if (results is System.Collections.IEnumerable enumerable)
                {
                    results = enumerable.Cast<object>().ToList();
                }

                return Json(new
                {
                    success = true,
                    results = results
                }, JsonRequestBehavior.AllowGet);
            }
            catch (System.ObjectDisposedException ex)
            {
                // Handle disposed DbContext - session is no longer valid
                return Json(new { success = false, message = "Session has expired. Please refresh the page." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                HandleControllerException(exception);
                var errorMessage = exception.Message;
                if (errorMessage.Contains("disposed") || errorMessage.Contains("DbContext"))
                {
                    errorMessage = "Session has expired. Please refresh the page.";
                }
                return Json(new { success = false, message = errorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public virtual ActionResult CheckActiveImport()
        {
            try
            {
                var userId = User.Identity.GetUserId();
                var hasActive = SessionService.HasActiveImport(userId);
                if (hasActive)
                {
                    var activeSession = SessionService.GetActiveSessionByUser(userId);
                    return Json(new
                    {
                        hasActiveImport = true,
                        sessionId = activeSession?.SessionId,
                        fileName = activeSession?.FileName
                    }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { hasActiveImport = false }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                HandleControllerException(exception);
                return Json(new { success = false, message = exception.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        protected class ValidationResult
        {
            public bool IsValid { get; set; }
            public string ErrorMessage { get; set; }
        }
    }
}

