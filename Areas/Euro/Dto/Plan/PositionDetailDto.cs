using System;
using System.Collections.Generic;

namespace Corno.Web.Areas.Euro.Dto.Plan;

public class PositionDetailDto
{
    public string ProductionOrderNo { get; set; }
    public string CarcassCode { get; set; }
    public string AssemblyCode { get; set; }
    public string Position { get; set; }
    public string LotNo { get; set; }
    public double? Quantity { get; set; }
    public string Category { get; set; }
    public int Count { get; set; }

    public List<PositionLabelDto> PositionLabelDtos { get; set; } = new();
    public List<PositionCartonDto> PositionCartonDtos { get; set; } = new();

    public class PositionLabelDto
    {
        public int Id { get; set; }
        public int? CartonNo { get; set; }
        public double Quantity { get; set; }
        public double PendingQuantity { get; set; }
        public double OrderQuantity { get; set; }
        public double PrintQuantity { get; set; }
        public DateTime? LabelDate { get; set; }
        public string Barcode { get; set; }
        public string Status { get; set; }
    }

    public class PositionCartonDto
    {
        public int Id { get; set; }
        public DateTime? PackingDate { get; set; }
        public string CartonBarcode { get; set; }
        public int? CartonNo { get; set; }
        public string Status { get; set; }
    }
}

