using Corno.Web.Models.Base;

namespace Corno.Web.Areas.Masters.ViewModels.Location;

public class LocationItemViewModel : BaseModel
{
    public int? LocationId { get; set; }
    public int ItemId { get; set; }
    public int ProductId { get; set; }
    public double? MaxQuantity { get; set; }
    public string ItemName { get; set; }
    public string ProductName { get; set; }

    public virtual LocationViewModel Location { get; set; }

}