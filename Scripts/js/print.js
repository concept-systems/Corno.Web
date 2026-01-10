function base64ToBlob(base64, type) {
    const byteCharacters = atob(base64);
    const byteNumbers = new Array(byteCharacters.length);
    for (let i = 0; i < byteCharacters.length; i++) {
        byteNumbers[i] = byteCharacters.charCodeAt(i);
    }
    const byteArray = new Uint8Array(byteNumbers);
    return new Blob([byteArray], { type: type });
}

function loadPdf(base64, frameId) {
    const blob = base64ToBlob(base64, 'application/pdf');
    const url = URL.createObjectURL(blob);
    document.getElementById(frameId).src = url; // ✅ Same-origin Blob URL
}

function printIFramePdf(frameId) {
    const iframe = document.getElementById(frameId);
    if (iframe && iframe.contentWindow) {
        iframe.contentWindow.focus();
        iframe.contentWindow.print();
    }
}

function printBlob(blob, onPrintComplete) {
    console.log("in print job");
    const url = URL.createObjectURL(blob);
    const iframe = document.createElement('iframe');
    iframe.style.display = 'none';
    iframe.src = url;
    document.body.appendChild(iframe);

    iframe.onload = function() {
        console.log("printing job");
        iframe.contentWindow.focus();
        iframe.contentWindow.print();

        // ✅ After print is triggered, run custom logic
        if (typeof onPrintComplete === 'function') {
            onPrintComplete();
        }

        // Optional cleanup
        // document.body.removeChild(iframe);
        // URL.revokeObjectURL(url);
    };
}

function fetchCommon(url, formData, onSuccess) {
    fetch(url, {
        method: 'POST',
        body: formData
    })
        .then(async response => {
            if (!response.ok) {
                throw new Error('Failed to fetch report');
            }

            const contentType = response.headers.get('Content-Type');
            console.log("Content-Type:", contentType);

            if (contentType && contentType.includes('application/json')) {
                const data = await response.json();

                if (!data.Success) {
                    throw new Error(data.Message || 'Unknown error occurred');
                }

                if (typeof onSuccess === 'function') {
                    onSuccess(data); // Pass parsed data
                }
            } else {
                // If response is not JSON, handle accordingly
                if (typeof onSuccess === 'function') {
                    onSuccess(); // No data to pass
                }
            }
        })
        .catch(error => {
            showErrorMessage(error.message);
        });
}

function fetchAndPreviewPdf(url, formData, windowTitle, onCloseCallback) {
    fetch(url, {
        method: 'POST',
        body: formData
    })
        .then(async response => {
            if (!response.ok) throw new Error('Failed to fetch PDF : ' + url);

            const contentType = response.headers.get('Content-Type');
            if (contentType && contentType.includes('application/json')) {
                const errorData = await response.json();
                throw new Error(errorData.Message || 'Unknown error occurred');
            }

            const blob = await response.blob();
            const pdfUrl = URL.createObjectURL(blob);
            showPdfInTelerikWindow(pdfUrl, windowTitle, onCloseCallback);
        })
        .catch(error => {
            showErrorMessage(error.message);
        });
}

function fetchAndPrint(url, formData, onPrintComplete) {
    fetch(url, {
        method: 'POST',
        body: formData
    })
        .then(async response => {
            if (!response.ok) {
                throw new Error('Failed to fetch report');
            }

            const contentType = response.headers.get('Content-Type');
            console.log("Content-Type:", contentType);

            if (contentType && contentType.includes('application/json')) {
                const errorData = await response.json();
                throw new Error(errorData.Message || 'Unknown error occurred');
            }

            const blob = await response.blob();
            printBlob(blob, onPrintComplete); // ✅ Pass callback to printBlob
        })
        .catch(error => {
            showErrorMessage(error.message);
        });
}

//function fetchAndPreviewPdf(url, formData, windowTitle) {
//    fetch(url, {
//        method: 'POST',
//        body: formData
//    })
//        .then(response => {
//            if (!response.ok) throw new Error('Failed to fetch PDF');
//            return response.blob(); // ✅ Get Blob directly
//        })
//        .then(blob => {
//            if (blob.Success === false) {
//                showErrorMessage(blob.Message);
//            }
//            else {
//                const pdfUrl = URL.createObjectURL(blob);
//                showPdfInTelerikWindow(pdfUrl, windowTitle);
//            }
//        })
//        .catch(error => {
//            showErrorMessage(error.message);
//        });
//}

//function fetchAndPrint(url, formData, onSuccess) {
//    fetch(url, {
//        method: 'POST',
//        body: formData
//    })
//        .then(response => {
//            if (!response.ok) {
//                throw new Error('Failed to fetch report');
//            }
//            return response.blob();
//        })
//        .then(blob => {
//            if (blob.Success === false) {
//                showErrorMessage(blob.Message);
//            }
//            else {
//                printBlob(blob);
//                if (typeof onSuccess === 'function') {
//                    onSuccess();
//                }
//            }
//        })
//        .catch(error => {
//            showErrorMessage(error.message);
//        });
//}
function showPdfInTelerikWindow(pdfUrl, title, onCloseCallback) {
    const sanitizedUrl = `${pdfUrl}#toolbar=0`; // 👈 Disable toolbar (no save/print)

    const wnd = $("<div />").kendoWindow({
        title: title || "PDF Preview",
        modal: true,
        width: "80%",
        height: "80%",
        actions: ["Close"],
        close: function () {
            URL.revokeObjectURL(pdfUrl); // ✅ Cleanup
            this.destroy();
            
            // ✅ Execute custom logic passed from the view
            if (typeof onCloseCallback === 'function') {
                onCloseCallback();
            }
        },
        open: function () {
            this.content(`
                <div style="width:100%; height:100%; overflow:hidden;">
                    <embed src="${sanitizedUrl}" type="application/pdf"
                           style="width:100%; height:100%; object-fit:contain;" />
                </div>
            `);
        }
    }).data("kendoWindow");

    wnd.center().open();
}

function printPdfFromIframe() {
    const iframe = document.getElementById('pdfPreviewFrame');
    if (iframe && iframe.contentWindow) {
        iframe.contentWindow.focus();
        iframe.contentWindow.print();
    }
}

//function ShowError(message) {
//    bootbox.alert({
//        title: "Error",
//        message: message,
//        centerVertical: true, // Center vertically (requires Bootstrap 4 or higher)
//    });
//};