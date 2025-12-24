using Corno.Web.Dtos;
using System;
using Volo.Abp.Data;

namespace Corno.Web.Areas.DemoProject.Dtos;

public class LabelIndexDto : BaseDto
{
    #region -- Properties --
    //public int Id { get; set; }
    public string Barcode { get; set; }
    public DateTime? LabelDate { get; set; }
    public int? ProductId { get; set; }
    public string ProductName { get; set; }
    public double? Mrp { get; set; }
    public double? NetWeight { get; set; }
    public int? PackingTypeId { get; set; }
    public string PackingTypeName { get; set; }
    public DateTime? ManufacturingDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    //public string Status { get; set; }

    #endregion
}