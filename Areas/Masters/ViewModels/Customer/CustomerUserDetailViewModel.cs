using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Corno.Web.Models.Base;
using Newtonsoft.Json;

namespace Corno.Web.Areas.Masters.ViewModels.Customer;

public class CustomerUserDetailViewModel : BaseModel
{
    public int? CustomerId { get; set; }
    public string UserId { get; set; }
    [NotMapped]
    public string UserName { get; set; }
    [Required]
    [JsonIgnore]
    protected virtual CustomerViewModel Customer { get; set; }
    //public void Copy(CustomerUserDetail other);

}