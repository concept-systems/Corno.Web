using System;
using System.ComponentModel.DataAnnotations;
using Corno.Web.Models.Base;
using Mapster;

namespace Corno.Web.Models.Packing;

[Serializable]
public class CartonRackingDetail : BaseModel
{
    public int? CartonId { get; set; }
    public DateTime? ScanDate { get; set; }
    public string PalletNo { get; set; }
    public string RackNo { get; set; }

    [Required]
    [AdaptIgnore]
    public virtual Carton Carton { get; set; }


    /*public void Copy(CartonRackingDetail other)
    {
        ScanDate = other.ScanDate;
        PalletNo = other.PalletNo;
        RackNo = other.RackNo;

        Status = other.Status;
        Code = other.Code;
        SerialNo = other.SerialNo;
    }*/
}