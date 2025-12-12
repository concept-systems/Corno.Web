using System;
using Corno.Web.Dtos;

namespace Corno.Web.Areas.Kitchen.Dto.Plan;

public class CartonViewChartDto : BaseDto
{
    #region -- Properties --
    public DateTime? PackingDate { get; set; }
    public int? Count { get; set; }
    #endregion
}