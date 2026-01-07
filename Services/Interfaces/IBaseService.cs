using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Corno.Web.Services.Interfaces;

public interface IBaseService<TEntity> : ICornoService<TEntity>
    where TEntity : class
{
    #region -- Properties --

    //IGenericRepository<TEntity> EntityRepository { get; }

    #endregion

    #region -- General Methods --
    Task<TEntity> GetByCodeAsync(string code);
    Task<TEntity> GetByStatusAsync(string status);

    Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        string includeProperties = "");

    Task<TEntity> GetExistingAsync(TEntity entity);
    Task<bool> ExistsAsync(string code);

    #endregion

    #region -- Public Import Methods --
    // OLD METHOD - REMOVED: ImportAsync with IBaseProgressService
    // This should be updated to use the new common import module
    // Task ImportAsync(string filePath, IBaseProgressService progressService,
    //     string miscMaster = null);
    #endregion

    #region -- Corno Methods --

    Task<int> GetNextIdAsync();
    Task<int> GetNextSerialNoAsync();
    Task<int> GetNextSerialNoAsync(int companyId);
    Task<int> GetCurrentSequenceAsync(string sequenceName);
    Task<int> GetNextSequenceAsync(string sequenceName);

    #endregion
}