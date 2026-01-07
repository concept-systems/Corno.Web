using System;
using LINQtoCSV;
using Corno.Web.Services.Import;

namespace Corno.Web.Areas.Euro.Dto.Label;

public class LabelImportDto
{
    [CsvColumn(Name = "Cutting Length")]
    public double? Length { get; set; }
    [CsvColumn(Name = "Cutting Width")]
    public double? Width { get; set; }
    [CsvColumn(Name = "Thickness")]
    public double? Thickness { get; set; }
    [CsvColumn(Name = "Final Length")]
    public double? FinalLength { get; set; }
    [CsvColumn(Name = "Final Width")]
    public double? FinalWidth { get; set; }
    [CsvColumn(Name = "Edge Band Thickness")]
    public double? EdgeBandThickness { get; set; }

    [CsvColumn(Name = "Qty")]
    public double? Quantity { get; set; }
    [CsvColumn(Name = "Part Name")]
    public string Description { get; set; }
    
    [CsvColumn(Name = "Material Description")]
    [CsvColumnAlternative("Material Description", "Material")]
    public string MaterialDescription { get; set; }

    [CsvColumn(Name = "Barcode 1")]
    public string Barcode { get; set; }

    [CsvColumn(Name = "Article Name")]
    public string ArticleNo { get; set; }

    [CsvColumn(Name = "Project Name")]
    public string Project { get; set; }
    [CsvColumn(Name = "CUSTOMER REF.")]
    public string CustomerReference { get; set; }
    [CsvColumn(Name = "Grains")]
    public string Grains { get; set; }

    [CsvColumn(Name = "OC No")]
    public string ProductionOrderNo { get; set; }
    [CsvColumn(Name = "Carcass Code")]
    public string CarcassCode { get; set; }
    [CsvColumn(Name = "UNIT")]
    public string BusinessUnit { get; set; }

    public string Status { get; set; }
    public string Remark { get; set; }
}

