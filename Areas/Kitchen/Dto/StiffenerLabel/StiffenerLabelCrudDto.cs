using System;
using Corno.Web.Dtos;

namespace Corno.Web.Areas.Kitchen.Dto.StiffenerLabel;

public class StiffenerLabelCrudDto : BaseDto
{
    #region -- Properties --

    public DateTime? DueDate { get; set; }
    public string LotNo { get; set; }
    public string DrawingNo { get; set; }

    public bool PrintToPrinter { get; set; }
   // public virtual List<StoreLabel.StoreLabelCrudDetailDto> StoreLabelCrudDetailDtos { get; set; } = new();
    #endregion
    #region -- Public Methods --
    public void Clear()
    {
        //WarehouseOrderNo = default;
        DrawingNo = default;
        //StoreLabelCrudDetailDtos = default;
    }
    #endregion
}