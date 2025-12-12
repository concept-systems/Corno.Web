using System;

namespace Corno.Web.Areas.Kitchen.Dto.Sorting;

public class SortingCrudDto
{
    public string Barcode { get; set; }
    public DateTime? DueDate { get; set; }
    //public string Barcode1 { get; set; }
    public string ItemCode { get; set; }
    public string ItemName { get; set; }
    public string WarehouseOrderNo { get; set; }
    public string WarehousePosition { get; set; }
    public string SoNo { get; set; }
    public string TrolleyNo { get; set; }
    public double? OrderQuantity { get; set; }
    public double? PrintQuantity { get; set; }
    public double? PendingQuantity { get; set; }

    public byte[] ItemPhoto { get; set; }
    public byte[] Item1Photo { get; set; }
}