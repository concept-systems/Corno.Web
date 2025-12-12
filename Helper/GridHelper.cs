using System;
using System.Collections.Generic;
using Corno.Web.Globals;
using Kendo.Mvc.UI;
using Kendo.Mvc.UI.Fluent;

namespace Corno.Web.Helper;

public static class GridHelper
{
    public static GridBuilder<T> ApplyIndexSettings<T>(
        this GridBuilder<T> grid,
        string defaultAlignment = "center",
        int? height = null,
        bool enableExcel = true,
        bool enablePdf = true,
        bool enableSearch = true,
        Action<GridToolBarCommandFactory<T>> customToolbar = null
    ) where T : class
    {
        // Use full height if not specified (calculated via CSS)
        return grid
            .AddCommonSettings(height)
            .AddExportToolBar(enableExcel, enablePdf, enableSearch, customToolbar)
            .AddResponsiveSettings();
    }

    public static GridBuilder<T> ApplyCommonSettings<T>(
        this GridBuilder<T> grid,
        string defaultAlignment = "center",
        int? height = null,
        bool enableExcel = true,
        bool enablePdf = true,
        bool enableSearch = true
    ) where T : class
    {
        var alignmentStyle = $"text-align: {defaultAlignment}";

        return grid
            //.HtmlAttributes(new { style = alignmentStyle })
            //.HtmlAttributes(new { style = $"{alignmentStyle};height: 540px; width: 99%; overflow: auto;" })
            .AddExportToolBar(enableExcel, enablePdf, enableSearch)
            .AddCommonSettings(height);
    }

    public static GridBuilder<T> AddCommonSettings<T>(this GridBuilder<T> grid, int? height = null
    ) where T : class
    {
        var cssClass = "k-grid-responsive";
        if (!height.HasValue)
        {
            cssClass += " k-grid-full-height";
        }

        var builder = grid
            .Scrollable(scroll => scroll.Virtual(false))
            .Pageable(pageable => pageable
                .ButtonCount(5)
                .PageSizes(new[] { 5, 10, 20, 50, 100, 500, 1000 }))
            .Filterable(f => f.Mode(GridFilterMode.Row))
            .Resizable(resize => resize.Columns(true))
            .PersistSelection()
            .Reorderable(r => r.Columns(true))
            .ColumnMenu(s => s.Sortable(true))
            .HtmlAttributes(new { @class = cssClass });

        // Set height only if specified, otherwise use CSS for full height
        if (height.HasValue)
        {
            builder = builder.Height(height.Value);
        }

        return builder;
    }

    /// <summary>
    /// Adds responsive settings for grids (mobile/tablet/desktop)
    /// </summary>
    public static GridBuilder<T> AddResponsiveSettings<T>(this GridBuilder<T> grid) where T : class
    {
        return grid.HtmlAttributes(new { @class = "k-grid-responsive" });
    }

    public static GridBuilder<T> AddExportToolBar<T>(this GridBuilder<T> grid, bool enableExcel = true,
        bool enablePdf = true,
        bool enableSearch = true,
        Action<GridToolBarCommandFactory<T>> customToolbar = null
    ) where T : class
    {
        return grid
            .ToolBar(toolbar =>
            {
                if (enableExcel) toolbar.Excel();
                if (enablePdf) toolbar.Pdf();
                if (enableSearch) toolbar.Search();
                customToolbar?.Invoke(toolbar);
            })
            .Excel(excel => excel.FileName("Export.xlsx"))
            .Pdf(pdf => pdf.FileName("Export.pdf")
                .AvoidLinks(true).PaperSize("A4")
                .RepeatHeaders(true)
                .Landscape());
    }

    public static GridBuilder<T> AddIndexToolBar<T>(this GridBuilder<T> grid, bool enableExcel = true,
        bool enablePdf = true,
        bool enableSearch = true,
        Action<GridToolBarCommandFactory<T>>? customToolbar = null
    ) where T : class
    {
        return grid
            .ToolBar(toolbar =>
            {
                if (enableExcel) toolbar.Excel();
                if (enablePdf) toolbar.Pdf();
                if (enableSearch) toolbar.Search();
                customToolbar?.Invoke(toolbar);
            })
            .Excel(excel => excel.FileName("Export.xlsx"))
            .Pdf(pdf => pdf.FileName("Export.pdf")
                .AvoidLinks(true).PaperSize("A4")
                .RepeatHeaders(true)
                .Landscape());
    }

    public static GridEventBuilder ApplyCommonEvents(this GridEventBuilder events)
    {
        return events
            .DataBound("onGridDataBound")
            .PdfExport("onPdfExport"); // Optional: if you want to handle errors globally
    }

    public static void AddCommandColumns<T>(this GridColumnFactory<T> columns, List<GridCommandConfig> commands, string title = "Actions") where T : class
    {
        columns.Command(command =>
            {
                foreach (var cmd in commands)
                {
                    command.Custom(cmd.Name)
                        .Text(cmd.Text ?? " ")
                        .IconClass(cmd.IconClass)
                        .HtmlAttributes(new { @class = cmd.CssClass ?? "k-grid-command" })
                        .Click(cmd.ClickHandler);
                }
            })
            .Title(title)
            .HtmlAttributes(new { @class = "no-export" })
            .Exportable(false);
    }

    public static void AddStatusColumn<T>(this GridColumnFactory<T> columns, string field = "Status",
        string template = "<span class='status-badge' data-status='#= Status #'>#= Status #</span>"
    ) where T : class
    {
        columns.Bound(field)
            .ClientTemplate(template)
            .HtmlAttributes(new { style = "text-align: center" });
    }

