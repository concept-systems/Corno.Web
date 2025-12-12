// Write your Javascript code.

window.kendoReady = function (callback) {
    $(function () {
        if (typeof kendo !== "undefined") {
            callback();
        } else {
            console.error("Kendo is not loaded.");
        }
    });
};

$(function () {
    if (typeof kendo !== "undefined") {
        // Safe to use Kendo here
        console.log("Kendo is loaded");
    } else {
        console.error("Kendo is still undefined");
    }
});

//function renderStatusBadge(status) {
//    let backgroundColor = 'transparent';
//    let textColor = 'black';

//    if (status === 'Active') {
//        backgroundColor = 'green';
//        textColor = 'white';
//    } else if (status === 'Deleted') {
//        backgroundColor = 'red';
//        textColor = 'white';
//    }

//    return `<span class='badge' style='
//        background-color: ${backgroundColor};
//        color: ${textColor};
//        text-align: center;
//        display: inline-block;
//      '>${status}</span>`;
//}

function renderStatusBadge(status) {
    let backgroundColor = 'transparent';
    let textColor = 'black';

    switch (status) {
        case 'Active':
            backgroundColor = '#28a745'; // green
            textColor = 'white';
            break;
        case 'Printed':
            backgroundColor = '#007bff'; // blue
            textColor = 'white';
            break;
        case 'Bent':
            backgroundColor = '#6f42c1'; // purple
            textColor = 'white';
            break;
        case 'Sorted':
            backgroundColor = '#17a2b8'; // teal
            textColor = 'white';
            break;
        case 'SubAssembled':
            backgroundColor = '#fd7e14'; // orange
            textColor = 'white';
            break;
        case 'Packed':
            backgroundColor = '#20c997'; // greenish-teal
            textColor = 'white';
            break;
        case 'Pallet In':
            backgroundColor = '#6610f2'; // indigo
            textColor = 'white';
            break;
        case 'Rack In':
            backgroundColor = '#6c757d'; // gray
            textColor = 'white';
            break;
        case 'Rack Out':
            backgroundColor = '#343a40'; // dark gray
            textColor = 'white';
            break;
        case 'Dispatch':
            backgroundColor = '#ffc107'; // yellow
            textColor = 'black';
            break;
        case 'Loaded':
            backgroundColor = '#007bff'; // blue
            textColor = 'white';
            break;
        case 'Unload':
            backgroundColor = '#17a2b8'; // cyan
            textColor = 'white';
            break;
        case 'InProcess':
            backgroundColor = '#fd7e14'; // orange
            textColor = 'white';
            break;
        case 'Deleted':
            backgroundColor = '#dc3545'; // red
            textColor = 'white';
            break;
        default:
            backgroundColor = '#f8f9fa'; // light gray
            textColor = '#212529'; // dark text
            break;
    }

    return `<span class='badge' style='
        background-color: ${backgroundColor};
        color: ${textColor};
        text-align: center;
        display: inline-block;
        padding: 4px 8px;
        border-radius: 4px;
        font-weight: 500;
        font-size: 0.85em;
      '>${status}</span>`;
}


function enhanceStatusBadges() {
    console.log("enhanceStatusBadges")
    $(".status-badge").each(function () {
        const status = $(this).data("status");
        $(this).replaceWith(renderStatusBadge(status));
    });
}

////Cell size auto grid responsive mobile to pc  
//$(function () {
//    const grid = $("#grid").data("kendoGrid");
//    console.log("enhanceStatusBadges : " + grid);
//    if (grid) {
//        grid.bind("dataBound", function () {
//            enhanceStatusBadges();
//        });
//    }
//});

function autoFitGridColumns(gridId) {
    var grid = $("#" + gridId).data("kendoGrid");
    if (!grid) return;

    // Temporarily set table-layout to auto
    $(grid.table).css("table-layout", "auto");

    grid.columns.forEach((col, index) => {
        grid.autoFitColumn(index);
    });
}

