using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Corno.Web.Models.Base;
using Ganss.Excel;
using Mapster;
using Newtonsoft.Json;

namespace Corno.Web.Models.Masters;

public class ProductPacketDetail : BaseModel
{
    [DisplayName("Product")]
    public int? ProductId { get; set; }
    [DisplayName("Packing Type")]
    public int PackingTypeId { get; set; }
    public double? Quantity { get; set; }
    public int? ProductCategoryId { get; set; }
    public int? LayerId { get; set; }
    public int? StationId { get; set; }
    [Ignore]
    public byte[] Photo { get; set; }
    public int? LoadingSequence { get; set; }

    [NotMapped]
    public string PackingTypeName { get; set; }

    [Required]
    [AdaptIgnore]
    [JsonIgnore]
    protected virtual Product Product { get; set; }


    /*#region -- Public Methods --
    public override void Copy(BaseModel other)
    {
        var detail = other as ProductPacketDetail ?? throw new ArgumentNullException(nameof(other));

        PackingTypeId = detail.PackingTypeId;
        Quantity = detail.Quantity;
        ProductCategoryId = detail.ProductCategoryId;
        LayerId = detail.LayerId;
        StationId = detail.StationId;
        Photo = detail.Photo;
        LoadingSequence = detail.LoadingSequence;

        ExtraProperties = detail.ExtraProperties;
    }
    #endregion*/
}