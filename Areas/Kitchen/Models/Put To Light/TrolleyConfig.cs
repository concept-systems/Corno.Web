using System;
using System.Collections.Generic;
using Corno.Web.Models.Base;

namespace Corno.Web.Areas.Kitchen.Models.Put_To_Light;

public class TrolleyConfig : BaseModel
{
    #region -- Properties --
    public DateTime? DueDate { get; set; }
    public string LotNo { get; set; }
    public string Family { get; set; }

    public List<TrolleyConfigDetail> TrolleyLightDetails { get; set; } = new();
    #endregion
}