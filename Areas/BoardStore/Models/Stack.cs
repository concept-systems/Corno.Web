using Corno.Web.Models.Base;

namespace Corno.Web.Areas.BoardStore.Models;

public class Stack : BaseModel
{
    #region -- Propertis --
    public int? RequestId { get; set; }
    public int? RequestNo { get; set; }
    public int? StackNo { get; set; }
    public int? BayNo { get; set; }

    public Request Request { get; set; }

    //public List<StackItem> StackItems { get; set; } = new();

    #endregion
}