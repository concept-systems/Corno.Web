using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Corno.Web.Models.Base;
using Mapster;

namespace Corno.Web.Models.Sales;

public class SalesReturnDetail : BaseModel
{
    public int? SalesReturnId { get; set; }

    [DisplayName(@"Product")]
    public int? ProductId { get; set; }
    public string Description { get; set; }
    public string Hsn { get; set; }
    public double? Quantity { get; set; }
    public double? Rate { get; set; }
    public double? Amount { get; set; }
    public double? DiscountPercent { get; set; }
    public double? DiscountAmount { get; set; }
    public double? Cgst { get; set; }
    public double? Sgst { get; set; }
    public double? Igst { get; set; }
    public double? Total { get; set; }

    public double? PendingQuantity { get; set; }

    public string Remark { get; set; }

    [Required]
    [AdaptIgnore]
    protected virtual SalesReturn SalesReturn { get; set; }
}