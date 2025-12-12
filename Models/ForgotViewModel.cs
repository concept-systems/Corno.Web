using System.ComponentModel.DataAnnotations;

namespace Corno.Web.Models;

public class ForgotViewModel
{
    [Required]
    [Display(Name = "Email")]
    public string Email { get; set; }
}