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
        SetIncludes(nameof(Supplier.SupplierItemDetails));
    }
    #endregion
}