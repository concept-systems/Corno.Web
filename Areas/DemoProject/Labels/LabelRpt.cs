using System;
using System.Collections.Generic;
using System.Linq;
using Corno.Web.Globals;
using Corno.Web.Models.Masters;
using Corno.Web.Models.Packing;
using Corno.Web.Reports;
using Corno.Web.Services.Interfaces;
using Corno.Web.Windsor;
using Volo.Abp.Data;

namespace Corno.Web.Areas.DemoProject.Labels;

public partial class LabelRpt : BaseReport
{
    public LabelRpt()
    {
        // Required for telerik Reporting designer support
        InitializeComponent();
    }

    public LabelRpt(List<Label> labels, Item item)
    {
        InitializeComponent();

        if (labels == null || labels.Count <= 0)
            return;

        if (item == null)
            return;

        var itemPacketDetail = item.ItemPacketDetails.FirstOrDefault();

        var miscMasterService = Bootstrapper.Get<IMiscMasterService>();
        var packingType = RunAsync(() => miscMasterService.GetByIdAsync(itemPacketDetail?.PackingTypeId ?? 0));

        DataSource = labels.Select(l =>
        {
            var labelRate = l.GetProperty(FieldConstants.Rate, 0);
            var rate = labelRate > 0 ? labelRate : item.Rate;
            return new
            {
                ItemName = item.Name,
                Weight = $"{itemPacketDetail?.Quantity} {packingType?.Name}",
                Mrp = $"Rs. {rate}",
                ManufacturingDate = l.GetProperty(FieldConstants.ManufacturingDate, DateTime.Now),
                ExpiryDate = l.GetProperty("ExpiryDate", DateTime.Now),
                l.Barcode,
            };
        }).ToList();
    }
}