using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using Corno.Web.Dtos;
using Ganss.Excel;

namespace Corno.Web.Areas.Masters.ViewModels.Product;

public class ProductViewModel : MasterDto
{

    #region -- Constructors --

    public ProductViewModel()
    {
        ProductItemDetails = new List<ProductItemDetailViewModel>();
        ProductPacketDetails = new List<ProductPacketDetailViewModel>();
        ProductStockDetails = new List<ProductStockDetailViewModel>();
    }
    #endregion

    #region -- Properties --

    [Ignore]
    public byte[] Photo { get; set; }
    [DisplayName("Product Type")]
    public int? ProductTypeId { get; set; }
    [DisplayName("Product Category")]
    public int? ProductCategoryId { get; set; }
    [DisplayName("Brand")]
    public int? BrandId { get; set; }
    [DisplayName("Unit")]
    public int? UnitId { get; set; }
    public double? Mrp { get; set; }
    public double? CostPrice { get; set; }
    public double? SalePrice { get; set; }
    public double? Length { get; set; }
    public double? LengthTolerance { get; set; }
    public int? LengthUnitId { get; set; }
    public double? Width { get; set; }
    public double? WidthTolerance { get; set; }
    public int? WidthUnitId { get; set; }
    public double? Thickness { get; set; }
    public double? ThicknessTolerance { get; set; }
    public int? ThicknessUnitId { get; set; }
    public double? Weight { get; set; }
    public double? WeightTolerance { get; set; }
    public double? Diameter { get; set; }
    public string Color { get; set; }
    public string DrawingNo { get; set; }
    public int? BoxesPerCarton { get; set; }
    public int? PiecesPerBox { get; set; }
    public string TreatmentSide { get; set; }
    public bool? PartialQc { get; set; }
    public double? StockQuantity { get; set; }
    public string Comment { get; set; }
    public string LabelFormat { get; set; }
    public string InstallationGuide { get; set; }
    [NotMapped]
    public string PhotoPath { get; set; }
    [NotMapped]
    public string ItemName { get; set; }
    [NotMapped]
    public string PackingTypeName { get; set; }
    [NotMapped]
    public string CustomerName { get; set; }
    public List<ProductItemDetailViewModel> ProductItemDetails { get; set; }
    public List<ProductPacketDetailViewModel> ProductPacketDetails { get; set; }
    public List<ProductStockDetailViewModel> ProductStockDetails { get; set; }
    /*public override bool UpdateDetails(CornoModel cornoModel);
    public ProductItemDetail GetProductItemDetail(int itemId);
    public ProductItemDetail GetProductItemDetail(int packingTypeId, int itemId);
    public IEnumerable<int> GetAssemblySequences(int itemId);
    public IEnumerable<int> GetAssemblySequences(int packingTypeId, int itemId);
    public int GetLayer(int packingTypeId, int itemId);
    public int GetLayer(int packingTypeId, int itemId, int assemblySequence);*/

    #endregion

}