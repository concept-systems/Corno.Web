using Corno.Web.Dtos;

namespace Corno.Web.Areas.Masters.Dtos.Item;

public class ItemIndexDto : MasterDto
{
    public string CurrentStock { get; set; }
    public double? Cost { get; set; }
    public string Location { get; set; }
    public string Category { get; set; }
    public double? Weight { get; set; }
}