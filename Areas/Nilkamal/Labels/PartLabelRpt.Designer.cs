namespace Corno.Web.Areas.Nilkamal.Labels
{
    partial class PartLabelRpt
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
            this.txtSerialNo = new Telerik.Reporting.TextBox();
            this.txtItemCode = new Telerik.Reporting.TextBox();
            this.textBox1 = new Telerik.Reporting.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // detail
            // 
            this.detail.Height = Telerik.Reporting.Drawing.Unit.Inch(0.807D);
            this.detail.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.barcode,
            this.txtSerialNo,
            this.txtItemCode,
            this.textBox1});
            this.detail.Name = "detail";
            this.detail.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.None;
            this.detail.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.detail.Style.Font.Bold = true;
            this.detail.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(12D);
            this.detail.ItemDataBound += new System.EventHandler(this.detail_ItemDataBound);
            // 
            // barcode
            // 
            this.barcode.Encoder = qrCodeEncoder1;
            this.barcode.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.76D), Telerik.Reporting.Drawing.Unit.Inch(0.268D));
            this.barcode.Name = "barcode";
            this.barcode.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.544D), Telerik.Reporting.Drawing.Unit.Inch(0.472D));
            this.barcode.Stretch = true;
            this.barcode.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.None;
            this.barcode.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.barcode.Style.Font.Bold = true;
            this.barcode.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.barcode.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.barcode.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Bottom;
            this.barcode.Value = "=Fields.Barcode";
            // 
            // txtSerialNo
            // 
            this.txtSerialNo.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.039D), Telerik.Reporting.Drawing.Unit.Inch(0.615D));
            this.txtSerialNo.Name = "txtSerialNo";
            this.txtSerialNo.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.671D), Telerik.Reporting.Drawing.Unit.Inch(0.153D));
            this.txtSerialNo.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.None;
            this.txtSerialNo.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.txtSerialNo.Style.Font.Bold = true;
            this.txtSerialNo.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(6.25D);
            this.txtSerialNo.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.txtSerialNo.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.txtSerialNo.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.txtSerialNo.Value = "= Fields.SerialNo";
            // 
            // txtItemCode
            // 
            this.txtItemCode.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.029D), Telerik.Reporting.Drawing.Unit.Inch(0.039D));
            this.txtItemCode.Name = "txtItemCode";
            this.txtItemCode.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.263D), Telerik.Reporting.Drawing.Unit.Inch(0.166D));
            this.txtItemCode.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.None;
            this.txtItemCode.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.txtItemCode.Style.Font.Bold = true;
            this.txtItemCode.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(6.1D);
            this.txtItemCode.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.txtItemCode.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.txtItemCode.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.txtItemCode.Value = "= Fields.ItemCode";
            // 
            // textBox1
            // 
            this.textBox1.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.041D), Telerik.Reporting.Drawing.Unit.Inch(0.227D));
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.701D), Telerik.Reporting.Drawing.Unit.Inch(0.354D));
            this.textBox1.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.None;
            this.textBox1.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox1.Style.Font.Bold = true;
            this.textBox1.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(6.1D);
            this.textBox1.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox1.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.textBox1.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Top;
            this.textBox1.Value = "= Fields.ItemName";
            // 
            // PartLabelRpt
            // 
            this.Attributes = null;
            this.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.detail});
            this.Name = "PartLabelRpt";
            this.PageSettings.ContinuousPaper = false;
            this.PageSettings.Landscape = false;
            this.PageSettings.Margins = new Telerik.Reporting.Drawing.MarginsU(Telerik.Reporting.Drawing.Unit.Mm(0D), Telerik.Reporting.Drawing.Unit.Mm(0D), Telerik.Reporting.Drawing.Unit.Mm(0D), Telerik.Reporting.Drawing.Unit.Mm(0D));
            this.PageSettings.PaperKind = System.Drawing.Printing.PaperKind.Custom;
            this.PageSettings.PaperSize = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Mm(34D), Telerik.Reporting.Drawing.Unit.Mm(20D));
            this.Style.Font.Name = "Segoe UI";
            this.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8.25D);
            styleRule1.Selectors.AddRange(new Telerik.Reporting.Drawing.ISelector[] {
            new Telerik.Reporting.Drawing.TypeSelector(typeof(Telerik.Reporting.TextItemBase)),
            new Telerik.Reporting.Drawing.TypeSelector(typeof(Telerik.Reporting.HtmlTextBox))});
            styleRule1.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(2D);
            styleRule1.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.StyleSheet.AddRange(new Telerik.Reporting.Drawing.StyleRule[] {
            styleRule1});
            this.UnitOfMeasure = Telerik.Reporting.Drawing.UnitType.Mm;
            this.Width = Telerik.Reporting.Drawing.Unit.Inch(1.346D);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }
        #endregion
        private Telerik.Reporting.DetailSection detail;
        private Telerik.Reporting.Barcode barcode;
        private Telerik.Reporting.TextBox txtSerialNo;
        private Telerik.Reporting.TextBox txtItemCode;
        private Telerik.Reporting.TextBox textBox1;
    }
}