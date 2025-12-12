using System;
using System.Collections.Generic;
using Corno.Web.Dtos;
using Corno.Web.Reports;

namespace Corno.Web.Areas.Kitchen.Dto.Label;

public class LabelViewDto : BaseDto
{
    #region -- Properties --
    public DateTime? DueDate { get; set; }
    public string LotNo { get; set; }
    public string WarehouseOrderNo { get; set; }
    public string CarcassCode { get; set; }
    public string AssemblyCode { get; set; }
    public string Position { get; set; }
    public string Barcode { get; set; }
    public string Family { get; set; }
    
    public string DrawingNo { get; set; }
    public string Color { get; set; }

    public int? ItemId { get; set; }
    public string Description { get; set; }
    public string ItemName { get; set; }
    public string ItemCode { get; set; }
    public double? Quantity { get; set; }
    public double? OrderQuantity { get; set; }
    public string LabelStatus { get; set; }
    public double? PrintQuantity { get; set; }
    public double? BendQuantity { get; set; }
    public double? SortQuantity { get; set; }
    public double? PackQuantity { get; set; }
    public bool PrintToPrinter { get; set; }
    public BaseReport LabelReport { get; set; }
    public string Base64 { get; set; }

    // Carton Information
    public int? CartonNo { get; set; }
    public string CartonBarcode { get; set; }
    public double? CartonQuantity { get; set; }

    public virtual List<LabelViewDetailDto> LabelViewDetailDto { get; set; } = new();
    #endregion
}