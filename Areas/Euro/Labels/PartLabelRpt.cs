using System.Collections.Generic;
using System.Linq;
using Corno.Web.Models.Packing;
using Corno.Web.Reports;
using Telerik.Reporting.Processing;

namespace Corno.Web.Areas.Euro.Labels;

public partial class PartLabelRpt : BaseReport
{
    #region -- Constructors --
    public PartLabelRpt(IEnumerable<Label> labels, bool bDuplicate = false)
    {
        // Required for telerik Reporting designer support
        InitializeComponent();

        labels = labels.ToList();
        if (!labels.Any()) return;

        txtDuplicate.Visible = bDuplicate;

        DataSource = labels.Select(p => new
        {
            ProjectName = p.Code,
            ItemCode = p.Reserved1,
            ItemName = p.Description,
            ArticleName = p.ArticleNo,
            p.Barcode,
            p.Length,
            p.Width,
            p.Thickness,

        }).ToList();
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
