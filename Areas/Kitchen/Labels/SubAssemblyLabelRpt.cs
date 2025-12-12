using System.Collections.Generic;
using System.Linq;
using Corno.Web.Models.Packing;
using Corno.Web.Reports;
using Telerik.Reporting.Processing;

namespace Corno.Web.Areas.Kitchen.Labels;

public partial class SubAssemblyLabelRpt : BaseReport
{
    #region -- Constructors --
    public SubAssemblyLabelRpt(IEnumerable<Label> labels)
    {
        // Required for telerik Reporting designer support
        InitializeComponent();

        labels = labels.ToList();
        if (!labels.Any()) return;

        DataSource = labels.ToList();
    }

    private void detail_ItemDataBound(object sender, System.EventArgs e)
    {
        if (sender is not DetailSection detailSection) return;

        var txtSerialNoP = (TextBox)ElementTreeHelper.GetChildByName(detailSection,
            "txtSerialNo");
        if (null == txtSerialNoP) return;
        txtSerialNoP.Value = txtSerialNoP.Value?.ToString().PadLeft(7, '0');
    }
    #endregion
}