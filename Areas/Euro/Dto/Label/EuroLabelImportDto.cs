using System;

namespace Corno.Web.Areas.Euro.Dto.Label;

public class EuroLabelImportDto
{
    public string OcNo { get; set; }
    public string Description { get; set; }
    public DateTime? LabelDate { get; set; }
    public string Barcode { get; set; }
    public string ItemCode { get; set; }
    public double? Quantity { get; set; }
    public string Status { get; set; }
    public string Remark { get; set; }
}
