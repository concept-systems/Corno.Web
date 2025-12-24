using System.Linq;
using Corno.Web.Models.Sales;
using Corno.Web.Repository.Interfaces;
using Corno.Web.Services.Sales.Interfaces;

namespace Corno.Web.Services.Sales;

public class SalesOrderService : TransactionService<SalesOrder>, ISalesOrderService
{
    public SalesOrderService(IGenericRepository<SalesOrder> genericRepository) : base(genericRepository)
    {
        // Do not enable includes globally; use explicit methods when details are needed.
        SetIncludes(nameof(SalesOrder.SalesOrderDetails));
    }

    public async System.Threading.Tasks.Task<SalesOrder> GetByIdWithDetailsAsync(int id)
    {
        //SetIncludes(nameof(SalesOrder.SalesOrderDetails));
        var list = await GetAsync<SalesOrder>(s => s.Id == id, s => s, null, ignoreInclude: false)
            .ConfigureAwait(false);
        return list.FirstOrDefault();
    }
}