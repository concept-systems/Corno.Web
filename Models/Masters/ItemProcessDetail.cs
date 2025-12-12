using System.ComponentModel.DataAnnotations;
using Corno.Web.Models.Base;
using Mapster;

namespace Corno.Web.Models.Masters;

public class ItemProcessDetail : BaseModel {
    public int? ItemId { get; set; }
    public int? ProcessId { get; set; }
    public string ProcessSymbol { get; set; }
    public int? ProcessSequence { get; set; }
    public double? CycleTime1 { get; set; }
    public double? CycleTime2 { get; set; }
    public double? ProcessTime { get; set; }

    public string ShortEdge1 { get; set; }
    public string ShortEdge2 { get; set; }
    public string LongEdge1 { get; set; }
    public string LongEdge2 { get; set; }

    public string ProgramFile { get; set; }

    [Required]
    [AdaptIgnore]
    public virtual Item Item { get; set; }

    /*#region -- Public Methods --
    public void Copy(ItemProcessDetail other) {
        ProcessId = other.ProcessId;
        ProcessSymbol = other.ProcessSymbol;
        ProcessSequence = other.ProcessSequence;
        CycleTime1 = other.CycleTime1;
        CycleTime2 = other.CycleTime2;
        ProcessTime = other.ProcessTime;
        ShortEdge1 = other.ShortEdge1;
        ShortEdge2 = other.ShortEdge2;
        LongEdge1 = other.LongEdge1;
        LongEdge2 = other.LongEdge2;
        ProgramFile = other.ProgramFile;
    }
    #endregion*/
}