# Migration Guide: Standardizing Buttons, Grids, and MultiColumnComboBox

This guide helps you migrate existing pages to use the new standardized components.

## 1. Buttons - Using ButtonHelper

### Before (Bootstrap Buttons):
```html
<button type="button" class="btn btn-primary btn-sm" id="btnImport">
    <i class="fas fa-upload"></i> Import
</button>
```

### After (Telerik Buttons with ButtonHelper):
```csharp
@using Corno.Concept.Portal.Helper

@(Html.Kendo().Button()
    .Name("btnImport")
    .Content("Import")
    .ApplyStandardSettingsWithType(ButtonType.Import, "import", ComponentSize.Small))
```

### Converting `<a>` Links with Button Classes:

**Before:**
```html
<a href="@Url.Action("Import", "Plan")" class="k-button-md k-rounded-lg k-button-outline k-button-outline-secondary k-button">
    <span class="k-icon k-i-import"></span> Import
</a>
```

**After:**
```csharp
@(Html.Kendo().Button()
    .Name("btnImport")
    .Content("Import")
    .ApplyStandardSettingsWithType(ButtonType.Import, "import", ComponentSize.Medium)
    .HtmlAttributes(new { onclick = $"window.location.href='{Url.Action("Import", "Plan")}'; return false;" }))
```

**Note:** For navigation buttons, use `onclick` with `window.location.href` instead of `href` attribute.

### Button Types and Their ThemeColors:
- `ButtonType.Primary` → ThemeColor.Primary
- `ButtonType.Success` → ThemeColor.Success
- `ButtonType.Warning` → ThemeColor.Warning
- `ButtonType.Danger` → ThemeColor.Error
- `ButtonType.Info` → ThemeColor.Info
- `ButtonType.Import` → ThemeColor.Info
- `ButtonType.Export` → ThemeColor.Success
- `ButtonType.Edit` → ThemeColor.Primary
- `ButtonType.Delete` → ThemeColor.Error
- `ButtonType.View` → ThemeColor.Info
- `ButtonType.Cancel` → ThemeColor.Secondary
- `ButtonType.Save` → ThemeColor.Success

### Using k-actions Container:
```html
<div class="k-card-actions k-card-actions-horizontal k-card-actions-end">
    @(Html.Kendo().Button()
        .Name("btnSave")
        .Content("Save")
        .ApplyStandardSettingsWithType(ButtonType.Save, "save", ComponentSize.Medium))
    @(Html.Kendo().Button()
        .Name("btnCancel")
        .Content("Cancel")
        .ApplyStandardSettingsWithType(ButtonType.Cancel, "cancel", ComponentSize.Medium))
</div>
```

## 2. Grids - Using GridHelper

### Before:
```csharp
@(Html.Kendo().Grid<YourDto>()
    .Name("grid")
    .Height(540)
    .ToolBar(toolbar => {
        toolbar.Excel();
        toolbar.Pdf();
        toolbar.Search();
    })
    // ... other settings
)
```

### After (Index Pages - Full Height & Responsive):
```csharp
@using Corno.Concept.Portal.Helper

@(Html.Kendo().Grid<YourDto>()
    .Name("grid")
    .ApplyIndexSettings() // Full height, responsive
    .AddExportToolBar(enableExcel: true, enablePdf: true, enableSearch: true, customToolbar: toolbar =>
    {
        // Add custom buttons to toolbar
        toolbar.Custom().Text("Import").Icon("import").Click("onImportClick");
        toolbar.Custom().Text("Add New").Icon("plus").Click("onAddNewClick");
    })
    .Events(events => events.ApplyCommonEvents())
    // ... columns
)
```

### Moving Buttons from Header to Toolbar:

**Before:**
```html
@section sectionCrudHeader
{
    <div class="k-card-actions">
        <a href="@Url.Action("Import", "Plan")" class="k-button">Import</a>
    </div>
}
```

**After:**
```csharp
.AddExportToolBar(customToolbar: toolbar =>
{
    toolbar.Custom().Text("Import").Icon("import").Click("onImportClick");
})
```

**JavaScript:**
```javascript
function onImportClick(e) {
    e.preventDefault();
    window.location.href = '@Url.Action("Import", "Plan")';
}
```

### For Non-Index Pages (Fixed Height):
```csharp
@(Html.Kendo().Grid<YourDto>()
    .Name("grid")
    .ApplyCommonSettings(height: 540) // Fixed height
    .Events(events => events.ApplyCommonEvents())
    // ... columns
)
```

## 3. MultiColumnComboBox - Auto Width & Multi-Column Filtering

### Before:
```csharp
@(Html.Kendo().MultiColumnComboBoxFor(m => m)
    .DataTextField("Name")
    .DataValueField("Id")
    .Columns(columns =>
    {
        columns.Add().Field("Code").Title("Code").Width("250px");
        columns.Add().Field("Name").Title("Name").Width("300px");
    }))
```

### After:
```csharp
@(Html.Kendo().MultiColumnComboBoxFor(m => m)
    .DataTextField("Name")
    .DataValueField("Id")
    .HtmlAttributes(new { @class = "w-100", data_multicolumn_filter = "true" })
    .Columns(columns =>
    {
        columns.Add().Field("Code").Title("Code"); // Auto width
        columns.Add().Field("Name").Title("Name"); // Auto width
    }))
```

