using Corno.Web.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Corno.Web.Services.Warehouse.Interfaces;

public interface IBaseRackInService : IService
{
    #region -- Methods (RM) --

    Task<string> RackInRmAsync(IEnumerable<Models.Packing.Label> barcodeLabels,
        string palletNo, string locationNo, int? quantity = null);


    #endregion
}