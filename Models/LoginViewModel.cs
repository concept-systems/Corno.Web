using System.ComponentModel.DataAnnotations;

namespace Corno.Web.Models;

public class LoginViewModel
{
    [Display(Name = "Company")]
    public int? CompanyId { get; set; }
    [Display(Name = "Financial Year")]
    public int? FinancialYearId { get; set; }

    /*[Required]*/
    [Display(Name = "User Name")]
    //[Display(Name = "Email")]
    //[EmailAddress]
    public string Email { get; set; }
    public string UserName { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; }

    [Display(Name = "Remember me")]
    public bool RememberMe { get; set; }

}