using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Corno.Web.Areas.Admin.Dto;
using Corno.Web.Areas.Admin.Models;
using Corno.Web.Areas.Admin.Services.Interfaces;
using Corno.Web.Repository.Interfaces;
using Corno.Web.Services;
using Corno.Web.Windsor;
using Mapster;

namespace Corno.Web.Areas.Admin.Services;

public class MenuService : CornoService<Menu>, IMenuService
{
    #region -- Constructors --
    public MenuService(IGenericRepository<Menu> genericRepository) : base(genericRepository)
    {
    }
    #endregion

    #region -- Public Methods --
    public async Task<List<MenuDto>> GetMenuTreeAsync()
    {
        var menus = await GetAsync<Menu>(
            m => m.IsActive,
            m => m,
            q => q.OrderBy(x => x.DisplayOrder)
        ).ConfigureAwait(false);

        var rootMenus = menus.Where(m => m.ParentMenuId == null).ToList();
        var menuDtos = new List<MenuDto>();

        foreach (var rootMenu in rootMenus)
        {
            var menuDto = BuildMenuTree(rootMenu, menus, 0);
            menuDtos.Add(menuDto);
        }

        return menuDtos;
    }

    public async Task<List<MenuDto>> GetAllMenusAsync(int? excludeMenuId = null)
    {
        var menus = await GetAsync<Menu>(
            m => m.IsActive && (excludeMenuId == null || m.Id != excludeMenuId.Value),
            m => m,
            q => q.OrderBy(x => x.DisplayOrder)
        ).ConfigureAwait(false);

        // Flatten the hierarchical structure for dropdown
        var flatList = new List<MenuDto>();
        var rootMenus = menus.Where(m => m.ParentMenuId == null).OrderBy(m => m.DisplayOrder).ToList();

        foreach (var rootMenu in rootMenus)
        {
            FlattenMenuTree(rootMenu, menus, flatList, 0, excludeMenuId);
        }

        return flatList;
    }

    private void FlattenMenuTree(Menu menu, List<Menu> allMenus, List<MenuDto> flatList, int level, int? excludeMenuId)
    {
        // Skip if this is the menu being excluded
        if (excludeMenuId.HasValue && menu.Id == excludeMenuId.Value)
            return;

        var dto = menu.Adapt<MenuDto>();
        dto.Level = level;
        // Add indentation prefix to display name for hierarchy
        dto.DisplayName = new string(' ', level * 2) + dto.DisplayName;
        flatList.Add(dto);

        // Process children
        var childMenus = allMenus.Where(m => m.ParentMenuId == menu.Id).OrderBy(m => m.DisplayOrder).ToList();
        foreach (var childMenu in childMenus)
        {
            FlattenMenuTree(childMenu, allMenus, flatList, level + 1, excludeMenuId);
        }
    }

    private MenuDto BuildMenuTree(Menu menu, List<Menu> allMenus, int level)
    {
        var dto = menu.Adapt<MenuDto>();
        dto.Level = level;
        dto.ChildMenus = new List<MenuDto>();

        var childMenus = allMenus.Where(m => m.ParentMenuId == menu.Id).OrderBy(m => m.DisplayOrder).ToList();
        dto.HasChildren = childMenus.Any();

        foreach (var childMenu in childMenus)
        {
            var childDto = BuildMenuTree(childMenu, allMenus, level + 1);
            dto.ChildMenus.Add(childDto);
        }

        return dto;
    }

    public async Task<MenuDto> GetByIdAsync(int id)
    {
        var menu = await FirstOrDefaultAsync(m => m.Id == id, m => m).ConfigureAwait(false);
        if (menu == null)
            return null;

        var dto = menu.Adapt<MenuDto>();
        if (menu.ParentMenuId.HasValue)
        {
            var parent = await FirstOrDefaultAsync(m => m.Id == menu.ParentMenuId.Value, m => m).ConfigureAwait(false);
            dto.ParentMenuName = parent?.DisplayName;
        }

        return dto;
    }

    public async Task<MenuDto> CreateAsync(MenuDto dto, string userId)
    {
        if (string.IsNullOrEmpty(dto.MenuName))
            throw new Exception("Menu Name is required");

        // Check for duplicate menu name
        var existing = await FirstOrDefaultAsync(m => m.MenuName == dto.MenuName, m => m).ConfigureAwait(false);
        if (existing != null)
            throw new Exception($"Menu with name '{dto.MenuName}' already exists");

        var menu = dto.Adapt<Menu>();
        menu.Source = "Manual"; // Mark as manually created
        menu.CreatedBy = userId;
        menu.CreatedDate = DateTime.Now;
        menu.UpdateCreated(userId);

        // Build menu path
        if (menu.ParentMenuId.HasValue)
        {
            var parent = await FirstOrDefaultAsync(m => m.Id == menu.ParentMenuId.Value, m => m).ConfigureAwait(false);
            menu.MenuPath = parent != null ? $"{parent.MenuPath}/{menu.MenuName}" : menu.MenuName;
        }
        else
        {
            menu.MenuPath = menu.MenuName;
        }

        await AddAndSaveAsync(menu).ConfigureAwait(false);

        return menu.Adapt<MenuDto>();
    }

