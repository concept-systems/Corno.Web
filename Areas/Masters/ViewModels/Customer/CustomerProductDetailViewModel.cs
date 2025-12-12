using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Corno.Web.Models.Base;
using Newtonsoft.Json;

namespace Corno.Web.Areas.Masters.ViewModels.Customer;

public class CustomerProductDetailViewModel : BaseModel
{
    [DisplayName("Customer")]
    public int? CustomerId { get; set; }
    [DisplayName("Product")]
    public int ProductId { get; set; }
    [DisplayName("Machine")]
    public int MachineId { get; set; }
    public string LabelFormat { get; set; }
    public string PrinterName { get; set; }
    [NotMapped]
    public string ProductName { get; set; }

    public string ProductCode { get; set; }
    [NotMapped]
    public string MachineName { get; set; }
    [Required]
    [JsonIgnore]
    protected virtual CustomerViewModel Customer { get; set; }
    //public void Copy(CustomerProductDetail other);

}