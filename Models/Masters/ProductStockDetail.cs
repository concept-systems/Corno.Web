using Corno.Web.Dtos;
using Corno.Web.Models.Base;
using Mapster;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Corno.Web.Models.Masters;

public class ProductStockDetail : BaseModel
{
    public int? ProductId { get; set; }
    public int CustomerId { get; set; }
    public int BranchId { get; set; }
    public double? CostPrice { get; set; }
    public double? SalesPrice { get; set; }
    public double? OpeningStock { get; set; }
    public double? CurrentStock { get; set; }

    [NotMapped]
    public virtual MasterDto Customer { get; set; }

    [NotMapped]
    public string CustomerName { get; set; }
    [NotMapped]
    public string BranchName { get; set; }


    [Required]
    [AdaptIgnore]
    [JsonIgnore]
    protected virtual Product Product { get; set; }

    /*#region -- Public Methods --
    public override void Copy(BaseModel other)
    {
        var detail = other as ProductStockDetail ?? throw new ArgumentNullException(nameof(other));

        ProductId = detail.ProductId;
        CustomerId = detail.CustomerId;
        BranchId = detail.BranchId;
        CostPrice = detail.CostPrice;
        SalesPrice = detail.SalesPrice;
        OpeningStock = detail.OpeningStock;
        CurrentStock = detail.CurrentStock;

        ExtraProperties = detail.ExtraProperties;
    }
    #endregion*/
}