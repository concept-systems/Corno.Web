using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using Corno.Web.Models.Base;

namespace Corno.Web.Areas.Masters.Dtos.Item;

public class ItemPacketDto : BaseModel
{

    [DisplayName("Item")]
    public int? ItemId { get; set; }
    [DisplayName("Packing Type")]
    public int PackingTypeId { get; set; }
    public double? Quantity { get; set; }
    [NotMapped]
    public string PackingTypeName { get; set; }
}