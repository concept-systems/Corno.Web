using System.ComponentModel.DataAnnotations;

namespace Corno.Web.Models;

public class OtpViewModel
{
    [Required]
    [Display(Name = "Mobile No")]
    //[Display(Name = "Email")]
    //[EmailAddress]
    public string MobileNo { get; set; }

        
    [Display(Name = "Otp")]
    public string Otp { get; set; }

    //[Display(Name = "Remember me")]
    //public bool RememberMe { get; set; }
}