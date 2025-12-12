using System;

namespace Corno.Web.Areas.Euro.Dto.Label;

public class LabelIndexDto
{
    public int Id { get; set; }
    public string ProductionOrderNo { get; set; }
    public string Barcode { get; set; }
    public string ArticleNo { get; set; }
    public string CarcassCode { get; set; }
    public double Length { get; set; }
    public double Width { get; set; }
    public double Thickness { get; set; }
    public DateTime? LabelDate { get; set; }
    public string Description { get; set; }
    public string Status { get; set; }
}
