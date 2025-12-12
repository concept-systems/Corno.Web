using Corno.Web.Models.Base;

namespace Corno.Web.Areas.Masters.ViewModels.Item;

public class ItemMachineViewModel : BaseModel
{
    public int? ItemId { get; set; }
    public int? ProcessId { get; set; }
    public int? MachineId { get; set; }
    public int? Priority { get; set; }
    public string ItemName { get; set; }
}