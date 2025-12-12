using System.Collections.Generic;
using System.Threading.Tasks;
using Corno.Web.Areas.Masters.Models;
using System.Web;
using Corno.Web.Services.Location.Interfaces;
using Corno.Web.Services.Progress.Interfaces;

namespace Corno.Web.Areas.Masters.Services.Interfaces;

public interface ILocationService : IBaseLocationService
{
    Task<IEnumerable<LocationImportModel>> ImportAsync(HttpPostedFileBase file, IBaseProgressService progressService);
}