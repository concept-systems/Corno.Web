using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Corno.Web.Areas.Admin.Models;
using Corno.Web.Areas.Admin.Services.Interfaces;
using Corno.Web.Repository.Interfaces;
using Corno.Web.Services;
using Corno.Web.Services.Masters.Interfaces;
using Newtonsoft.Json;
using System.Text;

namespace Corno.Web.Areas.Admin.Services;

/// <summary>
/// Service for migrating XML sitemap to database menus
/// </summary>
public class MenuMigrationService : IMenuMigrationService
{
    private readonly IGenericRepository<Menu> _menuRepository;
    private readonly IProjectService _projectService;
    private int _displayOrder = 0;
    
    // Track menu names during migration to detect duplicates
    private readonly Dictionary<string, int> _menuNameCounts = new Dictionary<string, int>();
    private readonly Dictionary<string, List<string>> _duplicateMenus = new Dictionary<string, List<string>>();
    
    // Track all menus being created for debugging
    private readonly List<Menu> _pendingMenus = new List<Menu>();
    private static readonly object _logLock = new object();

    public MenuMigrationService(
        IGenericRepository<Menu> menuRepository,
        IProjectService projectService)
    {
        _menuRepository = menuRepository;
        _projectService = projectService;
    }

    /// <summary>
    /// Migrates XML sitemap from Project.MenuXml to Menu table
    /// One-time migration: Only runs if no menus exist in database
    /// Protects manually created menus from being overwritten
    /// </summary>
    public async Task<MenuMigrationResult> MigrateFromXmlAsync(string projectName = "Active", string userId = "System")
    {
        var result = new MenuMigrationResult
        {
            TotalMenus = 0,
            CreatedMenus = 0,
            UpdatedMenus = 0,
            SkippedMenus = 0,
            Errors = new List<string>()
        };

        try
        {
            // Check if menus already exist (one-time migration check)
            var existingMenuCount = await _menuRepository.CountAsync(m => true).ConfigureAwait(false);
            if (existingMenuCount > 0)
            {
                result.Success = true;
                result.Message = $"Migration skipped: {existingMenuCount} menus already exist in database. Migration is a one-time operation.";
                result.SkippedMenus = existingMenuCount;
                return result;
            }

            // Get project
            var project = await _projectService.GetProjectAsync(projectName).ConfigureAwait(false);
            if (project == null)
            {
                result.Errors.Add($"Project '{projectName}' not found");
                return result;
            }

            if (string.IsNullOrEmpty(project.MenuXml))
            {
                result.Errors.Add("Project MenuXml is empty");
                return result;
            }

            // Parse XML
            var document = new XmlDocument();
            document.LoadXml(project.MenuXml);

            // Find root siteMapNode (skip the outer siteMap element)
            XmlNode rootNode = null;
            if (document.FirstChild?.Name == "siteMap")
            {
                rootNode = document.FirstChild.FirstChild; // Get first siteMapNode
            }
            else if (document.FirstChild?.Name == "siteMapNode")
            {
                rootNode = document.FirstChild;
            }

            if (rootNode == null)
            {
                result.Errors.Add("Invalid XML structure: Could not find root siteMapNode");
                return result;
            }

            // Step 1: Pre-validate XML for duplicate menu names (for reporting only)
            _menuNameCounts.Clear();
            _duplicateMenus.Clear();
            ValidateXmlForDuplicates(rootNode, "");
            
            // Store duplicate information for reporting (but don't stop migration)
            if (_duplicateMenus.Any())
            {
                result.DuplicateMenus = new Dictionary<string, List<string>>(_duplicateMenus);
                foreach (var duplicate in _duplicateMenus)
                {
                    var duplicateList = string.Join(", ", duplicate.Value);
                    result.Errors.Add($"⚠ Duplicate menu name '{duplicate.Key}' found at paths: {duplicateList}. Duplicates will be renamed automatically.");
                }
            }

            // Step 2: Import menus recursively (one-time migration, no existing menus)
            // Duplicates will be automatically renamed during import
            _menuNameCounts.Clear(); // Reset for actual migration
            _pendingMenus.Clear(); // Reset pending menus tracking
            var parentMenuId = (int?)null;
            
            // #region agent log
            WriteDebugLog("MenuMigrationService.cs:120", "START_MIGRATION", new {
                hypothesisId = "J",
                projectName = projectName,
                userId = userId
            });
            // #endregion
            
            await ImportXmlNodeAsync(rootNode, parentMenuId, userId, result, "").ConfigureAwait(false);

            // #region agent log
            WriteDebugLog("MenuMigrationService.cs:123", "BEFORE_SAVE", new {
                hypothesisId = "G",
                pendingMenusCount = _pendingMenus.Count,
                menusWithIdZero = _pendingMenus.Count(m => m.Id == 0),
                menusByParentId = _pendingMenus.GroupBy(m => m.ParentMenuId ?? -1).ToDictionary(g => g.Key.ToString(), g => g.Count()),
                duplicateParentIds = _pendingMenus.GroupBy(m => m.ParentMenuId ?? -1).Where(g => g.Count() > 1).Select(g => new { parentId = g.Key, count = g.Count() }).ToList(),
                allMenuIds = _pendingMenus.Select(m => m.Id).ToList(),
                allMenuNames = _pendingMenus.Select(m => m.MenuName).ToList(),
                allParentIds = _pendingMenus.Select(m => m.ParentMenuId).ToList()
            });
            // #endregion
            
            // Save all changes
            try
            {
                await _menuRepository.SaveAsync().ConfigureAwait(false);
                
                // #region agent log
                WriteDebugLog("MenuMigrationService.cs:140", "AFTER_SAVE_SUCCESS", new {
                    hypothesisId = "H",
                    savedMenusCount = _pendingMenus.Count
                });
                // #endregion
            }
            catch (Exception saveEx)
            {
                // #region agent log
                WriteDebugLog("MenuMigrationService.cs:148", "SAVE_ERROR", new {
                    hypothesisId = "I",
                    error = saveEx.Message,
                    errorType = saveEx.GetType().Name,
                    stackTrace = saveEx.StackTrace,
                    pendingMenusCount = _pendingMenus.Count,
                    menusWithIdZero = _pendingMenus.Count(m => m.Id == 0),
                    menusByParentId = _pendingMenus.GroupBy(m => m.ParentMenuId ?? -1).ToDictionary(g => g.Key.ToString(), g => g.Count())
                });
                // #endregion
                throw;
            }

            // Migration is successful even if there were duplicate warnings (they were auto-renamed)
            result.Success = true;
            if (result.DuplicateMenus != null && result.DuplicateMenus.Any())
            {
                result.Message = $"Migration completed with {result.CreatedMenus} menus created. {result.DuplicateMenus.Count} duplicate menu name(s) were found and automatically renamed. See errors for details.";
            }
            else if (string.IsNullOrEmpty(result.Message))
            {
                result.Message = $"Migration completed successfully. Created: {result.CreatedMenus}, Updated: {result.UpdatedMenus}";
            }
        }
        catch (Exception ex)
        {
            result.Errors.Add($"Migration failed: {ex.Message}");
            result.Success = false;
        }

        return result;
    }

