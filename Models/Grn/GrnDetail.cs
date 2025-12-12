using System.ComponentModel.DataAnnotations;
using Corno.Web.Models.Base;
using Mapster;

namespace Corno.Web.Models.Grn;

public class GrnDetail : BaseModel
{
    #region -- Propertis --
    public int? GrnId { get; set; }
    public int? ProductId { get; set; }
    public int? ItemId { get; set; }
    public string ItemCode { get; set; }
    public string Position { get; set; }
    public string InspectionNo { get; set; }
    public string LotNo { get; set; }
    public double? OrderQuantity { get; set; }
    public double? DeliverQuantity { get; set; }
    public double? AcceptQuantity { get; set; }
    public double? RejectQuantity { get; set; }
    public double? ExtraQuantity { get; set; }
    public double? SaleQuantity { get; set; }
    public double? PrintQuantity { get; set; }
    public double? Q1Quantity { get; set; }
    public double? RackInQuantity { get; set; }
    public double? RackOutQuantity { get; set; }

    public string Remark { get; set; }

    [Required]
    [AdaptIgnore]
    protected virtual Grn Grn { get; set; }
    #endregion

    #region -- Methods --
    public void Copy(GrnDetail other)
    {
        ProductId = other.ProductId;
        ItemId = other.ItemId;
        ItemCode = other.ItemCode;
        Position = other.Position;
        InspectionNo = other.InspectionNo;
        LotNo = other.LotNo;
        OrderQuantity = other.OrderQuantity;
        DeliverQuantity = other.DeliverQuantity;
        AcceptQuantity = other.AcceptQuantity;
        RejectQuantity = other.RejectQuantity;
        ExtraQuantity = other.ExtraQuantity;
        SaleQuantity = other.SaleQuantity;
        PrintQuantity = other.PrintQuantity;
        Q1Quantity = other.Q1Quantity;
        RackInQuantity = other.RackInQuantity;
        RackOutQuantity = other.RackOutQuantity;

        Status = other.Status;

        ModifiedBy = other.ModifiedBy;
        ModifiedDate = other.ModifiedDate;
    }
    #endregion
}