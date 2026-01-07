using System.Collections.Generic;
using System.Threading.Tasks;
using Corno.Web.Areas.Admin.Dto;

namespace Corno.Web.Areas.Admin.Services.Interfaces;

public interface IPermissionService
{
    // Menu Level (Level 1)
    Task<bool> HasMenuAccessAsync(string userId, int menuId);
    Task<bool> HasMenuAccessAsync(string userId, string menuName);
    Task<List<AccessControlDto>> GetMenuPermissionsAsync(int? menuId, string userId = null, string roleId = null);
    Task SaveMenuPermissionsAsync(string userId, string roleId, Dictionary<int, bool> permissions);
    
    // Page Level (Level 2)
    Task<bool> HasPageAccessAsync(string userId, string controller, string action, string area = null);
    Task<List<AccessControlDto>> GetPagePermissionsAsync(string controller, string action, string userId = null, string roleId = null);
    Task SavePagePermissionsAsync(string userId, string roleId, string controller, string action, string area, bool isAllowed);
    
    // Control Level (Level 3)
    Task<bool> HasControlAccessAsync(string userId, string controlId, string controller = null, string action = null);
    Task<List<AccessControlDto>> GetControlPermissionsAsync(string controlId, string userId = null, string roleId = null);
    Task SaveControlPermissionsAsync(string userId, string roleId, string controlId, string controlName, bool isAllowed, string controller = null, string action = null);
    
    // Combined
    Task<Dictionary<string, bool>> GetAllUserPermissionsAsync(string userId);
    Task<PermissionAssignmentDto> GetPermissionAssignmentAsync(int menuId, string userId = null, string roleId = null);
}

