using System.Collections.Generic;
using System.Threading.Tasks;
using Corno.Web.Areas.Admin.Dto;

namespace Corno.Web.Areas.Admin.Services.Interfaces;

public interface IMenuService
{
    Task<List<MenuDto>> GetMenuTreeAsync();
    Task<List<MenuDto>> GetAllMenusAsync(int? excludeMenuId = null);
    Task<MenuDto> GetByIdAsync(int id);
    Task<MenuDto> CreateAsync(MenuDto dto, string userId);
    Task<MenuDto> UpdateAsync(MenuDto dto, string userId);
    Task DeleteAsync(int id);
    Task<bool> MoveMenuAsync(int menuId, int? newParentId, int newOrder);
    Task<List<MenuDto>> GetUserMenusAsync(string userId);
    Task<List<MenuDto>> GetVisibleMenusAsync();
    Task<int> GetMenuCountAsync();
}

