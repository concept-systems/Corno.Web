using System.Collections.Generic;
using System.Linq;
using Corno.Web.Models.Packing;
using Corno.Web.Reports;
using Corno.Web.Services.Masters.Interfaces;
using Corno.Web.Windsor;
using Telerik.Reporting.Processing;

namespace Corno.Web.Areas.Nilkamal.Labels;

public partial class PartLabelRpt : BaseReport
{
    #region -- Constructors --
    public PartLabelRpt(IEnumerable<Label> labels, bool bDuplicate = false)
    {
        // Required for telerik Reporting designer support
        InitializeComponent();

        var enumerable = labels.ToList();
        if (!enumerable.Any()) return;

        var itemService = Bootstrapper.Get<IBaseItemService>();
        var itemIds = enumerable.Select(l => l.ItemId).Distinct().ToList();
        var items = RunAsync(() => itemService.GetViewModelListAsync(p => itemIds.Contains(p.Id)));

        DataSource = enumerable.Select(p =>
        {
            var item = items.FirstOrDefault(i => i.Id == p.ItemId);
            return new
            {
                p.SerialNo,
                ItemCode = item?.Code,
                ItemName = item?.Name,
                p.Barcode
            };
        });
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