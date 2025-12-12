using System;
using System.Collections.Generic;
using System.ComponentModel;
using Corno.Web.Models.Base;

namespace Corno.Web.Areas.DemoProject.ViewModels;

public class LabelViewModel : BaseModel

{

    #region -- Constructors --

    public LabelViewModel()
    {
        Clear();
    }

    #endregion

    #region -- Properties --
    public DateTime? LabelDate { get; set; }
    [DisplayName("Item")]
    public int? ItemId { get; set; }
    public string ItemName { get; set; }
    public int PackingTypeId { get; set; }
    
    public double? Quantity { get; set; }

    public bool PrintToPrinter { get; set; }

    public string SubmitType { get; set; }

    public virtual List<LabelDisplayDetailViewModel> Details { get; set; } = new();
    #endregion

    #region -- Methods --

    public void Clear()
    {
        PrintToPrinter = default;
        SubmitType = default;
    }

    #endregion

}