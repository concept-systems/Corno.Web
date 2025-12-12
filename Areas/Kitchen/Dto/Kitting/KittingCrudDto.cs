using System;

namespace Corno.Web.Areas.Kitchen.Dto.Kitting;

public class KittingCrudDto
{
    public string Barcode { get; set; }
    public DateTime? DueDate { get; set; }
    public string ItemCode { get; set; }
    public string ItemName { get; set; }
    public string SoNo { get; set; }
    public string WarehouseOrderNo { get; set; }
    public string LotNo { get; set; }
    public string WarehousePosition { get; set; }
    public double? OrderQuantity { get; set; }
    public double? PrintQuantity { get; set; }
    public double? PendingQuantity { get; set; }

    public byte[] ItemPhoto { get; set; }
    public byte[] Item1Photo { get; set; }
}