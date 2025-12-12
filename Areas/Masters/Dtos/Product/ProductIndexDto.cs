using Corno.Web.Dtos;

namespace Corno.Web.Areas.Masters.Dtos.Product;

public class ProductIndexDto : MasterDto
{
    public new string SerialNo { get; set; }
    public string CustomerName { get; set; }
    public string PoNo { get; set; }
    public string Category { get; set; }
}