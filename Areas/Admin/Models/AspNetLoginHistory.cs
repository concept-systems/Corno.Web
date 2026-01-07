using Mapster;
using System;
using System.ComponentModel.DataAnnotations;

namespace Corno.Web.Areas.Admin.Models;

/// <summary>
/// Tracks user authentication history and active sessions.
/// 
/// Dual Purpose:
/// 1. Historical Audit: Records all login attempts (success/failure) for audit trail
/// 2. Session Management: Tracks active user sessions for concurrent session control
/// 
/// Active Session: IsActive = true AND LogoutTime = null
/// Historical Record: LogoutTime != null (completed session)
/// </summary>
public class AspNetLoginHistory
{
    [Key]
    public string Id { get; set; }
    public string AspNetUserId { get; set; }
    public string UserName { get; set; }
    
    // Historical Audit Fields
    public DateTime? LoginTime { get; set; }
    public DateTime? LogoutTime { get; set; }
    public string IpAddress { get; set; }
    public string MachineName { get; set; }
    public string HostName { get; set; }
    [Required]
    public LoginResult LoginResult { get; set; }
    
    // Active Session Management Fields
    /// <summary>
    /// ASP.NET Session ID
    /// </summary>
    [MaxLength(200)]
    public string SessionId { get; set; }
    
    /// <summary>
    /// Last activity timestamp
    /// </summary>
    public DateTime? LastActivityTime { get; set; }
    
    /// <summary>
    /// Browser/device info
    /// </summary>
    [MaxLength(500)]
    public string UserAgent { get; set; }
    
    /// <summary>
    /// Device type, OS, etc.
    /// </summary>
    [MaxLength(200)]
    public string DeviceInfo { get; set; }
    
    /// <summary>
    /// Active session flag
    /// </summary>
    public bool IsActive { get; set; }

    [AdaptIgnore]
    public virtual AspNetUser AspNetUser { get; set; }
    
    /// <summary>
    /// Indicates if this is an active session
    /// </summary>
    public bool IsCurrentSession => IsActive && LogoutTime == null;
    
    /// <summary>
    /// Indicates if this is a completed/historical session
    /// </summary>
    public bool IsHistoricalRecord => LogoutTime != null;
}