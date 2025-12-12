namespace Corno.Web.Areas.DemoProject.Dtos;

public class SalesInvoiceDetailDto
{
    #region -- Properties --
    public int? ProductId { get; set; }
    public string ProductName { get; set; }
    public double Barcode { get; set; }
    public double Quantity { get; set; }
    public double Rate { get; set; }
    #endregion
}