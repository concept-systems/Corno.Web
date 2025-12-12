using System;
using Corno.Web.Dtos;

namespace Corno.Web.Areas.Kitchen.Dto.Pallet;

public class CartonPalletViewDto : BaseDto
{
    #region -- Properties --
    public string PalletNo { get; set; }
    public string Barcode { get; set; }
    public int? CartonId { get; set; }
    public DateTime? ScanDate { get; set; }
    public string RackNo { get; set; }
    #endregion
}