    private async Task<int?> ImportXmlNodeAsync(
        XmlNode xmlNode, 
        int? parentMenuId, 
        string userId, 
        MenuMigrationResult result, 
        string parentPath)
    {
        if (xmlNode?.Attributes == null)
            return null;

        try
        {
            var menuName = xmlNode.Attributes["name"]?.Value ?? xmlNode.Attributes["title"]?.Value;
            var title = xmlNode.Attributes["title"]?.Value;
            var controller = xmlNode.Attributes["controller"]?.Value;
            var action = xmlNode.Attributes["action"]?.Value ?? "Index";
            var area = xmlNode.Attributes["area"]?.Value;
            var icon = xmlNode.Attributes["icon"]?.Value;
            var roles = xmlNode.Attributes["roles"]?.Value;

            // Skip if no name or title
            if (string.IsNullOrEmpty(menuName) && string.IsNullOrEmpty(title))
            {
                // Process children anyway (for empty parent nodes)
                if (xmlNode.HasChildNodes)
                {
                    foreach (XmlNode childNode in xmlNode.ChildNodes)
                    {
                        if (childNode.Name == "siteMapNode")
                            await ImportXmlNodeAsync(childNode, parentMenuId, userId, result, parentPath).ConfigureAwait(false);
                    }
                }
                return parentMenuId;
            }

            // Use title as menuName if name is not provided
            if (string.IsNullOrEmpty(menuName))
                menuName = title;

            // Build menu path
            var menuPath = string.IsNullOrEmpty(parentPath) 
                ? menuName 
                : $"{parentPath}/{menuName}";

            // Create unique key for duplicate detection: MenuName + ParentMenuId
            var uniqueKey = $"{menuName}|{(parentMenuId?.ToString() ?? "root")}";

            // Track menu name usage to detect duplicates during migration
            if (!_menuNameCounts.ContainsKey(uniqueKey))
            {
                _menuNameCounts[uniqueKey] = 0;
            }
            _menuNameCounts[uniqueKey]++;

            // If duplicate detected, make menu name unique by appending counter
            var finalMenuName = menuName;
            if (_menuNameCounts[uniqueKey] > 1)
            {
                finalMenuName = $"{menuName}_{_menuNameCounts[uniqueKey] - 1}";
                var duplicatePath = menuPath;
                menuPath = string.IsNullOrEmpty(parentPath) 
                    ? finalMenuName 
                    : $"{parentPath}/{finalMenuName}";
                
                // Report duplicate
                result.Errors.Add($"Duplicate menu name '{menuName}' found at path '{duplicatePath}'. Renamed to '{finalMenuName}'.");
            }

            result.TotalMenus++;

            // Check if menu already exists (using final menu name)
            var existingMenu = await _menuRepository.FirstOrDefaultAsync<Menu>(
                m => m.MenuName == finalMenuName && m.MenuPath == menuPath,
                m => m
            ).ConfigureAwait(false);

            Menu menu;
            bool isNew = false;

            if (existingMenu != null)
            {
                // Only update if it was migrated from XML (protect manual menus)
                if (existingMenu.Source == "XML")
                {
                    menu = existingMenu;
                    result.UpdatedMenus++;
                }
                else
                {
                    // Menu was manually created, skip to protect user changes
                    result.SkippedMenus++;
                    // Still process children recursively
                    if (xmlNode.HasChildNodes)
                    {
                        foreach (XmlNode childNode in xmlNode.ChildNodes)
                        {
                            if (childNode.Name == "siteMapNode")
                            {
                                await ImportXmlNodeAsync(childNode, existingMenu.Id, userId, result, menuPath).ConfigureAwait(false);
                            }
                        }
                    }
                    return existingMenu.Id;
                }
            }
            else
            {
                menu = new Menu
                {
                    Source = "XML", // Mark as migrated from XML
                    CreatedBy = userId,
                    CreatedDate = DateTime.Now
                };
                isNew = true;
                result.CreatedMenus++;
            }

            // Set properties (use finalMenuName to handle duplicates)
            menu.MenuName = finalMenuName;
            menu.DisplayName = title ?? finalMenuName;
            menu.MenuPath = menuPath;
            menu.ParentMenuId = parentMenuId;
            menu.ControllerName = controller;
            menu.ActionName = action;
            menu.Area = area;
            menu.IconClass = icon;
            menu.DisplayOrder = _displayOrder++;
            menu.IsVisible = true;
            menu.IsActive = true;
            // Only update description if it's a new menu or XML-sourced menu
            if (isNew || menu.Source == "XML")
            {
                menu.Description = $"Migrated from XML sitemap. Roles: {roles}";
            }
            // Preserve existing description for manual menus

            // Store route values as JSON
            var routeValues = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(xmlNode.Attributes["misctype"]?.Value))
                routeValues["misctype"] = xmlNode.Attributes["misctype"].Value;
            if (!string.IsNullOrEmpty(xmlNode.Attributes["reportName"]?.Value))
                routeValues["reportName"] = xmlNode.Attributes["reportName"].Value;
            if (!string.IsNullOrEmpty(xmlNode.Attributes["formname"]?.Value))
                routeValues["formname"] = xmlNode.Attributes["formname"].Value;
            if (!string.IsNullOrEmpty(xmlNode.Attributes["web"]?.Value))
                routeValues["web"] = xmlNode.Attributes["web"].Value;
            if (!string.IsNullOrEmpty(xmlNode.Attributes["windows"]?.Value))
                routeValues["windows"] = xmlNode.Attributes["windows"].Value;

            if (routeValues.Any())
            {
                menu.RouteValues = Newtonsoft.Json.JsonConvert.SerializeObject(routeValues);
            }

            menu.UpdateModified(userId);

            // #region agent log
            WriteDebugLog("MenuMigrationService.cs:301", "BEFORE_ADD_UPDATE", new {
                hypothesisId = "A",
                menuId = menu.Id,
                menuName = finalMenuName,
                menuPath = menuPath,
                parentMenuId = parentMenuId,
                isNew = isNew,
                menuObjectHash = menu.GetHashCode()
            });
            // #endregion

            if (isNew)
            {
                // #region agent log
                _pendingMenus.Add(menu);
                WriteDebugLog("MenuMigrationService.cs:310", "ADDING_MENU", new {
                    hypothesisId = "B",
                    menuId = menu.Id,
                    menuName = finalMenuName,
                    parentMenuId = parentMenuId,
                    pendingMenusCount = _pendingMenus.Count,
                    menusWithSameParent = _pendingMenus.Count(m => m.ParentMenuId == parentMenuId),
                    menusWithIdZero = _pendingMenus.Count(m => m.Id == 0)
                });
                // #endregion
                
                await _menuRepository.AddAsync(menu).ConfigureAwait(false);
                await _menuRepository.SaveAsync().ConfigureAwait(false);

                // #region agent log
                WriteDebugLog("MenuMigrationService.cs:323", "AFTER_ADD", new {
                    hypothesisId = "C",
                    menuId = menu.Id,
                    menuName = finalMenuName,
                    menuObjectHash = menu.GetHashCode()
                });
                // #endregion
            }
            else
            {
                await _menuRepository.UpdateAsync(menu).ConfigureAwait(false);
            }

            // #region agent log
            var menuIdForChildren = menu.Id;
            WriteDebugLog("MenuMigrationService.cs:335", "PROCESSING_CHILDREN", new {
                hypothesisId = "D",
                menuId = menuIdForChildren,
                menuName = finalMenuName,
                parentMenuIdForChildren = menuIdForChildren,
                hasChildren = xmlNode.HasChildNodes
            });
            // #endregion

            // Process children recursively
            if (xmlNode.HasChildNodes)
            {
                foreach (XmlNode childNode in xmlNode.ChildNodes)
                {
                    if (childNode.Name == "siteMapNode")
                    {
                        // #region agent log
                        WriteDebugLog("MenuMigrationService.cs:348", "RECURSIVE_CALL", new {
                            hypothesisId = "E",
                            parentMenuId = menuIdForChildren,
                            parentMenuName = finalMenuName,
                            childNodeName = childNode.Attributes["name"]?.Value ?? childNode.Attributes["title"]?.Value
                        });
                        // #endregion
                        
                        await ImportXmlNodeAsync(childNode, menuIdForChildren, userId, result, menuPath).ConfigureAwait(false);
                    }
                }
            }

            // #region agent log
            WriteDebugLog("MenuMigrationService.cs:360", "RETURNING_MENU_ID", new {
                hypothesisId = "F",
                menuId = menuIdForChildren,
                menuName = finalMenuName
            });
            // #endregion

            return menuIdForChildren;
        }
        catch (Exception ex)
        {
            result.Errors.Add($"Error importing menu '{xmlNode.Attributes["name"]?.Value ?? xmlNode.Attributes["title"]?.Value}': {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Pre-validates XML to detect duplicate menu names (same name + same parent path)
    /// Uses parent path instead of parent ID since we don't have IDs during validation
    /// </summary>
    private void ValidateXmlForDuplicates(XmlNode xmlNode, string parentPath)
    {
        if (xmlNode?.Attributes == null)
            return;

        var menuName = xmlNode.Attributes["name"]?.Value ?? xmlNode.Attributes["title"]?.Value;
        var title = xmlNode.Attributes["title"]?.Value;

        // Skip if no name or title
        if (string.IsNullOrEmpty(menuName) && string.IsNullOrEmpty(title))
        {
            // Process children anyway
            if (xmlNode.HasChildNodes)
            {
                foreach (XmlNode childNode in xmlNode.ChildNodes)
                {
                    if (childNode.Name == "siteMapNode")
                        ValidateXmlForDuplicates(childNode, parentPath);
                }
            }
            return;
        }

        // Use title as menuName if name is not provided
        if (string.IsNullOrEmpty(menuName))
            menuName = title;

        // Build menu path
        var menuPath = string.IsNullOrEmpty(parentPath) 
            ? menuName 
            : $"{parentPath}/{menuName}";

        // Create unique key: MenuName + ParentPath (to identify duplicates under same parent)
        var uniqueKey = $"{menuName}|{parentPath ?? "root"}";

        // Track this menu
        if (!_menuNameCounts.ContainsKey(uniqueKey))
        {
            _menuNameCounts[uniqueKey] = 0;
        }
        _menuNameCounts[uniqueKey]++;

        // If duplicate found, add to duplicate list
        if (_menuNameCounts[uniqueKey] > 1)
        {
            if (!_duplicateMenus.ContainsKey(menuName))
            {
                _duplicateMenus[menuName] = new List<string>();
            }
            if (!_duplicateMenus[menuName].Contains(menuPath))
            {
                _duplicateMenus[menuName].Add(menuPath);
            }
        }

        // Process children recursively
        if (xmlNode.HasChildNodes)
        {
            foreach (XmlNode childNode in xmlNode.ChildNodes)
            {
                if (childNode.Name == "siteMapNode")
                {
                    ValidateXmlForDuplicates(childNode, menuPath);
                }
            }
        }
    }

    private void WriteDebugLog(string location, string message, object data)
    {
        try
        {
            lock (_logLock)
            {
                var logPath = @"d:\Development\Corno\Corno.Web\.cursor\debug.log";
                var logEntry = new
                {
                    id = $"log_{DateTime.Now.Ticks}_{Guid.NewGuid().ToString("N").Substring(0, 8)}",
                    timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                    location = location,
                    message = message,
                    data = data,
                    sessionId = "debug-session",
                    runId = "run1"
                };
                
                var json = JsonConvert.SerializeObject(logEntry);
                File.AppendAllText(logPath, json + Environment.NewLine, Encoding.UTF8);
            }
        }
        catch
        {
            // Ignore logging errors
        }
    }

    /// <summary>
    /// Creates default login module menus if they don't exist
    /// </summary>
    public async Task<MenuMigrationResult> CreateDefaultLoginModuleMenusAsync(string userId = "System")
    {
        var result = new MenuMigrationResult
        {
            TotalMenus = 0,
            CreatedMenus = 0,
            UpdatedMenus = 0,
            SkippedMenus = 0,
            Errors = new List<string>()
        };

        try
        {
            // #region agent log
            WriteDebugLog("MenuMigrationService.cs:559", "CREATE_DEFAULT_MENUS_START", new {
                hypothesisId = "J",
                userId = userId
            });
            // #endregion

            var displayOrder = 0;

            // First, create or get the parent menu "Administration" for login module menus
            var parentMenuName = "Administration";
            var parentMenu = await _menuRepository.FirstOrDefaultAsync<Menu>(
                m => m.MenuName == parentMenuName,
                m => m
            ).ConfigureAwait(false);

            int? parentMenuId = null;
            if (parentMenu == null)
            {
                // Create parent menu
                parentMenu = new Menu
                {
                    MenuName = parentMenuName,
                    DisplayName = "Administration",
                    MenuPath = parentMenuName,
                    ParentMenuId = null, // Root level menu
                    ControllerName = null, // Parent menu doesn't have controller/action
                    ActionName = null,
                    Area = null,
                    IconClass = "k-i-gear",
                    DisplayOrder = 0,
                    IsVisible = true,
                    IsActive = true,
                    Source = "System",
                    CreatedBy = userId,
                    CreatedDate = DateTime.Now,
                    Description = "System menu group for administration features"
                };

                // Add "web" route value so parent menu appears
                var parentRouteValues = new Dictionary<string, string>
                {
                    { "web", "true" }
                };
                parentMenu.RouteValues = JsonConvert.SerializeObject(parentRouteValues);

                parentMenu.UpdateCreated(userId);
                await _menuRepository.AddAsync(parentMenu).ConfigureAwait(false);
                
                // #region agent log
                WriteDebugLog("MenuMigrationService.cs:610", "PARENT_MENU_BEFORE_SAVE", new {
                    hypothesisId = "P1",
                    parentMenuName = parentMenuName,
                    parentMenuId = parentMenu.Id,
                    parentMenuHash = parentMenu.GetHashCode()
                });
                // #endregion
                
                await _menuRepository.SaveAsync().ConfigureAwait(false); // Save to get the ID
                
                // #region agent log
                WriteDebugLog("MenuMigrationService.cs:620", "PARENT_MENU_AFTER_SAVE", new {
                    hypothesisId = "P2",
                    parentMenuName = parentMenuName,
                    parentMenuId = parentMenu.Id,
                    parentMenuHash = parentMenu.GetHashCode()
                });
                // #endregion
                
                result.CreatedMenus++;
                result.TotalMenus++;
            }
            else
            {
                // #region agent log
                WriteDebugLog("MenuMigrationService.cs:595", "PARENT_MENU_EXISTS", new {
                    hypothesisId = "Q",
                    parentMenuName = parentMenuName,
                    parentMenuId = parentMenu.Id
                });
                // #endregion
            }

            parentMenuId = parentMenu.Id;
            displayOrder = 1; // Start child menus from order 1

            // Define default login module menus (all as children of Administration)
            var defaultMenus = new[]
            {
                new { MenuName = "Dashboard", DisplayName = "Dashboard", Controller = "Dashboard", Action = "Index", Area = "Admin", Icon = "k-i-home", ParentMenuId = parentMenuId },
                new { MenuName = "UserManagement", DisplayName = "User Management", Controller = "User", Action = "Index", Area = "Admin", Icon = "k-i-user", ParentMenuId = parentMenuId },
                new { MenuName = "RoleManagement", DisplayName = "Role Management", Controller = "Role", Action = "Index", Area = "Admin", Icon = "k-i-group", ParentMenuId = parentMenuId },
                new { MenuName = "MenuManagement", DisplayName = "Menu Management", Controller = "Menu", Action = "Index", Area = "Admin", Icon = "k-i-menu", ParentMenuId = parentMenuId },
                new { MenuName = "AccessRules", DisplayName = "Access Rules", Controller = "AccessControl", Action = "Index", Area = "Admin", Icon = "k-i-lock", ParentMenuId = parentMenuId },
                new { MenuName = "AuditLog", DisplayName = "Audit Log", Controller = "AuditLog", Action = "Index", Area = "Admin", Icon = "k-i-list", ParentMenuId = parentMenuId },
                new { MenuName = "MyProfile", DisplayName = "My Profile", Controller = "Profile", Action = "Index", Area = "Admin", Icon = "k-i-user", ParentMenuId = parentMenuId }
            };

            foreach (var menuDef in defaultMenus)
            {
                result.TotalMenus++;

                // Check if menu already exists
                var existingMenu = await _menuRepository.FirstOrDefaultAsync<Menu>(
                    m => m.MenuName == menuDef.MenuName,
                    m => m
                ).ConfigureAwait(false);

                if (existingMenu != null)
                {
                    // If menu exists but doesn't have a parent (or has wrong parent), update it
                    if (existingMenu.ParentMenuId != parentMenuId && menuDef.ParentMenuId.HasValue)
                    {
                        existingMenu.ParentMenuId = parentMenuId;
                        existingMenu.MenuPath = $"{parentMenuName}/{menuDef.MenuName}";
                        existingMenu.UpdateModified(userId);
                        await _menuRepository.UpdateAsync(existingMenu).ConfigureAwait(false);
                        
                        // #region agent log
                        WriteDebugLog("MenuMigrationService.cs:670", "MENU_UPDATED_WITH_PARENT", new {
                            hypothesisId = "R",
                            menuName = menuDef.MenuName,
                            existingMenuId = existingMenu.Id,
                            newParentMenuId = parentMenuId
                        });
                        // #endregion
                        
                        result.UpdatedMenus++;
                    }
                    else
                    {
                        // #region agent log
                        WriteDebugLog("MenuMigrationService.cs:687", "MENU_EXISTS_SKIP", new {
                            hypothesisId = "K",
                            menuName = menuDef.MenuName,
                            existingMenuId = existingMenu.Id,
                            existingIsVisible = existingMenu.IsVisible,
                            existingIsActive = existingMenu.IsActive,
                            existingParentMenuId = existingMenu.ParentMenuId
                        });
                        // #endregion
                        result.SkippedMenus++;
                    }
                    continue;
                }

                // #region agent log
                WriteDebugLog("MenuMigrationService.cs:600", "CREATING_MENU", new {
                    hypothesisId = "L",
                    menuName = menuDef.MenuName,
                    displayName = menuDef.DisplayName,
                    controller = menuDef.Controller,
                    action = menuDef.Action,
                    area = menuDef.Area
                });
                // #endregion

                // Create new menu
                var menu = new Menu
                {
                    MenuName = menuDef.MenuName,
                    DisplayName = menuDef.DisplayName,
                    MenuPath = menuDef.ParentMenuId.HasValue 
                        ? $"{parentMenuName}/{menuDef.MenuName}" 
                        : menuDef.MenuName,
                    ParentMenuId = menuDef.ParentMenuId,
                    ControllerName = menuDef.Controller,
                    ActionName = menuDef.Action,
                    Area = menuDef.Area,
                    IconClass = menuDef.Icon,
                    DisplayOrder = displayOrder++,
                    IsVisible = true,
                    IsActive = true,
                    Source = "System", // Mark as system menu
                    CreatedBy = userId,
                    CreatedDate = DateTime.Now,
                    Description = $"System menu for {menuDef.DisplayName}"
                };

                // Add "web" route value so menus appear in treeview and home page
                // The views filter menus by RouteValues["web"] == "true"
                var routeValues = new Dictionary<string, string>
                {
                    { "web", "true" }
                };
                menu.RouteValues = JsonConvert.SerializeObject(routeValues);

                menu.UpdateCreated(userId);
                await _menuRepository.AddAsync(menu).ConfigureAwait(false);
                
                // #region agent log
                WriteDebugLog("MenuMigrationService.cs:632", "MENU_ADDED", new {
                    hypothesisId = "M",
                    menuName = menuDef.MenuName,
                    menuId = menu.Id,
                    isVisible = menu.IsVisible,
                    isActive = menu.IsActive
                });
                // #endregion
                
                result.CreatedMenus++;
            }

            // #region agent log
            WriteDebugLog("MenuMigrationService.cs:750", "BEFORE_FINAL_SAVE", new {
                hypothesisId = "S1",
                createdMenus = result.CreatedMenus,
                skippedMenus = result.SkippedMenus,
                totalMenus = result.TotalMenus,
                parentMenuId = parentMenuId
            });
            // #endregion

            // Save all changes
            await _menuRepository.SaveAsync().ConfigureAwait(false);

            // #region agent log
            WriteDebugLog("MenuMigrationService.cs:760", "AFTER_FINAL_SAVE", new {
                hypothesisId = "S2",
                createdMenus = result.CreatedMenus,
                skippedMenus = result.SkippedMenus,
                totalMenus = result.TotalMenus
            });
            
            // Verify menus were actually saved
            var verifyCount = await _menuRepository.CountAsync(m => m.Source == "System").ConfigureAwait(false);
            WriteDebugLog("MenuMigrationService.cs:768", "VERIFY_SAVED_MENUS", new {
                hypothesisId = "S3",
                systemMenusInDb = verifyCount,
                expectedCreated = result.CreatedMenus
            });
            // #endregion

            result.Success = result.Errors.Count == 0;
            result.Message = $"Created {result.CreatedMenus} default login module menus. {result.SkippedMenus} already existed. Verified {verifyCount} system menus in database.";
        }
        catch (Exception ex)
        {
            // #region agent log
            WriteDebugLog("MenuMigrationService.cs:650", "CREATE_DEFAULT_MENUS_ERROR", new {
                hypothesisId = "O",
                error = ex.Message,
                stackTrace = ex.StackTrace
            });
            // #endregion
            result.Errors.Add($"Failed to create default menus: {ex.Message}");
            result.Success = false;
        }

        return result;
    }

    private async Task ClearAllMenusAsync()
    {
        var allMenus = await _menuRepository.GetAsync<Menu>(null, m => m).ConfigureAwait(false);
        foreach (var menu in allMenus)
        {
            await _menuRepository.DeleteAsync(menu).ConfigureAwait(false);
        }
        await _menuRepository.SaveAsync().ConfigureAwait(false);
    }
}

