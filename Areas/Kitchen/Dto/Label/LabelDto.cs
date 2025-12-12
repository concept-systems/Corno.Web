using System.Collections.Generic;
using Corno.Web.Models.Base;
using Corno.Web.Models.Packing;

namespace Corno.Web.Areas.Kitchen.Dto.Label
{
    public class LabelDto : BaseModel
    {
        public string WarehouseOrderNo { get; set; }
        public string Position { get; set; }
        public int? ItemId { get; set; }
        public double? OrderQuantity { get; set; }
        public double? Quantity { get; set; }

        public List<LabelDetail> LabelDetails { get; set; } = new();
    }
}