// Reusable function to dynamically adjust grid page size based on available height
function adjustGridPageSize(gridId) {
    if (!gridId) {
        return;
    }

    var gridElement = $("#" + gridId);
    if (!gridElement.length) {
        return;
    }

    var grid = gridElement.data("kendoGrid");
    if (!grid || !grid.dataSource) {
        return;
    }

    // Wait a moment for layout to settle (especially after resize)
    setTimeout(function() {
        try {
            // Get grid container height - try multiple methods
            var gridHeight = gridElement.height();
            if (!gridHeight || gridHeight <= 0) {
                // Try outerHeight
                gridHeight = gridElement.outerHeight();
                if (!gridHeight || gridHeight <= 0) {
                    // Try getting height from parent container
                    var parentHeight = gridElement.parent().height();
                    if (parentHeight && parentHeight > 0) {
                        gridHeight = parentHeight;
                    } else {
                        // Try parent outerHeight
                        parentHeight = gridElement.parent().outerHeight();
                        if (parentHeight && parentHeight > 0) {
                            gridHeight = parentHeight;
                        } else {
                            return;
                        }
                    }
                }
            }
        
        // Get heights of grid components
        var toolbarHeight = gridElement.find(".k-grid-toolbar").outerHeight() || 0;
        var headerHeight = gridElement.find(".k-grid-header").outerHeight() || 0;
        var pagerHeight = gridElement.find(".k-pager-wrap").outerHeight() || 0;
        var filterRowHeight = gridElement.find(".k-grid-header .k-filter-row").outerHeight() || 0;
        
        // Calculate available height for data rows
        var availableHeight = gridHeight - toolbarHeight - headerHeight - pagerHeight - filterRowHeight - 10; // 10px buffer
        
        if (availableHeight <= 0) {
            return;
        }
        
        // Estimate row height (typically around 30-35px per row)
        var estimatedRowHeight = 35;
        
        // Try to get actual row height if rows exist
        var firstRow = gridElement.find(".k-grid-content tbody tr:first");
        if (firstRow.length) {
            var actualRowHeight = firstRow.outerHeight();
            if (actualRowHeight && actualRowHeight > 0) {
                estimatedRowHeight = actualRowHeight;
            }
        }
        
        // Calculate optimal page size (ensure at least 1 row)
        var optimalPageSize = Math.max(1, Math.floor(availableHeight / estimatedRowHeight));
        
        // Set reasonable limits
        optimalPageSize = Math.min(optimalPageSize, 1000); // Max 1000 rows
        optimalPageSize = Math.max(optimalPageSize, 5); // Min 5 rows
        
        // Get current page size
        var currentPageSize = grid.dataSource.pageSize();
        
        // Only update if different (with a small tolerance to avoid unnecessary updates)
        if (Math.abs(currentPageSize - optimalPageSize) > 0 && optimalPageSize > 0) {
            console.log("Adjusting page size for grid '" + gridId + "' from " + currentPageSize + " to " + optimalPageSize + " (available height: " + availableHeight + "px, row height: " + estimatedRowHeight + "px)");
            
            // Temporarily unbind dataBound to prevent infinite loop
            var dataBoundHandler = grid._events && grid._events.dataBound ? grid._events.dataBound[0] : null;
            
            // Update page size
            grid.dataSource.pageSize(optimalPageSize);
            
            // Update the pager dropdown if it exists
            var pagerSelect = gridElement.find(".k-pager-sizes select");
            if (pagerSelect.length) {
                pagerSelect.val(optimalPageSize);
                pagerSelect.trigger("change");
            }
            
            // Reset to first page
            var currentPage = grid.dataSource.page();
            if (currentPage !== 1) {
                grid.dataSource.page(1);
            } else {
                // If already on page 1, just read the data to apply new page size
                grid.dataSource.read();
            }
        }
    } catch (e) {
        console.log("Error adjusting grid page size for " + gridId + ":", e);
    }
    }, 50);
}

