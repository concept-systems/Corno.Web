using System;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Validation;
using Corno.Concept.Portal.Logger;
using Corno.Concept.Portal.Repository.Interfaces;
using Corno.Concept.Portal.Windsor;
using Corno.Concept.Portal.Windsor.Context;

namespace Corno.Concept.Portal.Repository;

public class UnitOfWork : IUnitOfWork
{
    #region -- Constructors --
    /*public UnitOfWork()
    {
        _connectionString = ConfigurationManager.ConnectionStrings["CornoContext"].ConnectionString;
    }*/

    /*public UnitOfWork(string connectionString, Type contextType)
    {
        _connectionString = connectionString;
        _contextType = contextType;
    }*/

    public UnitOfWork(BaseDbContext dbContext)
    {
        _baseContext = dbContext;
    }
    #endregion

    #region -- Data Members --
    private readonly string _connectionString;
    private BaseDbContext _baseContext;
    private Type _contextType;

    private bool _disposed;

    private static readonly object _dbLock = new();
    #endregion

    #region -- Properties --
    public BaseDbContext DbContext
    {
        get
        {
            /*if (_baseContext != null && !_baseContext.IsDisposed())
            {
                _baseContext.CheckDatabaseConnection();
                return _baseContext;
            }

            _baseContext = Bootstrapper.Get<BaseDbContext>();*/
            //_baseContext.CheckDatabaseConnection();
            return _baseContext;
        }
    }
    /*public BaseDbContext DbContext
    {
        get
        {
            if (_baseContext != null && !_baseContext.IsDisposed())
            {
                _baseContext.CheckDatabaseConnection();
                return _baseContext;
            }

            _baseContext = (BaseDbContext)Activator.CreateInstance(_contextType, _connectionString);

            return _baseContext;
        }
    }*/
    #endregion

    #region -- Public Methods --
    public virtual void Save()
    {
        try
        {
            // Save changes to the database
            lock (_dbLock)
            {
                DbContext.SaveChanges();
            }
        }
        catch (DbEntityValidationException exception)
        {
            // Loop through the validation errors to find the one causing the "string or binary data would be truncated" error
            foreach (var validationErrors in exception.EntityValidationErrors)
            {
                foreach (var validationError in validationErrors.ValidationErrors)
                    LogHandler.LogError(new Exception($"Property or column '{validationError.PropertyName}' caused the error: {validationError.ErrorMessage}"));
            }
            foreach (var entry in _baseContext.ChangeTracker.Entries())
                entry.State = EntityState.Detached;

            throw;
        }
        catch (Exception exception)
        {
            LogHandler.LogError(exception);
            foreach (var entry in _baseContext.ChangeTracker.Entries())
                entry.State = EntityState.Detached;
            throw;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (_disposed) return;
        if (disposing)
        {
            _baseContext?.Dispose();  // <-- returns the connection to the pool
            _baseContext = null;
        }
        _disposed = true;
    }

    /*protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
            if (disposing)
                _baseContext?.Dispose();
        _disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }*/
    #endregion
}