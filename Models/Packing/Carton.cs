using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Corno.Web.Globals;
using Corno.Web.Models.Base;

namespace Corno.Web.Models.Packing;

[Serializable]
public class Carton : TransactionModel
{
    #region -- Constructors --
    public Carton()
    {
        CartonDetails = new List<CartonDetail>();
        CartonRackingDetails = new List<CartonRackingDetail>();
    }
    #endregion

    #region -- Properties --
    public DateTime? PackingDate { get; set; }
    public string PoNo { get; set; }
    public string SoNo { get; set; }
    public string ProductionOrderNo { get; set; }
    public string WarehouseOrderNo { get; set; }
    public string LoadNo { get; set; }
    public string PalletNo { get; set; }
    public string InvoiceNo { get; set; }
    public int? ProductId { get; set; }
    public string CustomerProductCode { get; set; }
    public int? PackingTypeId { get; set; }
    public double? TareWeight { get; set; }
    public int? LineId { get; set; }
    public int? CustomerId { get; set; }
    public int? SupplierId { get; set; }
    public string BatchNo { get; set; }
    public int? BatchCount { get; set; }
    public int? CartonNo { get; set; }
    public string CartonBarcode { get; set; }
        
    public string ReferenceNo { get; set; }
    //public int? FinancialYearId { get; set; }

    public string ShiftIncharge { get; set; }
    public string PdiInCharge { get; set; }

    [NotMapped]
    public DateTime? ManufacturingDate { get; set; }

    [NotMapped]
    public CartonUnmapped CartonUnmapped {get; set; }

    public List<CartonDetail> CartonDetails { get; set; }
    public List<CartonRackingDetail> CartonRackingDetails { get; set; }
    #endregion

    #region -- Methods --

    public string GetCartonNoString()
    {
        return "C" + CartonNo.ToString().PadLeft(3, '0');
    }

    /*public override void UpdateDetails(CornoModel cornoModel)
    {
        if (cornoModel is not Carton carton) return;

        foreach (var cartonDetail in carton.CartonDetails)
        {
            var thisDetail = CartonDetails.FirstOrDefault(d =>
                d.Id == cartonDetail.Id);
            if (null == thisDetail)
            {
                CartonDetails.Add(cartonDetail);
                continue;
            }

            thisDetail.Copy(cartonDetail);
        }

        foreach (var cartonRackingDetail in carton.CartonRackingDetails)
        {
            var thisDetail = CartonRackingDetails.FirstOrDefault(d =>
                d.Id == cartonRackingDetail.Id);
            if (null == thisDetail)
            {
                CartonRackingDetails.Add(cartonRackingDetail);
                continue;
            }

            thisDetail.Copy(cartonRackingDetail);
        }
    }*/


    public CartonDetail GetNextPackingItem()
    {
        return CartonDetails.OrderBy(b => b.PackingSequence)
            .FirstOrDefault(b => b.Status == StatusConstants.Active);
    }

    public int GetNextPackingSequence()
    {
        return CartonDetails.OrderBy(b => b.PackingSequence)
            .FirstOrDefault(b => b.Status == StatusConstants.Active)?
            .PackingSequence ?? 0;
    }

    public double GetTotalNetWeight()
    {
        return CartonDetails.Sum(b => b.NetWeight) ?? 0;
    }

    public bool IsWeightMatchingIncremental(double netWeight)
    {
        var cartonDetail = GetNextPackingItem();
        if (null == cartonDetail) return false;

        var totalNetWeight = GetTotalNetWeight();
        return cartonDetail.IsWeightMatching(netWeight, totalNetWeight);
    }

    public void UpdateWeighingDetails(double netWeight, double tareWeight, string barcode,
        string status)
    {
        var cartonDetail = GetNextPackingItem();
        cartonDetail.UpdateWeighingDetails(netWeight, tareWeight, GetTotalNetWeight(),
            barcode, status);
    }

    public bool IsNextItemAvailable()
    {
        return null != GetNextPackingItem();
    }
    #endregion
}