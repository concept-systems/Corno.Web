
// Helper function to check WebSocket connection
function waitForJSPMConnection(callback) {
    if (JSPM.JSPrintManager.websocket_status === JSPM.WSStatus.Open) {
        callback();
    } else {
        JSPM.JSPrintManager.onStatusChanged = function () {
            if (JSPM.JSPrintManager.websocket_status === JSPM.WSStatus.Open)
                callback();
        };
    }
}


function directprint(base64Pdf, printerName, printerTrayName, printerPaperName) {
    // Ensure JSPrintManager is started
    JSPM.JSPrintManager.auto_reconnect = true;

    JSPM.JSPrintManager.start().then(function () {

        console.log("JSPM Started");
        // Wait for WebSocket connection
        waitForJSPMConnection(function () {
            console.log("WebSocket connected");

            // Create a print job
            let printJob = new JSPM.ClientPrintJob();

            // Set the printer - Default printer is used for silent print
            //printJob.clientPrinter = new JSPM.DefaultPrinter(); // It can also be a specific printer if needed
            //printJob.clientPrinter = new JSPM.InstalledPrinter("TSC TE210");
            //printJob.clientPrinter = new JSPM.InstalledPrinter("TSC TE210", false, "Continuous Roll", "New Stock");
            printJob.clientPrinter = new JSPM.InstalledPrinter(printerName, false, printerTrayName, printerPaperName);
            //printJob.clientPrinter.mediaType = "";
            //printJob.paperName = "New Stock";//'New Stock (20.0 mm x 20.0 mm)';
            //printJob.binaryPrinterCommands = '\x1D\x4C\x14\x14'; // Example ESC/POS command for 20mm x 20mm

            // Create the PDF file from base64
            let pdfFile = new JSPM.PrintFilePDF(
                base64Pdf,
                JSPM.FileSourceType.Base64,
                "label.pdf"
            );
            //pdfFile.pageSizing = JSPM.Sizing["None"];

            // Push the file into the print job
            printJob.files.push(pdfFile);

            //saveBase64PDF(base64Pdf, "C:\Development\client.pdf")

            //printJob.pdfContent = base64Pdf;

            // Send to client for silent print (no print dialog will be shown)
            printJob.sendToClient();

            console.log("Print job sent to printer silently.");
        });
    });
}

function saveBase64PDF(base64String, fileName) {
    // Convert Base64 string to a Blob
    const byteCharacters = atob(base64String);
    const byteNumbers = new Array(byteCharacters.length);
    for (let i = 0; i < byteCharacters.length; i++) {
        byteNumbers[i] = byteCharacters.charCodeAt(i);
    }
    const byteArray = new Uint8Array(byteNumbers);
    const blob = new Blob([byteArray], { type: 'application/pdf' });

    // Create a link element and trigger the download
    const link = document.createElement('a');
    link.href = URL.createObjectURL(blob);
    link.download = fileName;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
}

