using System;
using Corno.Web.Dtos;

namespace Corno.Web.Areas.Nilkamal.Dto.Plan;

public class PlanViewLabelChartDto : BaseDto
{
    #region -- Properties --
    public DateTime? LabelDate { get; set; }
    public int? Count { get; set; }
    #endregion
}