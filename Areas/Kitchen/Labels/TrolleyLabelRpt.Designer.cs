namespace Corno.Web.Areas.Kitchen.Labels
{
    partial class TrolleyLabelRpt
    {
        #region Component Designer generated code
        /// <summary>
        /// Required method for telerik Reporting designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Telerik.Reporting.Drawing.StyleRule styleRule1 = new Telerik.Reporting.Drawing.StyleRule();
            this.detail = new Telerik.Reporting.DetailSection();
            this.txtWarehousePosition = new Telerik.Reporting.TextBox();
            this.txtWarehouseOrderNo = new Telerik.Reporting.TextBox();
            this.textBox1 = new Telerik.Reporting.TextBox();
            this.textBox2 = new Telerik.Reporting.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // detail
            // 
            this.detail.Height = Telerik.Reporting.Drawing.Unit.Inch(4D);
            this.detail.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.txtWarehousePosition,
            this.txtWarehouseOrderNo,
            this.textBox1,
            this.textBox2});
            this.detail.Name = "detail";
            this.detail.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.None;
            this.detail.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.detail.Style.Font.Bold = true;
            this.detail.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(12D);
            // 
            // txtWarehousePosition
            // 
            this.txtWarehousePosition.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.1D), Telerik.Reporting.Drawing.Unit.Inch(1D));
            this.txtWarehousePosition.Name = "txtWarehousePosition";
            this.txtWarehousePosition.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(3.2D), Telerik.Reporting.Drawing.Unit.Inch(0.7D));
            this.txtWarehousePosition.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.None;
            this.txtWarehousePosition.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.txtWarehousePosition.Style.Font.Bold = true;
            this.txtWarehousePosition.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(42D);
            this.txtWarehousePosition.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.txtWarehousePosition.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.txtWarehousePosition.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.txtWarehousePosition.Value = "= Fields.CarcassCode";
            // 
            // txtWarehouseOrderNo
            // 
            this.txtWarehouseOrderNo.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.1D), Telerik.Reporting.Drawing.Unit.Inch(0.1D));
            this.txtWarehouseOrderNo.Name = "txtWarehouseOrderNo";
            this.txtWarehouseOrderNo.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(5.8D), Telerik.Reporting.Drawing.Unit.Inch(0.9D));
            this.txtWarehouseOrderNo.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.None;
            this.txtWarehouseOrderNo.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.txtWarehouseOrderNo.Style.Font.Bold = true;
            this.txtWarehouseOrderNo.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(48D);
            this.txtWarehouseOrderNo.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.txtWarehouseOrderNo.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.txtWarehouseOrderNo.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.txtWarehouseOrderNo.Value = "= Fields.SoNo";
            // 
            // textBox1
            // 
            this.textBox1.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(3.4D), Telerik.Reporting.Drawing.Unit.Inch(1D));
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.5D), Telerik.Reporting.Drawing.Unit.Inch(0.7D));
            this.textBox1.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.None;
            this.textBox1.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox1.Style.Font.Bold = true;
            this.textBox1.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(42D);
            this.textBox1.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox1.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.textBox1.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox1.Value = "= \"(\" + Fields.SerialNo + \")\"";
            // 
            // textBox2
            // 
            this.textBox2.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.1D), Telerik.Reporting.Drawing.Unit.Inch(1.7D));
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(5.8D), Telerik.Reporting.Drawing.Unit.Inch(2.2D));
            this.textBox2.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.None;
            this.textBox2.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox2.Style.Font.Bold = true;
            this.textBox2.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(34D);
            this.textBox2.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox2.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.textBox2.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox2.Value = "= Fields.BaanItemCode";
            // 
            // TrolleyLabelRpt
            // 
            this.Attributes = null;
            this.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.detail});
            this.Name = "TrolleyLabelRpt";
            this.PageSettings.ContinuousPaper = false;
            this.PageSettings.Landscape = true;
            this.PageSettings.Margins = new Telerik.Reporting.Drawing.MarginsU(Telerik.Reporting.Drawing.Unit.Mm(0D), Telerik.Reporting.Drawing.Unit.Mm(0D), Telerik.Reporting.Drawing.Unit.Mm(0D), Telerik.Reporting.Drawing.Unit.Mm(0D));
            this.PageSettings.PaperKind = System.Drawing.Printing.PaperKind.Custom;
            this.PageSettings.PaperSize = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(4D), Telerik.Reporting.Drawing.Unit.Inch(6D));
            this.Style.Font.Name = "Segoe UI";
            this.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8.25D);
            styleRule1.Selectors.AddRange(new Telerik.Reporting.Drawing.ISelector[] {
            new Telerik.Reporting.Drawing.TypeSelector(typeof(Telerik.Reporting.TextItemBase)),
            new Telerik.Reporting.Drawing.TypeSelector(typeof(Telerik.Reporting.HtmlTextBox))});
            styleRule1.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(2D);
            styleRule1.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.StyleSheet.AddRange(new Telerik.Reporting.Drawing.StyleRule[] {
            styleRule1});
            this.Width = Telerik.Reporting.Drawing.Unit.Inch(6D);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }
        #endregion
        private Telerik.Reporting.DetailSection detail;
        private Telerik.Reporting.TextBox txtWarehousePosition;
        private Telerik.Reporting.TextBox txtWarehouseOrderNo;
        private Telerik.Reporting.TextBox textBox1;
        private Telerik.Reporting.TextBox textBox2;
    }
}