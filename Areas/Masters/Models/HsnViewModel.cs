using Corno.Web.Dtos;

namespace Corno.Web.Areas.Masters.Models;

public class HsnViewModel : MasterDto
{
    public double? Cgst { get; set; }

    public double? Sgst { get; set; }

    public double? Igst { get; set; }

}