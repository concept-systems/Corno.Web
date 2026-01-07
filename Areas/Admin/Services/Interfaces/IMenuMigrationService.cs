using System.Collections.Generic;
using System.Threading.Tasks;
using Corno.Web.Services.Interfaces;

namespace Corno.Web.Areas.Admin.Services.Interfaces;

/// <summary>
/// Service interface for migrating XML sitemap to database menus
/// </summary>
public interface IMenuMigrationService : IService
{
    /// <summary>
    /// Migrates XML sitemap from Project.MenuXml to Menu table
    /// One-time migration: Only runs if no menus exist in database
    /// Protects manually created menus from being overwritten
    /// </summary>
    /// <param name="projectName">Project name (default: "Active")</param>
    /// <param name="userId">User ID performing the migration (default: "System")</param>
    /// <returns>Migration result with success status, counts, and errors</returns>
    Task<MenuMigrationResult> MigrateFromXmlAsync(string projectName = "Active", string userId = "System");

    /// <summary>
    /// Creates default login module menus if they don't exist
    /// These menus are system menus that should always be available
    /// </summary>
    /// <param name="userId">User ID creating the menus (default: "System")</param>
    /// <returns>Result with success status and counts</returns>
    Task<MenuMigrationResult> CreateDefaultLoginModuleMenusAsync(string userId = "System");
}

/// <summary>
/// Result of menu migration operation
/// </summary>
public class MenuMigrationResult
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public int TotalMenus { get; set; }
    public int CreatedMenus { get; set; }
    public int UpdatedMenus { get; set; }
    public int SkippedMenus { get; set; }
    public List<string> Errors { get; set; } = new List<string>();
    public Dictionary<string, List<string>> DuplicateMenus { get; set; } = new Dictionary<string, List<string>>();
}

