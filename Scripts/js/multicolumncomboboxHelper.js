// Helper to configure MultiColumnComboBox to filter by all columns with 'contains' filter
(function ($, kendo) {
    'use strict';

    /**
     * Configures MultiColumnComboBox to filter by all defined columns
     * This should be called after the combo box is initialized
     */
    function configureMultiColumnComboBoxFilter(comboBoxElement) {
        if (!comboBoxElement) return;

        var comboBox = $(comboBoxElement).data("kendoMultiColumnComboBox");
        if (!comboBox) return;

        // Get all column fields from the combo box configuration
        var columns = comboBox.options.columns || [];
        if (columns.length === 0) return;

        // Get all field names from columns
        var fieldNames = [];
        columns.forEach(function (column) {
            if (column.field) {
                fieldNames.push(column.field);
            }
        });

        if (fieldNames.length === 0) return;

        // Handle the filtering event to search across all columns
        var isServerFiltering = comboBox.dataSource && comboBox.dataSource.options.serverFiltering;
        var lastFilterValue = ''; // Track last filter value to avoid unnecessary updates
        var isUpdatingFilter = false; // Flag to prevent recursion
        
        if (isServerFiltering) {
            // For server-side filtering, handle input changes directly
            comboBox.input.on("input", function() {
                if (isUpdatingFilter) return;
                
                var filterValue = $(this).val() || '';
                
                if (filterValue.trim() === '') {
                    if (lastFilterValue !== '') {
                        isUpdatingFilter = true;
                        comboBox.dataSource.filter(null);
                        lastFilterValue = '';
                        setTimeout(function() {
                            isUpdatingFilter = false;
                        }, 100);
                    }
                    return;
                }
                
                // Skip if filter value hasn't changed
                if (filterValue === lastFilterValue) {
                    return;
                }
                lastFilterValue = filterValue;
                
                var filterValueLower = filterValue.toString().toLowerCase();
                var columnFilters = [];
                
                // Create contains filters for all columns
                fieldNames.forEach(function (fieldName) {
                    columnFilters.push({
                        field: fieldName,
                        operator: "contains",
                        value: filterValueLower
                    });
                });
                
                if (columnFilters.length > 0) {
                    var multiColumnFilter = {
                        logic: "or",
                        filters: columnFilters
                    };
                    
                    // Set filter on dataSource - this will trigger a server request with the filter
                    isUpdatingFilter = true;
                    comboBox.dataSource.filter(multiColumnFilter);
                    setTimeout(function() {
                        isUpdatingFilter = false;
                    }, 100);
                }
            });
        }
        
        // Also handle the filtering event to prevent default single-column filtering
        comboBox.bind("filtering", function (e) {
            if (isServerFiltering) {
                // For server-side, we handle it via input event above
                // Prevent default filtering to avoid conflicts
                var filterValue = comboBox.input.val() || '';
                if (filterValue.trim() === '') {
                    e.filter = null;
                    return;
                }
                
                // Build multi-column filter
                var filterValueLower = filterValue.toString().toLowerCase();
                var columnFilters = [];
                fieldNames.forEach(function (fieldName) {
                    columnFilters.push({
                        field: fieldName,
                        operator: "contains",
                        value: filterValueLower
                    });
                });
                
                if (columnFilters.length > 0) {
                    e.filter = {
                        logic: "or",
                        filters: columnFilters
                    };
                }
            } else {
                // Client-side filtering
                var filterValue = comboBox.input.val() || '';
                if (filterValue.trim() === '') {
                    e.filter = null;
                    return;
                }
                
                var filterValueLower = filterValue.toString().toLowerCase();
                var columnFilters = [];
                fieldNames.forEach(function (fieldName) {
                    columnFilters.push({
                        field: fieldName,
                        operator: "contains",
                        value: filterValueLower
                    });
                });
                
                if (columnFilters.length > 0) {
                    e.filter = {
                        logic: "or",
                        filters: columnFilters
                    };
                }
            }
        });
    }

    /**
     * Automatically configure all MultiColumnComboBox components on the page
     */
    function configureAllMultiColumnComboBoxes() {
        // Configure by data attribute
        $("[data-multicolumn-filter='true']").each(function () {
            configureMultiColumnComboBoxFilter(this);
        });

        $("[data-role='multicolumncombobox']").each(function () {
            configureMultiColumnComboBoxFilter(this);
        });

        $(".k-multicolumncombobox").each(function () {
            configureMultiColumnComboBoxFilter(this);
        });
    }

    // Initialize when document is ready
    $(document).ready(function () {
        // Configure immediately
        configureAllMultiColumnComboBoxes();

        // Configure after delays to catch late-initialized combo boxes
        setTimeout(configureAllMultiColumnComboBoxes, 500);
        setTimeout(configureAllMultiColumnComboBoxes, 1000);

        // Listen for Kendo ready events
        if (typeof kendo !== "undefined") {
            $(document).on("kendoReady", function () {
                configureAllMultiColumnComboBoxes();
            });
        }

        // Use MutationObserver to catch dynamically added combo boxes
        if (typeof MutationObserver !== "undefined") {
            var observer = new MutationObserver(function (mutations) {
                var shouldProcess = false;
                mutations.forEach(function (mutation) {
                    if (mutation.addedNodes.length > 0) {
                        shouldProcess = true;
                    }
                });
                if (shouldProcess) {
                    setTimeout(configureAllMultiColumnComboBoxes, 100);
                }
            });

            observer.observe(document.body, {
                childList: true,
                subtree: true
            });
        }
    });

    // Export for manual use
    window.configureMultiColumnComboBoxFilter = configureMultiColumnComboBoxFilter;
    window.configureAllMultiColumnComboBoxes = configureAllMultiColumnComboBoxes;

})(jQuery, kendo);

