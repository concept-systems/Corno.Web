using Corno.Web.Dtos;
using Corno.Web.Globals;
using Corno.Web.Models.Masters;
using Corno.Web.Models.Packing;
using Corno.Web.Reports;
using Corno.Web.Services.Interfaces;
using Corno.Web.Windsor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Data;

namespace Corno.Web.Areas.DemoProject.Labels;

public partial class ProductLabelSmallRpt : BaseReport
{
    public ProductLabelSmallRpt()
    {
        // Required for telerik Reporting designer support
        InitializeComponent();
    }

    public ProductLabelSmallRpt(List<Label> labels, Product product)
    {
        InitializeComponent();

        if (labels == null || labels.Count <= 0)
            return;

        if (product == null)
            return;

        var productPacketDetail = product.ProductPacketDetails.FirstOrDefault(d => 
            d.PackingTypeId == labels.FirstOrDefault()?.PackingTypeId);

        var miscMasterService = Bootstrapper.Get<IMiscMasterService>();
        var packingType = RunAsync(() => miscMasterService.FirstOrDefaultAsync(p => p.Id == productPacketDetail.PackingTypeId, p => p));

        // Create grouped rows (5 labels = 2 rows)
        var groupedRows = new List<object>();
        var rowCount = (int)Math.Ceiling(labels.Count / 3.0); // 5/3 = 2 rows

        var mrp = productPacketDetail?.GetProperty(FieldConstants.Mrp, 0);
        for (var i = 0; i < rowCount; i++)
        {
            var startIndex = i * 3;
            var label1 = startIndex < labels.Count ? labels[startIndex] : null;
            var label2 = startIndex + 1 < labels.Count ? labels[startIndex + 1] : null;
            var label3 = startIndex + 2 < labels.Count ? labels[startIndex + 2] : null;

            groupedRows.Add(new
            {
                Barcode = label1?.Barcode ?? "",
                Barcode1 = label2?.Barcode ?? "",
                Barcode2 = label3?.Barcode ?? "",
                ProductName = product.Name,
                Weight = $"{productPacketDetail?.Quantity} {packingType?.Name}",
                Mrp = $"Rs. {mrp}",
                ManufacturingDate = label1?.GetProperty(FieldConstants.ManufacturingDate, DateTime.Now) ?? DateTime.Now,
                ExpiryDate = label1?.GetProperty(FieldConstants.ExpiryDate, DateTime.Now) ?? DateTime.Now
            });
        }

        DataSource = groupedRows;

        // Hide second group (panel2) when there is no second label (Barcode1 empty)
        panel2.Visible = true;
        panel2.Bindings.Add(new Telerik.Reporting.Binding(
            "Visible",
            "= IIF( IsNull(Fields.Barcode1, \"\") = \"\", False, True )"));

        // Hide third group (panel3) when there is no third label (Barcode2 empty)
        panel3.Visible = true;
        panel3.Bindings.Add(new Telerik.Reporting.Binding(
            "Visible",
            "= IIF( IsNull(Fields.Barcode2, \"\") = \"\", False, True )"));
    }
}