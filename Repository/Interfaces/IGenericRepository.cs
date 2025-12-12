using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Corno.Web.Repository.Interfaces;

public interface IGenericRepository<TEntity> where TEntity : class
{
    // Methods
    void SetIncludes(string includes);
    IQueryable<TEntity> GetQuery(); // Keep synchronous as it returns IQueryable (lazy evaluation)

    // Async methods
    Task<List<TEntity>> ListAsync();
    Task<List<TDest>> GetAsync<TDest>(Expression<Func<TEntity, bool>> filter = null,
        Expression<Func<TEntity, TDest>> select = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, bool ignoreInclude = false);

    Task<TDest> FirstOrDefaultAsync<TDest>(
        Expression<Func<TEntity, bool>> filter = null,
        Expression<Func<TEntity, TDest>> select = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null);
    Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate);
    Task<int> MinAsync(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, int?>> selector);
    Task<int> MaxAsync(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, int?>> selector);
    Task<TEntity> GetByIdAsync(object id);
    Task<TEntity> GetByIdNoTrackingAsync(object id);
    Task AddAsync(TEntity entity);
    Task AddRangeAsync(IEnumerable<TEntity> entities);
    Task DeleteAsync(object id);
    Task DeleteAsync(TEntity entity);
    Task DeleteRangeAsync(IEnumerable<TEntity> entities);
    Task UpdateAsync(TEntity entity);
    Task UpdateAsync(TEntity entityToUpdate, string id);
    Task UpdateRangeAsync(IEnumerable<TEntity> entities);
    Task UpdateRangeAsync(IEnumerable<TEntity> entities, string id);
    Task SaveAsync();
    Task<int> GetNextSequenceAsync(string sequenceName);
    Task<int> GetCurrentSequenceAsync(string sequenceName);

    Task<IList<T>> ExecuteStoredProcedureAsync<T>(string procedureName, params object[] parameters);
}

/*public interface IGenericRepository<TEntity> //: IDisposable
    where TEntity : class
{
    #region -- Methods --

    void SetIncludes(string includes);
    bool HasIncludes();

    void RefreshAllEntities();
    bool HasChanges();

    TEntity Create();
    IQueryable<TEntity> List();

    IQueryable<TEntity> GetQuery();
    IQueryable<TDest> Get<TDest>(Expression<Func<TEntity, bool>> filter = null,
        Expression<Func<TEntity, TDest>> select = null, 
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null);

    TEntity GetById(object id, bool bTracking = false);
    IQueryable<TEntity> Find(Func<TEntity, bool> predicate);
    void Add(TEntity entity);
    void AddRange(IEnumerable<TEntity> entities);
    void DeleteRange(IEnumerable<TEntity> entities);
    void Delete(object id);
    void Delete(TEntity entityToDelete);
    void Update(TEntity entity);
    void Update(TEntity entityToUpdate, string id);
    void UpdateRange(IEnumerable<TEntity> entities);
    void UpdateRange(IEnumerable<TEntity> entities, string id);
    void Save();
    int GetCurrentSequence(string sequenceName);
    int GetNextSequence(string sequenceName);

    // Aggregate functions
    int Count(Func<TEntity, bool> predicate);
    Task<int> MinAsync(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, int?>> selector)
    int Min(Expression<Func<TEntity, bool>> filter, Func<TEntity, int?> predicate);
    int Max(Expression<Func<TEntity, bool>> filter, Func<TEntity, int?> predicate);

    #endregion;
}*/