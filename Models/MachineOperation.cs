using Corno.Web.Models.Base;

namespace Corno.Web.Models;

public class MachineOperation : BaseModel
{
    public int? ProductId { get; set; }
    public string UserId { get; set; }
    public int? ProductTypeId { get; set; }
    public bool IsStarted { get; set; }
}