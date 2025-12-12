using System;

namespace Corno.Web.Areas.Kitchen.Models;

public class OperationViewModel
{
    public DateTime? PackingDate { get; set; }
    public int SerialNo { get; set; }
    public string Barcode { get; set; }
    public string WarehousePosition { get; set; }
    public string Position { get; set; }
    public string WarehouseOrderNo { get; set; }
    public string Description{ get; set; }
    public string Status { get; set; }
    public string ItemName { get; set; }
    public string ItemCode { get; set; }
    public double SystemWeight { get; set; }
    public double NetWeight { get; set; }
    public double? Quantity { get; set; }
       


}