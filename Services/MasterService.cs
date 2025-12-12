using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Corno.Web.Dtos;
using Corno.Web.Globals;
using Corno.Web.Logger;
using Corno.Web.Models;
using Corno.Web.Models.Masters;
using Corno.Web.Repository.Interfaces;
using Corno.Web.Services.File.Interfaces;
using Corno.Web.Services.Interfaces;
using Corno.Web.Services.Progress.Interfaces;
using Corno.Web.Windsor;

namespace Corno.Web.Services;

public class MasterService<TEntity> : BaseService<TEntity>, IMasterService<TEntity>
    where TEntity : MasterModel, new()
{
    #region -- Constructors --
    public MasterService(IGenericRepository<TEntity> genericRepository) : base(genericRepository)
    {
    }
    #endregion

    #region -- Protected Methods --
    protected virtual void UpdateFields(TEntity entity, TEntity newEntity)
    {
        newEntity.Name = entity.Name;
        newEntity.Description = entity.Description;
    }
    #endregion

    #region -- Public Methods --
    public override async Task AddAsync(TEntity entity)
    {
        if (!(entity is MasterModel masterModel))
            throw new Exception("Invalid Entity");
        if (string.IsNullOrEmpty(masterModel.Name))
            throw new Exception("Name is required.");

        await base.AddAsync(entity).ConfigureAwait(false);
    }

    public virtual async Task<List<MasterDto>> GetCommonListAsync(Expression<Func<TEntity, bool>> filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        string includeProperties = "")
    {
        filter = filter.And(x => x.Status == StatusConstants.Active);
        return await GetAsync(filter, m => new MasterDto
        {
            Id = m.Id,
            Code = m.Code,
            Name = m.Name,
            Description = m.Description
        }, orderBy).ConfigureAwait(false);
    }

    public virtual async Task<List<MasterDto>> GetViewModelListAsync(Expression<Func<TEntity, bool>> filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        string includeProperties = "")
    {
        filter = filter.And(x => x.Status == StatusConstants.Active);
        return await GetAsync(filter, m => new MasterDto
        {
            Id = m.Id,
            Code = m.Code,
            Name = m.Name,
            Description = m.Description,
            NameWithCode = m.Code + " - " + m.Name,
            NameWithId = "(" + m.Id + ")" + " - " + m.Name
        }, orderBy).ConfigureAwait(false);
    }

    public virtual async Task<List<MasterDto>> GetCommonIdNameListAsync()
    {
        return await GetAsync(x => x.Status == StatusConstants.Active,
            m => new MasterDto
            {
                Id = m.Id,
                Name = m.Name
            }).ConfigureAwait(false);
    }

    public virtual async Task<List<MasterDto>> GetIdNameListAsync(Expression<Func<TEntity, bool>> filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        string includeProperties = "")
    {
        filter = filter.And(x => x.Status == StatusConstants.Active);
        return await GetAsync(filter, m => new MasterDto
        {
            Id = m.Id,
            Name = m.Name,
        }, orderBy).ConfigureAwait(false);
    }

    public virtual async Task<TEntity> GetByNameAsync(string name, bool bTracking = false)
    {
        return await FirstOrDefaultAsync(p => p.Name == name, p => p).ConfigureAwait(false);
    }

    public virtual async Task<string> GetCodeAsync(int id)
    {
        return await FirstOrDefaultAsync(m => m.Id == id, m => m.Code).ConfigureAwait(false);
    }

    public virtual async Task<string> GetNameAsync(int id)
    {
        return await FirstOrDefaultAsync(m => m.Id == id, m => m.Name).ConfigureAwait(false);
    }

    public virtual async Task<string> GetNameAsync(string code)
    {
        return await FirstOrDefaultAsync(m => m.Code == code, m => m.Name).ConfigureAwait(false);
    }

    public virtual async Task<string> GetNameWithCodeAsync(int id)
    {
        return await FirstOrDefaultAsync(m => m.Id == id, m => m.Code + " - " + m.Name).ConfigureAwait(false);
    }

    public async Task<string> GetNameAsync(Expression<Func<TEntity, bool>> filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null)
    {
        return await FirstOrDefaultAsync(filter, m => m.Name, orderBy).ConfigureAwait(false);
    }

    public virtual async Task<List<string>> GetNamesAsync(Expression<Func<TEntity, bool>> filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        string includeProperties = "")
    {
        if (null != filter)
            return await GetAsync(filter, m => m.Name, orderBy).ConfigureAwait(false);

        return await GetAsync(null, m => m.Name, orderBy).ConfigureAwait(false);
    }

    public virtual async Task<MasterDto> GetViewModelAsync(int id)
    {
        return await FirstOrDefaultAsync(m => m.Id == id, m => new MasterDto
        {
            Id = m.Id,
            Code = m.Code,
            Name = m.Name,
            Description = m.Description,
            NameWithCode = m.Code + " - " + m.Name,
            NameWithId = "(" + m.Id + ")" + " - " + m.Name
        }).ConfigureAwait(false);
    }

    public virtual async Task<TEntity> CreateObjectAsync(string code, string name, int serialNo = 0)
    {
        if (null == code) return null;

        var nextSerialNo = serialNo > 0 ? serialNo : await GetNextSerialNoAsync().ConfigureAwait(false);
        return new TEntity
        {
            SerialNo = nextSerialNo,
            Code = code.Trim(),
            Name = string.IsNullOrEmpty(name) ? code.Trim() : name.Trim()
        };
    }

    public virtual async Task<TEntity> GetOrCreateAsync(string code, string name, bool bSave = true)
    {
        if (null == code) return null;

        if (!string.IsNullOrEmpty(code))
            code = code.Trim();
        if (!string.IsNullOrEmpty(name))
            name = name.Trim();
        var entity = await GetByCodeAsync(code).ConfigureAwait(false);
        if (null != entity)
            return entity;

        entity = await CreateObjectAsync(code, name).ConfigureAwait(false);

        if (false == bSave)
            await AddAsync(entity).ConfigureAwait(false);
        else
            await AddAndSaveAsync(entity).ConfigureAwait(false);

        return entity;
    }

    public virtual async Task<TEntity> GetOrCreateByNameAsync(string name)
    {
        if (string.IsNullOrEmpty(name)) return null;

        var entities = await GetAsync(e => e.Name == name, e => e).ConfigureAwait(false);
        var entity = entities.FirstOrDefault();
        if (null != entity)
            return entity;

        entity = await CreateObjectAsync(name, name).ConfigureAwait(false);
        await AddAndSaveAsync(entity).ConfigureAwait(false);

        return entity;
    }

    public override async Task ImportAsync(string filePath, IBaseProgressService progressService,
        string miscType = null)
    {
        try
        {
            var excelFileService = Bootstrapper.Get<IExcelFileService<TEntity>>();
            var entities = excelFileService.Read(filePath, 0).ToList();

            progressService.Initialize(filePath, 0, entities.Count(), 1);

            foreach (var entity in entities)
            {
                if (entity is MiscMaster master && !string.IsNullOrEmpty(miscType))
                    master.MiscType = miscType;

                var existing = await GetExistingAsync(entity).ConfigureAwait(false);
                if (null != existing)
                {
                    UpdateFields(entity, existing);
                    await UpdateAndSaveAsync(existing).ConfigureAwait(false);

                    progressService.Report(0, 1, 0);
                    continue;
                }

                entity.SerialNo = await GetNextSerialNoAsync().ConfigureAwait(false);
                UpdateFields(entity, entity);

                var photoPathField = entity.GetType().GetProperty(FieldConstants.PhotoPath);
                if (photoPathField != null)
                {
                    var imagePath = photoPathField.GetValue(entity)?.ToString();
                    if (!string.IsNullOrEmpty(imagePath) && System.IO.File.Exists(imagePath))
                    {
                        var photoField = entity.GetType().GetProperty(FieldConstants.Photo);
                        if (null != photoField)
                        {
                            /*var data = ImageHelper.GetBytes(imagePath);
                            photoField.SetValue(entity, data);*/
                        }
                    }
                }

                await AddAndSaveAsync(entity).ConfigureAwait(false);

                progressService.Report(1, 0, 0);
            }
        }
        catch (Exception exception)
        {
            LogHandler.LogInfo(exception);
            throw;
        }
    }

    public virtual async Task ExportAsync(string filePath, IBaseProgressService progressService)
    {
        var excelFileService = Bootstrapper.Get<IExcelFileService<TEntity>>();
        var records = await GetAsync<TEntity>().ConfigureAwait(false);
        excelFileService.Save(filePath, "Sheet1", records);
    }
    #endregion
}

