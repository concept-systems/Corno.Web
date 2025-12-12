using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Corno.Web.Areas.Kitchen.Dto.Carcass;

public class CarcassCrudDto 
{
    #region -- Properties --
    public int Id { get; set; }
    public string WarehouseOrderNo { get; set; }
    public DateTime? PackingDate { get; set; }
    public string Barcode { get; set; }
    public string CarcassCode { get; set; }
    public string CarcassDetailsDtosJson { get; set; }
    public bool PrintToPrinter { get; set; }

    public virtual List<CarcassDetailsDto> CarcassDetailsDtos { get; set; } = new();

    // Don't send to view
    [JsonIgnore]
    public virtual Web.Models.Plan.Plan Plan { get; set; }
    [JsonIgnore]
    public virtual List<Web.Models.Packing.Label> Labels { get; set; } = new();
    #endregion

    #region -- Public Methods --
    public void Clear()
    {
        WarehouseOrderNo = default;
        CarcassDetailsDtos = default;
        CarcassDetailsDtosJson = default;
    }
    #endregion
}