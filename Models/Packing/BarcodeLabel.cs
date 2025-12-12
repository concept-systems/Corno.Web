using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Corno.Web.Models.Base;

namespace Corno.Web.Models.Packing
{
    [Serializable]
    public class BarcodeLabel : BaseModel
    {
        #region -- Constructors --
        public BarcodeLabel()
        {
            BarcodeLabelDetails = new List<BarcodeLabelDetail>();
        }
        #endregion

        #region -- Properties --
        public DateTime? LabelDate { get; set; }

        [DisplayName("Product")]
        public int? ProductId { get; set; }
        [DisplayName("Item")]
        public int? ItemId { get; set; }
        public int? PackingTypeId { get; set; }
        public int? AssemblySequence { get; set; }
        public string Barcode { get; set; }
        public double? OrderQuantity { get; set; }
        public double? Quantity { get; set; }
        public double? AcceptedQuantity { get; set; }
        public double? RejectedQuantity { get; set; }
        public double? IssuedQuantity { get; set; }

        public int? CustomerId { get; set; }

        public double? SystemWeight { get; set; }
        public double? TareWeight { get; set; }
        public double? GrossWeight { get; set; }
        public double? NetWeight { get; set; }

        public int? BoxNo { get; set; }

        public string GrnNo { get; set; }
        public string IndentNo { get; set; }
        public string ProductionOrderNo { get; set; }
        public string VehicleNo { get; set; }
        public string SoNo { get; set; }
        public int? SoPosition { get; set; }
        public string PoNo { get; set; }
        public string AsnNo { get; set; }
        public string ReceiptNo { get; set; }

        public string Operation { get; set; }
        public string NextOperation { get; set; }
        public string Reserved1 { get; set; }
        public string Reserved2 { get; set; }

        [NotMapped]
        public int? PackingTypeCount { get; set; }

        public ICollection<BarcodeLabelDetail> BarcodeLabelDetails { get; set; }
        #endregion

        #region -- Methods --
        public string GetCurrentOutPalletNo()
        {
            return BarcodeLabelDetails.OrderByDescending(d => d.ScanDate)
                .FirstOrDefault()?.OutPalletNo;
        }

        public string GetMachineNo(string status)
        {
            return BarcodeLabelDetails.OrderByDescending(d => d.ScanDate)
                .FirstOrDefault(d => d.Status == status)?.MachineNo;
        }

        public void AddDetail(string machineNo, string inPalletNo, 
            string outPalletNo, string rackNo, string newStatus)
        {
            BarcodeLabelDetails.Add(new BarcodeLabelDetail
            {
                ScanDate = DateTime.Now,
                MachineNo = machineNo,
                InPalletNo = inPalletNo,
                OutPalletNo = outPalletNo,
                Location = rackNo,
                Status = newStatus
            });
        }
        #endregion
    }
}