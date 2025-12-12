using System;

namespace Corno.Web.Areas.Kitchen.Dto.Put_To_Light;

public class PutToLightkittingViewDto
{
    public string Barcode { get; set; }
    public string Family { get; set; }
    public DateTime? DueDate { get; set; }
    public string ItemCode { get; set; }
    public string ItemName { get; set; }
    public string SoNo { get; set; }
    public string WarehouseOrderNo { get; set; }
    public string LotNo { get; set; }
    public string ColorName { get; set; }
    public string LocationNames { get; set; }
    public string WarehousePosition { get; set; }
    public string Status { get; set; }

    public bool LedOn { get; set; }
}