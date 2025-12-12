using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Corno.Web.Models.Base;

namespace Corno.Web.Models.Sales;

public class SalesOrderDetail : BaseModel
{
    public int? SalesOrderId { get; set; }

    [DisplayName(@"Product")]
    public int? ProductId { get; set; }
    public string Description { get; set; }
    public string Hsn { get; set; }
    public double? Quantity { get; set; }
    public double? Rate { get; set; }
    public double? Amount { get; set; }
    [DisplayName("Discount \nPercent")]
    public double? DiscountPercent { get; set; }
    [DisplayName("Discount \nAmount")]
    public double? DiscountAmount { get; set; }
    public double? Cgst { get; set; }
    public double? Sgst { get; set; }
    public double? Igst { get; set; }
    public double? Total { get; set; }

    public double? PendingQuantity { get; set; }

    [Required]
    protected virtual SalesOrder SalesOrder { get; set; }
}