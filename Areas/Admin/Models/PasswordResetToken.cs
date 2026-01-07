using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Corno.Web.Models.Base;

namespace Corno.Web.Areas.Admin.Models;

/// <summary>
/// Represents a password reset token for forgot password functionality.
/// </summary>
public class PasswordResetToken : BaseModel
{
    #region -- Properties --
    
    /// <summary>
    /// User ID requesting password reset
    /// </summary>
    [Required]
    [MaxLength(128)]
    public string UserId { get; set; }
    
    [ForeignKey("UserId")]
    public virtual AspNetUser User { get; set; }
    
    /// <summary>
    /// Reset token (hashed)
    /// </summary>
    [Required]
    [MaxLength(500)]
    public string Token { get; set; }
    
    /// <summary>
    /// Token expiry date
    /// </summary>
    [Required]
    public DateTime ExpiryDate { get; set; }
    
    /// <summary>
    /// Whether token has been used
    /// </summary>
    public bool IsUsed { get; set; }
    
    /// <summary>
    /// When token was created
    /// </summary>
    public DateTime CreatedDate { get; set; }
    
    /// <summary>
    /// When token was used (if used)
    /// </summary>
    public DateTime? UsedDate { get; set; }
    
    /// <summary>
    /// IP address from which reset was requested
    /// </summary>
    [MaxLength(50)]
    public string IpAddress { get; set; }
    
    #endregion

    #region -- Methods --
    public override void Reset()
    {
        base.Reset();
        UserId = string.Empty;
        Token = string.Empty;
        ExpiryDate = DateTime.Now.AddHours(24); // Default 24 hours expiry
        IsUsed = false;
        CreatedDate = DateTime.Now;
        UsedDate = null;
        IpAddress = string.Empty;
    }
    
    /// <summary>
    /// Checks if token is valid (not used and not expired)
    /// </summary>
    public bool IsValid => !IsUsed && ExpiryDate > DateTime.Now;
    #endregion
}

