using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Corno.Web.Services.Import.Models;
using Corno.Web.Services.Interfaces;

namespace Corno.Web.Services.Import.Interfaces
{
    /// <summary>
    /// Interface for business-specific import logic.
    /// This interface focuses only on business logic - file reading, validation, progress tracking are handled by the common ImportService.
    /// </summary>
    /// <typeparam name="TDto">The DTO type that represents a single import record</typeparam>
    public interface IBusinessImportService<TDto> : IService where TDto : class
    {
        /// <summary>
        /// Gets the supported file extensions for this import service
        /// </summary>
        string[] SupportedExtensions { get; }

        /// <summary>
        /// Validates import data records. This is business-specific validation logic.
        /// </summary>
        /// <param name="records">Records to validate</param>
        /// <param name="session">Import session for tracking</param>
        /// <param name="sessionService">Session service for updating progress</param>
        Task ValidateImportDataAsync(List<TDto> records, ImportSession session, ImportSessionService sessionService);

        /// <summary>
        /// Processes validated records and performs the actual import/update operations.
        /// This is where the business logic for importing/updating entities happens.
        /// </summary>
        /// <param name="records">Validated records to process</param>
        /// <param name="session">Import session for tracking</param>
        /// <param name="sessionService">Session service for updating progress</param>
        /// <param name="userId">User ID performing the import</param>
        /// <returns>List of processed records with status and remarks</returns>
        Task<List<TDto>> ProcessImportAsync(List<TDto> records, ImportSession session, ImportSessionService sessionService, string userId);

        /// <summary>
        /// Reads records from file stream. This can be overridden for custom file reading logic.
        /// </summary>
        /// <param name="fileStream">File stream to read from</param>
        /// <param name="startRow">Starting row (0-based or 1-based depending on implementation)</param>
        /// <param name="headerRow">Header row index</param>
        /// <returns>List of DTOs read from file</returns>
        Task<List<TDto>> ReadFileAsync(Stream fileStream, int startRow = 0, int headerRow = 0);
    }
}

