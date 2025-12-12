using System.Collections.Generic;
using System.IO;
using Corno.Web.Areas.Kitchen.Models;
using Corno.Web.Areas.Kitchen.Services;
using Corno.Web.Services.Interfaces;
using Corno.Web.Services.Progress.Interfaces;

namespace Corno.Web.Services.Import.Interfaces
{
    /// <summary>
    /// Interface for file import services that support multiple file formats (XLS, CSV, etc.)
    /// </summary>
    /// <typeparam name="TDto">The DTO type that represents a single import record</typeparam>
    public interface IFileImportService<TDto> : IService 
        where TDto : class
    {
        /// <summary>
        /// Imports data from a file stream
        /// </summary>
        /// <param name="fileStream">The file stream to read from</param>
        /// <param name="filePath">The file path (for reference)</param>
        /// <param name="progressService">Progress service for reporting progress</param>
        /// <param name="userId">User ID performing the import</param>
        /// <param name="sessionId">Import session ID</param>
        /// <param name="sessionService">Session service for tracking import state</param>
        /// <returns>List of import DTOs with status and remarks</returns>
        System.Threading.Tasks.Task<List<TDto>> ImportAsync(Stream fileStream, string filePath, IBaseProgressService progressService,
            string userId, string sessionId, ImportSessionService sessionService);

        /// <summary>
        /// Validates import data
        /// </summary>
        /// <param name="records">Records to validate</param>
        /// <param name="session">Import session</param>
        /// <param name="sessionService">Session service</param>
        System.Threading.Tasks.Task ValidateImportDataAsync(List<TDto> records, ImportSession session, ImportSessionService sessionService);

        /// <summary>
        /// Gets the supported file extensions for this import service
        /// </summary>
        string[] SupportedExtensions { get; }
    }
}

