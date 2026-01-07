using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Corno.Web.Areas.Admin.Dto;
using Corno.Web.Areas.Admin.Models;
using Corno.Web.Areas.Admin.Services.Interfaces;
using Corno.Web.Repository.Interfaces;
using Corno.Web.Services;
using Mapster;

namespace Corno.Web.Areas.Admin.Services;

public class AuditLogService : CornoService<AuditLog>, IAuditLogService
{
    #region -- Constructors --
    public AuditLogService(IGenericRepository<AuditLog> genericRepository) : base(genericRepository)
    {
    }
    #endregion

    #region -- Public Methods --
    public async Task LogAsync(string userId, string action, string entityType, string entityId, string entityName, string details, string ipAddress, string userAgent)
    {
        var auditLog = new AuditLog
        {
            UserId = userId,
            Action = action,
            EntityType = entityType,
            EntityId = entityId,
            EntityName = entityName,
            Details = details,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            Timestamp = DateTime.Now
        };

        // Get username if userId provided
        if (!string.IsNullOrEmpty(userId))
        {
            // You may need to inject IUserService or IIdentityService here
            // For now, we'll set it later if needed
        }

        await AddAndSaveAsync(auditLog).ConfigureAwait(false);
    }

    public async Task<List<AuditLogDto>> GetLogsAsync(AuditLogFilterDto filter)
    {
        var query = GetQuery();

        if (filter.FromDate.HasValue)
            query = query.Where(a => a.Timestamp >= filter.FromDate.Value);

        if (filter.ToDate.HasValue)
            query = query.Where(a => a.Timestamp <= filter.ToDate.Value);

        if (!string.IsNullOrEmpty(filter.UserId))
            query = query.Where(a => a.UserId == filter.UserId);

        if (!string.IsNullOrEmpty(filter.Action))
            query = query.Where(a => a.Action == filter.Action);

        if (!string.IsNullOrEmpty(filter.EntityType))
            query = query.Where(a => a.EntityType == filter.EntityType);

        var logs = await query
            .OrderByDescending(a => a.Timestamp)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync()
            .ConfigureAwait(false);

        return logs.Adapt<List<AuditLogDto>>();
    }

    public async Task<AuditLogDto> GetByIdAsync(long id)
    {
        var log = await FirstOrDefaultAsync(a => a.Id == id, a => a).ConfigureAwait(false);
        return log?.Adapt<AuditLogDto>();
    }

    public async Task<int> GetLogCountAsync(AuditLogFilterDto filter)
    {
        var query = GetQuery();

        if (filter.FromDate.HasValue)
            query = query.Where(a => a.Timestamp >= filter.FromDate.Value);

        if (filter.ToDate.HasValue)
            query = query.Where(a => a.Timestamp <= filter.ToDate.Value);

        if (!string.IsNullOrEmpty(filter.UserId))
            query = query.Where(a => a.UserId == filter.UserId);

        if (!string.IsNullOrEmpty(filter.Action))
            query = query.Where(a => a.Action == filter.Action);

        if (!string.IsNullOrEmpty(filter.EntityType))
            query = query.Where(a => a.EntityType == filter.EntityType);

        return await query.CountAsync().ConfigureAwait(false);
    }
    #endregion
}

