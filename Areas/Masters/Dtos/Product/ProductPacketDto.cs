using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Corno.Web.Models.Base;
using Ganss.Excel;
using Newtonsoft.Json;

namespace Corno.Web.Areas.Masters.Dtos.Product;

public class ProductPacketDto : BaseModel
{
    public int? ProductId { get; set; }
    public int PackingTypeId { get; set; }
    public string PackingTypeName { get; set; }
    public double? Quantity { get; set; }
    public double? Mrp { get; set; }
    public int? ProductCategoryId { get; set; }
    public int? LayerId { get; set; }
    public int? StationId { get; set; }
    [Ignore]
    public byte[] Photo { get; set; }
    public int? LoadingSequence { get; set; }
    [NotMapped]
    
    [Required]
    [JsonIgnore]
    protected virtual Masters.ViewModels.Product.ProductViewModel Product { get; set; }
}