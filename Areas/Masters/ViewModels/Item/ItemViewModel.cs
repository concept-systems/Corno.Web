using System.Collections.Generic;
using System.ComponentModel;
using Corno.Web.Dtos;
using Ganss.Excel;

namespace Corno.Web.Areas.Masters.ViewModels.Item;

public class ItemViewModel : MasterDto
{
    #region -- Constructors --

    public ItemViewModel()
    {
        ItemProcessDetails = new List<ItemProcessViewModel>();
        ItemMachineDetails = new List<ItemMachineViewModel>();
        ItemPacketDetails = new List<ItemPacketViewModel>();
    }
    #endregion

    #region -- Properties --
    [Ignore]
    public byte[] Photo { get; set; }
    [DisplayName("Item Type")]
    public int? ItemTypeId { get; set; }
    public int? ItemCategoryId { get; set; }
    [DisplayName("Unit")]
    public int? UnitId { get; set; }
    public double? Rate { get; set; }
    public double? StockQuantity { get; set; }
    public int PackingTypeId { get; set; }
    public double? BoxQuantity { get; set; }
    public double? ReorderLevel { get; set; }
    public double? Length { get; set; }
    public double? LengthTolerance { get; set; }
    public double? Width { get; set; }
    public double? WidthTolerance { get; set; }
    public double? Thickness { get; set; }
    public double? ThicknessTolerance { get; set; }
    public double? Weight { get; set; }
    public double? WeightTolerance { get; set; }
    public double? Red { get; set; }
    public double? RedTolerance { get; set; }
    public double? Green { get; set; }
    public double? GreenTolerance { get; set; }
    public double? Blue { get; set; }
    public double? BlueTolerance { get; set; }
    public string Color { get; set; }
    public string DrawingNo { get; set; }
    public string RackNo { get; set; }
    public bool? Weighing { get; set; }
    public bool? QcCheck { get; set; }
    public bool? PartialQc { get; set; }
    public int? FlavorId { get; set; }
    public string Reserved1 { get; set; }
    public string Reserved2 { get; set; }
    public string Reserved3 { get; set; }
    public string Reserved4 { get; set; }
    public string ItemType { get; set; }
    public string ItemCategory { get; set; }
    public string Unit { get; set; }
    public string PackingTypeName { get; set; }
    public string Location { get; set; }

    public ICollection<ItemProcessViewModel> ItemProcessDetails { get; set; }
    public ICollection<ItemMachineViewModel> ItemMachineDetails { get; set; }
    public ICollection<ItemPacketViewModel> ItemPacketDetails { get; set; }
    #endregion
}