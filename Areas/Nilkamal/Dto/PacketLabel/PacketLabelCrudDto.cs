namespace Corno.Web.Areas.Nilkamal.Dto.PacketLabel;

public class PacketLabelCrudDto
{
    #region -- Properties --

    public string WarehouseOrderNo { get; set; }
    public string ProductionOrderNo { get; set; }
    public int? ProductId { get; set; }
    public int? PackingTypeId { get; set; }
    public double? Quantity { get; set; }

    public bool PrintToPrinter { get; set; }
    public string Base64 { get; internal set; }

    // Dispaly Properties
    public string ProductCode { get; set; }
    public string ProductName { get; set; }
    public string PackingTypeName { get; set; }
    public double? OrderQuantity { get; set; }
    public double? PrintQuantity { get; set; }
    #endregion

    #region -- Public Methods --

    public void Clear()
    {
        Quantity = default;
    }

    #endregion
}