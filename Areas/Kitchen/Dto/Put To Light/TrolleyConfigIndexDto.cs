using System;
using System.Collections.Generic;
using Corno.Web.Models.Base;

namespace Corno.Web.Areas.Kitchen.Dto.Put_To_Light;

public class TrolleyConfigIndexDto : BaseModel
{
    #region -- Properties --
    public DateTime? DueDate { get; set; }
    public string LotNo { get; set; }
    public string Family { get; set; }

    public virtual List<TrolleyConfigDetailDto> TrolleyLightDetailDtos { get; set; } = new();
    #endregion
}