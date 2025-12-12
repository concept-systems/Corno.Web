using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Corno.Web.Models.Base;

namespace Corno.Web.Models.Plan;

[Serializable]
public class Plan : TransactionModel
{
    #region -- Constructors --
    public Plan()
    {
        PlanItemDetails = new List<PlanItemDetail>();
        PlanPacketDetails = new List<PlanPacketDetail>();
    }
    #endregion

    #region -- Properties --
    public DateTime? PlanDate { get; set; }
    public string PlanType { get; set; }
    [DisplayName("Customer")]
    public int? CustomerId { get; set; }
    public int? SupplierId { get; set; }
    [DisplayName("Product")]
    public int? ProductId { get; set; }
    public int? PackingTypeId { get; set; }
    public string ProductionOrderNo { get; set; }
    public string WarehouseOrderNo { get; set; }
    public string LotNo { get; set; }
    public string BatchNo { get; set; }
    public double? BatchWeight { get; set; }
    public string ContainerNo { get; set; }
    public string LoadNo { get; set; }
    public int? WarehouseId { get; set; }
    public DateTime? ReleaseDate { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? DeliveryDate { get; set; }
    public DateTime? LoadingDate { get; set; }
    public DateTime? SoDate { get; set; }
    public string SoNo { get; set; }
    public string PoNo { get; set; }
    public string ProductLine { get; set; }
    public double? OrderQuantity { get; set; }
    public double? PlanQuantity { get; set; }
    public double? PrintQuantity { get; set; }
    public double? CutQuantity { get; set; }
    public double? BendQuantity { get; set; }
    public double? SortQuantity { get; set; }
    public double? EdgeBandQuantity { get; set; }
    public double? Q1Quantity { get; set; }
    public double? RoutingQuantity { get; set; }
    public double? ManualEdgeBandQuantity { get; set; }
    public double? DrillQuantity { get; set; }
    public double? CleanQuantity { get; set; }
    public double? SubAssemblyQuantity { get; set; }
    public double? Q2Quantity { get; set; }
    public double? PackQuantity { get; set; }
    public double? Q3Quantity { get; set; }
    public double? PalletInQuantity { get; set; }
    public double? HandoverQuantity { get; set; }
    public double? HandoverReceiveQuantity { get; set; }
    public double? RejectQuantity { get; set; }

    public string System { get; set; }
    public string Reserved1 { get; set; }

    public List<PlanItemDetail> PlanItemDetails { get; set; }
    public List<PlanPacketDetail> PlanPacketDetails { get; set; }
    #endregion

    #region -- Public Methods --
    public override bool UpdateDetails(CornoModel newModel)
    {
        if (newModel is not Plan newPlan) return false;

        // Update existing entries
        foreach (var planItemDetail in PlanItemDetails)
        {
            var newPlanItemDetail = newPlan.PlanItemDetails.FirstOrDefault(d =>
                d.Id == planItemDetail.Id);
            planItemDetail.Copy(newPlanItemDetail);
        }
        // Add new entries
        var newItemDetails = newPlan.PlanItemDetails.Where(d => d.Id <= 0).ToList();
        PlanItemDetails.AddRange(newItemDetails);
        // Remove items from list1 that are not in list2
        PlanItemDetails.RemoveAll(x => newPlan.PlanItemDetails.All(y => y.Id != x.Id));

        // Update existing entries
        foreach (var planPacketDetail in PlanPacketDetails)
        {
            var newPlanPacketDetail = newPlan.PlanPacketDetails.FirstOrDefault(d =>
                d.Id == planPacketDetail.Id);
            planPacketDetail.Copy(newPlanPacketDetail);
        }
        // Add New Entries
        var newPacketDetails = newPlan.PlanPacketDetails.Where(d => d.Id <= 0).ToList();
        PlanPacketDetails.AddRange(newPacketDetails);
        // Remove items from list1 that are not in list2
        PlanPacketDetails.RemoveAll(x => newPlan.PlanPacketDetails.All(y => y.Id != x.Id));

        return true;
    }
    #endregion
}