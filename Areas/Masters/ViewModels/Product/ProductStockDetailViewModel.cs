using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Corno.Web.Dtos;
using Corno.Web.Models.Base;
using Newtonsoft.Json;

namespace Corno.Web.Areas.Masters.ViewModels.Product;

public class ProductStockDetailViewModel : BaseModel
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
    [JsonIgnore]
    protected virtual ProductViewModel Product { get; set; }
    //public void Copy(ProductStockDetail other);
    
}