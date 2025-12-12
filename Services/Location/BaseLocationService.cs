using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Corno.Web.Areas.Admin.Services.Interfaces;
using Corno.Web.Models.Location;
using Corno.Web.Repository.Interfaces;
using Corno.Web.Services.Label.Interfaces;
using Corno.Web.Services.Location.Interfaces;

namespace Corno.Web.Services.Location;

public class BaseLocationService : MasterService<Models.Location.Location>, IBaseLocationService
{
    #region -- Constructors --
    public BaseLocationService(IGenericRepository<Models.Location.Location> genericRepository,
        IBaseLabelService barcodeLabelService, IUserService userService) : base(genericRepository)
    {
        _labelService = barcodeLabelService;
        _userService = userService;

        /*IncludeProperties = $"{nameof(Location.LocationItemDetails)}," +
                            $"{nameof(Location.LocationStockDetails)}," +
                            $"{nameof(Location.LocationUserDetails)}";*/
        SetIncludes($"{nameof(Models.Location.Location.LocationItemDetails)}," +
                    $"{nameof(Models.Location.Location.LocationStockDetails)}," +
                    $"{nameof(Models.Location.Location.LocationUserDetails)}");
    }
    #endregion

    #region -- Data Members --
    private readonly IBaseLabelService _labelService;
    private readonly IUserService _userService;
    #endregion

    #region -- Protected Methods --

    protected virtual bool UpdateItem(Models.Location.Location location, int itemId)
    {
        if (location.LocationItemDetails.Any(d => d.ItemId == itemId))
            return true;

        location.LocationItemDetails.Add(new LocationItemDetail
        {
            ItemId = itemId,
        });

        return false;
    }

    protected virtual async Task<bool> UpdateUserAsync(Models.Location.Location location, string userName, string password)
    {
        var user = await _userService.GetOrCreateAsync(userName, userName, userName, userName).ConfigureAwait(false);
        if (location.LocationUserDetails.Any(d => d.UserId == user?.Id))
            return true;

        location.LocationUserDetails.Add(new LocationUserDetail()
        {
            UserId = user.Id
        });

        return false;
    }
    #endregion

    #region -- Public Methods --

    public async Task AddStockAsync(Models.Packing.Label label, string locationCode, double quantity, DateTime? grnDate)
    {
        var location = await GetByCodeAsync(locationCode).ConfigureAwait(false);
        if (null == location)
            throw new Exception($"Location '{locationCode}' not found.");
        var existingQuantity = location.LocationStockDetails.Where(d =>
            d.ItemId == label.ItemId).Sum(d => d.Quantity);
        var locationItemDetail = location.LocationItemDetails.FirstOrDefault(d =>
            d.ItemId == label?.ItemId);
        if((existingQuantity ?? 0) + quantity > (locationItemDetail?.MaxQuantity ?? 0))
            throw new Exception($"Location ({locationCode}) has existing quantity {existingQuantity}. " +
                                $"You cannot rack-in quantity greater than max quantity {locationItemDetail?.MaxQuantity}");

        location.LocationStockDetails.Add(new LocationStockDetail
        {
            GrnNo = label.GrnNo,
            GrnDate = grnDate,
            Position = label.Position,
            ItemId = label.ItemId,
            Barcode = label.Barcode,
            Quantity = quantity
        });

        await UpdateAndSaveAsync(location).ConfigureAwait(false);
    }

    public async Task RemoveStockAsync(Models.Packing.Label label, string locationCode, double quantity)
    {
        var location = await GetByCodeAsync(locationCode).ConfigureAwait(false);
        if (null == location)
            throw new Exception($"Location '{locationCode}' not found.");
        var locationStockDetail = location.LocationStockDetails.
            OrderBy(d => d.GrnDate)
            .FirstOrDefault(d => d.ItemId == label.ItemId);
        if (null == locationStockDetail)
            throw new Exception($"Location '{locationCode}' doesn't contain item.");
        if (quantity > (locationStockDetail.Quantity ?? 0))
            throw new Exception($"Rack-out quantity ({quantity}) is greater than location quantity ({locationStockDetail.Quantity}).");

        locationStockDetail.Quantity = (locationStockDetail.Quantity ?? 0) - quantity;

        if (locationStockDetail.Quantity <= 0)
            location.LocationStockDetails.Remove(locationStockDetail);

        await UpdateAndSaveAsync(location).ConfigureAwait(false);
    }

    public async Task ValidateItemAndUserAsync(string locationCode, string itemBarcode,
        string userName, ICollection<string> oldStatus)
    {
        var location = await GetByCodeAsync(locationCode).ConfigureAwait(false);
        if (null == location)
            throw new Exception($"Invalid location {locationCode}");
        var user = await _userService.GetByUserNameAsync(userName).ConfigureAwait(false);
        if (null == user)
            throw new Exception($"Invalid User {userName}");
        if (!location.IsUserAvailable(user.Id))
            throw new Exception($"User {userName} is not assigned to location " +
                                $"{location.Code}");
        //
        var barcodeLabel = await _labelService.GetByBarcodeAsync(itemBarcode, oldStatus).ConfigureAwait(false);
        if (!location.IsItemAvailable(barcodeLabel.ItemId ?? 0))
            throw new Exception($"Scanned Item is not assigned to location {location.Code}");
    }

    public double GetAvailableQuantity(int id)
    {
        throw new NotImplementedException();
    }

    #endregion
}