function onGridDataBound(e) {
    var gridElement = e.sender.element; // This is a jQuery object
    var gridId = gridElement.attr("id"); // This gives you the ID of the grid

    console.log("Grid dataBound: " + gridId);

    enhanceStatusBadges();
    autoFitGridColumns(gridId);
    
    // Automatically adjust page size for full-height grids
    if (gridElement.hasClass("k-grid-full-height")) {
        // Use timeout to ensure grid is fully rendered
        setTimeout(function () {
            adjustGridPageSize(gridId);
        }, 300);
    }

    //var grid = $("#grid").data("kendoGrid");
    //if (grid) {
    //    // Optional: hide columns if needed
    //    grid.hideColumn("Code");
    //    grid.hideColumn("Description");
    //}

    //// Center-align cells
    //$(".k-grid td").css("text-align", "center");
    //$(".k-grid-header th").addClass("k-text-center !k-justify-content-center");

    //// Wrap grid in scrollable container if not already wrapped
    //var $gridElement = $("#grid");
    //if ($gridElement.length && !$gridElement.parent().hasClass("k-grid-wrapper")) {
    //    $gridElement.wrap('<div class="k-grid-wrapper" style="overflow-x:auto;"></div>');
    //}
}

//function onPdfExport(e) {
//    e.preventDefault(); // Cancel default export

//    var grid = $("#grid").data("kendoGrid");
//    var originalPageSize = grid.dataSource.pageSize();

//    // Fetch all data manually
//    grid.dataSource.query({
//        page: 1,
//        pageSize: grid.dataSource.total(), // Fetch all records
//        sort: grid.dataSource.sort()
//    }).then(function () {
//        // Export after data is loaded
//        grid.saveAsPDF();

//        // Restore original page size without triggering another read
//        setTimeout(function () {
//            grid.dataSource.query({
//                page: 1,
//                pageSize: originalPageSize,
//                sort: grid.dataSource.sort()
//            });
//        }, 1000);
//    });
//}



function onPdfExport(e) {
    var grid = $("#grid").data("kendoGrid");

    // Hide Actions column
    var actionsColumnIndex = grid.thead.find("th:contains('Actions')").index();
    grid.hideColumn(actionsColumnIndex);

    // Hide filter row
    $(".k-filter-row").hide();

    // Hide column menu icons
    $(".k-header-column-menu").hide();

    // Restore everything after export
    setTimeout(function () {
        grid.showColumn(actionsColumnIndex);
        $(".k-filter-row").show();
        $(".k-header-column-menu").show();
    }, 1000);
}

// Enhanced error handler for grids
// This function is now defined in errorHandler.js, but kept here for backward compatibility
// If errorHandler.js is loaded, it will override this function
if (typeof window.onGridError === 'undefined') {
window.onGridError = function (e) {
    console.log("Grid error:", e);

    if (e.errors) {
        let message = "";
        $.each(e.errors, function (key, value) {
            if (value && value.errors) {
                $.each(value.errors, function (_, error) {
                    message += error + "\n";
                });
            }
            else if (Array.isArray(value)) {
                // Sometimes errors come as an array of strings
                value.forEach(function (error) {
                    message += error + "\n";
                });
            }
            else if (typeof value === 'string') {
                // Sometimes error is a direct string
                message += value + "\n";
            }
        });

        if (message.trim()) {
            bootbox.alert({
                title: "Error",
                message: message.trim(),
                centerVertical: true
            });
        }
    } else if (e.xhr) {
        // Handle HTTP errors (404, 500, etc.)
        let errorMessage = "An error occurred while loading data.";
        
        if (e.xhr.responseJSON && e.xhr.responseJSON.Message) {
            errorMessage = e.xhr.responseJSON.Message;
        } else if (e.xhr.responseJSON && e.xhr.responseJSON.message) {
            errorMessage = e.xhr.responseJSON.message;
        } else if (e.xhr.responseText) {
            try {
                const response = JSON.parse(e.xhr.responseText);
                if (response.Message) {
                    errorMessage = response.Message;
                } else if (response.message) {
                    errorMessage = response.message;
                }
            } catch (ex) {
                // If parsing fails, use status text
                errorMessage = e.xhr.statusText || errorMessage;
            }
        }

        bootbox.alert({
            title: "Error",
            message: errorMessage,
            centerVertical: true
        });
    }
};
}

