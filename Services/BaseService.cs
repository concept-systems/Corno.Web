using Corno.Web.Extensions;
using Corno.Web.Repository.Interfaces;
using Corno.Web.Services.Interfaces;
using Corno.Web.Services.Progress.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Corno.Web.Models.Base;

namespace Corno.Web.Services;

public class BaseService<TEntity> : CornoService<TEntity>, IBaseService<TEntity>
    where TEntity : BaseModel, new()
{
    private readonly IGenericRepository<TEntity> _genericRepository;

    #region -- Constructors --
    protected BaseService(IGenericRepository<TEntity> genericRepository) : base(genericRepository)
    {
        _genericRepository = genericRepository;
    }

    #endregion

    #region -- Data Members --

    //protected string IncludeProperties;
    #endregion

    #region -- General Methods --

    public override async Task<TEntity> GetByIdAsync(object id)
    {
        if (id is string)
            return await base.GetByIdAsync(id).ConfigureAwait(false);

        var intId = id.ToInt();
        return await FirstOrDefaultAsync(d => d.Id == intId, p => p).ConfigureAwait(false);
    }

    public async Task<IQueryable<TEntity>> GetByIdsAsync(IEnumerable<object> ids)
    {
        var list = await GetAsync<TEntity>(null, p => p, null, false).ConfigureAwait(false);
        return list.AsQueryable();
    }

    public virtual async Task<TEntity> GetByCodeAsync(string code)
    {
        return await FirstOrDefaultAsync(p => p.Code == code, p => p).ConfigureAwait(false);
    }

    public virtual async Task<TEntity> GetByStatusAsync(string status)
    {
        return await FirstOrDefaultAsync(p => p.Status == status, p => p).ConfigureAwait(false);
    }

    /*public override async Task<List<TDest>> GetAsync<TDest>(Expression<Func<TEntity, bool>> filter = null,
        Expression<Func<TEntity, TDest>> select = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null)
    {
        return await _genericRepository.GetAsync(filter, select, orderBy);
    }*/

    public override async Task<TDest> FirstOrDefaultAsync<TDest>(Expression<Func<TEntity, bool>> filter = null,
        Expression<Func<TEntity, TDest>> select = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null)
    {
        return await _genericRepository.FirstOrDefaultAsync(filter, select, orderBy).ConfigureAwait(false);
    }

    /*public override async Task<TDest> FirstOrDefaultAsync<TDest>(
        Expression<Func<TEntity, bool>> filter = null,
        Expression<Func<TEntity, TDest>> select = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null)
    {
        return await _genericRepository.FirstOrDefaultAsync(filter, select, orderBy).ConfigureAwait(false);
    }*/

    /*public override TDest LastOrDefault<TDest>(Expression<Func<TEntity, bool>> filter = null,
        Expression<Func<TEntity, TDest>> select = null)
    {
        return Get(filter, select).LastOrDefault();
    }*/

    public virtual async Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        string includeProperties = "")
    {
        return await GetAsync<TEntity>(filter, null, orderBy).ConfigureAwait(false);
    }

    public virtual async Task<TEntity> GetExistingAsync(TEntity entity)
    {
        return await GetByCodeAsync(entity.Code).ConfigureAwait(false);
    }

    public virtual async Task<bool> ExistsAsync(string code)
    {
        var entity = await GetByCodeAsync(code).ConfigureAwait(false);
        return entity != null;
    }

    #endregion

    #region -- Update Methods --
    public override async Task UpdateAsync(TEntity entity)
    {
        //entity.ModifiedBy = ApplicationGlobals.UserId;
        entity.ModifiedDate = DateTime.Now;

        await base.UpdateAsync(entity).ConfigureAwait(false);
    }

    public override async Task UpdateAndSaveAsync(TEntity entity)
    {
        //entity.ModifiedBy = ApplicationGlobals.UserId;
        entity.ModifiedDate = DateTime.Now;

        await base.UpdateAsync(entity).ConfigureAwait(false);
        await SaveAsync().ConfigureAwait(false);
    }

    public override async Task UpdateAsync(TEntity entity, string id)
    {
        //entity.ModifiedBy = ApplicationGlobals.UserId;
        entity.ModifiedDate = DateTime.Now;

        await base.UpdateAsync(entity, id).ConfigureAwait(false);
    }

    public override async Task UpdateRangeAsync(IEnumerable<TEntity> entities)
    {
        var baseModels = entities.ToList();
        baseModels.ForEach(entity =>
        {
            //entity.ModifiedBy = ApplicationGlobals.UserId;
            entity.ModifiedDate = DateTime.Now;
        });

        await base.UpdateRangeAsync(baseModels).ConfigureAwait(false);
    }
    #endregion

    #region --  Import --
    #region -- Public Methods --
    public virtual Task ImportAsync(string filePath, IBaseProgressService progressService,
        string miscMaster = null)
    {
        throw new NotImplementedException();
    }
    #endregion
    #endregion

    #region -- Corno Methods --

    public async Task<int> GetNextIdAsync()
    {
        var max = await MaxAsync(g => g.Id > 0, g => g.Id).ConfigureAwait(false);
        return max + 1;
    }

    public async Task<int> GetNextSerialNoAsync()
    {
        var maxSerialNo = await MaxAsync(g => g.Id > 0, g => g.SerialNo ?? 0).ConfigureAwait(false);
        return maxSerialNo + 1;
    }

    public async Task<int> GetNextSerialNoAsync(int companyId)
    {
        var max = await MaxAsync(g => g.Id > 0, g => g.SerialNo).ConfigureAwait(false);
        return max + 1;
    }

    public async Task<int> GetCurrentSequenceAsync(string sequenceName)
    {
        return await _genericRepository.GetCurrentSequenceAsync(sequenceName).ConfigureAwait(false);
    }

    public async Task<int> GetNextSequenceAsync(string sequenceName)
    {
        return await _genericRepository.GetNextSequenceAsync(sequenceName).ConfigureAwait(false);
    }

    /// <summary>
    /// Helper method to execute async operations in synchronous contexts (like constructors, Mapster callbacks, LINQ queries).
    /// This prevents deadlocks by running async code on a thread pool thread, avoiding synchronization context capture.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="asyncFunc">The async function to execute.</param>
    /// <returns>The result of the async operation.</returns>
    protected static TResult RunAsync<TResult>(Func<Task<TResult>> asyncFunc)
    {
        return Task.Run(async () => await asyncFunc().ConfigureAwait(false))
            .GetAwaiter().GetResult();
    }

    #endregion
}