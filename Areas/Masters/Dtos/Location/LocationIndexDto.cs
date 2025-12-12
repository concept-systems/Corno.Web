using Corno.Web.Dtos;

namespace Corno.Web.Areas.Masters.Dtos.Location;

public class LocationIndexDto : MasterDto
{
    public int? ShelfId { get; set; }
    public int? AreaId { get; set; }
    public int? RowId { get; set; }
    public int? BayId { get; set; }
    public string ProductCode { get; set; }
    public string ProductName { get; set; }

}