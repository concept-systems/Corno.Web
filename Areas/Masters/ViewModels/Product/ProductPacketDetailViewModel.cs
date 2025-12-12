using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Corno.Web.Models.Base;
using Ganss.Excel;
using Newtonsoft.Json;

namespace Corno.Web.Areas.Masters.ViewModels.Product;

public class ProductPacketDetailViewModel : BaseModel
{
    [DisplayName("Product")]
    public int? ProductId { get; set; }
    [DisplayName("Packing Type")]
    public int PackingTypeId { get; set; }
    public double? Quantity { get; set; }
    public int? ProductCategoryId { get; set; }
    public int? LayerId { get; set; }
    public int? StationId { get; set; }
    [Ignore]
    public byte[] Photo { get; set; }
    public int? LoadingSequence { get; set; }
    [NotMapped]
    public string PackingTypeName { get; set; }
    [Required]
    [JsonIgnore]
    protected virtual ProductViewModel Product { get; set; }
    //public void Copy(ProductPacketDetail other);

}