using System;
using System.ComponentModel.DataAnnotations.Schema;
using Corno.Web.Models.Base;

namespace Corno.Web.Areas.Masters.Dtos.Location;

public class LocationStockDto : BaseModel
{

    public int? LocationId { get; set; }
    public string GrnNo { get; set; }
    public DateTime? GrnDate { get; set; }
    public int ItemId { get; set; }
    public string Position { get; set; }
    public string Barcode { get; set; }
    public double? Quantity { get; set; }
    [NotMapped]
    public string ItemName { get; set; }
}