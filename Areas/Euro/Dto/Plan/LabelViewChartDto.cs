using System;
using Corno.Web.Dtos;

namespace Corno.Web.Areas.Euro.Dto.Plan;

public class LabelViewChartDto : BaseDto
{
    #region -- Properties --
    public DateTime? LabelDate { get; set; }
    public int Count { get; set; }
    public DateTime? PlanDate { get; set; }
    #endregion
}

