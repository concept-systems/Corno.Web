using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Corno.Web.Models.Base;
using Mapster;
using Newtonsoft.Json;

namespace Corno.Web.Models.Masters;

public class CustomerUserDetail : BaseModel
{
    public int? CustomerId { get; set; }
    public string UserId { get; set; }

    [NotMapped]
    public string UserName { get; set; }

    [Required]
    [AdaptIgnore]
    [JsonIgnore]
    protected virtual Customer Customer { get; set; }

    #region -- Public Methods --
    public void Copy(CustomerUserDetail other)
    {
        UserId = other.UserId;
    }
    #endregion
}