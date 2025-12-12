using System.Collections.Generic;

namespace Corno.Web.Models;

public class ConfigureTwoFactorViewModel
{
    public string SelectedProvider { get; set; }
    public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
}