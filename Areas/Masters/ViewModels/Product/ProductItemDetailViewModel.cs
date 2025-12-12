using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Corno.Web.Dtos;
using Corno.Web.Models.Base;
using Newtonsoft.Json;

namespace Corno.Web.Areas.Masters.ViewModels.Product;

public class ProductItemDetailViewModel : BaseModel
{
    [DisplayName("Product")]
    public int? ProductId { get; set; }
    public int ItemId { get; set; }
    public double? StandardWeight { get; set; }
    public double? Quantity { get; set; }
    public double? SystemWeight { get; set; }
    public double? Tolerance { get; set; }
    public string QcCode { get; set; }
    public int? PackingTypeId { get; set; }
    public int? PackingTypeId1 { get; set; }
    public int? LayerId { get; set; }
    public int? Layer { get; set; }
    public int? PackingSequence { get; set; }
    public int? AssemblySequence { get; set; }
    public int? AssemblyQuantity { get; set; }
    public string InstallationGuide { get; set; }
    [NotMapped]
    public MasterDto PackingType { get; set; }
    [NotMapped]
    public MasterDto Item { get; set; }
    [NotMapped]
    public string ItemName { get; set; }
    [Required]
    [JsonIgnore]
    protected virtual ProductViewModel Product { get; set; }
    //public void Copy(ProductItemDetail other);

}