internal class SubstExpressionVisitor : ExpressionVisitor
{
    public readonly Dictionary<Expression, Expression> Subst = new Dictionary<Expression, Expression>();

    protected override Expression VisitParameter(ParameterExpression node)
    {
        return Subst.TryGetValue(node, out var newValue) ? newValue : node;
    }
}

public static class PredicateBuilder
{
    public static Expression<Func<T, bool>> False<T>() { return f => false; }

    public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> a, Expression<Func<T, bool>> b)
    {
        if (null == a) return b;

        var p = a.Parameters[0];
        var visitor = new SubstExpressionVisitor { Subst = { [b.Parameters[0]] = p } };
        Expression body = Expression.AndAlso(a.Body, visitor.Visit(b.Body));
        return Expression.Lambda<Func<T, bool>>(body, p);
    }

    public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> a, Expression<Func<T, bool>> b)
    {
        if (null == a) return b;

        var p = a.Parameters[0];
        var visitor = new SubstExpressionVisitor { Subst = { [b.Parameters[0]] = p } };
        Expression body = Expression.OrElse(a.Body, visitor.Visit(b.Body));
        return Expression.Lambda<Func<T, bool>>(body, p);
    }

    public static Expression<Func<T, bool>> Or_1<T>(this Expression<Func<T, bool>> expr1,
        Expression<Func<T, bool>> expr2)
    {
        var invokedExpr = Expression.Invoke(expr2, expr1.Parameters);
        return Expression.Lambda<Func<T, bool>>
            (Expression.OrElse(expr1.Body, invokedExpr), expr1.Parameters);
    }

    public static Expression<Func<T, bool>> And_1<T>(this Expression<Func<T, bool>> expr1,
        Expression<Func<T, bool>> expr2)
    {
        var invokedExpr = Expression.Invoke(expr2, expr1.Parameters);
        return Expression.Lambda<Func<T, bool>>
            (Expression.AndAlso(expr1.Body, invokedExpr), expr1.Parameters);
    }
}