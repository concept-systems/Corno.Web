using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using Corno.Web.Models.Base;


namespace Corno.Web.Models;

public sealed class DimensionScale : BaseModel
{
    public string IpAddress { get; set; }
    public double? Port { get; set; }

    public bool IsStarted { get; set; }

    [DisplayName("Actual Length")]
    public double? ActualLength { get; set; }

    [DisplayName("Actual Width")]
    public double? ActualWidth { get; set; }
    [DisplayName("Actual Height")]
    public double? ActualHeight { get; set; }


    [NotMapped]
    public string ProductName { get; set; }
    [NotMapped]
    public string ProductCode { get; set; }
    [NotMapped]
    [DisplayName("Standard Length")]
    public double? StandardLength { get; set; }
    [NotMapped]
    public double? LengthTolerance { get; set; }
    [NotMapped]
    [DisplayName("Standard Width")]
    public double? StandardWidth { get; set; }
    [NotMapped]
    public  double? WidthTolerance { get; set; }
}