using System;

namespace Corno.Web.Areas.Nilkamal.Dto.PartLabel;

public class SalvaginiXmlDto
{
    public DateTime? LabelDate { get; set; }
    public string CompanyCode { get; set; }
    public string SoNo { get; set; }
    public string WarehouseOrderNo { get; set; }
    public string Position { get; set; }
    public string Barcode { get; set; }
}