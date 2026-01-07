using System.Collections.Generic;
using System.IO;
using Corno.Web.Services.Import;
using Corno.Web.Services.Interfaces;

namespace Corno.Web.Services.Import.Interfaces
{
    /// <summary>
    /// Interface for file import services that support multiple file formats (XLS, CSV, etc.)
    /// This is now a wrapper around CommonImportService and IBusinessImportService
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
        /// <param name="userId">User ID performing the import</param>
        /// <param name="sessionId">Import session ID</param>
        /// <returns>List of import DTOs with status and remarks</returns>
        System.Threading.Tasks.Task<List<TDto>> ImportAsync(Stream fileStream, string filePath,
            string userId, string sessionId);

        /// <summary>
        /// Gets the supported file extensions for this import service
        /// </summary>
        string[] SupportedExtensions { get; }
    }
}

