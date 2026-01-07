namespace Corno.Web.Areas.Nilkamal.Dto.PacketLabel;

public class CartonCrudDetailDto
{
    #region -- Properties --
    public string ItemName { get; set; }
    public string ItemCode { get; set; }
    public bool IsSelected { get; set; }
    public double? PendingQuantity { get; set; }
    #endregion
}