using Corno.Web.Models.Base;

namespace Corno.Web.Areas.Masters.Dtos.Item;

public class ItemMachineDto : BaseModel
{
    public int? ItemId { get; set; }
    public int? ProcessId { get; set; }
    public int? MachineId { get; set; }
    public int? Priority { get; set; }
    public string ItemName { get; set; }
}