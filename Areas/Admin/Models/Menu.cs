using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Corno.Web.Models.Base;

namespace Corno.Web.Areas.Admin.Models;

/// <summary>
/// Represents a menu item in the application navigation structure.
/// Supports hierarchical menu structure with parent-child relationships.
/// </summary>
public class Menu : BaseModel
{
    #region -- Constructors --
    public Menu()
    {
        ChildMenus = new List<Menu>();
    }
    #endregion

    #region -- Properties --
    
    /// <summary>
    /// Unique identifier for the menu (used for permissions and references)
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string MenuName { get; set; }
    
    /// <summary>
    /// Display name shown in the UI
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string DisplayName { get; set; }
    
    /// <summary>
    /// Full path from root (e.g., "Dashboard/UserManagement/CreateUser")
    /// </summary>
    [MaxLength(500)]
    public string MenuPath { get; set; }
    
    /// <summary>
    /// Parent menu ID for hierarchical structure
    /// </summary>
    public int? ParentMenuId { get; set; }
    
    [ForeignKey("ParentMenuId")]
    public virtual Menu ParentMenu { get; set; }
    
    /// <summary>
    /// Child menus
    /// </summary>
    public virtual ICollection<Menu> ChildMenus { get; set; }
    
    /// <summary>
    /// MVC Controller name
    /// </summary>
    [MaxLength(100)]
    public string ControllerName { get; set; }
    
    /// <summary>
    /// MVC Action name
    /// </summary>
    [MaxLength(100)]
    public string ActionName { get; set; }
    
    /// <summary>
    /// MVC Area name
    /// </summary>
    [MaxLength(100)]
    public string Area { get; set; }
    
    /// <summary>
    /// CSS class for icon (e.g., "fa fa-dashboard", "k-icon k-i-home")
    /// </summary>
    [MaxLength(100)]
    public string IconClass { get; set; }
    
    /// <summary>
    /// Additional route values in JSON format
    /// </summary>
    [Column(TypeName = "NVARCHAR(MAX)")]
    public string RouteValues { get; set; }
    
    /// <summary>
    /// Display order for sorting
    /// </summary>
    public int DisplayOrder { get; set; }
    
    /// <summary>
    /// Whether menu is visible in navigation
    /// </summary>
    public bool IsVisible { get; set; }
    
    /// <summary>
    /// Whether menu is active/enabled
    /// </summary>
    public bool IsActive { get; set; }
    
    /// <summary>
    /// Menu description
    /// </summary>
    [MaxLength(500)]
    public string Description { get; set; }
    
    /// <summary>
    /// Source of menu: "XML" (migrated from XML), "Manual" (user-created), "System" (system-generated)
    /// Defaults to "Manual" for user-created menus
    /// </summary>
    [MaxLength(50)]
    public string Source { get; set; } = "Manual";
    
    #endregion

    //#region -- Methods --
    //public override void Reset()
    //{
    //    base.Reset();
    //    MenuName = string.Empty;
    //    DisplayName = string.Empty;
    //    MenuPath = string.Empty;
    //    ParentMenuId = null;
    //    ControllerName = string.Empty;
    //    ActionName = string.Empty;
    //    Area = string.Empty;
    //    IconClass = string.Empty;
    //    RouteValues = null;
    //    DisplayOrder = 0;
    //    IsVisible = true;
    //    IsActive = true;
    //    Description = string.Empty;
    //    Source = "Manual";
    //}
    //#endregion
}