// Function to attach error handler to a specific grid
function attachGridErrorHandler(gridElement) {
    if (!gridElement) return;
    
    var grid = $(gridElement).data("kendoGrid");
    if (grid && grid.dataSource) {
        // Unbind any existing error handlers to avoid duplicates
        grid.dataSource.unbind("error", onGridError);
        // Bind the error handler
        grid.dataSource.bind("error", onGridError);
    }
}

// Automatically attach error handler to all Grid components on the page
$(document).ready(function () {
    // Function to process all grids
    function processAllGrids() {
        // Find all elements that might be Grid
        $("div[data-role='grid'], table[data-role='grid']").each(function () {
            attachGridErrorHandler(this);
        });
        
        // Also try finding by class (Kendo adds specific classes)
        $(".k-grid").each(function () {
            attachGridErrorHandler(this);
        });
    }

    // Process immediately
    processAllGrids();

    // Process after a short delay to catch grids initialized after DOM ready
    setTimeout(processAllGrids, 500);
    setTimeout(processAllGrids, 1000);

    // Listen for Kendo ready events
    if (typeof kendo !== "undefined") {
        $(document).on("kendoReady", function () {
            processAllGrids();
        });
    }

    // Use MutationObserver to catch dynamically added grids
    if (typeof MutationObserver !== "undefined") {
        var observer = new MutationObserver(function (mutations) {
            var shouldProcess = false;
            mutations.forEach(function (mutation) {
                if (mutation.addedNodes.length > 0) {
                    shouldProcess = true;
                }
            });
            if (shouldProcess) {
                setTimeout(processAllGrids, 100);
            }
        });

        observer.observe(document.body, {
            childList: true,
            subtree: true
        });
    }

    // Function to process all grids for page size adjustment
    function processAllGridsForPageSize() {
        // Find grids by class
        $(".k-grid-full-height").each(function () {
            var gridId = $(this).attr("id");
            if (gridId) {
                var grid = $(this).data("kendoGrid");
                if (grid && grid.dataSource) {
                    adjustGridPageSize(gridId);
                }
            }
        });
        
        // Also find grids by data-role attribute
        $("div[data-role='grid'].k-grid-full-height, table[data-role='grid'].k-grid-full-height").each(function () {
            var gridId = $(this).attr("id");
            if (gridId) {
                var grid = $(this).data("kendoGrid");
                if (grid && grid.dataSource) {
                    adjustGridPageSize(gridId);
                }
            }
        });
    }

    // Adjust page size for all full-height grids on window resize
    var resizeTimer;
    $(window).on('resize', function () {
        clearTimeout(resizeTimer);
        resizeTimer = setTimeout(function () {
            console.log("Window resized - adjusting grid page sizes");
            processAllGridsForPageSize();
        }, 250);
    });

    // Adjust page size for all full-height grids on initial load (multiple attempts for reliability)
    setTimeout(function () {
        console.log("Initial load - adjusting grid page sizes (attempt 1)");
        processAllGridsForPageSize();
    }, 500);
    
    setTimeout(function () {
        console.log("Initial load - adjusting grid page sizes (attempt 2)");
        processAllGridsForPageSize();
    }, 1000);
    
    setTimeout(function () {
        console.log("Initial load - adjusting grid page sizes (attempt 3)");
        processAllGridsForPageSize();
    }, 2000);

    // Use MutationObserver to catch dynamically added grids and apply page size adjustment
    if (typeof MutationObserver !== "undefined") {
        var observer = new MutationObserver(function (mutations) {
            var shouldProcess = false;
            mutations.forEach(function (mutation) {
                if (mutation.addedNodes.length > 0) {
                    mutation.addedNodes.forEach(function (node) {
                        if ($(node).is(".k-grid.k-grid-full-height") || $(node).find(".k-grid.k-grid-full-height").length) {
                            shouldProcess = true;
                        }
                    });
                }
            });
            if (shouldProcess) {
                setTimeout(processAllGridsForPageSize, 100);
            }
        });

        observer.observe(document.body, {
            childList: true,
            subtree: true
        });
    }
});

