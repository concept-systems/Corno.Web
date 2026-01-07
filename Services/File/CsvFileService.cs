using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Corno.Web.Logger;
using Corno.Web.Services.File.Interfaces;
using Corno.Web.Services.Import;
using CsvContext = LINQtoCSV.CsvContext;
using CsvFileDescription = LINQtoCSV.CsvFileDescription;

namespace Corno.Web.Services.File
{
    /// <summary>
    /// CSV file service implementation using LINQtoCSV library
    /// Matches the pattern of ExcelFileService for consistency
    /// </summary>
    /// <typeparam name="TEntity">The entity type to read/write</typeparam>
    public class CsvFileService<TEntity> : ICsvFileService<TEntity>
        where TEntity : class, new()
    {
        #region -- Private Methods --

        private static CsvFileDescription GetStandardCsvFileDescription(int headerRow = 0)
        {
            return new CsvFileDescription
            {
                SeparatorChar = ',',
                FirstLineHasColumnNames = headerRow >= 0,
                IgnoreUnknownColumns = true,
                IgnoreTrailingSeparatorChar = true,
                EnforceCsvColumnAttribute = false
            };
        }

        #endregion

        #region -- Public Methods --

        /// <summary>
        /// Reads CSV records from a file path
        /// </summary>
        public IEnumerable<TEntity> Read(string filePath, int startRow = 0, int headerRow = 0)
        {
            try
            {
                var csvContext = new CsvContext();
                var fileDescription = GetStandardCsvFileDescription(headerRow);

                var records = csvContext.Read<TEntity>(filePath, fileDescription);
                
                if (records == null || !records.Any())
                    throw new Exception("No records in file to import.");

                // Skip rows if startRow is specified
                if (startRow > 0)
                {
                    records = records.Skip(startRow);
                }

                return records;
            }
            catch (Exception exception)
            {
                LogHandler.LogError(exception);
                throw;
            }
        }

        /// <summary>
        /// Reads CSV records from a stream, normalizing alternative column names
        /// </summary>
        public IEnumerable<TEntity> Read(Stream fileStream, int startRow = 0, int headerRow = 0)
        {
            try
            {
                // Normalize column headers to handle alternative naming conventions from attributes
                var normalizedStream = CsvHeaderNormalizer.NormalizeHeaders<TEntity>(fileStream);

                // LINQtoCSV requires a file path, so we need to write stream to temp file
                var tempFilePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".csv");
                
                try
                {
                    // Reset stream position
                    normalizedStream.Position = 0;
                    
                    // Write stream to temp file
                    using (var fileStreamWriter = new FileStream(tempFilePath, FileMode.Create, FileAccess.Write))
                    {
                        normalizedStream.CopyTo(fileStreamWriter);
                    }

                    // Read from temp file
                    var records = Read(tempFilePath, startRow, headerRow);
                    
                    // Convert to list to ensure data is read before temp file is deleted
                    var recordsList = records.ToList();
                    
                    return recordsList;
                }
                finally
                {
                    // Clean up temp file
                    if (System.IO.File.Exists(tempFilePath))
                    {
                        try
                        {
                            System.IO.File.Delete(tempFilePath);
                        }
                        catch
                        {
                            // Ignore cleanup errors
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                LogHandler.LogError(exception);
                throw;
            }
        }

        /// <summary>
        /// Gets the header mappings for alternative column names based on the entity type
        /// </summary>
        private Dictionary<string, string> GetHeaderMappingsForType()
        {
            var mappings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            // Add mappings for LabelImportDto
            if (typeof(TEntity).Name == "LabelImportDto")
            {
                mappings["Material"] = "Material Description";
            }

            return mappings;
        }

        /// <summary>
        /// Saves entities to a CSV file
        /// </summary>
        public void Save(string filePath, IEnumerable<TEntity> data)
        {
            try
            {
                var csvContext = new CsvContext();
                var fileDescription = GetStandardCsvFileDescription();
                csvContext.Write(data, filePath, fileDescription);
            }
            catch (Exception exception)
            {
                LogHandler.LogError(exception);
                throw;
            }
        }

        /// <summary>
        /// Appends entities to an existing CSV file
        /// </summary>
        public void Append(string filePath, IEnumerable<TEntity> data)
        {
            try
            {
                var fileDescription = GetStandardCsvFileDescription();
                // If file exists, don't write header again
                if (System.IO.File.Exists(filePath))
                {
                    fileDescription.FirstLineHasColumnNames = false;
                }

                using var writer = new StreamWriter(filePath, true);
                var context = new CsvContext();
                context.Write(data, writer, fileDescription);
            }
            catch (Exception exception)
            {
                LogHandler.LogError(exception);
                throw;
            }
        }

        #endregion
    }
}

