using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.IO;
using Autofac;
using Corno.Web.Areas.Kitchen.Dto.Label;
using Corno.Web.Areas.Kitchen.Services.Interfaces;
using Corno.Web.Services.Import;
using Corno.Web.Services.Import.Interfaces;
using Corno.Web.Services.Import.Models;
using Corno.Web.Windsor;
using Microsoft.AspNet.Identity;

namespace Corno.Web.Areas.Kitchen.Controllers
{
    /// <summary>
    /// Controller for Label imports (Sorting/Kitting).
    /// Extends BaseImportController and handles the newStatus parameter required for label imports.
    /// </summary>
    public class LabelImportController : BaseImportController<SalvaginiExcelDto>
    {
        private readonly ILabelService _labelService;

        public LabelImportController(IFileImportService<SalvaginiExcelDto> importService, ILabelService labelService)
            : base(importService)
        {
            _labelService = labelService;
        }

        protected override string ControllerName => "LabelImport";

        /// <summary>
        /// Override Import to accept newStatus parameter for Sorting/Kitting
        /// </summary>
        [HttpPost]
        public override Task<ActionResult> Import(IEnumerable<HttpPostedFileBase> files)
        {
            // Get newStatus from form data or query string
            var newStatus = Request.Form["newStatus"] ?? Request.QueryString["newStatus"];
            if (string.IsNullOrWhiteSpace(newStatus))
            {
                return Task.FromResult<ActionResult>(Json(new { success = false, message = "NewStatus parameter is required for label import." }, JsonRequestBehavior.AllowGet));
            }

            return ImportWithStatus(files, newStatus);
        }

        private Task<ActionResult> ImportWithStatus(IEnumerable<HttpPostedFileBase> files, string newStatus)
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

                // Store newStatus in session for business import service
                SessionService.UpdateSession(session.SessionId, s =>
                {
                    s.ProgressDetails["NewStatus"] = newStatus;
                });

                // Store file stream for background processing
                var fileBytes = new byte[file.InputStream.Length];
                file.InputStream.Read(fileBytes, 0, (int)file.InputStream.Length);
                var fileName = file.FileName;

                // Start import in background task
                _ = Task.Run(async () =>
                {
                    await using var scope = Bootstrapper.StaticContainer.BeginLifetimeScope();
                    try
                    {
                        var importService = scope.Resolve<IFileImportService<SalvaginiExcelDto>>();
                        var sessionService = new ImportSessionService();

                        using var stream = new MemoryStream(fileBytes);
                        var importModels = await importService.ImportAsync(stream, fileName,
                            userId, session.SessionId).ConfigureAwait(false);

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
    }
}

