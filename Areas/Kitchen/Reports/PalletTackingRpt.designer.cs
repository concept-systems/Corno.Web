namespace Corno.Web.Areas.Kitchen.Reports
{
    partial class PalletTrackingRpt
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
            Telerik.Reporting.TableGroup tableGroup8 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.Group group1 = new Telerik.Reporting.Group();
            Telerik.Reporting.ReportParameter reportParameter1 = new Telerik.Reporting.ReportParameter();
            this.groupFooterSection = new Telerik.Reporting.GroupFooterSection();
            this.groupHeaderSection = new Telerik.Reporting.GroupHeaderSection();
            this.table = new Telerik.Reporting.Table();
            this.textBox35 = new Telerik.Reporting.TextBox();
            this.textBox36 = new Telerik.Reporting.TextBox();
            this.textBox45 = new Telerik.Reporting.TextBox();
            this.textBox46 = new Telerik.Reporting.TextBox();
            this.textBox55 = new Telerik.Reporting.TextBox();
            this.textBox56 = new Telerik.Reporting.TextBox();
            this.textBox3 = new Telerik.Reporting.TextBox();
            this.textBox5 = new Telerik.Reporting.TextBox();
            this.textBox6 = new Telerik.Reporting.TextBox();
            this.textBox8 = new Telerik.Reporting.TextBox();
            this.textBox9 = new Telerik.Reporting.TextBox();
            this.textBox10 = new Telerik.Reporting.TextBox();
            this.textBox75 = new Telerik.Reporting.TextBox();
            this.textBox73 = new Telerik.Reporting.TextBox();
            this.shape5 = new Telerik.Reporting.Shape();
            this.textBox2 = new Telerik.Reporting.TextBox();
            this.textBox1 = new Telerik.Reporting.TextBox();
            this.textBox11 = new Telerik.Reporting.TextBox();
            this.textBox12 = new Telerik.Reporting.TextBox();
            this.pageHeaderSection1 = new Telerik.Reporting.PageHeaderSection();
            this.textBox7 = new Telerik.Reporting.TextBox();
            this.detail = new Telerik.Reporting.DetailSection();
            this.textBox4 = new Telerik.Reporting.TextBox();
            this.textBox28 = new Telerik.Reporting.TextBox();
            this.pageFooterSection1 = new Telerik.Reporting.PageFooterSection();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // groupFooterSection
            // 
            this.groupFooterSection.Height = Telerik.Reporting.Drawing.Unit.Inch(0.052D);
            this.groupFooterSection.Name = "groupFooterSection";
            this.groupFooterSection.PageBreak = Telerik.Reporting.PageBreak.After;
            this.groupFooterSection.PrintAtBottom = true;
            // 
            // groupHeaderSection
            // 
            this.groupHeaderSection.Height = Telerik.Reporting.Drawing.Unit.Inch(0.814D);
            this.groupHeaderSection.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.table,
            this.textBox75,
            this.textBox73,
            this.shape5,
            this.textBox2,
            this.textBox1,
            this.textBox11,
            this.textBox12});
            this.groupHeaderSection.Name = "groupHeaderSection";
            // 
            // table
            // 
            this.table.Bindings.Add(new Telerik.Reporting.Binding("DataSource", "=ReportItem.DataObject"));
            this.table.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Cm(2D)));
            this.table.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(2.8D)));
            this.table.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1D)));
            this.table.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1D)));
            this.table.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1D)));
            this.table.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.2D)));
            this.table.Body.Rows.Add(new Telerik.Reporting.TableBodyRow(Telerik.Reporting.Drawing.Unit.Inch(0.3D)));
            this.table.Body.Rows.Add(new Telerik.Reporting.TableBodyRow(Telerik.Reporting.Drawing.Unit.Cm(0.476D)));
            this.table.Body.SetCellContent(1, 3, this.textBox35);
            this.table.Body.SetCellContent(0, 3, this.textBox36);
            this.table.Body.SetCellContent(0, 2, this.textBox45);
            this.table.Body.SetCellContent(1, 2, this.textBox46);
            this.table.Body.SetCellContent(0, 4, this.textBox55);
            this.table.Body.SetCellContent(1, 4, this.textBox56);
            this.table.Body.SetCellContent(0, 1, this.textBox3);
            this.table.Body.SetCellContent(1, 1, this.textBox5);
            this.table.Body.SetCellContent(0, 5, this.textBox6);
            this.table.Body.SetCellContent(1, 5, this.textBox8);
            this.table.Body.SetCellContent(0, 0, this.textBox9);
            this.table.Body.SetCellContent(1, 0, this.textBox10);
            tableGroup1.Name = "group1";
            tableGroup2.Name = "group2";
            tableGroup4.Name = "Group1";
            tableGroup5.Name = "Group8";
            tableGroup6.Name = "group3";
            this.table.ColumnGroups.Add(tableGroup1);
            this.table.ColumnGroups.Add(tableGroup2);
            this.table.ColumnGroups.Add(tableGroup3);
            this.table.ColumnGroups.Add(tableGroup4);
            this.table.ColumnGroups.Add(tableGroup5);
            this.table.ColumnGroups.Add(tableGroup6);
            this.table.ColumnHeadersPrintOnEveryPage = true;
            this.table.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox9,
            this.textBox3,
            this.textBox45,
            this.textBox36,
            this.textBox55,
            this.textBox6,
            this.textBox10,
            this.textBox5,
            this.textBox46,
            this.textBox35,
            this.textBox56,
            this.textBox8});
            this.table.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(0.327D));
            this.table.Name = "table";
            tableGroup7.Name = "Group2";
            tableGroup8.Groupings.Add(new Telerik.Reporting.Grouping(null));
            tableGroup8.Name = "DetailGroup";
            this.table.RowGroups.Add(tableGroup7);
            this.table.RowGroups.Add(tableGroup8);
            this.table.RowHeadersPrintOnEveryPage = true;
            this.table.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(7.787D), Telerik.Reporting.Drawing.Unit.Cm(1.238D));
            this.table.Sortings.Add(new Telerik.Reporting.Sorting("= Fields.CartonNo", Telerik.Reporting.SortDirection.Asc));
            this.table.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.table.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            // 
            // textBox35
            // 
            this.textBox35.Format = "{0:N0}";
            this.textBox35.Name = "textBox35";
            this.textBox35.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1D), Telerik.Reporting.Drawing.Unit.Cm(0.476D));
            this.textBox35.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox35.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox35.Style.Padding.Bottom = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox35.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox35.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox35.Style.Padding.Top = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox35.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox35.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox35.StyleName = "";
            this.textBox35.Value = "= Fields.PalletNo";
            // 
            // textBox36
            // 
            this.textBox36.Name = "textBox36";
            this.textBox36.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1D), Telerik.Reporting.Drawing.Unit.Inch(0.3D));
            this.textBox36.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox36.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox36.Style.Font.Bold = true;
            this.textBox36.Style.Padding.Bottom = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox36.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Inch(0D);
            this.textBox36.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Inch(0D);
            this.textBox36.Style.Padding.Top = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox36.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox36.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox36.StyleName = "";
            this.textBox36.Value = "Pallet No";
            // 
            // textBox45
            // 
            this.textBox45.Name = "textBox45";
            this.textBox45.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1D), Telerik.Reporting.Drawing.Unit.Inch(0.3D));
            this.textBox45.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox45.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox45.Style.Font.Bold = true;
            this.textBox45.Style.Padding.Bottom = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox45.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Inch(0D);
            this.textBox45.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Inch(0D);
            this.textBox45.Style.Padding.Top = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox45.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox45.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox45.StyleName = "";
            this.textBox45.Value = "Carton No\t";
            // 
            // textBox46
            // 
            this.textBox46.Format = "{0:N0}";
            this.textBox46.Name = "textBox46";
            this.textBox46.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1D), Telerik.Reporting.Drawing.Unit.Cm(0.476D));
            this.textBox46.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox46.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox46.Style.Padding.Bottom = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox46.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox46.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox46.Style.Padding.Top = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox46.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox46.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox46.StyleName = "";
            this.textBox46.Value = "= Fields.CartonNo";
            // 
            // textBox55
            // 
            this.textBox55.Name = "textBox55";
            this.textBox55.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1D), Telerik.Reporting.Drawing.Unit.Inch(0.3D));
            this.textBox55.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox55.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox55.Style.Font.Bold = true;
            this.textBox55.Style.Padding.Bottom = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox55.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Inch(0D);
            this.textBox55.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Inch(0D);
            this.textBox55.Style.Padding.Top = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox55.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox55.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox55.StyleName = "";
            this.textBox55.Value = "Rack No";
            // 
            // textBox56
            // 
            this.textBox56.Format = "{0:N0}";
            this.textBox56.Name = "textBox56";
            this.textBox56.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1D), Telerik.Reporting.Drawing.Unit.Cm(0.476D));
            this.textBox56.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox56.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox56.Style.Padding.Bottom = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox56.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox56.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox56.Style.Padding.Top = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox56.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox56.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox56.StyleName = "";
            this.textBox56.Value = "= Fields.RackNo";
            // 
            // textBox3
            // 
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.8D), Telerik.Reporting.Drawing.Unit.Inch(0.3D));
            this.textBox3.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox3.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox3.Style.Font.Bold = true;
            this.textBox3.Style.Padding.Bottom = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox3.Style.Padding.Top = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox3.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox3.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox3.StyleName = "";
            this.textBox3.Value = "Branch";
            // 
            // textBox5
            // 
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.8D), Telerik.Reporting.Drawing.Unit.Cm(0.476D));
            this.textBox5.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox5.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox5.Style.Padding.Bottom = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox5.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox5.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox5.Style.Padding.Top = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox5.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox5.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox5.StyleName = "";
            this.textBox5.Value = "= Fields.BranchName";
            // 
            // textBox6
            // 
            this.textBox6.Name = "textBox6";
            this.textBox6.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.2D), Telerik.Reporting.Drawing.Unit.Inch(0.3D));
            this.textBox6.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox6.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox6.Style.Font.Bold = true;
            this.textBox6.Style.Padding.Bottom = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox6.Style.Padding.Top = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox6.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox6.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox6.StyleName = "";
            this.textBox6.Value = "Status";
            // 
            // textBox8
            // 
            this.textBox8.Name = "textBox8";
            this.textBox8.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.2D), Telerik.Reporting.Drawing.Unit.Cm(0.476D));
            this.textBox8.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox8.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox8.Style.Padding.Bottom = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox8.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox8.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox8.Style.Padding.Top = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox8.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox8.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox8.StyleName = "";
            this.textBox8.Value = "=Fields.Status";
            // 
            // textBox9
            // 
            this.textBox9.Name = "textBox9";
            this.textBox9.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Cm(2D), Telerik.Reporting.Drawing.Unit.Inch(0.3D));
            this.textBox9.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox9.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox9.Style.Font.Bold = true;
            this.textBox9.Style.Padding.Bottom = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox9.Style.Padding.Top = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox9.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox9.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox9.StyleName = "";
            this.textBox9.Value = "Sr. No.";
            // 
            // textBox10
            // 
            this.textBox10.Name = "textBox10";
            this.textBox10.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Cm(2D), Telerik.Reporting.Drawing.Unit.Cm(0.476D));
            this.textBox10.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox10.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox10.Style.Padding.Bottom = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox10.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox10.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox10.Style.Padding.Top = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox10.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox10.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox10.StyleName = "";
            this.textBox10.Value = "= RowNumber()";
            // 
            // textBox75
            // 
            this.textBox75.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Cm(0D), Telerik.Reporting.Drawing.Unit.Cm(0D));
            this.textBox75.Name = "textBox75";
            this.textBox75.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Cm(2.6D), Telerik.Reporting.Drawing.Unit.Cm(0.508D));
            this.textBox75.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox75.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox75.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.textBox75.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox75.Value = "WH Order No. :";
            // 
            // textBox73
            // 
            this.textBox73.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Cm(2.6D), Telerik.Reporting.Drawing.Unit.Cm(0D));
            this.textBox73.Name = "textBox73";
            this.textBox73.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Cm(4D), Telerik.Reporting.Drawing.Unit.Cm(0.508D));
            this.textBox73.Style.Font.Bold = true;
            this.textBox73.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox73.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox73.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.textBox73.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox73.Value = "=Fields.WarehouseOrderNo";
            // 
            // shape5
            // 
            this.shape5.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(0.209D));
            this.shape5.Name = "shape5";
            this.shape5.ShapeType = new Telerik.Reporting.Drawing.Shapes.LineShape(Telerik.Reporting.Drawing.Shapes.LineDirection.EW);
            this.shape5.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(7.86D), Telerik.Reporting.Drawing.Unit.Inch(0.1D));
            this.shape5.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            // 
            // textBox2
            // 
            this.textBox2.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Cm(10.2D), Telerik.Reporting.Drawing.Unit.Cm(0.008D));
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Cm(4.8D), Telerik.Reporting.Drawing.Unit.Cm(0.5D));
            this.textBox2.Style.Font.Bold = true;
            this.textBox2.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox2.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox2.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.textBox2.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox2.Value = "= Fields.OneLineItemCode";
            // 
            // textBox1
            // 
            this.textBox1.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Cm(6.6D), Telerik.Reporting.Drawing.Unit.Cm(0.008D));
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Cm(3.6D), Telerik.Reporting.Drawing.Unit.Cm(0.5D));
            this.textBox1.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox1.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox1.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.textBox1.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox1.Value = "One Line Item Code :";
            // 
            // textBox11
            // 
            this.textBox11.Format = "{0:N3}";
            this.textBox11.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Cm(17D), Telerik.Reporting.Drawing.Unit.Cm(0.03D));
            this.textBox11.Name = "textBox11";
            this.textBox11.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Cm(2.8D), Telerik.Reporting.Drawing.Unit.Cm(0.5D));
            this.textBox11.Style.Font.Bold = true;
            this.textBox11.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox11.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox11.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.textBox11.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox11.Value = "= SUM(Fields.NetWeight)";
            // 
            // textBox12
            // 
            this.textBox12.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Cm(15.1D), Telerik.Reporting.Drawing.Unit.Cm(0.03D));
            this.textBox12.Name = "textBox12";
            this.textBox12.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Cm(1.8D), Telerik.Reporting.Drawing.Unit.Cm(0.5D));
            this.textBox12.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox12.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox12.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.textBox12.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox12.Value = "Total Wt. :\r\n";
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
            this.textBox7.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(7.8D), Telerik.Reporting.Drawing.Unit.Inch(0.5D));
            this.textBox7.Style.Font.Bold = true;
            this.textBox7.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(25D);
            this.textBox7.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox7.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox7.Value = "Pallet Tracking Report";
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
            this.textBox4.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.textBox4.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox4.Value = "=Now()";
            // 
            // textBox28
            // 
            this.textBox28.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(6D), Telerik.Reporting.Drawing.Unit.Inch(0D));
            this.textBox28.Name = "textBox28";
            this.textBox28.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.8D), Telerik.Reporting.Drawing.Unit.Inch(0.2D));
            this.textBox28.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.textBox28.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox28.Value = "Page: {PageNumber}";
            // 
            // pageFooterSection1
            // 
            this.pageFooterSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(0.2D);
            this.pageFooterSection1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox4,
            this.textBox28});
            this.pageFooterSection1.Name = "pageFooterSection1";
            // 
            // PalletTrackingRpt
            // 
            this.Attributes = null;
            group1.DocumentMapText = "= Fields.WarehouseOrderNo";
            group1.GroupFooter = this.groupFooterSection;
            group1.GroupHeader = this.groupHeaderSection;
            group1.Groupings.Add(new Telerik.Reporting.Grouping("= Fields.WarehouseOrderNo"));
            group1.Name = "group";
            this.Groups.AddRange(new Telerik.Reporting.Group[] {
            group1});
            this.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.groupHeaderSection,
            this.groupFooterSection,
            this.pageHeaderSection1,
            this.detail,
            this.pageFooterSection1});
            this.Name = "PalletTrackingRpt";
            this.PageSettings.ContinuousPaper = false;
            this.PageSettings.Landscape = false;
            this.PageSettings.Margins = new Telerik.Reporting.Drawing.MarginsU(Telerik.Reporting.Drawing.Unit.Inch(0.2D), Telerik.Reporting.Drawing.Unit.Inch(0.2D), Telerik.Reporting.Drawing.Unit.Inch(0.2D), Telerik.Reporting.Drawing.Unit.Inch(0.2D));
            this.PageSettings.PaperKind = System.Drawing.Printing.PaperKind.A4;
            reportParameter1.AvailableValues.DisplayMember = "= Fields.WarehouseOrderNo";
            reportParameter1.AvailableValues.ValueMember = "= Fields.WarehouseOrderNo";
            reportParameter1.Name = "WarehouseOrderNo";
            reportParameter1.Text = "Warehouse Order No";
            reportParameter1.Value = "";
            reportParameter1.Visible = true;
            this.ReportParameters.Add(reportParameter1);
            this.Width = Telerik.Reporting.Drawing.Unit.Inch(7.86D);
            this.NeedDataSource += new System.EventHandler(this.PalletTrackingRpt_NeedDataSource);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }
        #endregion
        private Telerik.Reporting.PageHeaderSection pageHeaderSection1;
        private Telerik.Reporting.TextBox textBox7;
        private Telerik.Reporting.DetailSection detail;
        private Telerik.Reporting.TextBox textBox75;
        private Telerik.Reporting.TextBox textBox73;
        private Telerik.Reporting.Shape shape5;
        private Telerik.Reporting.Table table;
        private Telerik.Reporting.TextBox textBox35;
        private Telerik.Reporting.TextBox textBox36;
        private Telerik.Reporting.TextBox textBox45;
        private Telerik.Reporting.TextBox textBox46;
        private Telerik.Reporting.TextBox textBox55;
        private Telerik.Reporting.TextBox textBox56;
        private Telerik.Reporting.TextBox textBox4;
        private Telerik.Reporting.GroupHeaderSection groupHeaderSection;
        private Telerik.Reporting.GroupFooterSection groupFooterSection;
        private Telerik.Reporting.TextBox textBox28;
        private Telerik.Reporting.PageFooterSection pageFooterSection1;
        private Telerik.Reporting.TextBox textBox3;
        private Telerik.Reporting.TextBox textBox5;
        private Telerik.Reporting.TextBox textBox6;
        private Telerik.Reporting.TextBox textBox8;
        private Telerik.Reporting.TextBox textBox9;
        private Telerik.Reporting.TextBox textBox10;
        private Telerik.Reporting.TextBox textBox2;
        private Telerik.Reporting.TextBox textBox1;
        private Telerik.Reporting.TextBox textBox11;
        private Telerik.Reporting.TextBox textBox12;
    }
}