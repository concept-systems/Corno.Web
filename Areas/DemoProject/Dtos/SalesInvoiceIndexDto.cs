using System;
using Corno.Web.Models.Base;

namespace Corno.Web.Areas.DemoProject.Dtos;

public class SalesInvoiceIndexDto : BaseModel
{
    #region -- Properties --

    public string InvoiceNo { get; set; }
    public DateTime? InvoiceDate { get; set; }
    public string CustomerName { get; set; }
    public string Mobile { get; set; }
    public string PaymentMode { get; set; }
    public double? PaidAmount { get; set; }
    #endregion
}