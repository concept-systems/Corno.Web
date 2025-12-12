using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using Corno.Web.Areas.Admin.Models;
using Corno.Web.Models;
using Corno.Web.Models.Base;
using Corno.Web.Models.Masters;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Corno.Web.Windsor.Context;

public class CornoDbContext : BaseDbContext
{
    #region -- Constructors --
    protected CornoDbContext(string connectionString)
        : base(connectionString)
    {
        //Database.SetInitializer<CornoDbContext>(null);
    }
    #endregion

    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    {
        // Call base class function
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<IdentityUserRole>()
            .HasKey(r => new { r.UserId, r.RoleId })
            .ToTable("AspNetUserRoles");
        modelBuilder.Entity<IdentityUserLogin>()
            .HasKey(l => new { l.LoginProvider, l.ProviderKey, l.UserId })
            .ToTable("AspNetUserLogins");
        /*modelBuilder.Entity<AspNetUserRole>()
            .HasKey(l => new { l.UserId, l.RoleId })
            .ToTable($"{nameof(AspNetUserRole)}s");*/

        modelBuilder.Entity<AspNetLoginHistory>().ToTable("AspNetLoginHistories");

        // Masters
        modelBuilder.Entity<Project>().ToTable("Project");
        modelBuilder.Entity<MiscMaster>().ToTable("MiscMaster");

        modelBuilder.Entity<Process>().ToTable("Process");
    }
}

public static class EntityTypeConfigurationExtensions
{
    public static void AddExtraProperty<TEntity>(this EntityTypeConfiguration<TEntity> entity) 
        where TEntity : BaseModel
    {
        entity.Property(e => e.SerializedExtraProperties)
            .HasColumnName("ExtraProperties")
            .HasColumnType("nvarchar(max)");
    }
}