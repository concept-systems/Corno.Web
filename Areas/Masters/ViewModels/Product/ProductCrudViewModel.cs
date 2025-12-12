using System;
using System.Collections.Generic;
using Corno.Web.Models;

namespace Corno.Web.Areas.Masters.ViewModels.Product;

public class ProductCrudViewModel : MasterModel

{

    #region -- Constructors --

    public ProductCrudViewModel()
    {
        //Clear();
    }

    #endregion

    #region -- Properties --
    public DateTime? LabelDate { get; set; }
    public int? ProductId { get; set; }
    public int? CustomerId { get; set; }
    public string BatchNo { get; set; }
    public int? LabelCount { get; set; }
    
    public string ItemCode { get; set; }
    public string ProductCode { get; set; }
    public string ProductName { get; set; }
    public string GaId { get; set; }
    public double? Quantity { get; set; }
    public double? OrderQuantity { get; set; }
    public double? PrintQuantity { get; set; }
    public double? PendingQuantity { get; set; }

    public bool PrintToPrinter { get; set; }

    public string SubmitType { get; set; }

    public virtual List<ProductCrudDetailViewModel> Details { get; set; } = new();
    #endregion

    #region -- Methods --

    /*public void Clear()
    {
        ProductName = default;
        OrderQuantity = default;
        PrintQuantity = default;
        PendingQuantity = default;
        PrintToPrinter = default;
        SubmitType = default;
    }*/

    #endregion

}