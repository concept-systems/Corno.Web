using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Corno.Web.Dtos;
using Corno.Web.Models.Base;
using Mapster;

namespace Corno.Web.Models.Sales;

public class SalesInvoiceDetail : BaseModel
{
    public int? BranchId { get; set; }

    public int? SalesInvoiceId { get; set; }
    public string LotNo { get; set; }

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
    public string Barcode { get; set; }

    public int? PackingTypeId { get; set; }
    public double? NetWeight { get; set; }

    public double? PendingQuantity { get; set; }

    [NotMapped]
    public MasterDto HsnViewModel { get; set; }

    [Required]
    [AdaptIgnore]
    protected virtual SalesInvoice SalesInvoice { get; set; }
}