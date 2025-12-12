using Corno.Web.Models.Base;

namespace Corno.Web.Areas.BoardStore.Models;

public class StackItem : BaseModel
{
    public int StackId { get; set; }
    public int? StackNo { get; set; }
    public int? ItemId { get; set; }
    public int? Quantity { get; set; }

    public Stack Stack { get; set; }
}