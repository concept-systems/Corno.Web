using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Corno.Web.Globals;
using Corno.Web.Models.Location;
using Corno.Web.Models.Packing;
using Corno.Web.Services.Label.Interfaces;
using Corno.Web.Services.Location.Interfaces;
using Corno.Web.Services.Packing.Interfaces;
using Corno.Web.Services.Warehouse.Interfaces;

namespace Corno.Web.Services.Warehouse;

public class BaseRackInService : IBaseRackInService
{
    #region -- Constructors --
    public BaseRackInService(IBaseLabelService labelService, IBaseCartonService cartonService,
        IBaseLocationService locationService)
    {
        _labelService = labelService;
        _cartonService = cartonService;
        _locationService = locationService;
    }
    #endregion

    #region -- Data Members --

    private readonly IBaseLabelService _labelService;
    private readonly IBaseCartonService _cartonService;
    private readonly IBaseLocationService _locationService;

    #endregion

    #region -- Public Methods (RM) --

    public async Task<string> RackInRmAsync(IEnumerable<Models.Packing.Label> labels,
        string palletNo, string locationNo, int? quantity = null)
    {
        var locations = new List<Models.Location.Location>();
        foreach (var barcodeLabel in labels)
        {
            // Update barcode log
            barcodeLabel.LabelDetails.Add(new LabelDetail
            {
                ScanDate = DateTime.Now,
                InPalletNo = palletNo,
                Location = locationNo,
                Status = StatusConstants.RackIn
            });
            barcodeLabel.Status = StatusConstants.RackIn;

            var location = await _locationService.GetByCodeAsync(locationNo).ConfigureAwait(false);
            location?.LocationStockDetails.Add(new LocationStockDetail
            {
                ItemId = barcodeLabel.ItemId,
                Barcode = barcodeLabel.Barcode,
                Quantity = quantity ?? barcodeLabel.Quantity
            });
            locations.Add(location);
        }

        await _labelService.UpdateRangeAndSaveAsync(labels).ConfigureAwait(false);
        await _locationService.UpdateRangeAndSaveAsync(locations).ConfigureAwait(false);

        return labels.Sum(b => b.Quantity).ToString();
    }

    #endregion
}