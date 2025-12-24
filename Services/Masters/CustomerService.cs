using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Corno.Web.Models.Masters;
using Corno.Web.Repository.Interfaces;
using Corno.Web.Services.Masters.Interfaces;

namespace Corno.Web.Services.Masters;

public class CustomerService : MasterService<Customer>, ICustomerService
{
    #region -- Constructors --
    public CustomerService(IGenericRepository<Customer> genericRepository) : base(genericRepository)
    {
        // Do not enable includes globally; use explicit methods when details are needed.
        SetIncludes($"{nameof(Customer.CustomerProductDetails)}," +
                    $"{nameof(Customer.CustomerUserDetails)}");
    }

    public async Task<Customer> GetByIdWithDetailsAsync(int id)
    {
        /*SetIncludes($"{nameof(Customer.CustomerProductDetails)}," +
                    $"{nameof(Customer.CustomerUserDetails)}");*/
        var list = await GetAsync<Customer>(c => c.Id == id, c => c, null, ignoreInclude: false)
            .ConfigureAwait(false);
        return list.FirstOrDefault();
    }
    #endregion
}