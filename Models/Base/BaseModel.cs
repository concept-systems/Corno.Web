using Corno.Web.Globals;
using Corno.Web.Models.Masters;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Data;

namespace Corno.Web.Models.Base;

[Serializable]
public class BaseModel : CornoModel, IBaseModel, IHasExtraProperties
{
    #region -- Constructors --

    public BaseModel()
    {
        // ReSharper disable once VirtualMemberCallInConstructor
        Reset();
    }

    #endregion

    #region -- Methods --
    public override void Reset()
    {
        CompanyId = 1;
        SerialNo = 0;

        Id = 0;
        Code = string.Empty;
        /*Ip = IpUtil.GetLocalIp();
        ExtraProperties = new ExtraPropertyDictionary();*/
        Status = StatusConstants.Active;

        //CreatedBy = ApplicationGlobals.UserId;
        CreatedDate = DateTime.Now;
        // Update modified data
        UpdateModified();
    }

    public void UpdateCreated(string userId)
    {
        CreatedBy = userId;
        CreatedDate = DateTime.Now;
    }

    public void UpdateModified(string userId)
    {
        ModifiedBy = userId;
        ModifiedDate = DateTime.Now;
    }

    public void UpdateModified()
    {
        //ModifiedBy = ApplicationGlobals.UserId;
        ModifiedDate = DateTime.Now;
    }

    public virtual void Copy(BaseModel other)
    {

    }

    #endregion

    #region -- Data Members --
    public int? CompanyId { get; set; }
    [DefaultValue(0)]
    public int? SerialNo { get; set; }
    [Column("Code")]
    public string Code { get; set; }
    [Key]
    [Column("Id")]
    //[AdaptIgnore]
    public int Id { get; set; }

    /*public string Ip { get; set; }
    public virtual ExtraPropertyDictionary ExtraProperties { get; protected set; }*/

    public string Status { get; set; }
    //[AdaptIgnore]
    public string CreatedBy { get; set; }
    [DataType(DataType.Date)]
    //[AdaptIgnore]
    public DateTime? CreatedDate { get; set; }
    public string ModifiedBy { get; set; }
    [DataType(DataType.Date)]
    public DateTime? ModifiedDate { get; set; }
    public string DeletedBy { get; set; }
    public DateTime? DeletedDate { get; set; }

    [NotMapped]
    public ExtraPropertyDictionary ExtraProperties { get; set; } = new();

    [Column("ExtraProperties")]
    public string SerializedExtraProperties
    {
        get => JsonConvert.SerializeObject(ExtraProperties);
        set => ExtraProperties = null != value ? JsonConvert.DeserializeObject<ExtraPropertyDictionary>(value) : new();
    }

    #endregion
}