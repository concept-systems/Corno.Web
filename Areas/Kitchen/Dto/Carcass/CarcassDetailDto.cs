using System;

namespace Corno.Web.Areas.Kitchen.Dto.Carcass;

public class CarcassDetailsDto 
{
    public int Id { get; set; }
    public string Position { get; set; }
    public string WarehousePosition { get; set; }
    public string CarcassCode { get; set; }
    public string ItemCode { get; set; }
    public string Description { get; set; }
    public string AssemblyCode { get; set; }
    public double? Quantity { get; set; }
    public double OrderQuantity { get; set; }
    public string Barcode { get; set; }
}

public class CarcassRackingDetailDto
{
    public DateTime? ScanDate { get; set; }
    public string PalletNo { get; set; }
    public string RackNo { get; set; }
    public string Status { get; set; }
}