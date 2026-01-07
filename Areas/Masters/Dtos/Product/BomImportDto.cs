using Ganss.Excel;

namespace Corno.Web.Areas.Masters.Dtos.Product;

public class BomImportDto
{
    [Column("Product Code")]
    public string ProductCode { get; set; }

    [Column("Product Description")]
    public string ProductName { get; set; }

    [Column("Item Code")]
    public string ItemCode { get; set; }

    [Column("Item Description")]
    public string ItemName { get; set; }
    [Column("Quantity")]
    public double? Quantity { get; set; }

    [Column("Packet")]
    public string PackingTypeCode { get; set; }

    public string Status { get; set; }
    public string Remark { get; set; }
}