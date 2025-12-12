using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Corno.Web.Areas.Admin.Dto;
using Corno.Web.Areas.Admin.Models;
using Corno.Web.Services.Interfaces;

namespace Corno.Web.Areas.Admin.Services.Interfaces;

public interface IRoleService : ICornoService<AspNetRole>
{
    #region -- Methods --

    Task<IEnumerable<AspNetRole>> GetListAsync(Expression<Func<AspNetRole, bool>> filter = null,
        Func<IQueryable<AspNetRole>, IOrderedQueryable<AspNetRole>> orderBy = null,
        string includeProperties = "");

    Task<AspNetRole> CreateAsync(RoleDto viewModel);
    Task<AspNetRole> EditAsync(RoleDto viewModel);

    #endregion
}