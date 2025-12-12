using System;
using System.ComponentModel.DataAnnotations;
using Corno.Web.Models.Base;
using Mapster;

namespace Corno.Web.Models.Location;

public class LocationStockDetail : BaseModel 
{
    public int? LocationId { get; set; }

    public string GrnNo { get; set; }
    public DateTime? GrnDate { get; set; }
    public int? ItemId { get; set; }
    public int? ProductId { get; set; }
    public string Position { get; set; }
    public string Barcode { get; set; }
    public double? Quantity { get; set; }

    [Required]
    [AdaptIgnore]
    public virtual Location Location { get; set; }
}