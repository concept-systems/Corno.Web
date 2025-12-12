using System;
using System.ComponentModel.DataAnnotations.Schema;
using Corno.Web.Models.Base;

namespace Corno.Web.Areas.Euro.Models;

[Serializable]
[Table("EuroLabels")]
public class EuroLabel : TransactionModel
{
    public DateTime? LabelDate { get; set; }
    public string OcNo { get; set; }
    public string Description { get; set; }
    public string Barcode { get; set; }
    public string ItemCode { get; set; }
    public double? Quantity { get; set; }
    public string LotNo { get; set; }
    public string Reserved1 { get; set; }
    public string Reserved2 { get; set; }
}
