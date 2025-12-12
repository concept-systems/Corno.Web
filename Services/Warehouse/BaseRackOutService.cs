using Corno.Web.Services.Packing.Interfaces;
using Corno.Web.Services.Warehouse.Interfaces;

namespace Corno.Web.Services.Warehouse;

public class BaseRackOutService : IBaseRackOutService
{
    #region -- Constructors --
    public BaseRackOutService(IBaseCartonService cartonService)
    {
        _cartonService = cartonService;
    }
    #endregion

    #region -- Data Members --

    private readonly IBaseCartonService _cartonService;

    #endregion
}