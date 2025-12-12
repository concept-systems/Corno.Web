using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Corno.Web.Models.Base;
using Mapster;

namespace Corno.Web.Models.Masters;

public class ItemPacketDetail : BaseModel
{
    [DisplayName("Item")]
    public int? ItemId { get; set; }
    [DisplayName("Packing Type")]
    public int? PackingTypeId { get; set; }
    public double? Quantity { get; set; }

    [Required]
    [AdaptIgnore]
    public virtual Item Item { get; set; }

    /*#region -- Public Methods --
    public void Copy(ItemPacketDetail other)
    {
        PackingTypeId = other.PackingTypeId;
        Quantity = other.Quantity;
    }
    #endregion*/
}