using Corno.Web.Models.Masters;
using Corno.Web.Repository.Interfaces;
using Corno.Web.Services.Masters.Interfaces;

namespace Corno.Web.Services.Masters;

public class EmployeeService : MasterService<Employee>, IEmployeeService
{
    public EmployeeService(IGenericRepository<Employee> genericRepository) : base(genericRepository)
    {
    }
}