    public static void AddMasterColumns<T>(this GridColumnFactory<T> columns) where T : class
    {
        columns.Bound(FieldConstants.Code);
        columns.Bound(FieldConstants.Name);
        columns.Bound(FieldConstants.Description);
    }

    public static AjaxDataSourceBuilder<T> ApplyCommonDataSourceSettings<T>(
        this DataSourceBuilder<T> dataSource,
        string action,
        string controller,
        string area = default
    ) where T : class
    {
        return dataSource
            .Ajax()
            .PageSize(10)
            .Read(read => read.Action(action, controller, new { area }).Data("sendData"))
            .Model(model => model.Id(FieldConstants.Id))
            .Events(events => events.Error("onGridError"));
    }

    public static GridBoundColumnBuilder<T> Align<T>(this GridBoundColumnBuilder<T> column, TextAlign direction) where T : class
    {
        var alignValue = direction.ToString().ToLower(); // "left", "center", "right"
        var style = $"text-align: {alignValue} !important;";
        return column.HtmlAttributes(new { style });//.HeaderHtmlAttributes(new { style });
    }

    /*public static GridBoundColumnBuilder<T> AlignLeft<T>(this GridBoundColumnBuilder<T> column) where T : class =>
        column.Align(TextAlign.Left);

    public static GridBoundColumnBuilder<T> AlignRight<T>(this GridBoundColumnBuilder<T> column) where T : class =>
        column.Align(TextAlign.Right);*/
}

public class GridCommandConfig
{
    public string Name { get; set; }           // e.g., "Edit"
    public string Text { get; set; }           // e.g., "Edit"
    public string IconClass { get; set; }      // e.g., "fa fa-edit"
    public string ClickHandler { get; set; }   // e.g., "onEditClick"
    public string CssClass { get; set; }       // Optional additional classes
}

public enum TextAlign
{
    Left,
    Center,
    Right
}


/*using Corno.Globals.Constants;
using Kendo.Mvc.UI;
using Kendo.Mvc.UI.Fluent;

namespace Corno.Web.Areas.Helpers;

public static class GridHelper
{
    public static GridBuilder<T> ApplyCommonSettings<T>(this GridBuilder<T> grid) where T : class
    {
        return grid
            .ToolBar(toolbar =>
            {
                toolbar.Excel();
                toolbar.Pdf();
                toolbar.Search();
            })
            .Excel(excel => excel
                .AllPages(true) // Ensures all pages are exported
                .FileName("Export.xlsx"))
            .Pdf(pdf => pdf
                .AllPages()
                .FileName("Export.pdf")
                .AvoidLinks(true)
                .PaperSize("A4")
                //.Scale(0.8)
                .RepeatHeaders(true)
                .Landscape())
            .Scrollable(scroll => scroll.Virtual(true))
            .Pageable(pageable => pageable
                .ButtonCount(5)
                .PageSizes(new[] { 5, 10, 20, 50, 100, 500, 1000 }))
            .Filterable(f => f.Mode(GridFilterMode.Row))
            .Resizable(resize => resize.Columns(true))
            .PersistSelection()
            .ColumnMenu(s => s.Sortable(true))
            .Height(540)
            .HtmlAttributes(new { style = "width: auto;" });
    }

    public static GridEventBuilder ApplyCommonEvents(this GridEventBuilder events)
    {
        return events
            .DataBound("onGridDataBound")
            .PdfExport("onPdfExport"); // Optional: if you want to handle errors globally
    }

    public static void AddCommandColumns<T>(this GridColumnFactory<T> columns) where T : class
    {
        columns.Command(command =>
        {
            command.Custom("Edit").Text("").IconClass("fa fa-edit")
                .HtmlAttributes(new { @class = "k-grid-edit" }).Click("onEditClick");
            command.Custom("Delete").Text("").IconClass("k-icon k-i-trash k-text-error")
                .HtmlAttributes(new { @class = "k-grid-delete k-text-center k-justify-content-center" })
                .Click("onDeleteClick");
        })
            .Title("Actions")
            .HtmlAttributes(new { @class = "no-export" }) // 👈 Add this class
            .Exportable(false); // 👈 This hides it from Excel and PDF
    }

    public static void AddStatusColumn<T>(this GridColumnFactory<T> columns) where T : class
    {
        columns.Bound("Status")
            .ClientTemplate("<span class='status-badge' data-status='#= Status #'>#= Status #</span>")
            .HtmlAttributes(new { style = "text-align: center" });
    }

    public static void AddMasterColumns<T>(this GridColumnFactory<T> columns) where T : class
    {
        columns.Bound(FieldConstants.Code);
        columns.Bound(FieldConstants.Name);
        columns.Bound(FieldConstants.Description);
    }

    public static AjaxDataSourceBuilder<T> ApplyCommonDataSourceSettings<T>(this DataSourceBuilder<T> dataSource, string action, string controller, string area = default) where T : class
    {
        return dataSource
            .Ajax()
            .PageSize(10)
            //.Read(read => read.Url(readUrl).Data("sendData"))
            .Read(read => read.Action(action, controller, new { area })
                .Data("sendData"))
            .Model(model => model.Id(FieldConstants.Id))
            .Events(events => events.Error("onGridError"));
    }

    public static GridBoundColumnBuilder<T> AlignLeft<T>(this GridBoundColumnBuilder<T> column) where T : class
    {
        return column.HtmlAttributes(new { style = "text-align: left !important;" });
    }

    public static GridBoundColumnBuilder<T> AlignRight<T>(this GridBoundColumnBuilder<T> column) where T : class
    {
        return column.HtmlAttributes(new { style = "text-align: right !important;" });
    }

}*/