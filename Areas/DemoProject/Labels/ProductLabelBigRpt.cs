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

public partial class ProductLabelBigRpt : BaseReport
{
    public ProductLabelBigRpt()
    {
        // Required for telerik Reporting designer support
        InitializeComponent();
    }

    public ProductLabelBigRpt(List<Label> labels, Product product)
    {
        InitializeComponent();

        if (labels == null || labels.Count <= 0)
            return;

        if (product == null)
            return;

        var productPacketDetail = product.ProductPacketDetails.FirstOrDefault(d => 
            d.PackingTypeId == labels.FirstOrDefault()?.PackingTypeId);

        var miscMasterService = Bootstrapper.Get<IMiscMasterService>();
        var packingType = RunAsync(() => miscMasterService.GetByIdAsync(productPacketDetail?.PackingTypeId ?? 0));

        DataSource = labels.Select(l =>
        {
            var mrp = productPacketDetail?.GetProperty("Mrp", 0);
            return new
            {
                ItemName = product.Name,
                Weight = $"{productPacketDetail?.Quantity} {packingType?.Name}",
                Mrp = $"Rs. {mrp}",
                ManufacturingDate = l.GetProperty(FieldConstants.ManufacturingDate, DateTime.Now),
                ExpiryDate = l.GetProperty("ExpiryDate", DateTime.Now),
                l.Barcode,
            };
        }).ToList();
    }
}