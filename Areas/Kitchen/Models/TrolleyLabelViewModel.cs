using System.ComponentModel;
using System;

namespace Corno.Web.Areas.Kitchen.Models;

public class TrolleyLabelViewModel
{
    #region -- Constructors --
    public TrolleyLabelViewModel()
    {
        Clear();
    }
    #endregion
    #region -- Properties --
    public DateTime? DueDate { get; set; }

    public string CarcassCode { get; set; }

    public string Position { get; set; }

    [DisplayName("Item")]
    public int? ItemId { get; set; }
    public string LotNo { get; set; }
    public string Group { get; set; }
    public string WarehouseOrderNo { get; set; }
    public double? OrderQuantity { get; set; }
    public double? PrintQuantity { get; set; }
    public double? PendingQuantity { get; set; }
    public string Family { get; set; }
    public string ItemCode { get; set; }
    public string ProductGroup { get; set; }
    public bool PrintToPrinter { get; set; }
    #endregion

    #region -- Methods --

    public void Clear()
    {
        WarehouseOrderNo = string.Empty;
        PrintToPrinter = false;
    }
    #endregion
}