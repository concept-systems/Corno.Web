using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Corno.Web.Models.Sales;
using Corno.Web.Repository.Interfaces;
using Corno.Web.Services.Sales.Interfaces;

namespace Corno.Web.Services.Sales;

public class BaseSalesInvoiceService : TransactionService<SalesInvoice>, IBaseSalesInvoiceService
{
    #region -- Constructors --
    public BaseSalesInvoiceService(IGenericRepository<SalesInvoice> genericRepository) : base(genericRepository)
    {
        SetIncludes(nameof(SalesInvoice.SalesInvoiceDetails));
    }
    #endregion
}