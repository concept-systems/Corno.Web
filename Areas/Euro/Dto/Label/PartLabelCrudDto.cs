namespace Corno.Web.Areas.Euro.Dto.Label;

public class PartLabelCrudDto
{
    #region -- Properties --

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