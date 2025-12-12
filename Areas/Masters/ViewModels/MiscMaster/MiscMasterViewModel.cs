using Corno.Web.Dtos;
using Ganss.Excel;

namespace Corno.Web.Areas.Masters.ViewModels.MiscMaster;

public class MiscMasterDto : MasterDto
{
    public string MiscType { get; set; }
    [Ignore]
    public byte[] Photo { get; set; }
    public double? Weight { get; set; }
    public double? Tolerance { get; set; }
    public int? PackingUnitId { get; set; }

}