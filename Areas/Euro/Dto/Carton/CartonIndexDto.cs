using System;

namespace Corno.Web.Areas.Euro.Dto.Carton;
public class CartonIndexDto
{
    #region -- Properties --
    public int? Id { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? PackingDate { get; set; }
    public string ProductionOrderNo { get; set; }
    public string CartonNo { get; set; }
    public string CartonBarcode { get; set; }
    public string Status { get; set; }
    #endregion
}

