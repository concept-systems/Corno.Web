using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Corno.Web.Models.Base;
using Mapster;
using Newtonsoft.Json;

namespace Corno.Web.Models.Masters;

public class CustomerProductDetail : BaseModel
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
    [NotMapped]
    public string MachineName { get; set; }


    [Required]
    [AdaptIgnore]
    [JsonIgnore]
    protected virtual Customer Customer { get; set; }

    #region -- Public Methods --
    public void Copy(CustomerProductDetail other)
    {
        ProductId = other.ProductId;
        MachineId = other.MachineId;
        LabelFormat = other.LabelFormat;
        PrinterName = other.PrinterName;
    }
    #endregion
}