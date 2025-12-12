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
        SetIncludes($"{nameof(Customer.CustomerProductDetails)}," +
                    $"{nameof(Customer.CustomerUserDetails)}");
    }
    #endregion
}