using System;

namespace Corno.Web.Areas.Kitchen.Dto.Label;

public class SalvaginiXmlDto
{
    public DateTime? LabelDate { get; set; }
    public string CompanyCode { get; set; }
    public string SoNo { get; set; }
    public string WarehouseOrderNo { get; set; }
    public string Position { get; set; }
    public string Barcode { get; set; }
}