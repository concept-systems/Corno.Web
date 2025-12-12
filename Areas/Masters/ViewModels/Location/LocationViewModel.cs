using System.Collections.Generic;
using Corno.Web.Dtos;

namespace Corno.Web.Areas.Masters.ViewModels.Location;

public class LocationViewModel : MasterDto
{
    #region -- Constructors --

    public LocationViewModel()
    {
        LocationItemDetails = new List<LocationItemViewModel>();
        LocationStockDetails = new List<LocationStockViewModel>();
        LocationUserDetails = new List<LocationUserViewModel>();
    }
    #endregion

    #region -- Properties --
    public int? AreaId { get; set; }
    public int? RowId { get; set; }
    public int? BayId { get; set; }
    public int? LevelId { get; set; }
    public int? Position { get; set; }
    public double? Length { get; set; }
    public double? Width { get; set; }
    public double? Height { get; set; }

    public string ItemName { get; set; }
    public string UserName { get; set; }
    public ICollection<LocationItemViewModel> LocationItemDetails { get; set; }
    public ICollection<LocationStockViewModel> LocationStockDetails { get; set; }
    public ICollection<LocationUserViewModel> LocationUserDetails { get; set; }
    /*public bool IsItemAvailable(int itemId);
    public bool IsUserAvailable(string userId);*/
    #endregion

}