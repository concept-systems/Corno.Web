using Corno.Web.Dtos;
using Corno.Web.Models.Base;
using Mapster;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Corno.Web.Models.Masters;

public class ProductItemDetail : BaseModel
{
    public ProductItemDetail()
    {
        PackingTypeId = 0;
    }

    [DisplayName("Product")]
    public int? ProductId { get; set; }
    public int ItemId { get; set; }
    public double? StandardWeight { get; set; }
    public double? Quantity { get; set; }
    public double? SystemWeight { get; set; }
    public double? Tolerance { get; set; }
    public string QcCode { get; set; }
    public int? PackingTypeId { get; set; }
    public int? PackingTypeId1 { get; set; }
    public int? LayerId { get; set; }
    public int? Layer { get; set; }
    public int? PackingSequence { get; set; }
    public int? AssemblySequence { get; set; }
    public int? AssemblyQuantity { get; set; }
    public string InstallationGuide { get; set; }

    [NotMapped]
    public MasterDto PackingType { get; set; }
    [NotMapped]
    public MasterDto Item { get; set; }
    [NotMapped]
    public string ItemName { get; set; }

    [Required]
    [AdaptIgnore]
    [JsonIgnore]
    protected virtual Product Product { get; set; }

    /*#region -- Public Methods --
    public override void Copy(BaseModel other)
    {
        var detail = other as ProductItemDetail ?? throw new ArgumentNullException(nameof(other));

        ItemId = detail.ItemId;
        StandardWeight = detail.StandardWeight;
        Quantity = detail.Quantity;
        SystemWeight = detail.SystemWeight;
        Tolerance = detail.Tolerance;
        QcCode = detail.QcCode;
        PackingTypeId = detail.PackingTypeId;
        PackingTypeId1 = detail.PackingTypeId1;
        LayerId = detail.LayerId;
        PackingSequence = detail.PackingSequence;
        AssemblySequence = detail.AssemblySequence;
        AssemblyQuantity = detail.AssemblyQuantity;
        InstallationGuide = detail.InstallationGuide;
        PackingType = detail.PackingType;

        ExtraProperties = detail.ExtraProperties;
    }
    #endregion*/
}