using System;

namespace Corno.Web.Areas.Kitchen.Models;

public class StoreShirwalViewMode
{
    #region -- Constructors --
    //public StoreShirwalViewMode()
    //{
    //    Clear();
    //}
    #endregion

    #region -- Properties --
    public DateTime? DueDate { get; set; }

    public string LotNo { get; set; }
    public string Group { get; set; }
    public int? ItemId { get; set; }
    public string WarehouseOrderNo { get; set; }
    public double? Quantity { get; set; }
    public double? OrderQuantity { get; set; }
    public double? PrintQuantity { get; set; }
    public double? PendingQuantity { get; set; }
    public double? SetQuantity { get; set; }
    public string Family { get; set; }
    public string ItemCode { get; set; }

    public bool PrintToPrinter { get; set; }

    #endregion

    #region -- Methods --
    public void Clear()
    {
        //WarehouseOrderNo = string.Empty;
        SetQuantity = 0;
        Family = string.Empty;
        PrintToPrinter = false;
    }
    #endregion
}