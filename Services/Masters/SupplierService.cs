using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Corno.Web.Models.Masters;
using Corno.Web.Repository.Interfaces;
using Corno.Web.Services.Masters.Interfaces;

namespace Corno.Web.Services.Masters;

public class SupplierService : MasterService<Supplier>, ISupplierService
{
    #region -- Constructors --
    public SupplierService(IGenericRepository<Supplier> genericRepository) : base(genericRepository)
    {
        // Do not enable includes globally; use explicit methods when details are needed.
        SetIncludes(nameof(Supplier.SupplierItemDetails));
    }

    public async Task<Supplier> GetByIdWithDetailsAsync(int id)
    {
        //SetIncludes(nameof(Supplier.SupplierItemDetails));
        var list = await GetAsync<Supplier>(s => s.Id == id, s => s, null, ignoreInclude: false)
            .ConfigureAwait(false);
        return list.FirstOrDefault();
    }
    #endregion
}