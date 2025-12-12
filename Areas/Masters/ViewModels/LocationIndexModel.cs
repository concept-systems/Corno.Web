using Corno.Web.Dtos;

namespace Corno.Web.Areas.Masters.ViewModels;

public class LocationIndexModel : MasterDto
{
    public int? ShelfId { get; set; }
    public int? AreaId { get; set; }
    public int? RowId { get; set; }
    public int? BayId { get; set; }

}