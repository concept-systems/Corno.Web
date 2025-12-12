using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Corno.Web.Services.File;
using Corno.Web.Services.File.Interfaces;

namespace Corno.Web.Services.Import.FileReaders
{
    /// <summary>
    /// CSV file reader implementation using the existing CsvFileService
    /// </summary>
    /// <typeparam name="TDto">The DTO type that represents a single import record</typeparam>
    public class CsvFileReader<TDto> : Interfaces.IFileReader<TDto> where TDto : class, new()
    {
        private readonly ICsvFileService<TDto> _csvFileService;

        public CsvFileReader()
        {
            _csvFileService = new CsvFileService<TDto>();
        }

        public CsvFileReader(ICsvFileService<TDto> csvFileService)
        {
            _csvFileService = csvFileService ?? new CsvFileService<TDto>();
        }

        public string[] SupportedExtensions => new[] { ".csv" };

        public bool CanRead(string extension)
        {
            return SupportedExtensions.Contains(extension?.ToLower());
        }

        public IEnumerable<TDto> Read(Stream fileStream, int startRow = 0, int headerRow = 0)
        {
            try
            {
                return _csvFileService.Read(fileStream, startRow, headerRow);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error reading CSV file: {ex.Message}", ex);
            }
        }
    }
}

