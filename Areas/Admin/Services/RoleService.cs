using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Corno.Web.Areas.Admin.Dto;
using Corno.Web.Areas.Admin.Models;
using Corno.Web.Areas.Admin.Services.Interfaces;
using Corno.Web.Repository.Interfaces;
using Corno.Web.Services;

namespace Corno.Web.Areas.Admin.Services;

public class RoleService : CornoService<AspNetRole>, IRoleService
{
    #region -- Constructors --
    public RoleService(IGenericRepository<AspNetRole> genericRepository) : base(genericRepository)
    {
    }
    #endregion

    #region -- Public Methods --
    public async Task<IEnumerable<AspNetRole>> GetListAsync(Expression<Func<AspNetRole, bool>> filter = null,
        Func<IQueryable<AspNetRole>, IOrderedQueryable<AspNetRole>> orderBy = null,
        string includeProperties = "")
    {
        if (filter != null)
            return await GetAsync(filter, s => s, orderBy).ConfigureAwait(false);
        return await GetAsync<AspNetRole>(null, g => g, orderBy).ConfigureAwait(false);
    }

    public async Task<AspNetRole> CreateAsync(RoleDto dto)
    {
        if (string.IsNullOrEmpty(dto.Name))
            throw new Exception($"Invalid role name {dto.Name}");

        var role = new AspNetRole
        {
            Id = Guid.NewGuid().ToString(),
            Name = dto.Name
        };

        await AddAndSaveAsync(role).ConfigureAwait(false);

        return role;
    }

    public async Task<AspNetRole> EditAsync(RoleDto dto)
    {
        if (string.IsNullOrEmpty(dto.Name))
            throw new Exception($"Invalid role name {dto.Name}");

        var existing = await GetByIdAsync(dto.Id).ConfigureAwait(false);
        if (existing == null)
            throw new Exception($"Role with name {dto.Name} does not exist.");

        existing.Name = dto.Name;

        await UpdateAndSaveAsync(existing).ConfigureAwait(false);

        return existing;
    }
    #endregion
}