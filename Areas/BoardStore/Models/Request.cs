using System.Collections.Generic;
using Corno.Web.Models.Base;

namespace Corno.Web.Areas.BoardStore.Models;

public class Request : BaseModel
{
    #region -- Propertis --
    public int? HmiCode { get; set; }
    public int? RequestCode { get; set; }
    public int? RequestNo { get; set; }
    public int? RequestPriority { get; set; }

    public List<Stack> Stacks { get;set; } = new();

    #endregion
}