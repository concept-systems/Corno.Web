using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Corno.Web.Models.Base;

namespace Corno.Web.Areas.Admin.Models;

/// <summary>
/// Represents an audit log entry for tracking all user actions in the system.
/// </summary>
public class AuditLog : BaseModel
{
    #region -- Properties --
    
    /// <summary>
    /// User ID who performed the action
    /// </summary>
    [MaxLength(128)]
    public string UserId { get; set; }
    
    [ForeignKey("UserId")]
    public virtual AspNetUser User { get; set; }
    
    /// <summary>
    /// Username at the time of action (for historical reference)
    /// </summary>
    [MaxLength(256)]
    public string UserName { get; set; }
    
    /// <summary>
    /// Action performed (Login, Logout, Create, Edit, Delete, View, etc.)
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Action { get; set; }
    
    /// <summary>
    /// Entity type (User, Role, Menu, AccessControl, etc.)
    /// </summary>
    [MaxLength(100)]
    public string EntityType { get; set; }
    
    /// <summary>
    /// Entity ID
    /// </summary>
    [MaxLength(128)]
    public string EntityId { get; set; }
    
    /// <summary>
    /// Entity name (for display purposes)
    /// </summary>
    [MaxLength(200)]
    public string EntityName { get; set; }
    
    /// <summary>
    /// Additional details (JSON or text)
    /// </summary>
    [Column(TypeName = "NVARCHAR(MAX)")]
    public string Details { get; set; }
    
    /// <summary>
    /// IP address from which action was performed
    /// </summary>
    [MaxLength(50)]
    public string IpAddress { get; set; }
    
    /// <summary>
    /// User agent (browser/device info)
    /// </summary>
    [MaxLength(500)]
    public string UserAgent { get; set; }
    
    /// <summary>
    /// Timestamp when action was performed
    /// </summary>
    [Required]
    public DateTime Timestamp { get; set; }
    
    #endregion

    #region -- Methods --
    public override void Reset()
    {
        base.Reset();
        UserId = null;
        UserName = string.Empty;
        Action = string.Empty;
        EntityType = string.Empty;
        EntityId = null;
        EntityName = string.Empty;
        Details = null;
        IpAddress = string.Empty;
        UserAgent = string.Empty;
        Timestamp = DateTime.Now;
    }
    #endregion
}

