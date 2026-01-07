using System.Collections.Generic;
using Corno.Web.Models.Base;
using Corno.Web.Models.Packing;

namespace Corno.Web.Areas.Nilkamal.Dto.PacketLabel
{
    public class PacketDto : BaseModel
    {
        public string WarehouseOrderNo { get; set; }
        public string Position { get; set; }
        public int? ItemId { get; set; }
        public double? OrderQuantity { get; set; }
        public double? Quantity { get; set; }

        public List<CartonDetail> CartonDetails { get; set; } = new();
    }
}