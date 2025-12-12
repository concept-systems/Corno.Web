using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Corno.Web.Dtos;
using Corno.Web.Models.Masters;
using Corno.Web.Repository.Interfaces;
using Corno.Web.Services.Interfaces;

namespace Corno.Web.Services;

public class MiscMasterService : MasterService<MiscMaster>, IMiscMasterService
{
    #region -- Constructors --
    public MiscMasterService(IGenericRepository<MiscMaster> genericRepository) : base(genericRepository)
    {
    }
    #endregion

    #region -- Methods --

    public virtual async Task<MiscMaster> GetOrCreateAsync(string code, string name, string miscMasterType, bool bSave = true)
    {
        if (null == code) return null;

        code = code.Trim();
        name = name.Trim();
        var entity = await GetByCodeAsync(code, miscMasterType).ConfigureAwait(false);
        if (null != entity) return entity;

        entity = new MiscMaster
        {
            SerialNo = await GetNextSerialNoAsync().ConfigureAwait(false),
            Code = code,
            Name = string.IsNullOrEmpty(name) ? code : name,
            MiscType = miscMasterType
        };

        if (false == bSave)
            await AddAsync(entity).ConfigureAwait(false);
        else
            await AddAndSaveAsync(entity).ConfigureAwait(false);

        return entity;
    }

    public async Task<IEnumerable<MasterDto>> GetViewModelListAsync(string miscType)
    {
        var list = await GetAsync(p => p.MiscType == miscType, m => new MasterDto
        {
            Id = m.Id,
            Code = m.Code,
            Name = m.Name,
            Description = m.Description,
            NameWithCode = m.Code + " - " + m.Name
        }).ConfigureAwait(false);
        return list.OrderBy(x => x.Id);
    }

    public async Task<MiscMaster> GetListAsync(string code, string miscType)
    {
        return await FirstOrDefaultAsync(p => p.MiscType == miscType, p => p).ConfigureAwait(false);
    }

    public async Task<MiscMaster> GetByCodeAsync(string code, string miscType)
    {
        return await FirstOrDefaultAsync(p => p.Code == code && p.MiscType == miscType, p => p).ConfigureAwait(false);
    }

    public async Task<MiscMaster> GetByNameAsync(string name, string miscType)
    {
        return await FirstOrDefaultAsync(p => p.Name == name && p.MiscType == miscType, p => p).ConfigureAwait(false);
    }

    public override async Task<MiscMaster> GetExistingAsync(MiscMaster entity)
    {
        return await GetByCodeAsync(entity.Code, entity.MiscType).ConfigureAwait(false);
    }

    public async Task<bool> ExistsAsync(string code, string miscType)
    {
        var entity = await GetByCodeAsync(code, miscType).ConfigureAwait(false);
        return entity != null;
    }
    #endregion
}