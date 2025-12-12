using System;
using System.Collections.Generic;
using Corno.Web.Models.Base;

namespace Corno.Web.Areas.DemoProject.Dtos;

public class SalesInvoiceDto : BaseModel
{
    #region -- Properties --
    public string InvoiceNo { get; set; }
    public DateTime InvoiceDate { get; set; }
    public string CustomerName { get; set; }
    
    public string MobileNo { get; set; }
    public string PaymentMode { get; set; }
    public double? PaidAmount { get; set; }

    public bool PrintToPrinter { get; set; } = default;

    public virtual List<SalesInvoiceDetailDto> Details { get; set; } = new();
    #endregion
}