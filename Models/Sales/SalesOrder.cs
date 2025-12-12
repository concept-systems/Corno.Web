using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;
using Corno.Web.Models.Base;

namespace Corno.Web.Models.Sales;

public class SalesOrder : TransactionModel
{
    public SalesOrder()
    {
        SalesOrderDetails = new List<SalesOrderDetail>();
    }

    [Required]
    public DateTime? OrderDate { get; set; }
    public int? CustomerId { get; set; }
    public string PoNo { get; set; }
    public DateTime? PoDate { get; set; }
    public string PoFilePath { get; set; }
    public int? FinancialYearId { get; set; }

    [NotMapped] 
    public HttpPostedFileBase PoUploadFile { get; set; }

    public ICollection<SalesOrderDetail> SalesOrderDetails { get; set; }
}