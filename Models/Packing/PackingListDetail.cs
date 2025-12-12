using System;
using System.ComponentModel.DataAnnotations;
using Corno.Web.Models.Base;
using Mapster;

namespace Corno.Web.Models.Packing;

[Serializable]
public class PackingListDetail : BaseModel
{
    #region -- Properties --
    public int? PackingListId { get; set; }
    public string CoilNo { get; set; }
    public string Barcode { get; set; }
    public double? TareWeight { get; set; }
    public double? GrossWeight { get; set; }
    public double? NetWeight { get; set; }
    public double? Length { get; set; }
    public string SetNo { get; set; }
    public string HeatNo { get; set; }
    public string MachineNo { get; set; }
    public string Remark { get; set; }

    [Required]
    [AdaptIgnore]
    public virtual PackingList PackingList { get; set; }
    #endregion

    /*#region -- Public Methods --
    public void Copy(PackingListDetail other)
    {
        if (null == other) return;

        Barcode = other.Barcode;
        GrossWeight = other.GrossWeight;
        NetWeight = other.NetWeight;
        TareWeight = other.TareWeight;
        Length = other.Length;
        Remark = other.Remark;

        ModifiedBy = other.ModifiedBy;
        ModifiedDate = other.ModifiedDate;
    }
    #endregion*/
}
