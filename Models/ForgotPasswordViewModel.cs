using System.ComponentModel.DataAnnotations;

namespace Corno.Web.Models;

public class ForgotPasswordViewModel
{
    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; }
}