using System.Collections.Generic;
using System.Threading.Tasks;
using Corno.Web.Areas.Admin.Dto;

namespace Corno.Web.Areas.Admin.Services.Interfaces;

public interface IAuditLogService
{
    Task LogAsync(string userId, string action, string entityType, string entityId, string entityName, string details, string ipAddress, string userAgent);
    Task<List<AuditLogDto>> GetLogsAsync(AuditLogFilterDto filter);
    Task<AuditLogDto> GetByIdAsync(long id);
    Task<int> GetLogCountAsync(AuditLogFilterDto filter);
}

