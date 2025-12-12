using System.ComponentModel.DataAnnotations;
using Corno.Web.Models.Base;
using Mapster;

namespace Corno.Web.Models.Masters;

public class ItemMachineDetail : BaseModel
{
    public int? ItemId { get; set; }
    public int? ProcessId { get; set; }
    public int? MachineId { get; set; }
    public int? Priority { get; set; }

    [Required]
    [AdaptIgnore]
    public virtual Item Item { get; set; }

    /*#region -- Public Methods --
    public void Copy(ItemMachineDetail other)
    {
        ProcessId = other.ProcessId;
        MachineId = other.MachineId;
        Priority = other.Priority;
    }
    #endregion*/
}