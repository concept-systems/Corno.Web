using Corno.Web.Models.Base;

namespace Corno.Web.Areas.Masters.Dtos.Product;

public class ProductItemDto : BaseModel
{
    public int? ProductId { get; set; }
    public int ItemId { get; set; }
    public string ItemName { get; set; }
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
}