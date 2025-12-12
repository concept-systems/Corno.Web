using Corno.Web.Models;

namespace Corno.Web.Areas.Masters.ViewModels.Product;

public class ProductCrudDetailViewModel : MasterModel

{
    #region -- Properties --
    // General
    public string Unit { get; set; }
    public string Category { get; set; }
    public string Brand { get; set; }
    public string Type { get; set; }
    public double? StockQuantity { get; set; }
    public string comment { get; set; }
    public string LabelType { get; set; }


    // Cost
    public int? Mrp { get; set; }
    public int? Rate { get; set; }

    //Dimensions
    public string ThicknessUnit { get; set; }
    public string LengthUnit { get; set; }
    public string Color { get; set; }
    public string TreatmentSide { get; set; }

    // Item Deatils
    public int? ItemId { get; set; }
    public double? Quantity { get; set; }

    // Packet Deatils
    public int? PackingTypeId { get; set; }

    // Stock Deatils
    public int? CustomerId { get; set; }
    public double? OpeningStock { get; set; }
    #endregion
}