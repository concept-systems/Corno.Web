using System.Collections.Generic;
using Corno.Web.Areas.Kitchen.Dto.Carton;

namespace Corno.Web.Areas.Kitchen.Dto;

public class DispatchViewDto
{
    public string WarehouseOrderNo { get; set; }
    public string LoadNo { get; set; }
    public string CartonBarcode { get; set; }
    public virtual List<CartonDispatchDto> CartonDispatchDtos { get; set; } = new();
}