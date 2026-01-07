using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Corno.Web.Services.Interfaces;

public interface ICornoService<TEntity> : IService
    where TEntity : class {
    
    #region -- General Methods --

    void SetIncludes(string includes);
    IQueryable<TEntity> GetQuery(); // Keep synchronous as it returns IQueryable (lazy evaluation)

    Task<IQueryable> ListAsync();
    Task<List<TDest>> GetAsync<TDest>(
        Expression<Func<TEntity, bool>> filter = null,
        Expression<Func<TEntity, TDest>> select = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, bool ignoreInclude = false);

    Task<TDest> FirstOrDefaultAsync<TDest>(
        Expression<Func<TEntity, bool>> filter = null,
        Expression<Func<TEntity, TDest>> select = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null);

    Task<TEntity> GetByIdAsync(object id);

    Task AddAsync(TEntity entity);
    Task AddAndSaveAsync(TEntity entity);
    Task AddRangeAsync(IEnumerable<TEntity> entities);
    Task AddRangeAndSaveAsync(IEnumerable<TEntity> entities);

    Task UpdateAsync(TEntity entity);
    Task UpdateAndSaveAsync(TEntity entity);
    Task UpdateAsync(TEntity entityToUpdate, string id);
    Task UpdateRangeAsync(IEnumerable<TEntity> entities);
    Task UpdateRangeAsync(IEnumerable<TEntity> entities, string id, string includeProperties);
    Task UpdateRangeAndSaveAsync(IEnumerable<TEntity> entities);

    Task DeleteAsync(object id);
    Task DeleteAsync(TEntity entityToDelete);
    Task DeleteRangeAsync(IEnumerable<TEntity> entities);

    Task<bool> ExistsAsync(int id);

    Task<IList<T>> ExecuteStoredProcedureAsync<T>(string procedureName, params object[] parameters);

    TEntity Trim(TEntity entity);
    IEnumerable<TEntity> Trim(IEnumerable<TEntity> collection);

    // Aggregate methods
    Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate);
    Task<int> MinAsync(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, int?>> selector);
    Task<int> MaxAsync(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, int?>> selector);

    Task SaveAsync();
    void Dispose(bool disposing);

    bool ValidateModel(TEntity model);

    string GetColumnHeader(string fieldName);
    List<string> GetListColumns();

    #endregion
}