namespace Corno.Web.Areas.Kitchen.Labels
{
    partial class ShirwalPartLabelRpt
    {
        #region Component Designer generated code
        /// <summary>
        /// Required method for telerik Reporting designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Telerik.Reporting.Barcodes.QRCodeEncoder qrCodeEncoder1 = new Telerik.Reporting.Barcodes.QRCodeEncoder();
            Telerik.Reporting.Drawing.StyleRule styleRule1 = new Telerik.Reporting.Drawing.StyleRule();
            this.detail = new Telerik.Reporting.DetailSection();
            this.barcode = new Telerik.Reporting.Barcode();
            this.txtItemName = new Telerik.Reporting.TextBox();
            this.txtAglNo = new Telerik.Reporting.TextBox();
            this.txtProductName = new Telerik.Reporting.TextBox();
            this.txtDuplicate = new Telerik.Reporting.TextBox();
            this.textBox3 = new Telerik.Reporting.TextBox();
            this.txtSerialNo = new Telerik.Reporting.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // detail
            // 
            this.detail.Height = Telerik.Reporting.Drawing.Unit.Mm(15D);
            this.detail.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.barcode,
            this.txtItemName,
            this.txtAglNo,
            this.txtProductName,
            this.txtDuplicate,
            this.textBox3,
            this.txtSerialNo});
            this.detail.Name = "detail";
            this.detail.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.None;
            this.detail.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.detail.Style.Font.Bold = true;
            this.detail.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(12D);
            // 
            // barcode
            // 
            this.barcode.Encoder = qrCodeEncoder1;
            this.barcode.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(1.44D), Telerik.Reporting.Drawing.Unit.Inch(0.039D));
            this.barcode.Name = "barcode";
            this.barcode.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.512D), Telerik.Reporting.Drawing.Unit.Inch(0.512D));
            this.barcode.Stretch = true;
            this.barcode.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.None;
            this.barcode.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.barcode.Style.Font.Bold = true;
            this.barcode.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(5.25D);
            this.barcode.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.barcode.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Bottom;
            this.barcode.Value = "=Fields.Barcode";
            // 
            // txtItemName
            // 
            this.txtItemName.CanGrow = false;
            this.txtItemName.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.051D), Telerik.Reporting.Drawing.Unit.Inch(0.157D));
            this.txtItemName.Name = "txtItemName";
            this.txtItemName.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.385D), Telerik.Reporting.Drawing.Unit.Inch(0.157D));
            this.txtItemName.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.None;
            this.txtItemName.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.txtItemName.Style.Font.Bold = true;
            this.txtItemName.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(5.25D);
            this.txtItemName.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.txtItemName.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.txtItemName.Value = "= Fields.WarehouseOrderNo";
            // 
            // txtAglNo
            // 
            this.txtAglNo.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.051D), Telerik.Reporting.Drawing.Unit.Inch(0.315D));
            this.txtAglNo.Name = "txtAglNo";
            this.txtAglNo.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.743D), Telerik.Reporting.Drawing.Unit.Inch(0.118D));
            this.txtAglNo.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.None;
            this.txtAglNo.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.txtAglNo.Style.Font.Bold = true;
            this.txtAglNo.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(5.25D);
            this.txtAglNo.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.txtAglNo.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.txtAglNo.Value = "= Fields.Position";
            // 
            // txtProductName
            // 
            this.txtProductName.CanGrow = false;
            this.txtProductName.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.051D), Telerik.Reporting.Drawing.Unit.Inch(0.039D));
            this.txtProductName.Name = "txtProductName";
            this.txtProductName.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.385D), Telerik.Reporting.Drawing.Unit.Inch(0.118D));
            this.txtProductName.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.None;
            this.txtProductName.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.txtProductName.Style.Font.Bold = true;
            this.txtProductName.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(5.25D);
            this.txtProductName.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.txtProductName.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.txtProductName.Value = "= Fields.NotMapped.ItemCode";
            // 
            // txtDuplicate
            // 
            this.txtDuplicate.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.794D), Telerik.Reporting.Drawing.Unit.Inch(0.323D));
            this.txtDuplicate.Name = "txtDuplicate";
            this.txtDuplicate.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.191D), Telerik.Reporting.Drawing.Unit.Inch(0.1D));
            this.txtDuplicate.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.None;
            this.txtDuplicate.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.txtDuplicate.Style.Color = System.Drawing.Color.DarkRed;
            this.txtDuplicate.Style.Font.Bold = true;
            this.txtDuplicate.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(7.25D);
            this.txtDuplicate.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.txtDuplicate.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.txtDuplicate.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.txtDuplicate.Value = "RP";
            // 
            // textBox3
            // 
            this.textBox3.Format = "{0:N0}";
            this.textBox3.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(1.026D), Telerik.Reporting.Drawing.Unit.Inch(0.315D));
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.405D), Telerik.Reporting.Drawing.Unit.Inch(0.118D));
            this.textBox3.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.None;
            this.textBox3.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox3.Style.Font.Bold = true;
            this.textBox3.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(5.25D);
            this.textBox3.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox3.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.textBox3.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox3.Value = "= Fields.Quantity";
            // 
            // txtSerialNo
            // 
            this.txtSerialNo.CanGrow = false;
            this.txtSerialNo.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.055D), Telerik.Reporting.Drawing.Unit.Inch(0.437D));
            this.txtSerialNo.Name = "txtSerialNo";
            this.txtSerialNo.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.385D), Telerik.Reporting.Drawing.Unit.Inch(0.125D));
            this.txtSerialNo.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.None;
            this.txtSerialNo.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.txtSerialNo.Style.Font.Bold = true;
            this.txtSerialNo.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(5.25D);
            this.txtSerialNo.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.txtSerialNo.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.txtSerialNo.Value = "= Fields.Barcode";
            // 
            // ShirwalPartLabelRpt
            // 
            this.Attributes = null;
            this.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.detail});
            this.Name = "ShirwalPartLabelRpt";
            this.PageSettings.ColumnCount = 2;
            this.PageSettings.Margins = new Telerik.Reporting.Drawing.MarginsU(Telerik.Reporting.Drawing.Unit.Mm(0D), Telerik.Reporting.Drawing.Unit.Mm(0D), Telerik.Reporting.Drawing.Unit.Mm(0D), Telerik.Reporting.Drawing.Unit.Mm(0D));
            this.PageSettings.PaperKind = System.Drawing.Printing.PaperKind.Custom;
            this.PageSettings.PaperSize = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Mm(106D), Telerik.Reporting.Drawing.Unit.Mm(15D));
            this.Style.Font.Name = "Segoe UI";
            this.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8.25D);
            styleRule1.Selectors.AddRange(new Telerik.Reporting.Drawing.ISelector[] {
            new Telerik.Reporting.Drawing.TypeSelector(typeof(Telerik.Reporting.TextItemBase)),
            new Telerik.Reporting.Drawing.TypeSelector(typeof(Telerik.Reporting.HtmlTextBox))});
            styleRule1.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(2D);
            styleRule1.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.StyleSheet.AddRange(new Telerik.Reporting.Drawing.StyleRule[] {
            styleRule1});
            this.Width = Telerik.Reporting.Drawing.Unit.Mm(53D);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }
        #endregion
        private Telerik.Reporting.DetailSection detail;
        private Telerik.Reporting.TextBox txtProductName;
        private Telerik.Reporting.TextBox txtAglNo;
        private Telerik.Reporting.TextBox txtItemName;
        private Telerik.Reporting.Barcode barcode;
        private Telerik.Reporting.TextBox txtDuplicate;
        private Telerik.Reporting.TextBox textBox3;
        private Telerik.Reporting.TextBox txtSerialNo;
    }
}