using System.Linq;
using Corno.Web.Models.Sales;
using Corno.Web.Repository.Interfaces;
using Corno.Web.Services.Sales.Interfaces;

namespace Corno.Web.Services.Sales;

public class SalesReturnService : TransactionService<SalesReturn>, ISalesReturnService
{
    public SalesReturnService(IGenericRepository<SalesReturn> genericRepository) : base(genericRepository)
    {
        // Do not enable includes globally; use explicit methods when details are needed.
        SetIncludes(nameof(SalesReturn.SalesReturnDetails));
    }

    public async System.Threading.Tasks.Task<SalesReturn> GetByIdWithDetailsAsync(int id)
    {
        //SetIncludes(nameof(SalesReturn.SalesReturnDetails));
        var list = await GetAsync<SalesReturn>(s => s.Id == id, s => s, null, ignoreInclude: false)
            .ConfigureAwait(false);
        return list.FirstOrDefault();
    }
}