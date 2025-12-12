using Ganss.Excel;

namespace Corno.Web.Models.Masters;

public class MiscMaster : MasterModel
{
    public string MiscType { get; set; }
    [Ignore]
    public byte[] Photo { get; set; }
    public double? Weight { get; set; }
    public double? Tolerance { get; set; }
    public int? PackingUnitId { get; set; }
}