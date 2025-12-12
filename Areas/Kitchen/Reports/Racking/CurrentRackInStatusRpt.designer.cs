namespace Corno.Web.Areas.Kitchen.Reports.Racking
{
    partial class CurrentRackInStatusRpt
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
            Telerik.Reporting.TableGroup tableGroup9 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup10 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.Group group1 = new Telerik.Reporting.Group();
            Telerik.Reporting.ReportParameter reportParameter1 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.ReportParameter reportParameter2 = new Telerik.Reporting.ReportParameter();
            this.groupFooterSection = new Telerik.Reporting.GroupFooterSection();
            this.groupHeaderSection = new Telerik.Reporting.GroupHeaderSection();
            this.table = new Telerik.Reporting.Table();
            this.textBox35 = new Telerik.Reporting.TextBox();
            this.textBox46 = new Telerik.Reporting.TextBox();
            this.textBox56 = new Telerik.Reporting.TextBox();
            this.textBox10 = new Telerik.Reporting.TextBox();
            this.textBox14 = new Telerik.Reporting.TextBox();
            this.textBox16 = new Telerik.Reporting.TextBox();
            this.textBox18 = new Telerik.Reporting.TextBox();
            this.textBox20 = new Telerik.Reporting.TextBox();
            this.textBox1 = new Telerik.Reporting.TextBox();
            this.textBox2 = new Telerik.Reporting.TextBox();
            this.textBox3 = new Telerik.Reporting.TextBox();
            this.textBox5 = new Telerik.Reporting.TextBox();
            this.textBox6 = new Telerik.Reporting.TextBox();
            this.textBox8 = new Telerik.Reporting.TextBox();
            this.textBox11 = new Telerik.Reporting.TextBox();
            this.textBox12 = new Telerik.Reporting.TextBox();
            this.textBox75 = new Telerik.Reporting.TextBox();
            this.textBox73 = new Telerik.Reporting.TextBox();
            this.shape5 = new Telerik.Reporting.Shape();
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
            this.shape5});
            this.groupHeaderSection.Name = "groupHeaderSection";
            // 
            // table
            // 
            this.table.Bindings.Add(new Telerik.Reporting.Binding("DataSource", "=ReportItem.DataObject"));
            this.table.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.4D)));
            this.table.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.6D)));
            this.table.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(2D)));
            this.table.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(4.76D)));
            this.table.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.7D)));
            this.table.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.8D)));
            this.table.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1D)));
            this.table.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1D)));
            this.table.Body.Rows.Add(new Telerik.Reporting.TableBodyRow(Telerik.Reporting.Drawing.Unit.Inch(0.3D)));
            this.table.Body.Rows.Add(new Telerik.Reporting.TableBodyRow(Telerik.Reporting.Drawing.Unit.Cm(0.476D)));
            this.table.Body.SetCellContent(1, 6, this.textBox35);
            this.table.Body.SetCellContent(1, 5, this.textBox46);
            this.table.Body.SetCellContent(1, 7, this.textBox56);
            this.table.Body.SetCellContent(1, 0, this.textBox10);
            this.table.Body.SetCellContent(1, 1, this.textBox14);
            this.table.Body.SetCellContent(1, 2, this.textBox16);
            this.table.Body.SetCellContent(1, 3, this.textBox18);
            this.table.Body.SetCellContent(1, 4, this.textBox20);
            this.table.Body.SetCellContent(0, 0, this.textBox1);
            this.table.Body.SetCellContent(0, 1, this.textBox2);
            this.table.Body.SetCellContent(0, 2, this.textBox3);
            this.table.Body.SetCellContent(0, 3, this.textBox5);
            this.table.Body.SetCellContent(0, 4, this.textBox6);
            this.table.Body.SetCellContent(0, 5, this.textBox8);
            this.table.Body.SetCellContent(0, 6, this.textBox11);
            this.table.Body.SetCellContent(0, 7, this.textBox12);
            tableGroup1.Name = "group1";
            tableGroup2.Name = "group4";
            tableGroup3.Name = "group5";
            tableGroup4.Name = "group6";
            tableGroup5.Name = "group7";
            tableGroup7.Name = "Group1";
            tableGroup8.Name = "Group8";
            this.table.ColumnGroups.Add(tableGroup1);
            this.table.ColumnGroups.Add(tableGroup2);
            this.table.ColumnGroups.Add(tableGroup3);
            this.table.ColumnGroups.Add(tableGroup4);
            this.table.ColumnGroups.Add(tableGroup5);
            this.table.ColumnGroups.Add(tableGroup6);
            this.table.ColumnGroups.Add(tableGroup7);
            this.table.ColumnGroups.Add(tableGroup8);
            this.table.ColumnHeadersPrintOnEveryPage = true;
            this.table.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox1,
            this.textBox2,
            this.textBox3,
            this.textBox5,
            this.textBox6,
            this.textBox8,
            this.textBox11,
            this.textBox12,
            this.textBox10,
            this.textBox14,
            this.textBox16,
            this.textBox18,
            this.textBox20,
            this.textBox46,
            this.textBox35,
            this.textBox56});
            this.table.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(0.327D));
            this.table.Name = "table";
            tableGroup9.Name = "group2";
            tableGroup10.Groupings.Add(new Telerik.Reporting.Grouping(null));
            tableGroup10.Name = "DetailGroup";
            this.table.RowGroups.Add(tableGroup9);
            this.table.RowGroups.Add(tableGroup10);
            this.table.RowHeadersPrintOnEveryPage = true;
            this.table.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(11.26D), Telerik.Reporting.Drawing.Unit.Cm(1.238D));
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
            this.textBox35.Value = "= Fields.RackNo";
            // 
            // textBox46
            // 
            this.textBox46.Format = "{0:N0}";
            this.textBox46.Name = "textBox46";
            this.textBox46.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.8D), Telerik.Reporting.Drawing.Unit.Cm(0.476D));
            this.textBox46.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox46.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox46.Style.Padding.Bottom = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox46.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox46.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox46.Style.Padding.Top = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox46.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox46.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox46.StyleName = "";
            this.textBox46.Value = "= \"C\" + Fields.CartonNo";
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
            this.textBox56.Value = "= Fields.PalletNo";
            // 
            // textBox10
            // 
            this.textBox10.Name = "textBox10";
            this.textBox10.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.4D), Telerik.Reporting.Drawing.Unit.Cm(0.476D));
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
            // textBox14
            // 
            this.textBox14.Name = "textBox14";
            this.textBox14.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.6D), Telerik.Reporting.Drawing.Unit.Cm(0.476D));
            this.textBox14.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox14.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox14.Style.Padding.Bottom = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox14.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox14.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox14.Style.Padding.Top = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox14.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox14.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox14.StyleName = "";
            this.textBox14.Value = "= Fields.WarehousePosition";
            // 
            // textBox16
            // 
            this.textBox16.Name = "textBox16";
            this.textBox16.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2D), Telerik.Reporting.Drawing.Unit.Cm(0.476D));
            this.textBox16.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox16.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox16.Style.Padding.Bottom = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox16.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox16.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox16.Style.Padding.Top = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox16.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox16.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox16.StyleName = "";
            this.textBox16.Value = "= Fields.ItemCode";
            // 
            // textBox18
            // 
            this.textBox18.Name = "textBox18";
            this.textBox18.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(4.76D), Telerik.Reporting.Drawing.Unit.Cm(0.476D));
            this.textBox18.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox18.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox18.Style.Padding.Bottom = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox18.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(5D);
            this.textBox18.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox18.Style.Padding.Top = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox18.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.textBox18.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox18.StyleName = "";
            this.textBox18.Value = "= Fields.Description";
            // 
            // textBox20
            // 
            this.textBox20.Name = "textBox20";
            this.textBox20.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.7D), Telerik.Reporting.Drawing.Unit.Cm(0.476D));
            this.textBox20.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox20.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox20.Style.Padding.Bottom = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox20.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox20.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox20.Style.Padding.Top = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox20.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox20.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox20.StyleName = "";
            this.textBox20.Value = "= Fields.Quantity";
            // 
            // textBox1
            // 
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.4D), Telerik.Reporting.Drawing.Unit.Inch(0.3D));
            this.textBox1.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox1.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox1.Style.Font.Bold = true;
            this.textBox1.Style.Padding.Bottom = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox1.Style.Padding.Top = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox1.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox1.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox1.StyleName = "";
            this.textBox1.Value = "Sr.  No.";
            // 
            // textBox2
            // 
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.6D), Telerik.Reporting.Drawing.Unit.Inch(0.3D));
            this.textBox2.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox2.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox2.Style.Font.Bold = true;
            this.textBox2.Style.Padding.Bottom = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox2.Style.Padding.Top = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox2.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox2.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox2.StyleName = "";
            this.textBox2.Value = "Position No";
            // 
            // textBox3
            // 
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2D), Telerik.Reporting.Drawing.Unit.Inch(0.3D));
            this.textBox3.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox3.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox3.Style.Font.Bold = true;
            this.textBox3.Style.Padding.Bottom = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox3.Style.Padding.Top = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox3.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox3.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox3.StyleName = "";
            this.textBox3.Value = "Item Code";
            // 
            // textBox5
            // 
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(4.76D), Telerik.Reporting.Drawing.Unit.Inch(0.3D));
            this.textBox5.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox5.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox5.Style.Font.Bold = true;
            this.textBox5.Style.Padding.Bottom = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox5.Style.Padding.Top = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox5.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox5.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox5.StyleName = "";
            this.textBox5.Value = "Item Description";
            // 
            // textBox6
            // 
            this.textBox6.Name = "textBox6";
            this.textBox6.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.7D), Telerik.Reporting.Drawing.Unit.Inch(0.3D));
            this.textBox6.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox6.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox6.Style.Font.Bold = true;
            this.textBox6.Style.Padding.Bottom = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox6.Style.Padding.Top = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox6.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox6.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox6.StyleName = "";
            this.textBox6.Value = "Quantity";
            // 
            // textBox8
            // 
            this.textBox8.Name = "textBox8";
            this.textBox8.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.8D), Telerik.Reporting.Drawing.Unit.Inch(0.3D));
            this.textBox8.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox8.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox8.Style.Font.Bold = true;
            this.textBox8.Style.Padding.Bottom = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox8.Style.Padding.Top = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox8.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox8.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox8.StyleName = "";
            this.textBox8.Value = "Carton No\t";
            // 
            // textBox11
            // 
            this.textBox11.Name = "textBox11";
            this.textBox11.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1D), Telerik.Reporting.Drawing.Unit.Inch(0.3D));
            this.textBox11.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox11.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox11.Style.Font.Bold = true;
            this.textBox11.Style.Padding.Bottom = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox11.Style.Padding.Top = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox11.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox11.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox11.StyleName = "";
            this.textBox11.Value = "Rack No";
            // 
            // textBox12
            // 
            this.textBox12.Name = "textBox12";
            this.textBox12.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1D), Telerik.Reporting.Drawing.Unit.Inch(0.3D));
            this.textBox12.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox12.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox12.Style.Font.Bold = true;
            this.textBox12.Style.Padding.Bottom = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox12.Style.Padding.Top = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.textBox12.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox12.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox12.StyleName = "";
            this.textBox12.Value = "Pallet No";
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
            this.textBox73.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Cm(8.8D), Telerik.Reporting.Drawing.Unit.Cm(0.508D));
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
            this.shape5.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(11.26D), Telerik.Reporting.Drawing.Unit.Inch(0.058D));
            this.shape5.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            // 
            // pageHeaderSection1
            // 
            this.pageHeaderSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(0.433D);
            this.pageHeaderSection1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox7});
            this.pageHeaderSection1.Name = "pageHeaderSection1";
            // 
            // textBox7
            // 
            this.textBox7.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(0D));
            this.textBox7.Name = "textBox7";
            this.textBox7.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(11.26D), Telerik.Reporting.Drawing.Unit.Inch(0.433D));
            this.textBox7.Style.Font.Bold = true;
            this.textBox7.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(25D);
            this.textBox7.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox7.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox7.Value = "WIP Data";
            // 
            // detail
            // 
            this.detail.Height = Telerik.Reporting.Drawing.Unit.Inch(0.052D);
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
            this.textBox28.Docking = Telerik.Reporting.DockingStyle.Right;
            this.textBox28.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Cm(24.13D), Telerik.Reporting.Drawing.Unit.Cm(0D));
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
            // CurrentRackInStatusRpt
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
            this.Name = "CurrentRackInStatusRpt";
            this.PageSettings.ContinuousPaper = false;
            this.PageSettings.Landscape = true;
            this.PageSettings.Margins = new Telerik.Reporting.Drawing.MarginsU(Telerik.Reporting.Drawing.Unit.Inch(0.2D), Telerik.Reporting.Drawing.Unit.Inch(0.2D), Telerik.Reporting.Drawing.Unit.Inch(0.2D), Telerik.Reporting.Drawing.Unit.Inch(0.2D));
            this.PageSettings.PaperKind = System.Drawing.Printing.PaperKind.A4;
            reportParameter1.Name = "FromDate";
            reportParameter1.Text = "From";
            reportParameter1.Type = Telerik.Reporting.ReportParameterType.DateTime;
            reportParameter1.Value = "= Now()";
            reportParameter1.Visible = true;
            reportParameter2.Name = "ToDate";
            reportParameter2.Text = "To";
            reportParameter2.Type = Telerik.Reporting.ReportParameterType.DateTime;
            reportParameter2.Value = "= Now()";
            reportParameter2.Visible = true;
            this.ReportParameters.Add(reportParameter1);
            this.ReportParameters.Add(reportParameter2);
            this.Style.Font.Name = "Segoe UI";
            this.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8.25D);
            this.Width = Telerik.Reporting.Drawing.Unit.Inch(11.3D);
            this.NeedDataSource += new System.EventHandler(this.CurrentRackInStatusRpt_NeedDataSource);
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
        private Telerik.Reporting.TextBox textBox46;
        private Telerik.Reporting.TextBox textBox56;
        private Telerik.Reporting.TextBox textBox4;
        private Telerik.Reporting.GroupHeaderSection groupHeaderSection;
        private Telerik.Reporting.GroupFooterSection groupFooterSection;
        private Telerik.Reporting.TextBox textBox28;
        private Telerik.Reporting.PageFooterSection pageFooterSection1;
        private Telerik.Reporting.TextBox textBox10;
        private Telerik.Reporting.TextBox textBox14;
        private Telerik.Reporting.TextBox textBox16;
        private Telerik.Reporting.TextBox textBox18;
        private Telerik.Reporting.TextBox textBox20;
        private Telerik.Reporting.TextBox textBox1;
        private Telerik.Reporting.TextBox textBox2;
        private Telerik.Reporting.TextBox textBox3;
        private Telerik.Reporting.TextBox textBox5;
        private Telerik.Reporting.TextBox textBox6;
        private Telerik.Reporting.TextBox textBox8;
        private Telerik.Reporting.TextBox textBox11;
        private Telerik.Reporting.TextBox textBox12;
    }
}