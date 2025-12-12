using System;
using System.ComponentModel.DataAnnotations;
using Corno.Web.Models.Base;
using Mapster;

namespace Corno.Web.Models.Packing;

[Serializable]
public class LabelDetail : BaseModel
{
    public int? LabelId { get; set; }
    public DateTime? ScanDate { get; set; }
    public string MachineNo { get; set; }
    public string InPalletNo { get; set; }
    public string OutPalletNo { get; set; }
    public string Location { get; set; }
    public string IndentNo { get; set; }
    public double? Quantity { get; set; }

    // Qc
    public int? ReasonId { get; set; }
    public string InspectionInstruction { get; set; }
    public string InspectionResult { get; set; }
    public int? DispositionId { get; set; }


    [Required]
    [AdaptIgnore]
    public virtual Label Label { get; set; }
}