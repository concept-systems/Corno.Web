using System.Collections.Generic;
using System.Threading.Tasks;
using Corno.Web.Areas.Admin.Models;
using Corno.Web.Services.Interfaces;

namespace Corno.Web.Areas.Admin.Services.Interfaces;

public interface IUserRoleService : ICornoService<AspNetUserRole>
{
    Task AddRolesAsync(string userId, List<string> roleNames);
}