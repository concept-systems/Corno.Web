namespace Corno.Web.Areas.Kitchen.Dto.Label;

public class LabelCrudDetailDto
{
    #region -- Properties --
    public string Family { get; set; }
    public string Position { get; set; }
    public string ItemName { get; set; }
    public string ItemCode { get; set; }
    public bool IsSelected { get; set; }
    public double? PendingQuantity { get; set; }
    #endregion
}