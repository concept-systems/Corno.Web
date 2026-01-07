using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Corno.Web.Repository.Interfaces;
using Corno.Web.Services.Interfaces;

namespace Corno.Web.Services;

public class CornoService<TEntity> : ICornoService<TEntity>
    where TEntity : class, new()
{
    #region -- Constructors --
    protected CornoService(IGenericRepository<TEntity> genericRepository)
    {
        _genericRepository = genericRepository;
    }

    #endregion

    #region -- Data Members --

    //private readonly IUnitOfWork _unitOfWork;
    private readonly IGenericRepository<TEntity> _genericRepository;

    #endregion

    #region -- General Methods --
    public void SetIncludes(string includes)
    {
        _genericRepository.SetIncludes(includes);
    }

    /*public void RefreshAllEntities()
    {
        _genericRepository.RefreshAllEntities();
    }

    public bool HasChanges()
    {
        return _genericRepository.HasChanges();
    }*/


    public async Task<IQueryable> ListAsync()
    {
        var list = await _genericRepository.ListAsync().ConfigureAwait(false);
        return list.AsQueryable();
    }

    public virtual IQueryable<TEntity> GetQuery()
    {
        return _genericRepository.GetQuery();
    }

    /*public virtual async Task<List<TDest>> GetAsync<TDest>(Expression<Func<TEntity, bool>> filter = null,
        Expression<Func<TEntity, TDest>> select = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null)
    {
        return await _genericRepository.GetAsync(filter, select, orderBy);
    }*/

    public async Task<List<TDest>> GetAsync<TDest>(
        Expression<Func<TEntity, bool>> filter = null,
        Expression<Func<TEntity, TDest>> select = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, bool ignoreInclude = false)
    {
        return await _genericRepository.GetAsync(filter, select, orderBy, ignoreInclude).ConfigureAwait(false);
    }

    public virtual async Task<TDest> FirstOrDefaultAsync<TDest>(Expression<Func<TEntity, bool>> filter = null,
        Expression<Func<TEntity, TDest>> select = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null)
    {
        return await _genericRepository.FirstOrDefaultAsync(filter, select, orderBy).ConfigureAwait(false);
    }

    /*public virtual async Task<TDest> FirstOrDefaultAsync<TDest>(
        Expression<Func<TEntity, bool>> filter = null,
        Expression<Func<TEntity, TDest>> select = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null)
    {
        return await _genericRepository.FirstOrDefaultAsync(filter, select, orderBy);
    }*/

    /*public virtual TDest LastOrDefault<TDest>(Expression<Func<TEntity, bool>> filter = null,
        Expression<Func<TEntity, TDest>> select = null)
    {
        return _genericRepository.Get(filter, select).LastOrDefault();
    }*/

    public virtual async Task<TEntity> GetByIdAsync(object id)
    {
        return await _genericRepository.GetByIdAsync(id).ConfigureAwait(false);
    }

    public virtual IQueryable<TEntity> GetByIds(IEnumerable<object> ids)
    {
        throw new Exception("GetById not implemented");
    }

    /*public IQueryable<TEntity> Find(Func<TEntity, bool> predicate)
    {
        return _genericRepository.Fin(predicate);
    }*/

    public virtual async Task AddAsync(TEntity entity)
    {
        await _genericRepository.AddAsync(entity).ConfigureAwait(false);
    }

    public virtual async Task AddAndSaveAsync(TEntity entity)
    {
        await AddAsync(entity).ConfigureAwait(false);
        await SaveAsync().ConfigureAwait(false);
    }

    public virtual async Task AddRangeAsync(IEnumerable<TEntity> entities)
    {
        await _genericRepository.AddRangeAsync(entities).ConfigureAwait(false);
    }

    public virtual async Task AddRangeAndSaveAsync(IEnumerable<TEntity> entities)
    {
        await AddRangeAsync(entities).ConfigureAwait(false);
        await SaveAsync().ConfigureAwait(false);
    }

    public virtual async Task UpdateAsync(TEntity entityToUpdate)
    {
        await _genericRepository.UpdateAsync(entityToUpdate).ConfigureAwait(false);
    }

    public virtual async Task UpdateAndSaveAsync(TEntity entityToUpdate)
    {
        await UpdateAsync(entityToUpdate).ConfigureAwait(false);
        await SaveAsync().ConfigureAwait(false);
    }

    public virtual async Task UpdateAsync(TEntity entityToUpdate, string id)
    {
        await _genericRepository.UpdateAsync(entityToUpdate, id).ConfigureAwait(false);
    }

    public virtual async Task UpdateRangeAsync(IEnumerable<TEntity> entities)
    {
        await _genericRepository.UpdateRangeAsync(entities).ConfigureAwait(false);
    }

    public virtual async Task UpdateRangeAndSaveAsync(IEnumerable<TEntity> entities)
    {
        await UpdateRangeAsync(entities).ConfigureAwait(false);
        await SaveAsync().ConfigureAwait(false);
    }

    public virtual async Task UpdateRangeAsync(IEnumerable<TEntity> entities, string id, string includeProperties)
    {
        await _genericRepository.UpdateRangeAsync(entities, id).ConfigureAwait(false);
    }

    public virtual async Task DeleteAsync(object id)
    {
        await _genericRepository.DeleteAsync(id).ConfigureAwait(false);
    }

    public virtual async Task DeleteAsync(TEntity entityToDelete)
    {
        await _genericRepository.DeleteAsync(entityToDelete).ConfigureAwait(false);
    }

    public virtual async Task DeleteRangeAsync(IEnumerable<TEntity> entities)
    {
        await _genericRepository.DeleteRangeAsync(entities).ConfigureAwait(false);
    }

    /*public TEntity Create()
    {
        return _genericRepository.Create();
    }*/

    public async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await _genericRepository.CountAsync(predicate).ConfigureAwait(false);
    }

    public async Task<int> MinAsync(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, int?>> selector)
    {
        return await _genericRepository.MinAsync(filter, selector).ConfigureAwait(false);
    }
    public async Task<int> MaxAsync(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, int?>> selector)
    {
        return await _genericRepository.MaxAsync(filter, selector).ConfigureAwait(false);
    }
    public async Task<int> MissingAsync(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, int?>> selector)
    {
        return await _genericRepository.MaxAsync(filter, selector).ConfigureAwait(false);
    }

    public virtual async Task<bool> ExistsAsync(int id)
    {
        var entity = await _genericRepository.GetByIdAsync(id).ConfigureAwait(false);
        return entity != null;
    }

    public async Task<IList<T>> ExecuteStoredProcedureAsync<T>(string procedureName, params object[] parameters)
    {
        return await _genericRepository.ExecuteStoredProcedureAsync<T>(procedureName, parameters).ConfigureAwait(false);
    }

    public virtual TEntity Trim(TEntity entity)
    {
        var type = typeof(TEntity);

        var properties = TypeDescriptor.GetProperties(type).Cast<PropertyDescriptor>()
            .Where(p => p.PropertyType == typeof(string));

        foreach (var property in properties)
        {
            var value = (string)property.GetValue(entity);
            if (string.IsNullOrEmpty(value)) continue;
            value = value.TrimEnd();
            property.SetValue(entity, value);
        }
        return entity;
    }

    public virtual IEnumerable<TEntity> Trim(IEnumerable<TEntity> collection)
    {

        if (null == collection) return null;

        collection = collection.ToList();

        var type = typeof(TEntity);

        var properties = TypeDescriptor.GetProperties(type).Cast<PropertyDescriptor>()
            .Where(p => p.PropertyType == typeof(string)).ToList();

        foreach (var entity in collection)
        {
            foreach (var property in properties)
            {
                var value = (string)property.GetValue(entity);
                if (string.IsNullOrEmpty(value)) continue;
                value = value.TrimEnd();
                property.SetValue(entity, value);
            }
        }

        return collection;
    }



    public virtual async Task SaveAsync()
    {
        await _genericRepository.SaveAsync().ConfigureAwait(false);
    }


    public virtual bool ValidateModel(TEntity model)
    {
        return true;
    }

    public virtual void Dispose(bool disposing)
    {
        /*if (!_disposed)
            if (disposing)
                _unitOfWork.Dispose();
        _disposed = true;*/
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    //IQueryable ICornoService<TEntity>.GetQuery()
    //{
    //    return GetQuery();
    //    //throw new NotImplementedException();
    //}

    public virtual string GetColumnHeader(string fieldName)
    {
        return fieldName;
    }

    public virtual List<string> GetListColumns()
    {
        return [];
    }

    #endregion
}