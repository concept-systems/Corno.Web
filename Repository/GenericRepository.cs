using Corno.Web.Globals;
using Corno.Web.Repository.Interfaces;
using Corno.Web.Windsor.Context;
using Mapster;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Corno.Web.Models.Base;
using Microsoft.AspNet.Identity.EntityFramework;
using static System.Linq.Expressions.Expression;

namespace Corno.Web.Repository;

public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
{
    #region -- Constructors --
    public GenericRepository(BaseDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _dbSet = _dbContext.Set<TEntity>();
        //_includeProperties = string.Empty;
    }

    #endregion

    #region -- Data Members --

    private readonly BaseDbContext _dbContext;
    private readonly DbSet<TEntity> _dbSet;

    //private string _includeProperties;
    private string[] _cachedIncludeProperties;

    // Cached reflection results for performance
    private static readonly Dictionary<Type, PropertyInfo[]> KeyPropertiesCache = new();
    private static readonly Dictionary<Type, PropertyInfo[]> CollectionPropertiesCache = new();
    private static readonly Dictionary<Type, Func<TEntity, object[]>> KeyValueExtractorsCache = new();

    //private bool _disposed;

    #endregion

    // Methods

    public void SetIncludes(string includes)
    {
        //_includeProperties = includes;
        _cachedIncludeProperties = string.IsNullOrEmpty(includes) ? [] :
            includes.Split([','], StringSplitOptions.RemoveEmptyEntries)
                      .Select(i => i.Trim())
                      .ToArray();
    }

    public IQueryable<TEntity> GetQuery() => _dbSet.AsQueryable().AsNoTracking();

    /// <summary>
    /// Gets cached key properties for the entity type
    /// </summary>
    private PropertyInfo[] GetKeyProperties()
    {
        var entityType = typeof(TEntity);
        if (!KeyPropertiesCache.TryGetValue(entityType, out var keyProperties))
        {
            var keyPropertiesList = new List<PropertyInfo>();
            
            // First, get properties with [Key] attribute
            var keyAttributeProperties = entityType.GetProperties()
                .Where(p => Attribute.IsDefined(p, typeof(KeyAttribute)))
                .ToList();
            keyPropertiesList.AddRange(keyAttributeProperties);

            // Check if entity is derived from Identity base classes (IdentityUser, IdentityRole, etc.)
            if (IsIdentityEntity(entityType))
            {
                // Get the Id property from the entity type or its base types
                var idProperty = GetIdentityIdProperty(entityType);
                if (idProperty != null && !keyPropertiesList.Any(p => p.Name == idProperty.Name && p.DeclaringType == idProperty.DeclaringType))
                {
                    keyPropertiesList.Add(idProperty);
                }
            }

            keyProperties = keyPropertiesList.ToArray();

            if (!keyProperties.Any())
            {
                throw new Exception($"GenericRepositoryCore : No [Key] attributes found on {entityType.Name}. Composite key support requires explicit [Key] attributes.");
            }

            KeyPropertiesCache[entityType] = keyProperties;
        }
        return keyProperties;
    }

    /// <summary>
    /// Checks if the entity type is derived from Identity base classes
    /// </summary>
    private static bool IsIdentityEntity(Type entityType)
    {
        var currentType = entityType;
        while (currentType != null && currentType != typeof(object))
        {
            // Check for non-generic Identity types (IdentityUser, IdentityRole)
            if (typeof(IdentityUser).IsAssignableFrom(currentType) ||
                typeof(IdentityRole).IsAssignableFrom(currentType))
            {
                return true;
            }
            
            // Check for generic Identity types
            if (currentType.IsGenericType)
            {
                var genericTypeDef = currentType.GetGenericTypeDefinition();
                
                // Check if it's a generic Identity type
                if (genericTypeDef == typeof(IdentityUser<,,,>) ||
                    genericTypeDef == typeof(IdentityRole<,>) ||
                    genericTypeDef == typeof(IdentityUserLogin<>) ||
                    genericTypeDef == typeof(IdentityUserRole<>) ||
                    genericTypeDef == typeof(IdentityUserClaim<>))
                {
                    return true;
                }
                
                // Check if any base type or interface is a generic Identity type
                var baseType = currentType.BaseType;
                if (baseType != null && baseType.IsGenericType)
                {
                    var baseGenericTypeDef = baseType.GetGenericTypeDefinition();
                    if (baseGenericTypeDef == typeof(IdentityUser<,,,>) ||
                        baseGenericTypeDef == typeof(IdentityRole<,>) ||
                        baseGenericTypeDef == typeof(IdentityUserLogin<>) ||
                        baseGenericTypeDef == typeof(IdentityUserRole<>) ||
                        baseGenericTypeDef == typeof(IdentityUserClaim<>))
                    {
                        return true;
                    }
                }
            }
            
            currentType = currentType.BaseType;
        }
        
        return false;
    }

    /// <summary>
    /// Gets the Id property from Identity entity types
    /// </summary>
    private static PropertyInfo GetIdentityIdProperty(Type entityType)
    {
        var currentType = entityType;
        while (currentType != null && currentType != typeof(object))
        {
            var idProperty = currentType.GetProperty("Id", BindingFlags.Public | BindingFlags.Instance);
            if (idProperty != null && idProperty.CanRead)
            {
                return idProperty;
            }
            currentType = currentType.BaseType;
        }
        return null;
    }

    public virtual async Task UpdateAsync(TEntity entityToUpdate)
    {
        await UpdateAsync(entityToUpdate, FieldConstants.Id);
    }

    public virtual async Task UpdateAsync(TEntity entityToUpdate, string primaryKeyName)
    {
        var entry = _dbContext.Entry(entityToUpdate);

        if (entry.State != EntityState.Detached) return;

        // Get cached key properties
        var keyProperties = GetKeyProperties();

        // Extract key values using cached extractor if available
        var keyValues = ExtractKeyValues(entityToUpdate, keyProperties);

        var set = _dbContext.Set<TEntity>();
        var attachedEntity = await set.FindAsync(keyValues);
        if (attachedEntity != null)
        {
            set.Attach(attachedEntity); // Attach it to the context
            var attachedEntry = _dbContext.Entry(attachedEntity); // Re-fetch the entry

            attachedEntry.CurrentValues.SetValues(entityToUpdate);

            //// Fix FK/navigation conflicts for ALL entities - USE attachedEntry, not entry!
            //var parentMenuIdProperty = attachedEntry.Property("ParentMenuId");
            //if (parentMenuIdProperty != null)
            //{
            //    var newFkValue = entityToUpdate.GetType().GetProperty("ParentMenuId")?.GetValue(entityToUpdate);
            //    parentMenuIdProperty.CurrentValue = newFkValue;
            //    parentMenuIdProperty.IsModified = true;  // FORCE UPDATE
            //}

            if (_cachedIncludeProperties != null && _cachedIncludeProperties.Length > 0)
            {
                foreach (var include in _cachedIncludeProperties)
                {
                    var collection = attachedEntry.Collection(include);
                    if (!collection.IsLoaded)
                    {
                        await collection.LoadAsync();
                    }
                }
            }

            //var isUpdated = (attachedEntry.Entity as CornoModel)?.UpdateDetails(entityToUpdate as CornoModel);
            //if (false == (isUpdated ?? false))
            {
                //entityToUpdate.Adapt(attachedEntry.Entity);

                // Automatically sync all detail collections for any master-detail entity
                await SyncDetailCollectionsAsync(attachedEntry.Entity, entityToUpdate, attachedEntry);
            }
        }
        else
        {
            // Entity not found in DB, treat as new or modified
            set.Attach(entityToUpdate);
            entry.State = EntityState.Modified;
        }
    }

    /// <summary>
    /// Extracts key values from an entity using cached key properties
    /// </summary>
    private object[] ExtractKeyValues(TEntity entity, PropertyInfo[] keyProperties)
    {
        var entityType = typeof(TEntity);

        // Try to get cached extractor
        if (!KeyValueExtractorsCache.TryGetValue(entityType, out var extractor))
        {
            // Build a compiled expression for faster extraction
            var parameter = Parameter(typeof(TEntity), "e");
            var propertyAccesses = keyProperties.Select(prop =>
                Convert(Property(parameter, prop), typeof(object))
            ).ToArray();

            var arrayInit = NewArrayInit(typeof(object), propertyAccesses);
            var lambda = Lambda<Func<TEntity, object[]>>(arrayInit, parameter);
            extractor = lambda.Compile();
            KeyValueExtractorsCache[entityType] = extractor;
        }

        return extractor(entity);
    }

    /// <summary>
    /// Gets cached collection properties for an entity type
    /// </summary>
    private static PropertyInfo[] GetCollectionProperties(Type entityType)
    {
        if (!CollectionPropertiesCache.TryGetValue(entityType, out var properties))
        {
            properties = entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && p.CanWrite && IsCollectionType(p.PropertyType, out _))
                .Where(p => IsCollectionType(p.PropertyType, out var elementType) && typeof(BaseModel).IsAssignableFrom(elementType))
                .ToArray();
            CollectionPropertiesCache[entityType] = properties;
        }
        return properties;
    }

    /// <summary>
    /// Automatically detects and syncs all detail collections (collections of BaseModel-derived entities)
    /// for any master-detail entity when Adapt is used instead of UpdateDetails
    /// </summary>
    private void SyncDetailCollections(object attachedEntity, object entityToUpdate, DbEntityEntry attachedEntry)
    {
        var entityType = attachedEntity.GetType();
        // Use the base type for reflection to avoid proxy type issues
        if (entityType.BaseType != null && entityType.Namespace != null && entityType.Namespace.Contains("DynamicProxies"))
        {
            entityType = entityType.BaseType;
        }

        var properties = GetCollectionProperties(entityType);

        foreach (var property in properties)
        {
            // Check if property is a collection type
            if (!IsCollectionType(property.PropertyType, out var elementType))
                continue;

            // Check if collection elements are BaseModel-derived
            if (!typeof(BaseModel).IsAssignableFrom(elementType))
                continue;

            // Get collection values using reflection - don't cast to ICollection<BaseModel> due to covariance issues
            var attachedCollectionObj = property.GetValue(attachedEntity, null);
            var newCollectionObj = property.GetValue(entityToUpdate, null);

            if (attachedCollectionObj == null || newCollectionObj == null)
                continue;

            // Load the collection if not already loaded
            try
            {
                var collection = attachedEntry.Collection(property.Name);
                if (!collection.IsLoaded)
                {
                    collection.Load();
                }
            }
            catch
            {
                // Collection might not be a navigation property, skip it
                continue;
            }

            // Sync the collection - pass the actual collection objects
            SyncCollection(attachedCollectionObj, newCollectionObj, elementType, property.Name);
        }
    }

    /// <summary>
    /// Automatically detects and syncs all detail collections (collections of BaseModel-derived entities)
    /// for any master-detail entity when Adapt is used instead of UpdateDetails (Async version)
    /// </summary>
    private async Task SyncDetailCollectionsAsync(object attachedEntity, object entityToUpdate, DbEntityEntry attachedEntry)
    {
        var entityType = attachedEntity.GetType();
        // Use the base type for reflection to avoid proxy type issues
        if (entityType.BaseType != null && entityType.Namespace != null && entityType.Namespace.Contains("DynamicProxies"))
        {
            entityType = entityType.BaseType;
        }

        var properties = GetCollectionProperties(entityType);

        foreach (var property in properties)
        {
            // Get element type (already validated in GetCollectionProperties)
            IsCollectionType(property.PropertyType, out var elementType);

            // Get collection values using reflection - don't cast to ICollection<BaseModel> due to covariance issues
            var attachedCollectionObj = property.GetValue(attachedEntity, null);
            var newCollectionObj = property.GetValue(entityToUpdate, null);

            if (attachedCollectionObj == null || newCollectionObj == null)
                continue;

            // Load the collection if not already loaded
            try
            {
                var collection = attachedEntry.Collection(property.Name);
                if (!collection.IsLoaded)
                {
                    await collection.LoadAsync();
                }
            }
            catch
            {
                // Collection might not be a navigation property, skip it
                continue;
            }

            // Sync the collection - pass the actual collection objects
            SyncCollection(attachedCollectionObj, newCollectionObj, elementType, property.Name);
        }
    }

    /// <summary>
    /// Checks if a type is a collection and extracts the element type
    /// </summary>
    private static bool IsCollectionType(Type type, out Type elementType)
    {
        elementType = null;

        // Check for ICollection<T>
        if (type.IsGenericType)
        {
            var genericTypeDef = type.GetGenericTypeDefinition();
            if (genericTypeDef == typeof(ICollection<>) ||
                genericTypeDef == typeof(List<>) ||
                genericTypeDef == typeof(IList<>))
            {
                elementType = type.GetGenericArguments()[0];
                return true;
            }
        }

        // Check for non-generic ICollection
        if (typeof(ICollection).IsAssignableFrom(type) && type != typeof(string))
        {
            // Try to get element type from IEnumerable<T>
            var enumerableInterface = type.GetInterfaces()
                .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>));

            if (enumerableInterface != null)
            {
                elementType = enumerableInterface.GetGenericArguments()[0];
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Syncs a collection by handling Add, Update, and Delete operations
    /// </summary>
    private void SyncCollection(object existingCollectionObj, object newCollectionObj, Type elementType, string collectionName)
    {
        if (existingCollectionObj == null || newCollectionObj == null) return;

        // Convert to IEnumerable for processing
        var existingCollection = existingCollectionObj as IEnumerable;
        var newCollection = newCollectionObj as IEnumerable;

        if (existingCollection == null || newCollection == null) return;

        // Cast to BaseModel for ID-based operations
        var existingItems = existingCollection.Cast<BaseModel>().ToList();
        var newItems = newCollection.Cast<BaseModel>().ToList();

        var newById = newItems.Where(d => d.Id > 0).ToDictionary(d => d.Id);
        var existingById = existingItems.Where(d => d.Id > 0).ToDictionary(d => d.Id);

        // Get the actual ICollection interface to add/remove items using reflection
        var collectionType = existingCollectionObj.GetType();
        var removeMethod = collectionType.GetMethod("Remove");
        var addMethod = collectionType.GetMethod("Add");

        // 1. Delete items that are in existing but not in new
        var itemsToDelete = existingItems.Where(e => e.Id > 0 && !newById.ContainsKey(e.Id)).ToList();
        foreach (var itemToDelete in itemsToDelete)
        {
            removeMethod?.Invoke(existingCollectionObj, new object[] { itemToDelete });

            // Mark for deletion in Entity Framework using reflection
            var entry = _dbContext.Entry(itemToDelete);
            if (entry.State == EntityState.Detached)
            {
                // Use reflection to get the appropriate DbSet and mark for deletion
                var setMethod = typeof(DbContext).GetMethod(nameof(DbContext.Set), Type.EmptyTypes)
                    ?.MakeGenericMethod(elementType);
                var dbSet = setMethod?.Invoke(_dbContext, null);
                if (dbSet != null)
                {
                    var attachMethod = dbSet.GetType().GetMethod(nameof(DbSet<BaseModel>.Attach));
                    attachMethod?.Invoke(dbSet, new[] { itemToDelete });
                }
            }
            // Mark as deleted - EF will handle the actual deletion on SaveChanges
            entry.State = EntityState.Deleted;
        }

        // 2. Update existing items
        foreach (var existingItem in existingItems.Where(e => e.Id > 0))
        {
            if (newById.TryGetValue(existingItem.Id, out var newItem))
            {
                newItem.Adapt(existingItem);
            }
        }

        // 3. Add new items (Id <= 0) or items that don't exist in the current collection
        var itemsToAdd = newItems.Where(n => n.Id <= 0 || !existingById.ContainsKey(n.Id)).ToList();
        foreach (var itemToAdd in itemsToAdd)
        {
            // Create a new instance of the element type and map values using Mapster
            if (Activator.CreateInstance(elementType) is BaseModel newItem)
            {
                // Copy values from the incoming object into the newly created entity
                itemToAdd.Adapt(newItem);
                addMethod?.Invoke(existingCollectionObj, new object[] { newItem });
            }
        }
    }




    // Async methods
    public async Task<List<TEntity>> ListAsync() => await _dbSet.AsNoTracking().ToListAsync();
    public async Task<List<TDest>> GetAsync<TDest>(Expression<Func<TEntity, bool>> filter = null,
        Expression<Func<TEntity, TDest>> select = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, bool ignoreInclude = false)
    {
        try
        {
            // Optional: Check database connection
            _dbContext.CheckDatabaseConnection();

            IQueryable<TEntity> query = _dbSet.AsNoTracking();

            if (filter != null)
                query = query.Where(filter);

            // Don't apply includes when select projection is used to avoid N+1 and performance issues
            if (!ignoreInclude && _cachedIncludeProperties != null && _cachedIncludeProperties.Length > 0)
            {
                query = _cachedIncludeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
            }

            if (orderBy != null)
                query = orderBy(query);

            if (select != null)
                return await query.Select(select).ToListAsync();

            // If select is null, assume TDest == TEntity
            if (typeof(TDest) == typeof(TEntity))
                return await query.Cast<TDest>().ToListAsync();

            throw new InvalidOperationException("Select expression is required when TDest is not TEntity.");
        }
        catch (Exception ex)
        {
            throw GetTableDetails(ex);
        }
    }

    public async Task<TDest> FirstOrDefaultAsync<TDest>(Expression<Func<TEntity, bool>> filter = null,
        Expression<Func<TEntity, TDest>> select = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null)
    {
        try
        {
            _dbContext.CheckDatabaseConnection();

            IQueryable<TEntity> query = _dbSet.AsNoTracking();

            if (filter != null)
                query = query.Where(filter);

            if (_cachedIncludeProperties != null && _cachedIncludeProperties.Length > 0)
            {
                query = _cachedIncludeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
            }

            if (orderBy != null)
                query = orderBy(query);

            if (select != null)
                return await query.Select(select).FirstOrDefaultAsync();

            // If select is null, assume TDest == TEntity
            if (typeof(TDest) == typeof(TEntity))
                return await query.Cast<TDest>().FirstOrDefaultAsync();

            throw new InvalidOperationException("Select expression is required when TDest is not TEntity.");
        }
        catch (Exception ex)
        {
            throw GetTableDetails(ex);
        }
    }

    public async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate) => await _dbSet.AsNoTracking().CountAsync(predicate);
    public async Task<int> MinAsync(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, int?>> selector) => await _dbSet.AsNoTracking().Where(filter).Select(selector).DefaultIfEmpty(0).MinAsync() ?? 0;
    public async Task<int> MaxAsync(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, int?>> selector) => await _dbSet.AsNoTracking().Where(filter).Select(selector).DefaultIfEmpty(0).MaxAsync() ?? 0;
    public async Task<TEntity> GetByIdAsync(object id) => await _dbSet.FindAsync(id);

    public async Task<TEntity> GetByIdNoTrackingAsync(object id)
    {
        var keyProperty = GetKeyProperties()[0];
        var entityType = typeof(TEntity);

        // Build expression: entity => entity.Id == id
        var parameter = Parameter(entityType, "entity");
        var property = Property(parameter, keyProperty);
        var convertedId = System.Convert.ChangeType(id, keyProperty.PropertyType);
        var constant = Constant(convertedId, keyProperty.PropertyType);
        var equality = Equal(property, constant);
        var lambda = Lambda<Func<TEntity, bool>>(equality, parameter);

        return await _dbSet.AsNoTracking().FirstOrDefaultAsync(lambda);
    }

    public async Task AddAsync(TEntity entity)
    {
        _dbSet.Add(entity);
        await Task.CompletedTask;
    }

    public async Task AddRangeAsync(IEnumerable<TEntity> entities)
    {
        _dbSet.AddRange(entities);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(object id)
    {
        var entity = await _dbSet.FindAsync(id);
        if (entity != null) await DeleteAsync(entity);
    }

    public async Task DeleteAsync(TEntity entity)
    {
        var entry = _dbContext.Entry(entity);
        if (entry.State == EntityState.Detached)
        {
            _dbSet.Attach(entity);
        }
        _dbSet.Remove(entity);
        await Task.CompletedTask;
    }

    public async Task DeleteRangeAsync(IEnumerable<TEntity> entities)
    {
        var entitiesList = entities.ToList();
        if (!entitiesList.Any()) return;

        // Batch delete - attach all entities first, then remove them
        foreach (var entity in entitiesList)
        {
            var entry = _dbContext.Entry(entity);
            if (entry.State == EntityState.Detached)
            {
                _dbSet.Attach(entity);
            }
            _dbSet.Remove(entity);
        }
        await Task.CompletedTask;
    }

    public virtual async Task UpdateRangeAsync(IEnumerable<TEntity> entities)
    {
        foreach (var entity in entities)
            await UpdateAsync(entity);
        //await UpdateRangeAsync(entities, FieldConstants.Id);
    }

    public virtual async Task UpdateRangeAsync(IEnumerable<TEntity> entities, string id)
    {
        foreach (var entity in entities)
            await UpdateAsync(entity, FieldConstants.Id);

        /*
        var entitiesList = entities.ToList();
        if (!entitiesList.Any()) return;

        var keyProperties = GetKeyProperties();
        
        // Batch process entities
        foreach (var entityToUpdate in entitiesList)
        {
            var entry = _dbContext.Entry(entityToUpdate);
            if (entry.State != EntityState.Detached) continue;

            var keyValues = ExtractKeyValues(entityToUpdate, keyProperties);
            var set = _dbContext.Set<TEntity>();
            var attachedEntity = await set.FindAsync(keyValues);
            
            if (attachedEntity != null)
            {
                set.Attach(attachedEntity);
                var attachedEntry = _dbContext.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entityToUpdate);
                
                if (_cachedIncludeProperties != null && _cachedIncludeProperties.Length > 0)
                {
                    foreach (var include in _cachedIncludeProperties)
                    {
                        var collection = attachedEntry.Collection(include);
                        if (!collection.IsLoaded)
                        {
                            await collection.LoadAsync();
                        }
                    }
                }

                entityToUpdate.Adapt(attachedEntry.Entity);
                await SyncDetailCollectionsAsync(attachedEntry.Entity, entityToUpdate, attachedEntry);
            }
            else
            {
                set.Attach(entityToUpdate);
                entry.State = EntityState.Modified;
            }
        }*/
    }

    public async Task SaveAsync()
    {
        var retries = 3;
        while (retries > 0)
        {
            try
            {
                await _dbContext.SaveChangesAsync().ConfigureAwait(false);
                break;
            }
            catch (DbUpdateException ex) when (ex.InnerException?.InnerException is SqlException sqlEx && sqlEx.Number == 1205)
            {
                retries--;
                await Task.Delay(1000).ConfigureAwait(false);
            }
        }
    }

    public async Task<int> GetNextSequenceAsync(string sequenceName)
    {
        var query = $"SELECT NEXT VALUE FOR dbo.{sequenceName};";
        return await _dbContext.Database.SqlQuery<int>(query).SingleAsync();
    }

    public async Task<int> GetCurrentSequenceAsync(string sequenceName)
    {
        var query = $"SELECT current_value FROM sys.sequences WHERE name = '{sequenceName}';";
        return await _dbContext.Database.SqlQuery<int>(query).SingleAsync();
    }

    private Exception GetTableDetails(Exception exception)
    {
        if (exception.InnerException?.GetType() == typeof(SqlException))
            return new Exception("Table : " + GetTableName<TEntity>() + "\n\n" +
                                 exception.InnerException?.Message);
        return exception;
    }

    private string GetTableName<T>()
        where T : class
    {
        var entitySet = GetEntitySet<T>(_dbContext);
        if (entitySet == null)
            throw new Exception("Unable to find entity set '{0}' in edm metadata " + typeof(T).Name);
        var tableName = GetStringProperty(entitySet, "Table");
        return tableName;
    }

    private static EntitySet GetEntitySet<T>(IObjectContextAdapter context)
    {
        var type = typeof(T);
        var entityName = type.Name;
        var metadata = context.ObjectContext.MetadataWorkspace;

        var entitySets = metadata.GetItemCollection(DataSpace.SSpace)
            .GetItems<EntityContainer>()
            .Single()
            .BaseEntitySets
            .OfType<EntitySet>()
            .Where(s => !s.MetadataProperties.Contains("Type")
                        || s.MetadataProperties["Type"].ToString() == "Tables");
        var entitySet = entitySets.FirstOrDefault(t => t.Name == entityName);
        return entitySet;
    }

    private string GetStringProperty(MetadataItem entitySet, string propertyName)
    {
        if (entitySet == null)
            throw new ArgumentNullException(nameof(entitySet));
        if (!entitySet.MetadataProperties.TryGetValue(propertyName, false, out var property))
            return string.Empty;
        if (property?.Value is string str && !string.IsNullOrEmpty(str))
            return str;
        return string.Empty;
    }

    public async Task<IList<T>> ExecuteStoredProcedureAsync<T>(string procedureName, params object[] parameters)
    {
        var sql = $"{procedureName} {string.Join(", ", parameters.Select((p, i) => $"@p{i}"))}";
        return await _dbContext.Database.SqlQuery<T>(sql, parameters).ToListAsync();
    }

    public async Task<int> ExecuteSqlCommandAsync(string sql, params object[] parameters)
    {
        return await _dbContext.Database.ExecuteSqlCommandAsync(sql, parameters);
    }
}
