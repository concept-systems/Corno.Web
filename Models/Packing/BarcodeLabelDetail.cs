using System;
using System.ComponentModel.DataAnnotations;
using Corno.Web.Models.Base;

namespace Corno.Web.Models.Packing
{
    [Serializable]
    public class BarcodeLabelDetail : BaseModel
    {
        public int? BarcodeLabelId { get; set; }
        public DateTime? ScanDate { get; set; }
        public string MachineNo { get; set; }
        public string InPalletNo { get; set; }
        public string OutPalletNo { get; set; }
        public string Location { get; set; }

        [Required]
        public virtual BarcodeLabel Barcode { get; set; }
    }
}