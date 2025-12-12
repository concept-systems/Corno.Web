using System.Collections.Generic;
using System.Linq;

namespace Corno.Web.Models.Location;

public class Location : MasterModel
{
    #region -- Constructors --
    public Location()
    {
        LocationItemDetails = new List<LocationItemDetail>();
        LocationUserDetails = new List<LocationUserDetail>();
        LocationStockDetails = new List<LocationStockDetail>();
    }
    #endregion

    #region -- Properties --
    // Area / Section / Zone
    public int? AreaId { get; set; }
    // Rack / Row
    public int? RowId { get; set; }
    // Bay / Column
    public int? BayId { get; set; }
    // Level / Shelf
    public int? LevelId { get; set; }
    // Position / Bin
    public int? Position { get; set; }
    public double? Length { get; set; }
    public double? Width { get; set; }
    public double? Height { get; set; }

    public ICollection<LocationItemDetail> LocationItemDetails { get; set; }
    public ICollection<LocationUserDetail> LocationUserDetails { get; set; }
    public ICollection<LocationStockDetail> LocationStockDetails { get; set; }

    #endregion

    #region -- Public Methods --
    public bool IsItemAvailable(int itemId)
    {
        return LocationItemDetails.Any(d => d.ItemId == itemId);
    }

    public bool IsUserAvailable(string userId)
    {
        return LocationUserDetails.Any(d => d.UserId == userId);
    }
    #endregion
}