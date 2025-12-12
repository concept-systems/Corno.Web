using System;
using System.Collections.Generic;
using Corno.Web.Dtos;

namespace Corno.Web.Areas.Kitchen.Dto.StoreShirwalLabel;

public class StoreShirwalCrudDto : BaseDto
{
    #region -- Properties --

    public DateTime? DueDate { get; set; }
    public string LotNo { get; set; }
    public string Position { get; set; }
    public string WarehouseOrderNo { get; set; }
    public string Family { get; set; }

    public bool PrintToPrinter { get; set; }
    public virtual List<StoreShirwalLabelCrudDetailDto> StoreShirwalLabelCrudDetailDtos { get; set; } = new();
    #endregion
    #region -- Public Methods --
    public void Clear()
    {
        //WarehouseOrderNo = default;
        Family = default;
        StoreShirwalLabelCrudDetailDtos = default;
    }
    #endregion
}