using System;
using System.Linq;
using Corno.Web.Areas.DemoProject.Dtos;
using Corno.Web.Extensions;
using Corno.Web.Reports;
using Telerik.Reporting;
using Telerik.Reporting.Drawing;

namespace Corno.Web.Areas.DemoProject.Reports;

public partial class SalesInvoiceReceiptRpt : BaseReport
{
    private object _itemsDataSource;

    public SalesInvoiceReceiptRpt()
    {
        InitializeComponent();
    }

    public SalesInvoiceReceiptRpt(SalesInvoiceDto dto)
    {
        InitializeComponent();

        if (dto == null)
            return;

        // Calculate totals
        var totalAmount = dto.SalesInvoiceDetailDtos?.Sum(d => (d.Mrp.ToInt()) * (d.Quantity.ToInt())) ?? 0;
        var paidAmount = dto.PaidAmount ?? 0;
        var balance = totalAmount - paidAmount;
        
        // Calculate dynamic page height based on content
        // Base height for header and footer sections (in mm)
        // Store info: ~28mm (StoreName 8mm + Address 10mm + Phone 5mm + spacing 5mm)
        // Invoice/Customer: ~15mm (InvoiceNo 5mm + Date 5mm + Customer 5mm + Mobile 5mm + spacing)
        // Totals/Payment: ~25mm (Total 5mm + Paid 5mm + Balance 5mm + PaymentMode 5mm + spacing 5mm)
        // Footer: ~13mm (ThankYou 8mm + Footer 5mm)
        double baseHeight = 98; // Fixed content height (without table)
        
        // Table height calculation
        double tableHeaderHeight = 5; // Table header row
        double tableRowHeight = 5; // Each item row (approximate)
        int itemCount = dto.SalesInvoiceDetailDtos?.Count ?? 0;
        double tableHeight = itemCount > 0 ? tableHeaderHeight + (itemCount * tableRowHeight) : 0;
        
        // Total content height
        double totalHeight = baseHeight + tableHeight;
        
        // Minimum height for thermal printer (ensure at least 100mm for proper printing)
        double minHeight = 100;
        double pageHeight = Math.Max(totalHeight, minHeight);
        
        // Configure page for thermal printer (80mm width) with dynamic height
        PageSettings.PaperSize = new SizeU(Unit.Mm(80), Unit.Mm(pageHeight));
        PageSettings.Margins = new MarginsU(Unit.Mm(2), Unit.Mm(2), Unit.Mm(2), Unit.Mm(2));

        // Create header data source with hardcoded store information
        var reportData = new
        {
            // Header - Hardcoded as requested
            StoreName = "Harsh Agro Farms",
            StoreAddress = "Sr. No. 314/5, Umbri, A/P. Ashvi, Tal. Sangamner, Dist. AhmedNagar",
            StorePhone = "9689818888",
            
            // Invoice details from DTO
            InvoiceNo = dto.Code ?? "N/A",
            InvoiceDate = dto.InvoiceDate.ToString("dd/MM/yyyy HH:mm"),
            
            // Customer details from DTO
            CustomerName = dto.CustomerName ?? "",
            MobileNo = dto.MobileNo ?? "",
            
            // Totals
            TotalAmount = totalAmount.ToString("N2"),
            PaidAmount = paidAmount.ToString("N2"),
            Balance = balance.ToString("N2"),
            PaymentMode = dto.PaymentMode ?? "",
            
            // Footer
            ThankYouMessage = "Thank you for your purchase!",
            FooterMessage = "Visit us again!"
        };

        // Set main data source for header fields
        DataSource = new[] { reportData };
        
        // Prepare items data source for table (store for NeedDataSource event)
        if (dto.SalesInvoiceDetailDtos != null && dto.SalesInvoiceDetailDtos.Any())
        {
            _itemsDataSource = dto.SalesInvoiceDetailDtos.Select((item, index) => new
            {
                SerialNo = (index + 1).ToString(),
                ProductName = item.ProductName ?? "",
                Weight = $"{item.NetWeight} {item.PackingTypeShortName}",
                Rate = item.Mrp.ToDouble().ToString("N2"),
                Amount = item.Amount.ToString("N2")
            }).ToList();
        }
        else
        {
            // If no items, hide the table
            table1.Visible = false;
        }

        // Subscribe to NeedDataSource event for table
        table1.DataSource = _itemsDataSource;
        //table1.NeedDataSource += Table1_NeedDataSource;
        
        // Adjust detail section height to match page height (minus margins)
        double detailHeight = pageHeight - 4; // Subtract top and bottom margins (2mm each)
        detail.Height = Unit.Mm(detailHeight);
    }
    
    private void Table1_NeedDataSource(object sender, System.EventArgs e)
    {
        // Set DataSource in NeedDataSource event - this is the correct place for Telerik tables
        if (sender is Table table && _itemsDataSource != null)
            table.DataSource = _itemsDataSource;
    }
}

