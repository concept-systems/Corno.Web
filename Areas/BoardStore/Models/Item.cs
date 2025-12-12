using System.ComponentModel;
using Ganss.Excel;
using System.Collections.Generic;
using Corno.Web.Models;

namespace Corno.Web.Areas.BoardStore.Models;

public class Item : MasterModel
{
    #region -- Constructors --
    public Item()
    {
        StackItems = new List<StackItem>();
    }
    #endregion

    #region -- Propertis --
    [Ignore]
    public byte[] Photo { get; set; }

    [DisplayName("Item Type")]
    public int? ItemTypeId { get; set; }

    public int? ItemCategoryId { get; set; }

    [DisplayName("Unit")]
    public int? UnitId { get; set; }

    public double? Rate { get; set; }

    public double? StockQuantity { get; set; }

    public double? Length { get; set; }

    public double? LengthTolerance { get; set; }

    public double? Width { get; set; }

    public double? WidthTolerance { get; set; }

    public double? Thickness { get; set; }

    public double? ThicknessTolerance { get; set; }

    public double? Weight { get; set; }

    public double? WeightTolerance { get; set; }

    public string Color { get; set; }

    public string DrawingNo { get; set; }

    public int? Priority { get; set; }

    public string InfeedFlatLamn { get; set; }

    public string FinishType { get; set; }

    public string Type { get; set; }

    public string SubType { get; set; }
    
    public List<StackItem> StackItems { get; set; }
    #endregion
}