using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Corno.Web.Models.Base;

namespace Corno.Web.Models.Grn;

public class Grn : TransactionModel
{
    #region -- Constructors --
    public Grn()
    {
        GrnDetails = new List<GrnDetail>();
    }
    #endregion

    #region -- Properties --
    [Required]
    public DateTime? GrnDate { get; set; }
    public int? SupplierId { get; set; }
    public string SupplierCode { get; set; }
    public string WorkOrderNo { get; set; }
    public DateTime? WorkOrderDate { get; set; }
    public string PoNo { get; set; }
    public DateTime? PoDate { get; set; }
    public string InvoiceNo { get; set; }
    public string GrnTransactionCode { get; set; }

    public List<GrnDetail> GrnDetails { get; set; }
    #endregion

    #region -- Public Methods --
    public override bool UpdateDetails(CornoModel newModel)
    {
        if (newModel is not Grn newGrn) return false;

        var toDelete = new List<GrnDetail>();
        foreach (var grnDetail in GrnDetails)
        {
            var newGrnDetail = newGrn.GrnDetails.FirstOrDefault(d =>
                d.Id == grnDetail.Id);
            //if (null == newPlanItemDetail)
            //{
            //    toDelete.Add(planItemDetail);
            //    continue;
            //}

            grnDetail.Copy(newGrnDetail);
        }

        // Add new entries
        var newGrnDetails = newGrn.GrnDetails.Where(d => d.Id <= 0).ToList();
        GrnDetails.AddRange(newGrnDetails);
        // Delete existing entries
        foreach (var grnDetail in toDelete)
            GrnDetails.Remove(grnDetail);

        return true;
    }
    #endregion
}