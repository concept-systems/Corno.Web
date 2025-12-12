using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Corno.Web.Areas.Kitchen.Models;

public class StoreKhalapurViewMode
{
    #region -- Constructors --
    public StoreKhalapurViewMode()
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

    [Required]
    [Display(Name = "Lot No")]
    public string LotNo { get; set; }
    public string Group { get; set; }

    [DisplayName("Warehouse Order No")]
    public string WarehouseOrderNo { get; set; }
    public double? OrderQuantity { get; set; }
    public double? PrintQuantity { get; set; }
    public double? PendingQuantity { get; set; }
    public string Family { get; set; }
    public string ItemCode { get; set; }
    public bool PrintToPrinter { get; set; }
    #endregion

    #region -- Methods --

    public void Clear()
    {
        //WarehouseOrderNo = string.Empty;
        Position = string.Empty;
        ItemId = null;
        OrderQuantity = 0;
        PrintQuantity = 0;
        PendingQuantity = 0;
        Family = string.Empty;
        PrintToPrinter = false;
    }
    #endregion
}