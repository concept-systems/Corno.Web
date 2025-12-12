using System;
using System.ComponentModel;

namespace Corno.Web.Areas.Kitchen.Models;

public class StiffenerLabelViewModel
{
    #region -- Constructors --
    //public StiffenerLabelViewMode()
    //{
    //    Clear();
    //}
    #endregion

    #region -- Properties --
    public DateTime? DueDate { get; set; }
    public string ItemCode { get; set; }
    public string Plan { get; set; }
    public string LotNo { get; set; }
    public string DrawingNo { get; set; }
    public string Position { get; set; }

    [DisplayName("Item")]
    public int? ItemId { get; set; }
    public string WarehouseOrderNo { get; set; }
    public double? OrderQuantity { get; set; }
    public double? PrintQuantity { get; set; }
    public double? PendingQuantity { get; set; }
       
    public bool PrintToPrinter { get; set; }
    #endregion

    #region -- Methods --

    public void Clear()
    {
        WarehouseOrderNo = string.Empty;
        Position = string.Empty;
        ItemId = null;
        DrawingNo = string.Empty;
        OrderQuantity = 0;
        PrintQuantity = 0;
        PendingQuantity = 0;

        PrintToPrinter = false;
    }
    #endregion
}