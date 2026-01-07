namespace Corno.Web.Areas.Nilkamal.Dto.PartLabel;

public class PartLabelCrudDto
{
    #region -- Properties --

    public string WarehouseOrderNo { get; set; }
    public string ProductionOrderNo { get; set; }
    public string Position { get; set; }
    public double? Quantity { get; set; }

    public bool PrintToPrinter { get; set; }
    public string Base64 { get; internal set; }
    #endregion

    #region -- Public Methods --

    public void Clear()
    {
        //WarehouseOrderNo = default;
        Position = default;
        Quantity = default;
    }

    #endregion
}