using System;
using System.Collections.Generic;
using Corno.Web.Dtos;
using Telerik.Reporting;

namespace Corno.Web.Areas.Kitchen.Dto.Carcass;

public sealed class CarcassViewDto : BaseDto
{
    public DateTime? DueDate { get; set; }
    public string WarehouseOrderNo { get; set; }
    public string SoNo { get; set; }
    public string LotNo { get; set; }
    public string ItemName { get; set; }
    public string ItemCode { get; set; }
    public double? OrderQuantity { get; set; }
    public double? PackedQuantity { get; set; }
    public double? PrintQuantity { get; set; }
    public bool PrintToPrinter { get; set; }
    public ReportBook ReportBook { get; set; }
    public List<CarcassDetailsDto> CarcassDetailsDtos { get; set; }
    public List<CarcassRackingDetailDto> CarcassRackingDetailDtos { get; set; }

}

