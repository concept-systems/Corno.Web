using Corno.Web.Dtos;

namespace Corno.Web.Areas.Nilkamal.Dto.Plan;

public class PlanViewItemDto : BaseDto
{
    #region -- Properties --
    public int PlanId { get; set; }
    public string ProductionOrderNo { get; set; }
    public int ProductId { get; set; }
    public string ProductCode { get; set; }
    public string ProductName { get; set; }
    public int ItemId { get; set; }
    public string ItemCode { get; set; }
    public string Description { get; set; }
    public int PackingTypeId { get; set; }
    public string PackingTypeCode { get; set; }
    public string PackingTypeName { get; set; }
    public double OrderQuantity { get; set; }
    public double PrintQuantity { get; set; }
    public double? BomQuantity { get; set; }
    public double? PackQuantity { get; set; }
    public double? RackQuantity { get; set; }
    public double? RackOutQuantity { get; set; }
    #endregion
}