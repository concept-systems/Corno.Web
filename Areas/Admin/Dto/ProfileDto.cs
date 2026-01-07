using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Corno.Web.Areas.Admin.Dto;

public class ProfileDto
{
    public string Id { get; set; }
    
    [Required]
    [MaxLength(256)]
    public string UserName { get; set; }
    
    [MaxLength(100)]
    public string FirstName { get; set; }
    
    [MaxLength(100)]
    public string LastName { get; set; }
    
    [EmailAddress]
    [MaxLength(256)]
    public string Email { get; set; }
    
    [Phone]
    [MaxLength(50)]
    public string PhoneNumber { get; set; }
    
    public string ProfilePicturePath { get; set; }
    
    public List<string> Roles { get; set; }
    public DateTime? AccountCreatedDate { get; set; }
    public DateTime? LastLoginTime { get; set; }
    public int ActiveSessions { get; set; }
    public string LastIpAddress { get; set; }
}

public class ChangePasswordDto
{
    [Required]
    public string UserId { get; set; }
    
    [Required]
    [DataType(DataType.Password)]
    public string CurrentPassword { get; set; }
    
    [Required]
    [DataType(DataType.Password)]
    public string OldPassword { get; set; }
    
    [Required]
    [DataType(DataType.Password)]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
    public string NewPassword { get; set; }
    
    [Required]
    [DataType(DataType.Password)]
    [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
    public string ConfirmPassword { get; set; }
}

public class ForgotPasswordDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
}

public class ResetPasswordDto
{
    [Required]
    public string Token { get; set; }
    
    [Required]
    public string UserId { get; set; }
    
    [Required]
    [DataType(DataType.Password)]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
    public string NewPassword { get; set; }
    
    [Required]
    [DataType(DataType.Password)]
    [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
    public string ConfirmPassword { get; set; }
}

