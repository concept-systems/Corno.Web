using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Corno.Web.Models.Packing;

public class CartonUnmapped
{
    [NotMapped]
    public DateTime? PackingDate { get; set; }
    [NotMapped]
    public string ProductCode { get; set; }
    [NotMapped]
    public string ProductName { get; set; }
    [NotMapped]
    public string ProductDescription { get; set; }
    [NotMapped]
    public string CustomerName { get; set; }
    [NotMapped]
    public string CustomerAddress { get; set; }
    [NotMapped]
    public byte[] ItemPhoto { get; set; }
    [NotMapped]
    public string ProductCategoryName { get; set; }
    [NotMapped]
    public string Location { get; set; }
    [NotMapped]
    public double? GrossWeight { get; set; }
    [NotMapped]
    public double? NetWeight { get; set; }
    [NotMapped]
    public double? Quantity { get; set; }
    [NotMapped]
    public string Shift { get; set; }

    [NotMapped]
    public string UserName { get; set; }
}