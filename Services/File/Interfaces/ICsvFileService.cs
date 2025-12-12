using System.Collections.Generic;
using System.IO;
using Corno.Web.Services.Interfaces;

namespace Corno.Web.Services.File.Interfaces
{
    /// <summary>
    /// Interface for CSV file operations, matching the pattern of IExcelFileService
    /// </summary>
    /// <typeparam name="TEntity">The entity type to read/write</typeparam>
    public interface ICsvFileService<TEntity> : IService
    {
        /// <summary>
        /// Reads CSV records from a file path
        /// </summary>
        /// <param name="filePath">Path to the CSV file</param>
        /// <param name="startRow">Starting row index (0-based, default: 0)</param>
        /// <param name="headerRow">Header row index (0-based, default: 0)</param>
        /// <returns>Enumerable of entities</returns>
        IEnumerable<TEntity> Read(string filePath, int startRow = 0, int headerRow = 0);

        /// <summary>
        /// Reads CSV records from a stream
        /// </summary>
        /// <param name="fileStream">Stream containing CSV data</param>
        /// <param name="startRow">Starting row index (0-based, default: 0)</param>
        /// <param name="headerRow">Header row index (0-based, default: 0)</param>
        /// <returns>Enumerable of entities</returns>
        IEnumerable<TEntity> Read(Stream fileStream, int startRow = 0, int headerRow = 0);

        /// <summary>
        /// Saves entities to a CSV file
        /// </summary>
        /// <param name="filePath">Path to save the CSV file</param>
        /// <param name="data">Data to save</param>
        void Save(string filePath, IEnumerable<TEntity> data);

        /// <summary>
        /// Appends entities to an existing CSV file
        /// </summary>
        /// <param name="filePath">Path to the CSV file</param>
        /// <param name="data">Data to append</param>
        void Append(string filePath, IEnumerable<TEntity> data);
    }
}

