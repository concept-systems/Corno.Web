using System;

namespace Corno.Web.Areas.Nilkamal.Dto.PacketLabel;

public class PacketIndexDto
{
    public int Id { get; set; }
    public DateTime? PackingDate { get; set; }
    public string ProductionOrderNo { get; set; }
    public int? ProductId { get; set; }
    public string ProductCode { get; set; }
    public string ProductName { get; set; }
    public double? OrderQuantity { get; set; }
    public double? Quantity { get; set; }
    public string CartonBarcode { get; set; }
    public string Status { get; set; }
}