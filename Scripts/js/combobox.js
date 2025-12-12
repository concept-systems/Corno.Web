// Common error handler for MultiColumnComboBox components
// This function is now defined in errorHandler.js, but kept here for backward compatibility
// If errorHandler.js is loaded, it will override this function
if (typeof window.onComboBoxError === 'undefined') {
window.onComboBoxError = function (e) {
    console.log("ComboBox error:", e);

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
        
        // Check if response contains error information from controller
        if (e.xhr.responseJSON) {
            // Controller returns { error: true, message: "..." }
            if (e.xhr.responseJSON.error && e.xhr.responseJSON.message) {
                errorMessage = e.xhr.responseJSON.message;
            }
            // Alternative format: { Message: "..." }
            else if (e.xhr.responseJSON.Message) {
                errorMessage = e.xhr.responseJSON.Message;
            }
            // Alternative format: { message: "..." }
            else if (e.xhr.responseJSON.message) {
                errorMessage = e.xhr.responseJSON.message;
            }
        } else if (e.xhr.responseText) {
            try {
                const response = JSON.parse(e.xhr.responseText);
                if (response.error && response.message) {
                    errorMessage = response.message;
                } else if (response.Message) {
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

// Function to attach error handler to a specific combo box
function attachComboBoxErrorHandler(comboBoxElement) {
    if (!comboBoxElement) return;
    
    var comboBox = $(comboBoxElement).data("kendoMultiColumnComboBox");
    if (comboBox && comboBox.dataSource) {
        // Unbind any existing error handlers to avoid duplicates
        comboBox.dataSource.unbind("error", onComboBoxError);
        // Bind the error handler
        comboBox.dataSource.bind("error", onComboBoxError);
        
    }
}

// Automatically attach error handler to all MultiColumnComboBox components on the page
$(document).ready(function () {
    // Function to process all combo boxes
    function processAllComboBoxes() {
        // Find all elements that might be MultiColumnComboBox
        $("input[data-role='multicolumncombobox'], select[data-role='multicolumncombobox']").each(function () {
            attachComboBoxErrorHandler(this);
        });
        
        // Also try finding by class (Kendo adds specific classes)
        $(".k-multicolumncombobox").each(function () {
            attachComboBoxErrorHandler(this);
        });
    }

    // Process immediately
    processAllComboBoxes();

    // Process after a short delay to catch combo boxes initialized after DOM ready
    setTimeout(processAllComboBoxes, 500);
    setTimeout(processAllComboBoxes, 1000);

    // Listen for Kendo ready events
    if (typeof kendo !== "undefined") {
        $(document).on("kendoReady", function () {
            processAllComboBoxes();
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
                setTimeout(processAllComboBoxes, 100);
            }
        });

        observer.observe(document.body, {
            childList: true,
            subtree: true
        });
    }
});

