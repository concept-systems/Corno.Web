using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Corno.Web.Models.Base;
using Mapster;

namespace Corno.Web.Models.Plan;

[Serializable]
public class PlanPacketDetail : BaseModel
{
    [DisplayName("Plan")]
    public int? PlanId { get; set; }
    [DisplayName("Packing Type")]
    public int? PackingTypeId { get; set; }
    public int? OrderQuantity { get; set; }
    public int? PrintedQuantity { get; set; }
    public int? PackedQuantity { get; set; }

    [Required]
    [AdaptIgnore]
    public virtual Plan PLan { get; set; }


    #region -- Methods --
    public void Copy(PlanPacketDetail other)
    {
        PackingTypeId = other.PackingTypeId;
        OrderQuantity = other.OrderQuantity;
        PrintedQuantity = other.PrintedQuantity;
        PackedQuantity = other.PackedQuantity;

        ModifiedBy = other.ModifiedBy;
        ModifiedDate = other.ModifiedDate;

        ExtraProperties = other.ExtraProperties;
    }
    #endregion
}