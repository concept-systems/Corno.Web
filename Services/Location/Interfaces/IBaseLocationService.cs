using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Corno.Web.Services.Interfaces;

namespace Corno.Web.Services.Location.Interfaces;

public interface IBaseLocationService : IMasterService<Models.Location.Location>
{
    #region -- Methods --

    Task AddStockAsync(Models.Packing.Label label, string locationCode, double quantity, DateTime? grnDate);
    Task RemoveStockAsync(Models.Packing.Label label, string locationCode, double quantity);

    Task ValidateItemAndUserAsync(string locationCode, string itemBarcode,
        string userName, ICollection<string> oldStatus);

    #endregion
}