using System;

namespace Corno.Web.Areas.Kitchen.Dto.Label;

public class LabelIndexDto
{
    public int Id { get; set; }
    public DateTime? LabelDate { get; set; }
    public string WarehouseOrderNo { get; set; }
    public string CarcassCode { get; set; }
    public string AssemblyCode { get; set; }
    public string Position { get; set; }
    public string Barcode { get; set; }
    public string LotNo { get; set; }
    public string Family { get; set; }
    public double? OrderQuantity { get; set; }
    public double? Quantity { get; set; }
    public string Status { get; set; }
}