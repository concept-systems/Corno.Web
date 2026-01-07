using System;

namespace Corno.Web.Areas.Nilkamal.Dto;
public class CartonIndexDto
{
    #region -- Properties --
    public int? Id { get; set; }
    public DateTime? DueDate { get; set; }
    public string LotNo { get; set; }
    public DateTime? PackingDate { get; set; }
    public string SoNo { get; set; }
    public string WarehouseOrderNo { get; set; }
    public string CartonNo { get; set; }
    public string CartonBarcode { get; set; }
    public string OneLineItemCode { get; set; }
    public string Status { get; set; }
    #endregion
}
