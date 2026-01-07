using System;
using System.Collections.Generic;
using System.Linq;
using Corno.Web.Models.Packing;
using Corno.Web.Reports;
using Corno.Web.Services.Interfaces;
using Corno.Web.Services.Masters.Interfaces;
using Corno.Web.Windsor;
using Telerik.Reporting.Processing;

namespace Corno.Web.Areas.Nilkamal.Labels;

public partial class PacketLabelRpt : BaseReport
{
    #region -- Constructors --
    public PacketLabelRpt(IEnumerable<Carton> cartons, bool bDuplicate = false)
    {
        // Required for telerik Reporting designer support
        InitializeComponent();

        var enumerable = cartons.ToList();
        if (!enumerable.Any()) return;

        var miscService = Bootstrapper.Get<IMiscMasterService>();
        var packingTypeIds = enumerable.Select(l => l.PackingTypeId).Distinct().ToList();
        var packingTypes = RunAsync(() => miscService.GetViewModelListAsync(p => packingTypeIds.Contains(p.Id)));

        DataSource = enumerable.Select(p =>
        {
            var packingType = packingTypes.FirstOrDefault(i => i.Id == p.PackingTypeId);
            return new
            {
                p.SerialNo,
                PacketCode = packingType?.Code,
                Date = p.PackingDate?.ToString("dd/MM/yyyy"),
                Barcode = p.CartonBarcode
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