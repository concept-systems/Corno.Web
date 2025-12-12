using Corno.Web.Models.Sales;
using Corno.Web.Repository.Interfaces;
using Corno.Web.Services.Sales.Interfaces;

namespace Corno.Web.Services.Sales;

public class SalesReturnService : TransactionService<SalesReturn>, ISalesReturnService
{
    public SalesReturnService(IGenericRepository<SalesReturn> genericRepository) : base(genericRepository)
    {
        SetIncludes(nameof(SalesReturn.SalesReturnDetails));
    }
}