using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Corno.Web.Areas.Euro.Dto.Carton;

public class CartonCrudDto
{
    #region -- Properties --
    public int Id { get; set; }
    public string WarehouseOrderNo { get; set; }
    public DateTime? PackingDate { get; set; }
    public string Barcode { get; set; }
    public string AssemblyCode { get; set; }
    public string CartonDetailsDtosJson { get; set; }
    public bool PrintToPrinter { get; set; }

    public virtual List<CartonDetailsDto> CartonDetailsDtos { get; set; } = new();

    // Don't send to view
    [JsonIgnore]
    public virtual Web.Models.Plan.Plan Plan { get; set; }

    [JsonIgnore]
    public virtual Web.Models.Packing.Carton Carton { get; set; }

    [JsonIgnore]
    public virtual List<Web.Models.Packing.Label> Labels { get; set; } = new();
    #endregion
    #region -- Public Methods --
    public void Clear()
    {
        WarehouseOrderNo = default;
        CartonDetailsDtos = default;
        CartonDetailsDtosJson = default;
    }
    #endregion
}

