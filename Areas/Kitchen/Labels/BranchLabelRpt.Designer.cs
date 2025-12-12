namespace Corno.Web.Areas.Kitchen.Labels
{
    partial class BranchLabelRpt
    {
        #region Component Designer generated code
        /// <summary>
        /// Required method for telerik Reporting designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Telerik.Reporting.Barcodes.QRCodeEncoder qrCodeEncoder1 = new Telerik.Reporting.Barcodes.QRCodeEncoder();
            this.detail = new Telerik.Reporting.DetailSection();
            this.txtWhOrderNo = new Telerik.Reporting.TextBox();
            this.txtCartonNo = new Telerik.Reporting.TextBox();
            this.txtWarehouseName = new Telerik.Reporting.TextBox();
            this.txtDuplicate = new Telerik.Reporting.TextBox();
            this.txtLine1 = new Telerik.Reporting.TextBox();
            this.txtLine2 = new Telerik.Reporting.TextBox();
            this.txtOneLineItemCode = new Telerik.Reporting.TextBox();
            this.qrCode = new Telerik.Reporting.Barcode();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // detail
            // 
            this.detail.Height = Telerik.Reporting.Drawing.Unit.Cm(15D);
            this.detail.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.txtWhOrderNo,
            this.txtCartonNo,
            this.txtWarehouseName,
            this.txtDuplicate,
            this.txtLine1,
            this.txtLine2,
            this.txtOneLineItemCode,
            this.qrCode});
            this.detail.Name = "detail";
            this.detail.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.None;
            this.detail.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.detail.Style.Font.Name = "Calibri";
            // 
            // txtWhOrderNo
            // 
            this.txtWhOrderNo.Angle = 90D;
            this.txtWhOrderNo.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Cm(6.9D), Telerik.Reporting.Drawing.Unit.Cm(0.1D));
            this.txtWhOrderNo.Name = "txtWhOrderNo";
            this.txtWhOrderNo.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Cm(1.5D), Telerik.Reporting.Drawing.Unit.Cm(13D));
            this.txtWhOrderNo.Style.Font.Bold = true;
            this.txtWhOrderNo.Style.Font.Name = "Calibri";
            this.txtWhOrderNo.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(35D);
            this.txtWhOrderNo.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.txtWhOrderNo.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.txtWhOrderNo.Value = "= \"SO - \" + Fields.SoNo";
            // 
            // txtCartonNo
            // 
            this.txtCartonNo.Angle = 90D;
            this.txtCartonNo.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Cm(8.4D), Telerik.Reporting.Drawing.Unit.Cm(0.1D));
            this.txtCartonNo.Name = "txtCartonNo";
            this.txtCartonNo.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Cm(1.5D), Telerik.Reporting.Drawing.Unit.Cm(13D));
            this.txtCartonNo.Style.Font.Bold = true;
            this.txtCartonNo.Style.Font.Name = "Calibri";
            this.txtCartonNo.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(35D);
            this.txtCartonNo.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.txtCartonNo.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.txtCartonNo.Value = "= \"Carton No - \" + Fields.CartonNo";
            // 
            // txtWarehouseName
            // 
            this.txtWarehouseName.Angle = 90D;
            this.txtWarehouseName.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Cm(5.4D), Telerik.Reporting.Drawing.Unit.Cm(0.1D));
            this.txtWarehouseName.Name = "txtWarehouseName";
            this.txtWarehouseName.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Cm(1.5D), Telerik.Reporting.Drawing.Unit.Cm(14.8D));
            this.txtWarehouseName.Style.Font.Bold = true;
            this.txtWarehouseName.Style.Font.Name = "Calibri";
            this.txtWarehouseName.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(40D);
            this.txtWarehouseName.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.txtWarehouseName.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.txtWarehouseName.Value = "= Fields.StoreName";
            // 
            // txtDuplicate
            // 
            this.txtDuplicate.Angle = 90D;
            this.txtDuplicate.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(2.756D), Telerik.Reporting.Drawing.Unit.Inch(5.433D));
            this.txtDuplicate.Name = "txtDuplicate";
            this.txtDuplicate.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.2D), Telerik.Reporting.Drawing.Unit.Inch(0.2D));
            this.txtDuplicate.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.txtDuplicate.Style.BorderStyle.Left = Telerik.Reporting.Drawing.BorderType.Solid;
            this.txtDuplicate.Style.BorderStyle.Right = Telerik.Reporting.Drawing.BorderType.Solid;
            this.txtDuplicate.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.txtDuplicate.Style.Font.Bold = true;
            this.txtDuplicate.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.txtDuplicate.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.txtDuplicate.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.txtDuplicate.Style.Visible = false;
            this.txtDuplicate.Value = "RP";
            // 
            // txtLine1
            // 
            this.txtLine1.Angle = 90D;
            this.txtLine1.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Cm(3.9D), Telerik.Reporting.Drawing.Unit.Cm(0.1D));
            this.txtLine1.Name = "txtLine1";
            this.txtLine1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Cm(1.4D), Telerik.Reporting.Drawing.Unit.Cm(14.8D));
            this.txtLine1.Style.Font.Bold = true;
            this.txtLine1.Style.Font.Name = "Calibri";
            this.txtLine1.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(35D);
            this.txtLine1.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.txtLine1.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.txtLine1.Value = "= Fields.Line1";
            // 
            // txtLine2
            // 
            this.txtLine2.Angle = 90D;
            this.txtLine2.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Cm(2.5D), Telerik.Reporting.Drawing.Unit.Cm(0.1D));
            this.txtLine2.Name = "txtLine2";
            this.txtLine2.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Cm(1.4D), Telerik.Reporting.Drawing.Unit.Cm(14.8D));
            this.txtLine2.Style.Font.Bold = true;
            this.txtLine2.Style.Font.Name = "Calibri";
            this.txtLine2.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(35D);
            this.txtLine2.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.txtLine2.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.txtLine2.Value = "= Fields.Line2";
            // 
            // txtOneLineItemCode
            // 
            this.txtOneLineItemCode.Angle = 90D;
            this.txtOneLineItemCode.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Cm(0.2D), Telerik.Reporting.Drawing.Unit.Cm(0.1D));
            this.txtOneLineItemCode.Name = "txtOneLineItemCode";
            this.txtOneLineItemCode.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Cm(2.3D), Telerik.Reporting.Drawing.Unit.Cm(14.8D));
            this.txtOneLineItemCode.Style.Font.Bold = true;
            this.txtOneLineItemCode.Style.Font.Name = "Calibri";
            this.txtOneLineItemCode.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(35D);
            this.txtOneLineItemCode.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.txtOneLineItemCode.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.txtOneLineItemCode.Value = "= Fields.OneLineItemCode";
            // 
            // qrCode
            // 
            this.qrCode.Angle = 0D;
            this.qrCode.Encoder = qrCodeEncoder1;
            this.qrCode.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(2.992D), Telerik.Reporting.Drawing.Unit.Inch(5.157D));
            this.qrCode.Name = "qrCode";
            this.qrCode.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Mm(23D), Telerik.Reporting.Drawing.Unit.Mm(18D));
            this.qrCode.Stretch = true;
            this.qrCode.Value = "= Fields.QrCode";
            // 
            // BranchLabelRpt
            // 
            this.Attributes = null;
            this.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.detail});
            this.Name = "BranchLabelReport";
            this.PageSettings.ContinuousPaper = false;
            this.PageSettings.Landscape = false;
            this.PageSettings.Margins = new Telerik.Reporting.Drawing.MarginsU(Telerik.Reporting.Drawing.Unit.Cm(0D), Telerik.Reporting.Drawing.Unit.Cm(0D), Telerik.Reporting.Drawing.Unit.Cm(0D), Telerik.Reporting.Drawing.Unit.Cm(0D));
            this.PageSettings.PaperKind = System.Drawing.Printing.PaperKind.Custom;
            this.PageSettings.PaperSize = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(4D), Telerik.Reporting.Drawing.Unit.Inch(6D));
            this.Width = Telerik.Reporting.Drawing.Unit.Cm(10D);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }
        #endregion

        private Telerik.Reporting.DetailSection detail;
        private Telerik.Reporting.TextBox txtWhOrderNo;
        private Telerik.Reporting.TextBox txtCartonNo;
        private Telerik.Reporting.TextBox txtWarehouseName;
        private Telerik.Reporting.TextBox txtDuplicate;
        private Telerik.Reporting.TextBox txtLine1;
        private Telerik.Reporting.TextBox txtLine2;
        private Telerik.Reporting.TextBox txtOneLineItemCode;
        private Telerik.Reporting.Barcode qrCode;
    }
}