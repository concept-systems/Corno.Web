using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Corno.Web.Models.Base;
using Ganss.Excel;
using Mapster;

namespace Corno.Web.Models.Masters;

public class Product : MasterModel
{
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

    public List<ProductItemDetail> ProductItemDetails { get; set; } = [];
    public List<ProductPacketDetail> ProductPacketDetails { get; set; } = [];
    public List<ProductStockDetail> ProductStockDetails { get; set; } = [];

    #endregion

    //#region -- Methods --
    //public override bool UpdateDetails(CornoModel cornoModel)
    //{
    //    if (cornoModel is not Product newProduct) return false;

    //    // Sync ProductItemDetails
    //    SyncDetails(ProductItemDetails, newProduct.ProductItemDetails);

    //    // Sync ProductPacketDetails
    //    SyncDetails(ProductPacketDetails, newProduct.ProductPacketDetails);

    //    // Sync ProductStockDetails
    //    SyncDetails(ProductStockDetails, newProduct.ProductStockDetails);

    //    return true;
    //}

    //private static void SyncDetails<TDetail>(List<TDetail> existingList, List<TDetail> newList) 
    //    where TDetail : BaseModel
    //{
    //    if (existingList == null) return;
    //    newList ??= [];

    //    // Map of new items by Id for quick lookup
    //    var newById = newList.Where(d => d.Id > 0).ToDictionary(d => d.Id);

    //    // 1. Update existing and mark those present
    //    var presentIds = new HashSet<int>();
    //    foreach (var existing in existingList.ToList())
    //    {
    //        if (existing.Id <= 0 || !newById.TryGetValue(existing.Id, out var newItem)) 
    //            continue;
    //        newItem.Adapt(existing); // update
    //        presentIds.Add(existing.Id);
    //    }

    //    // 2. Delete items that are not present in new list
    //    existingList.RemoveAll(e => e.Id > 0 && !presentIds.Contains(e.Id));

    //    // 3. Add items that are new (Id <= 0) or not matched
    //    var additions = newList.Where(n => n.Id <= 0 || !newById.ContainsKey(n.Id)).ToList();
    //    if (additions.Any())
    //        existingList.AddRange(additions);
    //}

    /*public override bool UpdateDetails(CornoModel cornoModel)
    {
        if (cornoModel is not Product product) return false;

        foreach (var productItemDetail in product.ProductItemDetails)
        {
            var existingDetail = ProductItemDetails.FirstOrDefault(d =>
                d.Id == productItemDetail.Id);
            if (null == existingDetail)
            {
                ProductItemDetails.Add(productItemDetail);
                continue;
            }

            existingDetail.Copy(productItemDetail);
        }
        // Remove items from list1 that are not in list2
        ProductItemDetails.RemoveAll(x => product.ProductItemDetails.All(y => y.Id != x.Id));

        foreach (var productPacketDetail in product.ProductPacketDetails)
        {
            var existingDetail = ProductPacketDetails.FirstOrDefault(d =>
                d.Id == productPacketDetail.Id);
            if (null == existingDetail)
            {
                ProductPacketDetails.Add(productPacketDetail);
                continue;
            }

            existingDetail.Copy(productPacketDetail);
        }
        // Remove items from list1 that are not in list2
        ProductPacketDetails.RemoveAll(x => product.ProductPacketDetails.All(y => y.Id != x.Id));
            
        foreach (var productStockDetail in product.ProductStockDetails)
        {
            var existingDetail = ProductStockDetails.FirstOrDefault(d =>
                d.Id == productStockDetail.Id);
            if (null == existingDetail)
            {
                ProductStockDetails.Add(productStockDetail);
                continue;
            }

            existingDetail.Copy(productStockDetail);
        }
        // Remove items from list1 that are not in list2
        ProductStockDetails.RemoveAll(x => product.ProductStockDetails.All(y => y.Id != x.Id));

        return true;
    }*/
    //#endregion
}