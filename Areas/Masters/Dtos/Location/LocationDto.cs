using System.Collections.Generic;
using Corno.Web.Dtos;

namespace Corno.Web.Areas.Masters.Dtos.Location;

public class LocationDto : MasterDto
{
    #region -- Constructors --

    public LocationDto()
    {
        LocationItemDetails = new List<LocationItemDto>();
        LocationStockDetails = new List<LocationStockDto>();
        LocationUserDetails = new List<LocationUserDto>();
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
    public ICollection<LocationItemDto> LocationItemDetails { get; set; }
    public ICollection<LocationStockDto> LocationStockDetails { get; set; }
    public ICollection<LocationUserDto> LocationUserDetails { get; set; }
    #endregion

}