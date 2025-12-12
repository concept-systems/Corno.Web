using System.ComponentModel;
using Corno.Web.Models.Base;

namespace Corno.Web.Areas.Masters.ViewModels.Supplier;

public class SupplierItemDetailViewModel : BaseModel
{
    [DisplayName("Supplier")]
    public int? SupplierId { get; set; }
    [DisplayName("Item")]
    public int ItemId { get; set; }

    public string ItemName { get; set; }
    //public virtual SupplierViewModel Supplier { get; set; }

}