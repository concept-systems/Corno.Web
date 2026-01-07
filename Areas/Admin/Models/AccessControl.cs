using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Corno.Web.Models.Base;

namespace Corno.Web.Areas.Admin.Models;

/// <summary>
/// Represents access control permissions for Menu, Page, or Control level.
/// Supports both User-level and Role-level permissions.
/// </summary>
public class AccessControl : BaseModel
{
    #region -- Properties --
    
    /// <summary>
    /// Permission type (Menu, Page, or Control)
    /// </summary>
    [Required]
    public int PermissionTypeId { get; set; }
    
    [ForeignKey("PermissionTypeId")]
    public virtual PermissionType PermissionType { get; set; }
    
    /// <summary>
    /// Reference to Menu (for Menu and Page level permissions)
    /// </summary>
    public int? MenuId { get; set; }
    
    [ForeignKey("MenuId")]
    public virtual Menu Menu { get; set; }
    
    /// <summary>
    /// Controller name (for Page and Control level permissions)
    /// </summary>
    [MaxLength(100)]
    public string ControllerName { get; set; }
    
    /// <summary>
    /// Action name (for Page and Control level permissions)
    /// </summary>
    [MaxLength(100)]
    public string ActionName { get; set; }
    
    /// <summary>
    /// Area name (for Page and Control level permissions)
    /// </summary>
    [MaxLength(100)]
    public string Area { get; set; }
    
    /// <summary>
    /// Control ID (for Control level permissions only, e.g., "btnCreate", "btnDelete")
    /// </summary>
    [MaxLength(200)]
    public string ControlId { get; set; }
    
    /// <summary>
    /// Control display name (for UI purposes)
    /// </summary>
    [MaxLength(200)]
    public string ControlName { get; set; }
    
    /// <summary>
    /// User ID (NULL = Role-based, NOT NULL = User-specific)
    /// </summary>
    [MaxLength(128)]
    public string UserId { get; set; }
    
    [ForeignKey("UserId")]
    public virtual AspNetUser User { get; set; }
    
    /// <summary>
    /// Role ID (NULL = User-based, NOT NULL = Role-based)
    /// </summary>
    [MaxLength(128)]
    public string RoleId { get; set; }
    
    [ForeignKey("RoleId")]
    public virtual AspNetRole Role { get; set; }
    
    /// <summary>
    /// Whether access is allowed (true) or denied (false)
    /// </summary>
    public bool IsAllowed { get; set; }
    
    #endregion

    #region -- Methods --
    public override void Reset()
    {
        base.Reset();
        PermissionTypeId = 0;
        MenuId = null;
        ControllerName = string.Empty;
        ActionName = string.Empty;
        Area = string.Empty;
        ControlId = string.Empty;
        ControlName = string.Empty;
        UserId = null;
        RoleId = null;
        IsAllowed = true;
    }
    #endregion
}

