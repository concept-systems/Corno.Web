using System;
using System.Collections.Generic;
using Corno.Web.Models.Sales;
using Corno.Web.Services.Interfaces;

namespace Corno.Web.Services.Sales.Interfaces;

public interface IBaseSalesInvoiceService : ITransactionService<SalesInvoice>
{
}
