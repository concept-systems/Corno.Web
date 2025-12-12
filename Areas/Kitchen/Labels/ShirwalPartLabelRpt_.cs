using System.Collections.Generic;
using System.Linq;
using Corno.Web.Models.Packing;
using Corno.Web.Reports;
using Telerik.Reporting.Processing;

namespace Corno.Web.Areas.Kitchen.Labels;

public partial class ShirwalPartLabelRpt_ : BaseReport
{
    #region -- Constructors --
    public ShirwalPartLabelRpt_(IEnumerable<Label> barcodeLabels, bool bDuplicate = false)
    {
        // Required for telerik Reporting designer support
        InitializeComponent();

        if (!barcodeLabels.Any()) return;

        txtDuplicate.Visible = bDuplicate;

        DataSource = barcodeLabels;
    }

    private void detail_ItemDataBound(object sender, System.EventArgs e)
    {
        if (sender is not DetailSection detailSection) return;

        var txtSerialNoP = (TextBox)ElementTreeHelper.GetChildByName(detailSection,
            "txtSerialNo");
        if (null == txtSerialNoP) return;
        txtSerialNoP.Value = txtSerialNoP.Value.ToString().PadLeft(7, '0');
    }
    #endregion
}