// Professional Error Handler for the entire application
// Handles errors from AJAX requests, grids, combo boxes, and general exceptions

(function ($) {
    'use strict';

    // Error types and their configurations
    const ErrorConfig = {
        ERROR: {
            icon: 'fa-exclamation-circle',
            title: 'Error',
            className: 'text-danger'
        },
        WARNING: {
            icon: 'fa-exclamation-triangle',
            title: 'Warning',
            className: 'text-warning'
        },
        INFO: {
            icon: 'fa-info-circle',
            title: 'Information',
            className: 'text-info'
        }
    };

    // Track handled errors to prevent duplicate dialogs
    const handledErrors = new Set();
    
    /**
     * Generates a unique key for an error to track if it's been handled
     */
    function getErrorKey(xhr) {
        if (!xhr) return null;
        // Use URL and a more stable identifier to create unique key
        // Use responseURL if available, otherwise use a combination
        const url = xhr.responseURL || (xhr.status ? `${xhr.status}_${xhr.statusText}` : '');
        // Use a shorter timestamp window (rounded to seconds) to group similar errors
        const timestamp = Math.floor(new Date().getTime() / 1000);
        return `${url}_${timestamp}`;
    }

    /**
     * Shows a professional error dialog using bootbox with enhanced UX
     * @param {string} message - Error message to display
     * @param {string} title - Title of the dialog (default: "Error")
     * @param {string} icon - FontAwesome icon class (default: "fa-exclamation-circle")
     * @param {string} errorKey - Unique key to prevent duplicate dialogs
     */
    function showErrorDialog(message, title, icon, errorKey) {
        // Prevent duplicate error dialogs
        // But allow specific handlers (_combobox, _grid) to always show
        const isSpecificHandler = errorKey && (errorKey.endsWith('_combobox') || errorKey.endsWith('_grid'));
        
        if (errorKey && !isSpecificHandler && handledErrors.has(errorKey)) {
            return;
        }
        
        if (errorKey) {
            handledErrors.add(errorKey);
            // Clean up old error keys after 5 seconds
            setTimeout(function() {
                handledErrors.delete(errorKey);
            }, 5000);
        }

        title = title || ErrorConfig.ERROR.title;
        icon = icon || ErrorConfig.ERROR.icon;

        // Determine dialog class and styling based on error type
        let dialogClass = 'bootbox-error-dialog';
        let alertClass = 'alert-danger';
        let iconColor = '#dc3545';
        let buttonClass = 'btn-danger';
        
        if (title === ErrorConfig.WARNING.title) {
            dialogClass = 'bootbox-warning-dialog';
            alertClass = 'alert-warning';
            iconColor = '#ffc107';
            buttonClass = 'btn-warning';
        } else if (title === ErrorConfig.INFO.title) {
            dialogClass = 'bootbox-info-dialog';
            alertClass = 'alert-info';
            iconColor = '#17a2b8';
            buttonClass = 'btn-info';
        }

        // Create professional error dialog with enhanced UX
        // Message text is in regular color, not red
        const messageHtml = `
            <div class="text-center py-3">
                <div class="mb-3">
                    <i class="fas ${icon} fa-4x" style="color: ${iconColor};"></i>
                </div>
                <h5 class="mb-3" style="color: ${iconColor}; font-weight: 600;">${title}</h5>
                <div class="alert ${alertClass}" role="alert" style="text-align: left; margin: 0;">
                    <p class="mb-0" style="white-space: pre-wrap; word-wrap: break-word; line-height: 1.6; color: #212529;">${escapeHtml(message)}</p>
                </div>
            </div>
        `;

        bootbox.alert({
            title: `<i class="fas ${icon}"></i> ${title}`,
            message: messageHtml,
            centerVertical: true,
            size: 'large',
            backdrop: true,
            className: dialogClass,
            buttons: {
                ok: {
                    label: 'OK',
                    className: buttonClass
                }
            },
            callback: function () {
                // Optional: Add any cleanup or additional actions
            }
        });
    }

    /**
     * Escapes HTML to prevent XSS attacks
     */
    function escapeHtml(text) {
        const map = {
            '&': '&amp;',
            '<': '&lt;',
            '>': '&gt;',
            '"': '&quot;',
            "'": '&#039;'
        };
        return text.replace(/[&<>"']/g, function (m) { return map[m]; });
    }

    /**
     * Handles AJAX errors globally
     * Only handles errors that haven't been handled by specific handlers (Grid/ComboBox)
     */
    function handleAjaxError(event, jqXHR, ajaxSettings, thrownError) {
        // Skip if error is already handled or should be skipped
        if (jqXHR.status === 0 || ajaxSettings.skipGlobalErrorHandler) {
            return;
        }

        // Skip if this is a Kendo DataSource error (handled by specific handlers)
        // Kendo DataSource errors are handled by onGridError or onComboBoxError
        // Check for Kendo-specific request patterns
        const url = ajaxSettings.url || '';
        const data = ajaxSettings.data || '';
        const dataString = typeof data === 'string' ? data : JSON.stringify(data || {});
        
        // Common Kendo DataSource endpoint patterns (Grids and ComboBoxes)
        const kendoPatterns = [
            'GetIndexViewModels', 'GetIndexViewDtos', 'GetIndexDtos', 'GetIndexViewDto',
            'GetSuppliers', 'GetProducts', 'GetCustomers', 'GetItems', 'GetMiscMasters',
            'GetUsers', 'GetGrns', 'GetPoNos', 'GetBatchNos', 'GetFamilies',
            'GetCustomerProducts', 'GetProductPackingTypes', 'GetProjectElements',
            'GetSupplierIdoPlans', 'GetGaIds', 'GetPoProducts', 'ReadDimensions',
            'GetPackingListNos', 'GetPlanNos', 'GetLotNos'
        ];
        
        const isKendoDataSource = url && (
            kendoPatterns.some(pattern => url.indexOf(pattern) !== -1) ||
            (dataString.indexOf('DataSourceRequest') !== -1) ||
            (dataString.indexOf('"take"') !== -1 && dataString.indexOf('"skip"') !== -1) // Kendo paging parameters
        );
        
        const errorKey = getErrorKey(jqXHR);
        
        // If this is a Kendo DataSource request, skip global handler
        // The specific handler (onGridError/onComboBoxError) will handle it
        // DO NOT mark as handled here - let the specific handler do it
        if (isKendoDataSource) {
            // Just skip - don't mark as handled, let specific handler show the dialog
            return;
        }
        
        // Also check if error was already handled by a specific handler
        if (errorKey && handledErrors.has(errorKey)) {
            return;
        }

        let errorMessage = "An unexpected error occurred. Please try again.";

        // First, try to get error message from response header (X-Error-Message)
        var errorHeader = jqXHR.getResponseHeader ? jqXHR.getResponseHeader("X-Error-Message") : null;
        if (errorHeader) {
            errorMessage = errorHeader;
        }
        // Then try to extract error message from response body
        else if (jqXHR.responseJSON) {
            if (jqXHR.responseJSON.error && jqXHR.responseJSON.message) {
                errorMessage = jqXHR.responseJSON.message;
            } else if (jqXHR.responseJSON.Message) {
                errorMessage = jqXHR.responseJSON.Message;
            } else if (jqXHR.responseJSON.message) {
                errorMessage = jqXHR.responseJSON.message;
            } else if (typeof jqXHR.responseJSON === 'string') {
                errorMessage = jqXHR.responseJSON;
            }
        } else if (jqXHR.responseText) {
            try {
                const response = JSON.parse(jqXHR.responseText);
                if (response.error && response.message) {
                    errorMessage = response.message;
                } else if (response.Message) {
                    errorMessage = response.Message;
                } else if (response.message) {
                    errorMessage = response.message;
                }
            } catch (e) {
                // If parsing fails, use status text
                if (jqXHR.statusText) {
                    errorMessage = jqXHR.statusText;
                }
            }
        } else if (jqXHR.statusText) {
            errorMessage = jqXHR.statusText;
        }

        // Handle specific HTTP status codes (only if we don't have a specific message)
        if (!errorHeader && errorMessage === "An unexpected error occurred. Please try again.") {
            switch (jqXHR.status) {
                case 401:
                    errorMessage = "You are not authorized to perform this action. Please login again.";
                    break;
                case 403:
                    errorMessage = "Access denied. You don't have permission to perform this action.";
                    break;
                case 404:
                    errorMessage = "The requested resource was not found.";
                    break;
                case 500:
                    errorMessage = "A server error occurred. Please contact support if the problem persists.";
                    break;
                case 503:
                    errorMessage = "Service temporarily unavailable. Please try again later.";
                    break;
            }
        }

        showErrorDialog(errorMessage, ErrorConfig.ERROR.title, ErrorConfig.ERROR.icon, errorKey);
    }

    /**
     * Enhanced error handler for combo boxes
     */
    window.onComboBoxError = function (e) {
        console.log("ComboBox error:", e);

        const errorKey = getErrorKey(e.xhr);
        
        // Mark as handled FIRST to prevent global handler from showing duplicate
        // This must happen before showing dialog to prevent race condition
        if (errorKey) {
            handledErrors.add(errorKey);
        }

        // Always show the dialog for combobox errors - this is the specific handler
        let shouldShowDialog = false;
        let errorMessage = "An error occurred while loading data.";

        if (e.errors) {
            let message = "";
            $.each(e.errors, function (key, value) {
                if (value && value.errors) {
                    $.each(value.errors, function (_, error) {
                        message += error + "\n";
                    });
                } else if (Array.isArray(value)) {
                    value.forEach(function (error) {
                        message += error + "\n";
                    });
                } else if (typeof value === 'string') {
                    message += value + "\n";
                }
            });

            if (message.trim()) {
                errorMessage = message.trim();
                shouldShowDialog = true;
            }
        } else if (e.xhr) {
            // Always show dialog for combobox errors if xhr exists
            shouldShowDialog = true;
            
            // Check HTTP status code
            const status = e.xhr.status || (e.status === 'error' ? 500 : 0);
            
            if (status >= 400 || e.status === 'error') {
                // First, try to get error message from response header (X-Error-Message)
                var errorHeader = e.xhr.getResponseHeader ? e.xhr.getResponseHeader("X-Error-Message") : null;
                if (errorHeader) {
                    errorMessage = errorHeader;
                }
                // Then try to get error message from response body
                else if (e.xhr.responseJSON) {
                    if (e.xhr.responseJSON.error && e.xhr.responseJSON.message) {
                        errorMessage = e.xhr.responseJSON.message;
                    } else if (e.xhr.responseJSON.Message) {
                        errorMessage = e.xhr.responseJSON.Message;
                    } else if (e.xhr.responseJSON.message) {
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
                        // If parsing fails, use status text or a generic message
                        if (status === 500 || e.status === 'error') {
                            errorMessage = e.errorThrown || "A server error occurred. Please try again or contact support.";
                        } else {
                            errorMessage = e.xhr.statusText || e.errorThrown || errorMessage;
                        }
                    }
                } else {
                    // For 500 errors without response body, use errorThrown or generic message
                    if (status === 500 || e.status === 'error') {
                        errorMessage = e.errorThrown || "A server error occurred. Please try again or contact support.";
                    } else {
                        errorMessage = e.xhr.statusText || e.errorThrown || errorMessage;
                    }
                }
            }
        }

        // Always show dialog for combobox errors (this is the specific handler)
        if (shouldShowDialog) {
            // Use a different error key to ensure dialog shows even if global handler marked it
            const dialogKey = errorKey ? errorKey + '_combobox' : null;
            showErrorDialog(errorMessage, ErrorConfig.ERROR.title, ErrorConfig.ERROR.icon, dialogKey);
        }
    };

    /**
     * Enhanced error handler for grids
     */
    window.onGridError = function (e) {
        console.log("Grid error:", e);

        const errorKey = getErrorKey(e.xhr);
        
        // Mark as handled FIRST to prevent global handler from showing duplicate
        // This must happen before showing dialog to prevent race condition
        if (errorKey) {
            handledErrors.add(errorKey);
        }

        // Always show the dialog for grid errors - this is the specific handler
        let shouldShowDialog = false;
        let errorMessage = "An error occurred while loading data.";

        if (e.errors) {
            let message = "";
            $.each(e.errors, function (key, value) {
                if (value && value.errors) {
                    $.each(value.errors, function (_, error) {
                        message += error + "\n";
                    });
                } else if (Array.isArray(value)) {
                    value.forEach(function (error) {
                        message += error + "\n";
                    });
                } else if (typeof value === 'string') {
                    message += value + "\n";
                }
            });

            if (message.trim()) {
                errorMessage = message.trim();
                shouldShowDialog = true;
            }
        } else if (e.xhr) {
            // Check HTTP status code first
            if (e.xhr.status >= 400) {
                shouldShowDialog = true;
                
                // First, try to get error message from response header (X-Error-Message)
                var errorHeader = e.xhr.getResponseHeader ? e.xhr.getResponseHeader("X-Error-Message") : null;
                if (errorHeader) {
                    errorMessage = errorHeader;
                }
                // Then try to get error message from response body
                else if (e.xhr.responseJSON) {
                    if (e.xhr.responseJSON.error && e.xhr.responseJSON.message) {
                        errorMessage = e.xhr.responseJSON.message;
                    } else if (e.xhr.responseJSON.Message) {
                        errorMessage = e.xhr.responseJSON.Message;
                    } else if (e.xhr.responseJSON.message) {
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
                        // If parsing fails, use status text or a generic message
                        if (e.xhr.status === 500) {
                            errorMessage = "A server error occurred. Please try again or contact support.";
                        } else {
                            errorMessage = e.xhr.statusText || errorMessage;
                        }
                    }
                } else {
                    // For 500 errors without response body, show generic message
                    if (e.xhr.status === 500) {
                        errorMessage = "A server error occurred. Please try again or contact support.";
                    }
                }
            }
        }

        // Always show dialog for grid errors (this is the specific handler)
        if (shouldShowDialog || (e.xhr && e.xhr.status >= 400)) {
            // Use a different error key to ensure dialog shows even if global handler marked it
            const dialogKey = errorKey ? errorKey + '_grid' : null;
            showErrorDialog(errorMessage, ErrorConfig.ERROR.title, ErrorConfig.ERROR.icon, dialogKey);
        }
    };

    /**
     * Global error message function - can be called from anywhere in the application
     * Usage: showErrorMessage("Your error message here");
     *        showErrorMessage("Your error message", "Custom Title");
     *        showErrorMessage("Your error message", "Custom Title", "fa-exclamation-triangle");
     */
    window.showErrorMessage = function(message, title, icon) {
        if (!message) {
            console.warn("showErrorMessage called without a message");
            return;
        }
        showErrorDialog(message, title || ErrorConfig.ERROR.title, icon || ErrorConfig.ERROR.icon, null);
    };

    /**
     * Global warning message function
     * Usage: showWarningMessage("Your warning message here");
     */
    window.showWarningMessage = function(message, title, icon) {
        if (!message) {
            console.warn("showWarningMessage called without a message");
            return;
        }
        showErrorDialog(message, title || ErrorConfig.WARNING.title, icon || ErrorConfig.WARNING.icon, null);
    };

    /**
     * Global info message function
     * Usage: showInfoMessage("Your info message here");
     */
    window.showInfoMessage = function(message, title, icon) {
        if (!message) {
            console.warn("showInfoMessage called without a message");
            return;
        }
        showErrorDialog(message, title || ErrorConfig.INFO.title, icon || ErrorConfig.INFO.icon, null);
    };

    /**
     * Global success message function
     * Usage: showSuccessMessage("Your success message here");
     */
    window.showSuccessMessage = function(message, title, icon) {
        if (!message) {
            console.warn("showSuccessMessage called without a message");
            return;
        }
        // Use success-specific styling
        const messageHtml = `
            <div class="text-center py-3">
                <div class="mb-3">
                    <i class="fas ${icon || "fa-check-circle"} fa-4x" style="color: #28a745;"></i>
                </div>
                <h5 class="mb-3" style="color: #28a745; font-weight: 600;">${title || "Success"}</h5>
                <div class="alert alert-success" role="alert" style="text-align: left; margin: 0; border-left: 4px solid #28a745; background-color: #d4edda; border-color: #c3e6cb; color: #212529; padding: 15px; border-radius: 4px;">
                    <p class="mb-0" style="white-space: pre-wrap; word-wrap: break-word; line-height: 1.6; color: #212529;">${escapeHtml(message)}</p>
                </div>
            </div>
        `;
        
        bootbox.alert({
            title: `<i class="fas ${icon || "fa-check-circle"}"></i> ${title || "Success"}`,
            message: messageHtml,
            centerVertical: true,
            size: 'large',
            backdrop: true,
            className: 'bootbox-success-dialog',
            buttons: {
                ok: {
                    label: 'OK',
                    className: 'btn-success'
                }
            }
        });
    };

    // Initialize when document is ready
    $(document).ready(function () {
        // Set up global AJAX error handler
        $(document).ajaxError(handleAjaxError);

        // Override window.onerror for JavaScript errors
        window.onerror = function (message, source, lineno, colno, error) {
            console.error("JavaScript error:", message, source, lineno, colno, error);
            showErrorDialog(
                "A JavaScript error occurred: " + (error ? error.message : message),
                ErrorConfig.ERROR.title,
                ErrorConfig.ERROR.icon
            );
            return false; // Don't prevent default error handling
        };

        // Handle unhandled promise rejections
        window.addEventListener('unhandledrejection', function (event) {
            console.error("Unhandled promise rejection:", event.reason);
            const errorMessage = event.reason && event.reason.message 
                ? event.reason.message 
                : "An unexpected error occurred.";
            showErrorDialog(errorMessage, ErrorConfig.ERROR.title, ErrorConfig.ERROR.icon);
        });
    });

    // Export for use in other scripts
    window.showErrorDialog = showErrorDialog;

})(jQuery);

