using Corno.Web.Models.Sales;
using Corno.Web.Repository.Interfaces;
using Corno.Web.Services.Sales.Interfaces;

namespace Corno.Web.Services.Sales;

public class SalesOrderService : TransactionService<SalesOrder>, ISalesOrderService
{
    public SalesOrderService(IGenericRepository<SalesOrder> genericRepository) : base(genericRepository)
    {
        SetIncludes(nameof(SalesOrder.SalesOrderDetails));
    }
}