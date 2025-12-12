using System;
using System.Collections.Generic;
using Corno.Web.Models.Base;

namespace Corno.Web.Models.Packing;

[Serializable]
public class PackingList : TransactionModel
{
    #region -- Constructors --
    public PackingList()
    {
        PackingListDetails = new List<PackingListDetail>();
    }
    #endregion

    #region -- Properties --
    public DateTime? Date { get; set; }

    public int? ProductId { get; set; }
    public int? CustomerId { get; set; }

    public string InvoiceNo { get; set; }
    public string PoNo { get; set; }
    public string LotNo { get; set; }
    public string HeatNo { get; set; }
    public string VehicleNo { get; set; }

    public int? TypeId { get; set; }
    public int? StrandId { get; set; }
    public int? StrandDesignationId { get; set; }
    public int? LayDirectionId { get; set; }
    public int? FinishTypeId { get; set; }
    public int? StandardId { get; set; }

    public List<PackingListDetail> PackingListDetails { get; set; }
    #endregion

    /*#region -- Public Methods --
    public override bool UpdateDetails(CornoModel newModel)
    {
        if (newModel is not PackingList newPackingList) return false;

        var toDelete = new List<PackingListDetail>();
        foreach (var packingListDetail in PackingListDetails)
        {
            var newPackingListDetail = newPackingList.PackingListDetails.FirstOrDefault(d =>
                d.Id == packingListDetail.Id);
            packingListDetail.Copy(newPackingListDetail);
        }

        // Delete existing entries
        foreach (var packingListDetail in toDelete)
            PackingListDetails.Remove(packingListDetail);

        return true;
    }
    #endregion*/
}