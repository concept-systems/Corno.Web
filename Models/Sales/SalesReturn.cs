using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Corno.Web.Models.Base;

namespace Corno.Web.Models.Sales;

public class SalesReturn : TransactionModel
{
    public SalesReturn()
    {
        SalesReturnDetails = new List<SalesReturnDetail>();
    }

    [Required]
    public DateTime? ReturnDate { get; set; }
    public string ReturnType { get; set; }
    public int? CustomerId { get; set; }
    public string PoNo { get; set; }
    public DateTime? PoDate { get; set; }
    public int? FinancialYearId { get; set; }

    public ICollection<SalesReturnDetail> SalesReturnDetails { get; set; }
}