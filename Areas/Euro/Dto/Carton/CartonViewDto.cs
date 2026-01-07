using System;
using System.Collections.Generic;
using Corno.Web.Dtos;

namespace Corno.Web.Areas.Euro.Dto.Carton;

public sealed class CartonViewDto : BaseDto
{
    public DateTime? DueDate { get; set; }
    public string ProductionOrderNo { get; set; }
    public string SoNo { get; set; }
    public string LotNo { get; set; }
    public string ItemName { get; set; }
    public string ItemCode { get; set; }
    public double? OrderQuantity { get; set; }
    public double? PackedQuantity { get; set; }
    public double? PrintQuantity { get; set; }

    /// <summary>
    /// Base64 encoded PDF of the carton label, used for inline preview/print.
    /// </summary>
    public string Base64 { get; set; }

    public List<CartonDetailsDto> CartonDetailsDtos { get; set; }
    public List<CartonRackingDetailDto> CartonRackingDetailDtos { get; set; }

}

