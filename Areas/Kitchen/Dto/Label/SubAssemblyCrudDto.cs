using System.ComponentModel;

namespace Corno.Web.Areas.Kitchen.Dto.Label;

public class SubAssemblyCrudDto
{
    #region -- Constructors --
    public SubAssemblyCrudDto()
    {
        Clear();
    }
    #endregion

    #region -- Properties --

    public string WarehouseOrderNo { get; set; }
    public string Position { get; set; }
    public string Barcode1 { get; set; }
    public string Barcode2 { get; set; }
    public string Barcode3 { get; set; }
    public string Barcode4 { get; set; }
    public bool CheckboxField { get; set; }
    public string Operation { get; set; }

    [DisplayName("Item")]
    public int? ItemId { get; set; }
    public bool PrintToPrinter { get; set; }
    //public  ScreenActionEnum Action { get; set; }
    #endregion

    #region -- Methods --

    public void Clear()
    {
        //WarehouseOrderNo = string.Empty;
        Barcode1 = string.Empty;
        Barcode2 = string.Empty;
        Barcode3 = string.Empty;
        Barcode4 = string.Empty;

        PrintToPrinter = false;
    }
    #endregion
}