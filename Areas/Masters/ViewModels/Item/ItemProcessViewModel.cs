using Corno.Web.Models.Base;

namespace Corno.Web.Areas.Masters.ViewModels.Item;

public class ItemProcessViewModel : BaseModel
{
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
}