using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Corno.Web.Models.Base;
using Mapster;

namespace Corno.Web.Models.Plan;

[Serializable]
public class PlanItemDetail : BaseModel
{
    #region -- Properties --
    //[ForeignKey("Plan")]
    [AdaptIgnore]
    public int? PlanId { get; set; }
    public string SoPosition { get; set; }
    public string ProductionOrderNo { get; set; }
    public string WarehousePosition { get; set; }
    public int? PlantId { get; set; }

    [DisplayName("Product")]
    public int? ProductId { get; set; }
    public int? PackingTypeId { get; set; }
    public int? SecondPackingTypeId { get; set; }
    [DisplayName("Item")]
    public int? ItemId { get; set; }
    public string ItemCode { get; set; }
    public string Description { get; set; }
    public string ItemType { get; set; }
    public string Group { get; set; }
    public string DrawingNo { get; set; }
    [DisplayName("Parent Item")]
    public int? ParentItemId { get; set; }
    public string Position { get; set; }
    public string CarcassCode { get; set; }
    public string AssemblyCode { get; set; }

    public string SupervisorId { get; set; }
    public string OperatorId { get; set; }
    public string ProductLine { get; set; }

    public double? BomQuantity { get; set; }
    public double? OrderQuantity { get; set; }
    public double? PlanQuantity { get; set; }
    public double? PrintQuantity { get; set; }
    public double? CutQuantity { get; set; }
    public double? BendQuantity { get; set; }
    public double? SortQuantity { get; set; }
    public double? EdgeBandQuantity { get; set; }
    public double? Q1Quantity { get; set; }
    public double? RoutingQuantity { get; set; }
    public double? ManualEdgeBandQuantity { get; set; }
    public double? DrillQuantity { get; set; }
    public double? CleanQuantity { get; set; }
    public double? SubAssemblyQuantity { get; set; }
    public double? Q2Quantity { get; set; }
    public double? PackQuantity { get; set; }
    public double? Q3Quantity { get; set; }
    public double? PalletInQuantity { get; set; }
    public double? HandoverQuantity { get; set; }
    public double? HandoverReceiveQuantity { get; set; }
    public double? RejectQuantity { get; set; }

    public int? TrolleyNo { get; set; }
    public string Remark { get; set; }
    public string Reserved1 { get; set; }
    public string Reserved2 { get; set; }

    [Required]
    [AdaptIgnore]
    public virtual Plan Plan { get; set; }
    #endregion

    #region -- Public Methods --
    public void Copy(PlanItemDetail other)
    {
        if (null == other) return;

        SoPosition = other.SoPosition;
        ProductionOrderNo = other.ProductionOrderNo;
        WarehousePosition = other.WarehousePosition;
        PlantId = other.PlantId;

        ProductId = other.ProductId;
        PackingTypeId = other.PackingTypeId;
        ItemId = other.ItemId;
        ItemCode = other.ItemCode;
        Description = other.Description;
        ItemType = other.ItemType;
        Group = other.Group;
        DrawingNo = other.DrawingNo;

        ParentItemId = other.ParentItemId;
        Position = other.Position;
        CarcassCode = other.CarcassCode;
        AssemblyCode = other.AssemblyCode;

        SupervisorId = other.SupervisorId;
        OperatorId = other.OperatorId;
        ProductLine = other.ProductLine;

        BomQuantity = other.BomQuantity;
        OrderQuantity = other.OrderQuantity;
        PlanQuantity = other.PlanQuantity;
        PrintQuantity = other.PrintQuantity;
        CutQuantity = other.CutQuantity;
        BendQuantity = other.BendQuantity;
        SortQuantity = other.SortQuantity;
        EdgeBandQuantity = other.EdgeBandQuantity;
        Q1Quantity = other.Q1Quantity;
        RoutingQuantity = other.RoutingQuantity;
        ManualEdgeBandQuantity = other.ManualEdgeBandQuantity;
        DrillQuantity = other.DrillQuantity;
        CleanQuantity = other.CleanQuantity;
        SubAssemblyQuantity = other.SubAssemblyQuantity;
        Q2Quantity = other.Q2Quantity;
        PackQuantity = other.PackQuantity;
        Q3Quantity = other.Q3Quantity;
        PalletInQuantity = other.PalletInQuantity;
        HandoverQuantity = other.HandoverQuantity;
        HandoverReceiveQuantity = other.HandoverReceiveQuantity;
        RejectQuantity = other.RejectQuantity;

        TrolleyNo = other.TrolleyNo;
        Remark = other.Remark;

        Reserved1 = other.Reserved1;
        Reserved2 = other.Reserved2;

        ModifiedBy = other.ModifiedBy;
        ModifiedDate = other.ModifiedDate;

        ExtraProperties = other.ExtraProperties;
    }
    #endregion
}