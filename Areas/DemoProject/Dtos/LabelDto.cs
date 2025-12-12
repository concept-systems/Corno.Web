using System;
using System.Collections.Generic;
using Corno.Web.Models.Base;

namespace Corno.Web.Areas.DemoProject.Dtos;

public class LabelDto : BaseModel
{
    #region -- Constructors --
    public LabelDto()
    {
        Clear();
    }
    #endregion

    #region -- Properties --
    public int? ItemId { get; set; }
    public string ItemName { get; set; }
    public int? ProductId { get; set; }
    public int? PackingTypeId { get; set; }
    public string ProductName { get; set; }
    public double? Rate { get; set; }
    public double? Weight { get; set; }
    public double? Quantity { get; set; }
    public DateTime? LabelDate { get; set; }
    public DateTime? ManufacturingDate { get; set; }
    public DateTime? ExpiryDate { get; set; }

    public bool PrintToPrinter { get; set; }

    public string SubmitType { get; set; }
    public int? LabelFormatId { get; set; }

    public virtual List<LabelDetailDto> Details { get; set; } = new();
    #endregion

    #region -- Methods --
    public void Clear()
    {
        PrintToPrinter = default;
        SubmitType = default;
    }
    #endregion
}