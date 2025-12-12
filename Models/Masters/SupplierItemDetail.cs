using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Corno.Web.Models.Base;
using Mapster;

namespace Corno.Web.Models.Masters;

public class SupplierItemDetail : BaseModel
{
    [DisplayName("Supplier")]
    public int? SupplierId { get; set; }
    [DisplayName("Item")]
    public int? ItemId { get; set; }

    [Required]
    [AdaptIgnore]
    public virtual Supplier Supplier { get; set; }

    /*#region -- Public Methods --
    public void Copy(SupplierItemDetail other)
    {
       ItemId = other.ItemId;
    }
    #endregion*/
}