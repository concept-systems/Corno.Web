namespace Corno.Web.Areas.Kitchen.Labels
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
            this.txtWarehousePosition = new Telerik.Reporting.TextBox();
            this.txtItemName = new Telerik.Reporting.TextBox();
            this.txtSerialNo = new Telerik.Reporting.TextBox();
            this.txtItemCode = new Telerik.Reporting.TextBox();
            this.txtWarehouseOrderNo = new Telerik.Reporting.TextBox();
            this.textBox1 = new Telerik.Reporting.TextBox();
            this.txtDuplicate = new Telerik.Reporting.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // detail
            // 
            this.detail.Height = Telerik.Reporting.Drawing.Unit.Inch(0.984D);
            this.detail.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.barcode,
            this.txtWarehousePosition,
            this.txtItemName,
            this.txtSerialNo,
            this.txtItemCode,
            this.txtWarehouseOrderNo,
            this.textBox1,
            this.txtDuplicate});
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
            this.barcode.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(1.26D), Telerik.Reporting.Drawing.Unit.Inch(0.039D));
            this.barcode.Name = "barcode";
            this.barcode.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.669D), Telerik.Reporting.Drawing.Unit.Inch(0.67D));
            this.barcode.Stretch = true;
            this.barcode.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.None;
            this.barcode.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.barcode.Style.Font.Bold = true;
            this.barcode.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.barcode.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.barcode.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Bottom;
            this.barcode.Value = "=Fields.Barcode";
            // 
            // txtWarehousePosition
            // 
            this.txtWarehousePosition.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.039D), Telerik.Reporting.Drawing.Unit.Inch(0.394D));
            this.txtWarehousePosition.Name = "txtWarehousePosition";
            this.txtWarehousePosition.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.661D), Telerik.Reporting.Drawing.Unit.Inch(0.157D));
            this.txtWarehousePosition.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.None;
            this.txtWarehousePosition.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.txtWarehousePosition.Style.Font.Bold = true;
            this.txtWarehousePosition.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.txtWarehousePosition.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.txtWarehousePosition.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.txtWarehousePosition.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.txtWarehousePosition.Value = "= Fields.Position + \"(\" + Fields.Reserved1 + \")\"";
            // 
            // txtItemName
            // 
            this.txtItemName.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.039D), Telerik.Reporting.Drawing.Unit.Inch(0.709D));
            this.txtItemName.Name = "txtItemName";
            this.txtItemName.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.89D), Telerik.Reporting.Drawing.Unit.Inch(0.236D));
            this.txtItemName.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.None;
            this.txtItemName.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.txtItemName.Style.Font.Bold = true;
            this.txtItemName.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(6.25D);
            this.txtItemName.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.txtItemName.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.txtItemName.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.txtItemName.Value = "= Fields.NotMapped.ItemName";
            // 
            // txtSerialNo
            // 
            this.txtSerialNo.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.039D), Telerik.Reporting.Drawing.Unit.Inch(0.551D));
            this.txtSerialNo.Name = "txtSerialNo";
            this.txtSerialNo.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.221D), Telerik.Reporting.Drawing.Unit.Inch(0.157D));
            this.txtSerialNo.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.None;
            this.txtSerialNo.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.txtSerialNo.Style.Font.Bold = true;
            this.txtSerialNo.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.txtSerialNo.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.txtSerialNo.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.txtSerialNo.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.txtSerialNo.Value = "= Fields.Barcode";
            // 
            // txtItemCode
            // 
            this.txtItemCode.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.039D), Telerik.Reporting.Drawing.Unit.Inch(0.039D));
            this.txtItemCode.Name = "txtItemCode";
            this.txtItemCode.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.221D), Telerik.Reporting.Drawing.Unit.Inch(0.197D));
            this.txtItemCode.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.None;
            this.txtItemCode.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.txtItemCode.Style.Font.Bold = true;
            this.txtItemCode.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(7.25D);
            this.txtItemCode.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.txtItemCode.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.txtItemCode.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.txtItemCode.Value = "= Fields.NotMapped.ItemCode";
            // 
            // txtWarehouseOrderNo
            // 
            this.txtWarehouseOrderNo.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.039D), Telerik.Reporting.Drawing.Unit.Inch(0.236D));
            this.txtWarehouseOrderNo.Name = "txtWarehouseOrderNo";
            this.txtWarehouseOrderNo.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.221D), Telerik.Reporting.Drawing.Unit.Inch(0.157D));
            this.txtWarehouseOrderNo.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.None;
            this.txtWarehouseOrderNo.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.txtWarehouseOrderNo.Style.Font.Bold = true;
            this.txtWarehouseOrderNo.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.txtWarehouseOrderNo.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.txtWarehouseOrderNo.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.txtWarehouseOrderNo.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.txtWarehouseOrderNo.Value = "= Fields.WarehouseOrderNo";
            // 
            // textBox1
            // 
            this.textBox1.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.96D), Telerik.Reporting.Drawing.Unit.Inch(0.393D));
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.3D), Telerik.Reporting.Drawing.Unit.Inch(0.157D));
            this.textBox1.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.None;
            this.textBox1.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox1.Style.Font.Bold = true;
            this.textBox1.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.textBox1.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox1.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.textBox1.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox1.Value = "= Fields.Quantity";
            // 
            // txtDuplicate
            // 
            this.txtDuplicate.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.731D), Telerik.Reporting.Drawing.Unit.Inch(0.4D));
            this.txtDuplicate.Name = "txtDuplicate";
            this.txtDuplicate.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.2D), Telerik.Reporting.Drawing.Unit.Inch(0.147D));
            this.txtDuplicate.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.None;
            this.txtDuplicate.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.txtDuplicate.Style.Color = System.Drawing.Color.DarkRed;
            this.txtDuplicate.Style.Font.Bold = true;
            this.txtDuplicate.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.txtDuplicate.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.txtDuplicate.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.txtDuplicate.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.txtDuplicate.Style.Visible = false;
            this.txtDuplicate.Value = "RP";
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
            this.PageSettings.PaperSize = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Mm(50D), Telerik.Reporting.Drawing.Unit.Mm(25D));
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
            this.Width = Telerik.Reporting.Drawing.Unit.Inch(1.929D);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }
        #endregion
        private Telerik.Reporting.DetailSection detail;
        private Telerik.Reporting.Barcode barcode;
        private Telerik.Reporting.TextBox txtWarehousePosition;
        private Telerik.Reporting.TextBox txtItemName;
        private Telerik.Reporting.TextBox txtSerialNo;
        private Telerik.Reporting.TextBox txtWarehouseOrderNo;
        private Telerik.Reporting.TextBox txtItemCode;
        private Telerik.Reporting.TextBox textBox1;
        private Telerik.Reporting.TextBox txtDuplicate;
    }
}