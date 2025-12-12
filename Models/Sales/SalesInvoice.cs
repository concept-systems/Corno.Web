using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Corno.Web.Models.Base;

namespace Corno.Web.Models.Sales;

public class SalesInvoice : TransactionModel
{
    public SalesInvoice()
    {
        SalesInvoiceDetails = new List<SalesInvoiceDetail>();
    }

    public int? BranchId { get; set; }
    [Required]
    public DateTime? InvoiceDate { get; set; }
    public string InvoiceType { get; set; }
    public int? CustomerId { get; set; }
    public string CustomerName { get; set; }
    public string PoNo { get; set; }
    public DateTime? PoDate { get; set; }
    public string PaymentMode { get; set; }
    public double? PaidAmount { get; set; }
    public int? FinancialYearId { get; set; }

    public List<SalesInvoiceDetail> SalesInvoiceDetails { get; set; }
}