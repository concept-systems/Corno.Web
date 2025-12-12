using System.Collections.Generic;
using System.IO;

namespace Corno.Web.Services.Import.Interfaces
{
    /// <summary>
    /// Interface for reading files of different formats (XLS, CSV, etc.)
    /// </summary>
    /// <typeparam name="TDto">The DTO type that represents a single import record</typeparam>
    public interface IFileReader<TDto> where TDto : class
    {
        /// <summary>
        /// Reads records from a file stream
        /// </summary>
        /// <param name="fileStream">The file stream to read from</param>
        /// <param name="startRow">Starting row index (0-based)</param>
        /// <param name="headerRow">Header row index (0-based)</param>
        /// <returns>Enumerable of DTOs</returns>
        IEnumerable<TDto> Read(Stream fileStream, int startRow = 0, int headerRow = 0);

        /// <summary>
        /// Gets the file extensions supported by this reader
        /// </summary>
        string[] SupportedExtensions { get; }

        /// <summary>
        /// Checks if this reader can handle the specified file extension
        /// </summary>
        /// <param name="extension">File extension (e.g., ".xls", ".csv")</param>
        /// <returns>True if supported, false otherwise</returns>
        bool CanRead(string extension);
    }
}