**Key Changes:**
1. Removed fixed `.Width()` - columns now auto-size
2. Added `data_multicolumn_filter="true"` attribute for multi-column filtering
3. The helper JavaScript (`multicolumncomboboxHelper.js`) automatically configures filtering for all columns

## 4. Step-by-Step Migration Process

### For Index Pages:

1. **Add using statements:**
   ```csharp
   @using Corno.Concept.Portal.Helper
   ```

2. **Update Grid:**
   - Change `.ApplyCommonSettings()` to `.ApplyIndexSettings()`
   - Remove fixed height if present
   - Move buttons from header section to grid toolbar using `customToolbar` parameter

3. **Update Buttons:**
   - Replace Bootstrap buttons with Telerik buttons
   - Use `ButtonHelper.ApplyStandardSettingsWithType()`
   - Place in `k-card-actions` container where appropriate

4. **Test:**
   - Verify grid takes full height
   - Verify buttons work correctly
   - Verify responsive behavior on different screen sizes

### For Import/Form Pages:

1. **Add using statements:**
   ```csharp
   @using Corno.Concept.Portal.Helper
   ```

2. **Replace all Bootstrap buttons:**
   ```csharp
   // Old
   <button type="button" class="btn btn-primary" id="btnSave">Save</button>
   
   // New
   @(Html.Kendo().Button()
       .Name("btnSave")
       .Content("Save")
       .ApplyStandardSettingsWithType(ButtonType.Save, "save", ComponentSize.Medium))
   ```

3. **Update JavaScript (if needed):**
   - Button IDs remain the same, so existing JavaScript should work
   - Only change is the HTML structure

## 5. Common Patterns

### Pattern 1: Action Buttons in k-actions
```html
<div class="k-card-actions k-card-actions-horizontal k-card-actions-end">
    @(Html.Kendo().Button()
        .Name("btnSave")
        .Content("Save")
        .ApplyStandardSettingsWithType(ButtonType.Save, "save"))
    @(Html.Kendo().Button()
        .Name("btnCancel")
        .Content("Cancel")
        .ApplyStandardSettingsWithType(ButtonType.Cancel, "cancel"))
</div>
```

### Pattern 2: Buttons in Grid Toolbar
```csharp
.AddExportToolBar(customToolbar: toolbar =>
{
    toolbar.Custom().Text("Import").Icon("import").Click("onImportClick");
    toolbar.Custom().Text("Export").Icon("export").Click("onExportClick");
})
```

### Pattern 3: Inline Buttons
```csharp
@(Html.Kendo().Button()
    .Name("btnAction")
    .Content("Action")
    .ApplyStandardSettingsWithType(ButtonType.Primary, "action", ComponentSize.Small))
```

## 6. Files to Update

### Priority 1 (High Impact):
- All `Index.cshtml` files in `Areas/Kitchen/Views/`
- All `Index.cshtml` files in `Areas/Masters/Views/`
- All `Index.cshtml` files in `Areas/Admin/Views/`

### Priority 2 (Medium Impact):
- All `Import.cshtml` files
- All `Create.cshtml` and `Edit.cshtml` files with buttons
- All form pages with action buttons

### Priority 3 (Low Impact):
- Other views with buttons
- Partial views with buttons

## 7. Testing Checklist

After migration, verify:
- [ ] Buttons display correctly with proper styling
- [ ] Buttons have icons
- [ ] Buttons are rounded (Rounded.Full)
- [ ] Buttons use outline style (FillMode.Outline)
- [ ] Grid takes full height on index pages
- [ ] Grid is responsive on different screen sizes
- [ ] MultiColumnComboBox columns auto-size
- [ ] MultiColumnComboBox filters work on all columns
- [ ] All button click handlers work
- [ ] No JavaScript errors in console

## 8. Examples

See these files for reference implementations:
- `Areas/Kitchen/Views/Plan/Index.cshtml` - Grid with toolbar button
- `Areas/Kitchen/Views/Plan/Import.cshtml` - All buttons converted
- `Areas/Kitchen/Views/Carcass/Index.cshtml` - Full height grid
- `Areas/Masters/Views/Supplier/Index.cshtml` - Full height grid

## 9. Troubleshooting

### Issue: Button not showing
- Check that `ButtonHelper` namespace is imported
- Verify button name is unique
- Check browser console for JavaScript errors

### Issue: Grid not full height
- Ensure `.ApplyIndexSettings()` is used (not `.ApplyCommonSettings()`)
- Check that `k-grid-full-height` CSS class is applied
- Verify `Content/css/grid.css` is loaded

### Issue: MultiColumnComboBox not filtering all columns
- Ensure `data_multicolumn_filter="true"` attribute is present
- Verify `multicolumncomboboxHelper.js` is loaded
- Check browser console for JavaScript errors

## 10. Support

For questions or issues during migration, refer to:
- `Helper/ButtonHelper.cs` - Button helper implementation
- `Helper/GridHelper.cs` - Grid helper implementation
- `Scripts/js/multicolumncomboboxHelper.js` - MultiColumnComboBox filtering logic

