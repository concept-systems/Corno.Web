using Corno.Web.Dtos;

namespace Corno.Web.Areas.DemoProject.Dtos;

public class SalesInvoiceDetailDto : BaseDto
{
    #region -- Properties --
    public int? ProductId { get; set; }
    public string ProductName { get; set; }
    public int? PackingTypeId { get; set; }
    public string PackingTypeShortName { get; set; }
    public string Barcode { get; set; }
    public double Quantity { get; set; }
    public double Mrp { get; set; }
    public double NetWeight { get; set; }
    public double Amount { get; set; }
    #endregion
}