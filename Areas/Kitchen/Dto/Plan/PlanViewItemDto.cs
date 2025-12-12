

using Corno.Web.Dtos;

namespace Corno.Web.Areas.Kitchen.Dto.Plan;

public class PlanViewItemDto : BaseDto
{
    #region -- Properties --
    public int PlanId { get; set; }

    public string Position { get; set; }
    public string ItemType { get; set; }
    public string Group { get; set; }
   
    public string DrawingNo { get; set; }
    public string CarcassCode { get; set; }
    public string AssemblyCode { get; set; }
    public string SoPosition { get; set; }
    public string WarehousePosition { get; set; }
    public string ItemCode { get; set; }
    public string Description { get; set; }
    public string ProductLine { get; set; }
    public double OrderQuantity { get; set; }
    public double PrintQuantity { get; set; }
    public double? BendQuantity { get; set; }
    public double? SortQuantity { get; set; }
    public double? PackQuantity { get; set; }
    public double? RackQuantity { get; set; }
    public double? RackOutQuantity { get; set; }
    #endregion
}