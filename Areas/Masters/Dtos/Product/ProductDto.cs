using System.Collections.Generic;
using Corno.Web.Dtos;
using Ganss.Excel;

namespace Corno.Web.Areas.Masters.Dtos.Product;

public class ProductDto : MasterDto
{
    #region -- Constructors --

    public ProductDto()
    {
        ProductItemDtos = new List<ProductItemDto>();
        ProductPacketDtos = new List<ProductPacketDto>();
        ProductStockDtos = new List<ProductStockDto>();
    }
    #endregion

    #region -- Properties --

    [Ignore]
    public byte[] Photo { get; set; }
    public int? ProductTypeId { get; set; }
    public int? ProductCategoryId { get; set; }
    public int? BrandId { get; set; }
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
    public string PhotoPath { get; set; }

    public string SupplierCode { get; set; }
    public string PoNo { get; set; }
    public string RL { get; set; }
    public string CustomerName { get; set; }

    public List<ProductItemDto> ProductItemDtos { get; set; }
    public List<ProductPacketDto> ProductPacketDtos { get; set; }
    public List<ProductStockDto> ProductStockDtos { get; set; }
    #endregion
}
