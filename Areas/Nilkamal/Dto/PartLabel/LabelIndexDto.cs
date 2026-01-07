using System;

namespace Corno.Web.Areas.Nilkamal.Dto.PartLabel;

public class LabelIndexDto
{
    public int Id { get; set; }
    public DateTime? LabelDate { get; set; }
    public string ProductionOrderNo { get; set; }
    public int? ItemId { get; set; }
    public string ItemCode { get; set; }
    public string ItemName { get; set; }
    public double? OrderQuantity { get; set; }
    public double? Quantity { get; set; }
    public string Barcode { get; set; }
    public string Status { get; set; }
}