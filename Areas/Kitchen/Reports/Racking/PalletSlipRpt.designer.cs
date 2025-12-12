namespace Corno.Web.Areas.Kitchen.Reports.Racking
{
    partial class PalletSlipRpt
    {
        #region Component Designer generated code
        /// <summary>
        /// Required method for telerik Reporting designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Telerik.Reporting.TableGroup tableGroup1 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup2 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup3 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup4 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup5 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup6 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup7 = new Telerik.Reporting.TableGroup();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PalletSlipRpt));
            Telerik.Reporting.Group group1 = new Telerik.Reporting.Group();
            Telerik.Reporting.Group group2 = new Telerik.Reporting.Group();
            Telerik.Reporting.ReportParameter reportParameter1 = new Telerik.Reporting.ReportParameter();
            this.groupFooterSection = new Telerik.Reporting.GroupFooterSection();
            this.groupHeaderSection = new Telerik.Reporting.GroupHeaderSection();
            this.textBox75 = new Telerik.Reporting.TextBox();
            this.textBox73 = new Telerik.Reporting.TextBox();
            this.shape5 = new Telerik.Reporting.Shape();
            this.groupFooterSection1 = new Telerik.Reporting.GroupFooterSection();
            this.groupHeaderSection1 = new Telerik.Reporting.GroupHeaderSection();
            this.table = new Telerik.Reporting.Table();
            this.textBox41 = new Telerik.Reporting.TextBox();
            this.textBox42 = new Telerik.Reporting.TextBox();
            this.textBox47 = new Telerik.Reporting.TextBox();
            this.textBox52 = new Telerik.Reporting.TextBox();
            this.textBox55 = new Telerik.Reporting.TextBox();
            this.textBox56 = new Telerik.Reporting.TextBox();
            this.textBox57 = new Telerik.Reporting.TextBox();
            this.textBox58 = new Telerik.Reporting.TextBox();
            this.textBox77 = new Telerik.Reporting.TextBox();
            this.textBox43 = new Telerik.Reporting.TextBox();
            this.textBox1 = new Telerik.Reporting.TextBox();
            this.textBox2 = new Telerik.Reporting.TextBox();
            this.sdsPalletNo = new Telerik.Reporting.SqlDataSource();
            this.sdsMain = new Telerik.Reporting.SqlDataSource();
            this.pageHeaderSection1 = new Telerik.Reporting.PageHeaderSection();
            this.textBox7 = new Telerik.Reporting.TextBox();
            this.detail = new Telerik.Reporting.DetailSection();
            this.textBox4 = new Telerik.Reporting.TextBox();
            this.textBox28 = new Telerik.Reporting.TextBox();
            this.pageFooterSection1 = new Telerik.Reporting.PageFooterSection();
            this.textBox3 = new Telerik.Reporting.TextBox();
            this.textBox5 = new Telerik.Reporting.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // groupFooterSection
            // 
            this.groupFooterSection.Height = Telerik.Reporting.Drawing.Unit.Inch(0.052D);
            this.groupFooterSection.Name = "groupFooterSection";
            this.groupFooterSection.PageBreak = Telerik.Reporting.PageBreak.After;
            this.groupFooterSection.PrintAtBottom = true;
            this.groupFooterSection.Style.Visible = false;
            // 
            // groupHeaderSection
            // 
            this.groupHeaderSection.Height = Telerik.Reporting.Drawing.Unit.Inch(0.3D);
            this.groupHeaderSection.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox75,
            this.textBox73,
            this.shape5});
            this.groupHeaderSection.Name = "groupHeaderSection";
            // 
            // textBox75
            // 
            this.textBox75.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Cm(0D), Telerik.Reporting.Drawing.Unit.Cm(0D));
            this.textBox75.Name = "textBox75";
            this.textBox75.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Cm(2.032D), Telerik.Reporting.Drawing.Unit.Cm(0.508D));
            this.textBox75.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox75.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox75.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.textBox75.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox75.Value = "Pallet No. :";
            // 
            // textBox73
            // 
            this.textBox73.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Cm(2.032D), Telerik.Reporting.Drawing.Unit.Cm(0D));
            this.textBox73.Name = "textBox73";
            this.textBox73.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Cm(4.826D), Telerik.Reporting.Drawing.Unit.Cm(0.508D));
            this.textBox73.Style.Font.Bold = true;
            this.textBox73.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox73.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox73.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.textBox73.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox73.Value = "=Fields.PalletNo";
            // 
            // shape5
            // 
            this.shape5.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(0.2D));
            this.shape5.Name = "shape5";
            this.shape5.ShapeType = new Telerik.Reporting.Drawing.Shapes.LineShape(Telerik.Reporting.Drawing.Shapes.LineDirection.EW);
            this.shape5.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(7.2D), Telerik.Reporting.Drawing.Unit.Inch(0.1D));
            this.shape5.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            // 
            // groupFooterSection1
            // 
            this.groupFooterSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(0.052D);
            this.groupFooterSection1.Name = "groupFooterSection1";
            this.groupFooterSection1.Style.Visible = false;
            // 
            // groupHeaderSection1
            // 
            this.groupHeaderSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(0.9D);
            this.groupHeaderSection1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.table,
            this.textBox1,
            this.textBox2,
            this.textBox5,
            this.textBox3});
            this.groupHeaderSection1.Name = "groupHeaderSection1";
            // 
            // table
            // 
            this.table.Bindings.Add(new Telerik.Reporting.Binding("DataSource", "=ReportItem.DataObject"));
            this.table.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.699D)));
            this.table.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.6D)));
            this.table.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.6D)));
            this.table.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.6D)));
            this.table.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.6D)));
            this.table.Body.Rows.Add(new Telerik.Reporting.TableBodyRow(Telerik.Reporting.Drawing.Unit.Inch(0.402D)));
            this.table.Body.Rows.Add(new Telerik.Reporting.TableBodyRow(Telerik.Reporting.Drawing.Unit.Cm(0.478D)));
            this.table.Body.SetCellContent(0, 1, this.textBox41);
            this.table.Body.SetCellContent(1, 1, this.textBox42);
            this.table.Body.SetCellContent(0, 0, this.textBox47);
            this.table.Body.SetCellContent(1, 0, this.textBox52);
            this.table.Body.SetCellContent(0, 3, this.textBox55);
            this.table.Body.SetCellContent(1, 3, this.textBox56);
            this.table.Body.SetCellContent(0, 4, this.textBox57);
            this.table.Body.SetCellContent(1, 4, this.textBox58);
            this.table.Body.SetCellContent(0, 2, this.textBox77);
            this.table.Body.SetCellContent(1, 2, this.textBox43);
            tableGroup1.Name = "Group5";
            tableGroup2.Name = "Group3";
            tableGroup3.Name = "Group4";
            tableGroup4.Name = "Group8";
            tableGroup5.Name = "Group9";
            this.table.ColumnGroups.Add(tableGroup1);
            this.table.ColumnGroups.Add(tableGroup2);
            this.table.ColumnGroups.Add(tableGroup3);
            this.table.ColumnGroups.Add(tableGroup4);
            this.table.ColumnGroups.Add(tableGroup5);
            this.table.ColumnHeadersPrintOnEveryPage = true;
            this.table.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox47,
            this.textBox41,
            this.textBox77,
            this.textBox55,
            this.textBox57,
            this.textBox52,
            this.textBox42,
            this.textBox43,
            this.textBox56,
            this.textBox58});
            this.table.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(0.302D));
            this.table.Name = "table";
            tableGroup6.Name = "Group2";
            tableGroup7.Groupings.Add(new Telerik.Reporting.Grouping(null));
            tableGroup7.Name = "DetailGroup";
            this.table.RowGroups.Add(tableGroup6);
            this.table.RowGroups.Add(tableGroup7);
            this.table.RowHeadersPrintOnEveryPage = true;
            this.table.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(7.099D), Telerik.Reporting.Drawing.Unit.Cm(1.5D));
            this.table.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.table.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            // 
            // textBox41
            // 
            this.textBox41.Name = "textBox41";
            this.textBox41.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.6D), Telerik.Reporting.Drawing.Unit.Inch(0.402D));
            this.textBox41.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox41.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox41.Style.Font.Bold = true;
            this.textBox41.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9D);
            this.textBox41.Style.Padding.Bottom = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox41.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Inch(0D);
            this.textBox41.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Inch(0D);
            this.textBox41.Style.Padding.Top = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox41.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox41.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox41.StyleName = "";
            this.textBox41.Value = "Carton No";
            // 
            // textBox42
            // 
            this.textBox42.Format = "{0:N3}";
            this.textBox42.Name = "textBox42";
            this.textBox42.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.6D), Telerik.Reporting.Drawing.Unit.Cm(0.478D));
            this.textBox42.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox42.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox42.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9D);
            this.textBox42.Style.Padding.Bottom = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox42.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox42.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox42.Style.Padding.Top = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox42.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox42.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox42.StyleName = "";
            this.textBox42.Value = "= \"C\" + Fields.CartonNo";
            // 
            // textBox47
            // 
            this.textBox47.Name = "textBox47";
            this.textBox47.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.699D), Telerik.Reporting.Drawing.Unit.Inch(0.402D));
            this.textBox47.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox47.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox47.Style.Font.Bold = true;
            this.textBox47.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9D);
            this.textBox47.Style.Padding.Bottom = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox47.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Inch(0D);
            this.textBox47.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Inch(0D);
            this.textBox47.Style.Padding.Top = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox47.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox47.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox47.StyleName = "";
            this.textBox47.Value = "Sr. No.";
            // 
            // textBox52
            // 
            this.textBox52.Format = "{0:N0}";
            this.textBox52.Name = "textBox52";
            this.textBox52.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.699D), Telerik.Reporting.Drawing.Unit.Cm(0.478D));
            this.textBox52.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox52.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox52.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9D);
            this.textBox52.Style.Padding.Bottom = Telerik.Reporting.Drawing.Unit.Inch(0D);
            this.textBox52.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox52.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox52.Style.Padding.Top = Telerik.Reporting.Drawing.Unit.Inch(0D);
            this.textBox52.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox52.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox52.StyleName = "";
            this.textBox52.Value = "= RowNumber()";
            // 
            // textBox55
            // 
            this.textBox55.Name = "textBox55";
            this.textBox55.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.6D), Telerik.Reporting.Drawing.Unit.Inch(0.402D));
            this.textBox55.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox55.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox55.Style.Font.Bold = true;
            this.textBox55.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9D);
            this.textBox55.Style.Padding.Bottom = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox55.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Inch(0D);
            this.textBox55.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Inch(0D);
            this.textBox55.Style.Padding.Top = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox55.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox55.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox55.StyleName = "";
            this.textBox55.Value = "Theoritical Weight";
            // 
            // textBox56
            // 
            this.textBox56.Format = "{0:N0}";
            this.textBox56.Name = "textBox56";
            this.textBox56.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.6D), Telerik.Reporting.Drawing.Unit.Cm(0.478D));
            this.textBox56.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox56.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox56.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9D);
            this.textBox56.Style.Padding.Bottom = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox56.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox56.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox56.Style.Padding.Top = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox56.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox56.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox56.StyleName = "";
            this.textBox56.Value = "= Fields.SystemWeight";
            // 
            // textBox57
            // 
            this.textBox57.Name = "textBox57";
            this.textBox57.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.6D), Telerik.Reporting.Drawing.Unit.Inch(0.402D));
            this.textBox57.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox57.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox57.Style.Font.Bold = true;
            this.textBox57.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9D);
            this.textBox57.Style.Padding.Bottom = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox57.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Inch(0D);
            this.textBox57.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Inch(0D);
            this.textBox57.Style.Padding.Top = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox57.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox57.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox57.StyleName = "";
            this.textBox57.Value = "Total Quantity";
            // 
            // textBox58
            // 
            this.textBox58.Format = "{0:N0}";
            this.textBox58.Name = "textBox58";
            this.textBox58.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.6D), Telerik.Reporting.Drawing.Unit.Cm(0.478D));
            this.textBox58.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox58.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox58.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9D);
            this.textBox58.Style.Padding.Bottom = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox58.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox58.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox58.Style.Padding.Top = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox58.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox58.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox58.StyleName = "";
            this.textBox58.Value = "= Fields.TotalQuantity";
            // 
            // textBox77
            // 
            this.textBox77.Name = "textBox77";
            this.textBox77.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.6D), Telerik.Reporting.Drawing.Unit.Inch(0.402D));
            this.textBox77.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox77.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox77.Style.Font.Bold = true;
            this.textBox77.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9D);
            this.textBox77.Style.Padding.Bottom = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox77.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Inch(0D);
            this.textBox77.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Inch(0D);
            this.textBox77.Style.Padding.Top = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox77.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox77.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox77.StyleName = "";
            this.textBox77.Value = "Actual Weight ";
            // 
            // textBox43
            // 
            this.textBox43.Format = "{0:N3}";
            this.textBox43.Name = "textBox43";
            this.textBox43.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.6D), Telerik.Reporting.Drawing.Unit.Cm(0.478D));
            this.textBox43.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox43.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox43.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9D);
            this.textBox43.Style.Padding.Bottom = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox43.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox43.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox43.Style.Padding.Top = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox43.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox43.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox43.StyleName = "";
            this.textBox43.Value = "= Fields.NetWeight";
            // 
            // textBox1
            // 
            this.textBox1.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Cm(2.794D), Telerik.Reporting.Drawing.Unit.Cm(0.254D));
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Cm(6.35D), Telerik.Reporting.Drawing.Unit.Cm(0.508D));
            this.textBox1.Style.Font.Bold = true;
            this.textBox1.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox1.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox1.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.textBox1.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox1.Value = "= Fields.WarehouseOrderNo";
            // 
            // textBox2
            // 
            this.textBox2.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Cm(0D), Telerik.Reporting.Drawing.Unit.Cm(0.254D));
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Cm(2.794D), Telerik.Reporting.Drawing.Unit.Cm(0.508D));
            this.textBox2.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox2.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox2.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.textBox2.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox2.Value = "WH Order No. :";
            // 
            // sdsPalletNo
            // 
            this.sdsPalletNo.ConnectionString = "CornoContext";
            this.sdsPalletNo.Name = "sdsPalletNo";
            this.sdsPalletNo.SelectCommand = resources.GetString("sdsPalletNo.SelectCommand");
            // 
            // sdsMain
            // 
            this.sdsMain.ConnectionString = "CornoContext";
            this.sdsMain.Name = "sdsMain";
            this.sdsMain.Parameters.Add(new Telerik.Reporting.SqlDataSourceParameter("@PalletNo", System.Data.DbType.String, "= Parameters.PalletNo.Value"));
            this.sdsMain.SelectCommand = resources.GetString("sdsMain.SelectCommand");
            // 
            // pageHeaderSection1
            // 
            this.pageHeaderSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(0.5D);
            this.pageHeaderSection1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox7});
            this.pageHeaderSection1.Name = "pageHeaderSection1";
            // 
            // textBox7
            // 
            this.textBox7.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(0D));
            this.textBox7.Name = "textBox7";
            this.textBox7.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(7.2D), Telerik.Reporting.Drawing.Unit.Inch(0.5D));
            this.textBox7.Style.Font.Bold = true;
            this.textBox7.Style.Font.Italic = false;
            this.textBox7.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(20D);
            this.textBox7.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox7.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox7.Value = "Pallet Slip";
            // 
            // detail
            // 
            this.detail.Height = Telerik.Reporting.Drawing.Unit.Inch(0.035D);
            this.detail.Name = "detail";
            this.detail.Style.Visible = true;
            // 
            // textBox4
            // 
            this.textBox4.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.1D), Telerik.Reporting.Drawing.Unit.Inch(0D));
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.4D), Telerik.Reporting.Drawing.Unit.Inch(0.2D));
            this.textBox4.Style.Font.Name = "Arial";
            this.textBox4.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.textBox4.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox4.Value = "=Now()";
            // 
            // textBox28
            // 
            this.textBox28.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(5.4D), Telerik.Reporting.Drawing.Unit.Inch(0D));
            this.textBox28.Name = "textBox28";
            this.textBox28.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.8D), Telerik.Reporting.Drawing.Unit.Inch(0.2D));
            this.textBox28.Style.Font.Name = "Arial";
            this.textBox28.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.textBox28.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox28.Value = "Page: {PageNumber}";
            // 
            // pageFooterSection1
            // 
            this.pageFooterSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(0.2D);
            this.pageFooterSection1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox28,
            this.textBox4});
            this.pageFooterSection1.Name = "pageFooterSection1";
            // 
            // textBox3
            // 
            this.textBox3.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Cm(11.9D), Telerik.Reporting.Drawing.Unit.Cm(0.268D));
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Cm(1.6D), Telerik.Reporting.Drawing.Unit.Cm(0.5D));
            this.textBox3.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox3.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox3.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.textBox3.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox3.Value = "So. No. :";
            // 
            // textBox5
            // 
            this.textBox5.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Cm(13.5D), Telerik.Reporting.Drawing.Unit.Cm(0.268D));
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Cm(4.788D), Telerik.Reporting.Drawing.Unit.Cm(0.5D));
            this.textBox5.Style.Font.Bold = true;
            this.textBox5.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox5.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox5.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.textBox5.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox5.Value = "= Fields.SoNo";
            // 
            // PalletSlipRpt
            // 
            this.DataSource = this.sdsMain;
            group1.GroupFooter = this.groupFooterSection;
            group1.GroupHeader = this.groupHeaderSection;
            group1.Groupings.Add(new Telerik.Reporting.Grouping("= Fields.PalletNo"));
            group1.Name = "grpPalletNo";
            group2.GroupFooter = this.groupFooterSection1;
            group2.GroupHeader = this.groupHeaderSection1;
            group2.Groupings.Add(new Telerik.Reporting.Grouping("= Fields.WarehouseOrderNo"));
            group2.Name = "grpWHOrderNo";
            this.Groups.AddRange(new Telerik.Reporting.Group[] {
            group1,
            group2});
            this.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.groupHeaderSection,
            this.groupFooterSection,
            this.groupHeaderSection1,
            this.groupFooterSection1,
            this.pageHeaderSection1,
            this.detail,
            this.pageFooterSection1});
            this.Name = "PalletSlip";
            this.PageSettings.ContinuousPaper = false;
            this.PageSettings.Landscape = false;
            this.PageSettings.Margins = new Telerik.Reporting.Drawing.MarginsU(Telerik.Reporting.Drawing.Unit.Inch(0.5D), Telerik.Reporting.Drawing.Unit.Inch(0.5D), Telerik.Reporting.Drawing.Unit.Inch(0.2D), Telerik.Reporting.Drawing.Unit.Inch(0.2D));
            this.PageSettings.PaperKind = System.Drawing.Printing.PaperKind.A4;
            reportParameter1.AvailableValues.DataSource = this.sdsPalletNo;
            reportParameter1.AvailableValues.DisplayMember = "= Fields.PalletNo";
            reportParameter1.AvailableValues.ValueMember = "= Fields.PalletNo";
            reportParameter1.Name = "PalletNo";
            reportParameter1.Text = "Pallet No";
            reportParameter1.Value = "= Fields.PalletNo";
            reportParameter1.Visible = true;
            this.ReportParameters.Add(reportParameter1);
            this.Width = Telerik.Reporting.Drawing.Unit.Inch(7.2D);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }
        #endregion

        private Telerik.Reporting.SqlDataSource sdsMain;
        private Telerik.Reporting.PageHeaderSection pageHeaderSection1;
        private Telerik.Reporting.TextBox textBox7;
        private Telerik.Reporting.DetailSection detail;
        private Telerik.Reporting.TextBox textBox75;
        private Telerik.Reporting.TextBox textBox73;
        private Telerik.Reporting.Shape shape5;
        private Telerik.Reporting.Table table;
        private Telerik.Reporting.TextBox textBox41;
        private Telerik.Reporting.TextBox textBox42;
        private Telerik.Reporting.TextBox textBox47;
        private Telerik.Reporting.TextBox textBox52;
        private Telerik.Reporting.TextBox textBox55;
        private Telerik.Reporting.TextBox textBox56;
        private Telerik.Reporting.TextBox textBox57;
        private Telerik.Reporting.TextBox textBox58;
        private Telerik.Reporting.TextBox textBox77;
        private Telerik.Reporting.TextBox textBox43;
        private Telerik.Reporting.TextBox textBox4;
        private Telerik.Reporting.GroupHeaderSection groupHeaderSection;
        private Telerik.Reporting.GroupFooterSection groupFooterSection;
        private Telerik.Reporting.TextBox textBox28;
        private Telerik.Reporting.PageFooterSection pageFooterSection1;
        private Telerik.Reporting.SqlDataSource sdsPalletNo;
        private Telerik.Reporting.TextBox textBox2;
        private Telerik.Reporting.TextBox textBox1;
        private Telerik.Reporting.GroupHeaderSection groupHeaderSection1;
        private Telerik.Reporting.GroupFooterSection groupFooterSection1;
        private Telerik.Reporting.TextBox textBox5;
        private Telerik.Reporting.TextBox textBox3;
    }
}