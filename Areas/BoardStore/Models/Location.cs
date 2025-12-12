using System.Collections.Generic;
using Corno.Web.Models;

namespace Corno.Web.Areas.BoardStore.Models;

public class Location : MasterModel
{
    #region -- Constructors --

    public Location()
    {
        LocationStacks = new List<LocationStack>();
    }
    #endregion

    #region -- Propertis --
    public int? BayId { get; set; }
    public int? LocationTypeId { get; set; }
    public int? RequestCode { get; set; }
    public int? PositionX { get; set; }
    public int? PositionY { get; set; }
    public double? Length { get; set; }
    public double? Width { get; set; }
    public int? RequestPriority { get; set; }
    public int? MovePriority { get; set; }
    public int? StockQuantity { get; set; }

    public double? MaxHeight { get; set; }
    public double? CurrentHeight { get; set; }
    
    public List<LocationStack> LocationStacks { get; set; }
    #endregion
}