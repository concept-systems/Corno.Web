using System.ComponentModel.DataAnnotations;
using Corno.Web.Models.Base;

namespace Corno.Web.Areas.Admin.Models;

/// <summary>
/// Represents a type of permission (Menu, Page, or Control)
/// </summary>
public class PermissionType : BaseModel
{
    #region -- Properties --
    
    /// <summary>
    /// Permission type name (Menu, Page, Control)
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Name { get; set; }
    
    /// <summary>
    /// Description of the permission type
    /// </summary>
    [MaxLength(200)]
    public string Description { get; set; }
    
    /// <summary>
    /// Display order for UI
    /// </summary>
    public int DisplayOrder { get; set; }
    
    #endregion

    #region -- Methods --
    public override void Reset()
    {
        base.Reset();
        Name = string.Empty;
        Description = string.Empty;
        DisplayOrder = 0;
    }
    #endregion
}