    public async Task<MenuDto> UpdateAsync(MenuDto dto, string userId)
    {
        var menu = await FirstOrDefaultAsync(m => m.Id == dto.Id, m => m).ConfigureAwait(false);
        if (menu == null)
            throw new Exception($"Menu with ID {dto.Id} not found");

        // Check for duplicate menu name (excluding current menu)
        var existing = await FirstOrDefaultAsync(m => m.MenuName == dto.MenuName && m.Id != dto.Id, m => m).ConfigureAwait(false);
        if (existing != null)
            throw new Exception($"Menu with name '{dto.MenuName}' already exists");

        // Rebuild menu path if parent changed (before updating ParentMenuId)
        string newMenuPath;
        if (dto.ParentMenuId.HasValue)
        {
            var parent = await FirstOrDefaultAsync(m => m.Id == dto.ParentMenuId.Value, m => m).ConfigureAwait(false);
            newMenuPath = parent != null ? $"{parent.MenuPath}/{dto.MenuName}" : dto.MenuName;
        }
        else
        {
            newMenuPath = dto.MenuName;
        }

        // Create a clean entity with only scalar properties to avoid navigation property issues
        // This prevents Mapster Adapt from trying to process navigation properties in GenericRepository
        var menuToUpdate = new Menu
        {
            Id = menu.Id,
            MenuName = dto.MenuName,
            DisplayName = dto.DisplayName,
            ControllerName = dto.ControllerName,
            ActionName = dto.ActionName,
            Area = dto.Area,
            IconClass = dto.IconClass,
            RouteValues = dto.RouteValues,
            DisplayOrder = dto.DisplayOrder,
            IsVisible = dto.IsVisible,
            IsActive = dto.IsActive,
            Description = dto.Description,
            ParentMenuId = dto.ParentMenuId,
            MenuPath = newMenuPath,
            Source = menu.Source, // Preserve Source - don't overwrite
            CreatedBy = menu.CreatedBy,
            CreatedDate = menu.CreatedDate,
            ModifiedBy = userId,
            ModifiedDate = DateTime.Now,
            CompanyId = menu.CompanyId,
            SerialNo = menu.SerialNo,
            Code = menu.Code,
            Status = menu.Status,
            ExtraProperties = menu.ExtraProperties,
            DeletedBy = menu.DeletedBy,
            DeletedDate = menu.DeletedDate
        };
        // Explicitly set navigation properties to null to prevent Mapster Adapt from processing them
        // This is critical to avoid "NoTracking merge option" errors in GenericRepository.UpdateAsync
        //menuToUpdate.ParentMenu = null;
        menuToUpdate.ChildMenus = null; // Override constructor initialization

        menuToUpdate.UpdateModified(userId);

        var menuService = Bootstrapper.Get<IMenuService>();
        await UpdateAndSaveAsync(menuToUpdate).ConfigureAwait(false);

        // Return updated entity from database
        var updatedMenu = await FirstOrDefaultAsync(m => m.Id == dto.Id, m => m).ConfigureAwait(false);
        return updatedMenu?.Adapt<MenuDto>();
    }

    public async Task DeleteAsync(int id)
    {
        var menu = await FirstOrDefaultAsync(m => m.Id == id, m => m).ConfigureAwait(false);
        if (menu == null)
            throw new Exception($"Menu with ID {id} not found");

        // Check if menu has children
        var hasChildren = await FirstOrDefaultAsync(m => m.ParentMenuId == id, m => m).ConfigureAwait(false);
        if (hasChildren != null)
            throw new Exception("Cannot delete menu with child menus. Please delete or move child menus first.");

        await DeleteAsync(menu).ConfigureAwait(false);
        await SaveAsync().ConfigureAwait(false);
    }

    public async Task<bool> MoveMenuAsync(int menuId, int? newParentId, int newOrder)
    {
        var menu = await FirstOrDefaultAsync(m => m.Id == menuId, m => m).ConfigureAwait(false);
        if (menu == null)
            return false;

        menu.ParentMenuId = newParentId;
        menu.DisplayOrder = newOrder;

        // Rebuild menu path
        if (menu.ParentMenuId.HasValue)
        {
            var parent = await FirstOrDefaultAsync(m => m.Id == menu.ParentMenuId.Value, m => m).ConfigureAwait(false);
            menu.MenuPath = parent != null ? $"{parent.MenuPath}/{menu.MenuName}" : menu.MenuName;
        }
        else
        {
            menu.MenuPath = menu.MenuName;
        }

        await UpdateAndSaveAsync(menu).ConfigureAwait(false);
        return true;
    }

    public async Task<List<MenuDto>> GetUserMenusAsync(string userId)
    {
        // This will be filtered by permission service
        var menus = await GetMenuTreeAsync().ConfigureAwait(false);
        return menus;
    }

    public async Task<List<MenuDto>> GetVisibleMenusAsync()
    {
        var menus = await GetAsync<Menu>(
            m => m.IsActive && m.IsVisible,
            m => m,
            q => q.OrderBy(x => x.DisplayOrder)
        ).ConfigureAwait(false);

        return menus.Adapt<List<MenuDto>>();
    }

    public async Task<int> GetMenuCountAsync()
    {
        return await CountAsync(m => true).ConfigureAwait(false);
    }
    #endregion
}

