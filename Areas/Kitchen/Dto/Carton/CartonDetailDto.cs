namespace Corno.Web.Areas.Kitchen.Dto.Carton;

public class CartonDetailDto

{
    #region -- Properties --
    public string WarehousePosition { get; set; }

    public string Position { get; set; }
    public string CarcassCode { get; set; }
    public string AssemblyCode { get; set; }
    public string Description { get; set; }
    public string ItemCode { get; set; }
    public string Barcode { get; set; }
    public double? Quantity { get; set; }
    public double? SystemWeight { get; set; }
    public double? NetWeight { get; set; }
    public double? Tolerance { get; set; }
    public string Status { get; set; }
   
    #endregion
}