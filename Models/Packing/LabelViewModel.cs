using System;
using Corno.Web.Dtos;

namespace Corno.Web.Models.Packing
{
    [Serializable]
    public class LabelViewModel : BaseDto
    {
        #region -- Properties --
        public int? SupplierId { get; set; }

        public int? ProductId { get; set; }
        public int? ItemId { get; set; }
        public int? PackingTypeId { get; set; }

        public double? TareWeight { get; set; }
        public double? GrossWeight { get; set; }

        public int? CartonNo { get; set; }

        public double? Quantity { get; set; }
        public double? OrderQuantity { get; set; }
        public double? PrintQuantity { get; set; }
        public double? DeliverQuantity { get; set; }
        public double? AcceptQuantity { get; set; }
        public double? RejectQuantity { get; set; }
        public double? PendingQuantity { get; set; }

        public int? LabelCount { get; set; }
        public int? RejectLabelCount { get; set; }

        public string GrnNo { get; set; }
        public int? GrnId { get; set; }
        public string IndentNo { get; set; }
        public string ProductionOrderNo { get; set; }
        public string WarehouseOrderNo { get; set; }
        public int WarehousePosition { get; set; }
        public string SoNo { get; set; }
        public string PoNo { get; set; }
        public int AsnId { get; set; }
        public string ReceiptNo { get; set; }
        public string InvoiceNo { get; set; }
        public string BatchNo { get; set; }
        #endregion
    }
}