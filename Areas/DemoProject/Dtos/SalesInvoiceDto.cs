using System;
using System.Collections.Generic;
using Corno.Web.Dtos;

namespace Corno.Web.Areas.DemoProject.Dtos;

public class SalesInvoiceDto : BaseDto
{
    #region -- Properties --
    public DateTime InvoiceDate { get; set; }
    public string CustomerName { get; set; }
    
    public string MobileNo { get; set; }
    public string PaymentMode { get; set; }
    public double? PaidAmount { get; set; }

    public virtual List<SalesInvoiceDetailDto> SalesInvoiceDetailDtos { get; set; } = [];
    #endregion
}