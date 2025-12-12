// Page Load
document.addEventListener("DOMContentLoaded", function () {
    hideGlobalLoader(); // Hide loader once page is fully loaded
});

// Page Unload
window.addEventListener("beforeunload", function () {
    showGlobalLoader(); // Show loader before navigating away
});

// Fetch API Interception
const originalFetch = window.fetch;
window.fetch = function (...args) {
    showGlobalLoader(); // Show loader before request

    return originalFetch(...args)
        .then(response => response)
        .catch(error => { throw error; })
        .finally(() => {
            hideGlobalLoader(); // Hide loader after request
        });
};

// jQuery AJAX Global Handlers
$(document).ajaxStart(function () {
    showGlobalLoader(); // Show loader on any AJAX start
});

$(document).ajaxStop(function () {
    hideGlobalLoader(); // Hide loader when all AJAX calls complete
});

//// For all kendo 
//(function () {
//    const originalProgress = kendo.ui.progress;

//    kendo.ui.progress = function (container, show) {
//        if (show) {
//            showGlobalLoader();
//        } else {
//            hideGlobalLoader();
//        }

//        // Call the original method to preserve default behavior
//        return originalProgress.call(this, container, show);
//    };
//})();


// Loader Functions
function showGlobalLoader() {
    $("#globalLoaderWrapper").show();
    $("#globalLoader").data("kendoLoader").show();
}

function hideGlobalLoader() {
    $("#globalLoader").data("kendoLoader").hide();
    $("#globalLoaderWrapper").hide();
}

//document.addEventListener("DOMContentLoaded", function () {
//    hideGlobalLoader(); // Hide loader once page is fully loaded
//});

//window.addEventListener("beforeunload", function () {
//    showGlobalLoader(); // Show loader before navigating away
//});


//const originalFetch = window.fetch;
//window.fetch = function (...args) {
//    showGlobalLoader(); // Show loader before request

//    return originalFetch(...args)
//        .then(response => {
//            return response;
//        })
//        .catch(error => {
//            throw error;
//        })
//        .finally(() => {
//            hideGlobalLoader(); // Hide loader after request
//        });
//};

//function showGlobalLoader() {
//    $("#globalLoaderWrapper").show();
//    $("#globalLoader").data("kendoLoader").show();
//}

//function hideGlobalLoader() {
//    $("#globalLoader").data("kendoLoader").hide();
//    $("#globalLoaderWrapper").hide();
//}