using System.Collections.Generic;
using System.Threading.Tasks;
using Corno.Web.Areas.Masters.Models;
using System.Web;
using Corno.Web.Services.Location.Interfaces;

namespace Corno.Web.Areas.Masters.Services.Interfaces;

public interface ILocationService : IBaseLocationService
{
    // OLD METHOD - REMOVED: ImportAsync with IBaseProgressService
    // This should be updated to use the new common import module
    // Task<IEnumerable<LocationImportModel>> ImportAsync(HttpPostedFileBase file, IBaseProgressService progressService);
}