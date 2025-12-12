using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Corno.Web.Models.Base;
using Mapster;

namespace Corno.Web.Models.Packing;

[Serializable]
public class CartonDetail : BaseModel
{
    #region -- Constructors --

    public CartonDetail()
    {
        NotMapped = new NotMapped();
    }
    #endregion

    #region -- Properties --
    public int? CartonId { get; set; }

    public int? PackingTypeId { get; set; }
    public string BatchNo { get; set; }
    public string SoPosition { get; set; }
    public string WarehousePosition { get; set; }
    public string Position { get; set; }
    public int? ProductId { get; set; }
    public int? ItemId { get; set; }

    public string ItemCode { get; set; }

    public string Barcode { get; set; }
    public double? SystemWeight { get; set; }
    public double? Tolerance { get; set; }
    public double? TareWeight { get; set; }
    public double? GrossWeight { get; set; }
    public double? NetWeight { get; set; }
    public double? Sqm { get; set; }

    public double? Length { get; set; }
    public double? Width { get; set; }
    public double? Thickness { get; set; }
    public double? OrderQuantity { get; set; }
    public double? BomQuantity { get; set; }
    public double? Quantity { get; set; }
    public double? PackingTypeCount { get; set; }
    public double? Variance { get; set; }
    public int? Layer { get; set; }
    public int? PackingSequence { get; set; }

    //public string Reserved1 { get; set; }
    //public string Reserved2 { get; set; }

    [NotMapped]
    public NotMapped NotMapped { get; set; }

    [Required]
    [AdaptIgnore]
    public virtual Carton Carton { get; set; }
    #endregion

    #region -- Methods --
    /*public void Copy(CartonDetail other)
    {
        PackingTypeId = other.PackingTypeId;
        BatchNo = other.BatchNo;
        SoPosition = other.SoPosition;
        WarehousePosition = other.WarehousePosition;
        Position = other.Position;
        ProductId = other.ProductId;
        ItemId = other.ItemId;
        ItemCode = other.ItemCode;

        Barcode = other.Barcode;
        SystemWeight = other.SystemWeight;
        Tolerance = other.Tolerance;
        TareWeight = other.TareWeight;
        GrossWeight = other.GrossWeight;
        NetWeight = other.NetWeight;
        Sqm = other.Sqm;

        Length = other.Length;
        Width = other.Width;
        Thickness = other.Thickness;
        OrderQuantity = other.OrderQuantity;
        BomQuantity = other.BomQuantity;
        Quantity = other.Quantity;
        PackingTypeCount = other.PackingTypeCount;
        Variance = other.Variance;
        Layer = other.Layer;
        PackingSequence = other.PackingSequence;

        Status = other.Status;
        Code = other.Code;
        SerialNo = other.SerialNo;
    }*/

    public bool IsWeightMatching(double netWeight)
    {
        return netWeight >= SystemWeight - Tolerance &&
               netWeight <= SystemWeight + Tolerance;
    }

    public bool IsWeightMatching(double netWeight, double totalNetWeight)
    {
        return netWeight >= SystemWeight + totalNetWeight - Tolerance &&
               netWeight <= SystemWeight + totalNetWeight + Tolerance;
    }

    public void UpdateWeighingDetails(double netWeight, double tareWeight,
        double totalNetWeight, string barcode, string status)
    {
        GrossWeight = netWeight;
        TareWeight = 0;
        NetWeight = netWeight - totalNetWeight;
        TareWeight = tareWeight;
        Variance = SystemWeight - NetWeight;
        Barcode = barcode;
        Status = status;
    }
    #endregion
}