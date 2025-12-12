using System;
using System.Data;
using System.Data.Entity.Infrastructure;
using Corno.Web.Areas.Admin.Models;
using Corno.Web.Logger;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Corno.Web.Windsor.Context;

public class BaseDbContext : IdentityDbContext<AspNetUser, AspNetRole, 
    string, IdentityUserLogin, IdentityUserRole, IdentityUserClaim>
{
    #region -- Constructors --

    public BaseDbContext(string connectionString)
        : base(connectionString)
    {
        //Database.SetInitializer<BaseDbContext>(null);

        try
        {
            // Check Database Connection
            //CheckDatabaseConnection();

            // Get the ObjectContext related to this DbContext
            var objectContext = (this as IObjectContextAdapter).ObjectContext;
            // Sets the command timeout for all the commands
            objectContext.CommandTimeout = int.MaxValue;
        }
        catch (Exception exception)
        {
            // ignored
            LogHandler.LogError(exception);
        }
    }

    #endregion

    /*#region -- Private Methods --

    public bool IsDisposed()
    {
        var result = true;

        var typeDbContext = typeof(DbContext);
        var typeInternalContext = typeDbContext.Assembly.GetType("System.Data.Entity.Internal.InternalContext");

        var fiInternalContext = typeDbContext.GetField("_internalContext", BindingFlags.NonPublic | BindingFlags.Instance);
        var piIsDisposed = typeInternalContext.GetProperty("IsDisposed");

        var ic = fiInternalContext?.GetValue(this);

        if (ic == null) return true;

        if (piIsDisposed != null)
            result = (bool)piIsDisposed.GetValue(ic);

        return result;
    }
    #endregion*/

    #region -- Public Methods --

    public void CheckDatabaseConnection()
    {
        if (Database?.Connection is { State: ConnectionState.Open })
            return;
        //Database?.Connection?.Close();
        Database?.Connection?.Open();
    }
    #endregion
}