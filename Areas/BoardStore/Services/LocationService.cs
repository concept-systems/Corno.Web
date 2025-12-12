using Corno.Web.Areas.BoardStore.Models;
using Corno.Web.Areas.BoardStore.Services.Interfaces;
using Corno.Web.Repository.Interfaces;
using Corno.Web.Services;

namespace Corno.Web.Areas.BoardStore.Services;

public class LocationService : MasterService<Location>, ILocationService
{
    #region -- Constructors --
    public LocationService(IGenericRepository<Location> genericRepository) : base(genericRepository)
    {
        SetIncludes($"{nameof(Location.LocationStacks)}");
    }
    #endregion
}