using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Corno.Web.Dtos;

namespace Corno.Web.Services.Interfaces;

public interface IMasterService<TEntity> : IBaseService<TEntity>
    where TEntity : class
{
    #region -- Methods --
    Task<List<MasterDto>> GetCommonListAsync(Expression<Func<TEntity, bool>> filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        string includeProperties = "");
    Task<List<MasterDto>> GetViewModelListAsync(Expression<Func<TEntity, bool>> filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        string includeProperties = "");
    Task<List<MasterDto>> GetCommonIdNameListAsync();
    Task<List<MasterDto>> GetIdNameListAsync(Expression<Func<TEntity, bool>> filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        string includeProperties = "");

    Task<TEntity> GetByNameAsync(string name, bool bTracking = false);
    Task<string> GetCodeAsync(int id);
    Task<string> GetNameAsync(int id);
    Task<string> GetNameAsync(string code);
    Task<string> GetNameWithCodeAsync(int id);

    Task<List<string>> GetNamesAsync(Expression<Func<TEntity, bool>> filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        string includeProperties = "");

    Task<MasterDto> GetViewModelAsync(int id);
    Task<TEntity> CreateObjectAsync(string code, string name, int serialNo = 0);
    Task<TEntity> GetOrCreateAsync(string code, string name, bool bSave = true);
    Task<TEntity> GetOrCreateByNameAsync(string name);

    Task ExportAsync(string filePath);

    #endregion
}