using Corno.Web.Models.Base;

namespace Corno.Web.Areas.BoardStore.Models;

public class LocationStack : BaseModel
{
    public int? LocationId { get; set; }
    public int? ItemId { get; set; }
    public int? Position { get; set; }
    public int? TransactionId { get; set; }
    public int? BoardCentering { get; set; }
    public int? IntermediateId { get; set; }

    public Location Location { get; set; }
}