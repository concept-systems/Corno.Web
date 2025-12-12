using Corno.Web.Models.Base;

namespace Corno.Web.Areas.BoardStore.Models;

public class Movement : BaseModel
{
    public int? RequestCode { get; set; }
    public int? RequestId { get; set; }
    public int? ItemId { get; set; }

    public int? RequestNo { get; set; }
    public int? RequestPriority { get; set; }
    public int? StackId { get; set; }
    public int? StackNo { get; set; }
    public int? StackSerialNo { get; set; }
    public int? ItemPriority { get; set; }

    public int? Quantity { get; set; }
    public double? Length { get; set; }
    public double? Width { get; set; }
    public double? Weight { get; set; }

    public int? FromBayId { get; set; }
    public int? FromLocationId { get; set; }
    public int? FromLocationTypeId { get; set; }

    public int? FromLocationX { get; set; }
    public int? FromLocationY { get; set; }

    public int? ToBayId { get; set; }
    public int? ToLocationId { get; set; }
    public int? ToLocationTypeId { get; set; }

    public int? ToLocationX { get; set; }
    public int? ToLocationY { get; set; }

    public int? QueuePosition { get; set; }

    public double? FromLocationWidth { get; set; }
    public double? FromLocationLength { get; set; }
    public double? ToLocationWidth { get; set; }
    public double? ToLocationLength { get; set; }
}