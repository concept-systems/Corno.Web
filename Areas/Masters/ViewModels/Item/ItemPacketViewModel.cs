using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using Corno.Web.Models.Base;

namespace Corno.Web.Areas.Masters.ViewModels.Item;

public class ItemPacketViewModel : BaseModel
{

    [DisplayName("Item")]
    public int? ItemId { get; set; }
    [DisplayName("Packing Type")]
    public int PackingTypeId { get; set; }
    public double? Quantity { get; set; }
    [NotMapped]
    public string PackingTypeName { get; set; }
}