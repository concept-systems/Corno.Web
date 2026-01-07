using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Corno.Web.Services.Import.Interfaces;

namespace Corno.Web.Services.Import
{
    /// <summary>
    /// Generic file import service that wraps CommonImportService and delegates to IBusinessImportService
    /// </summary>
    /// <typeparam name="TDto">The DTO type that represents a single import record</typeparam>
    public class FileImportService<TDto> : IFileImportService<TDto> where TDto : class
    {
        private readonly CommonImportService<TDto> _commonImportService;
        private readonly IBusinessImportService<TDto> _businessImportService;

        public FileImportService(IBusinessImportService<TDto> businessImportService, ImportSessionService sessionService)
        {
            _businessImportService = businessImportService;
            _commonImportService = new CommonImportService<TDto>(businessImportService, sessionService);
        }

        public string[] SupportedExtensions => _businessImportService.SupportedExtensions;

        public async Task<List<TDto>> ImportAsync(Stream fileStream, string filePath, string userId, string sessionId)
        {
            return await _commonImportService.ImportAsync(fileStream, filePath, userId, sessionId);
        }
    }
}

