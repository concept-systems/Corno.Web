let port; // Track the serial port connection globally
let reader; // Track serial port reader globally.
let weightDisplay;
let connectionStartTime;
let intervalId;
let isDisconnecting = false;

function getLastCharAscii(str) {
    if (str.length === 0) return null; // Handle empty string case
    return str.charCodeAt(str.length - 1); // Get ASCII value of the last character
}

function updateConnectionDuration() {
    if (connectionStartTime) {
        let elapsedTime = Math.floor((new Date() - connectionStartTime) / 1000);
        let hours = Math.floor(elapsedTime / 3600);
        let minutes = Math.floor((elapsedTime % 3600) / 60);
        let seconds = elapsedTime % 60;

        let formattedTime =
            String(hours).padStart(2, '0') + ":" +
            String(minutes).padStart(2, '0') + ":" +
            String(seconds).padStart(2, '0');

        document.getElementById("duration").innerText = "Connected for: " + formattedTime;
    }
}

async function connectSerialPort() {
    try {
        port = await navigator.serial.requestPort();
        await port.open({ baudRate: 9600 });
        console.log("Serial port connected.");

        // Change button attributes
        var button = $("#btnConnect").data("kendoButton");
        var buttonText = "Disconnect";// "<span class='k-icon k-i-pause k-button-icon'></span><span class='k-button-text'>Disconnect</span>";
        button.element.text(buttonText); // Change button text
        button.element.find(".k-icon").removeClass("k-i-play").addClass("k-i-pause"); // Change icon

        const textDecoder = new TextDecoderStream();
        const readableStreamClosed = port.readable.pipeTo(textDecoder.writable);
        reader = textDecoder.readable.getReader();

        // Show Connection Status
        document.getElementById("connectionStatus").innerText = "Connected";
        // Start a timer to update connection duration in HH:MM:SS format
        intervalId = setInterval(updateConnectionDuration, 1000);

        let buffer = '';

        while (isDisconnecting == false) {
            const { value, done } = await reader.read();
            if (done) break;
            if (value) {
                buffer += value; // Append incoming data
                //console.log("Data : " + buffer);
                //console.log("ASCII Value:", getLastCharAscii(buffer)); // Output: 57 (ASCII for '9')

                console.log("Data : " + buffer + ", Port : " + port);

                if (buffer.endsWith("\r")) {

                    let lines = buffer.split("\r"); // Split the buffer into lines
                    let lastLine = lines[lines.length - 2]; // Get the last non-empty line
                    document.getElementById('weight').innerText = lastLine;

                    // Load label
                    loadLabel(buffer);

                    buffer = '';
                }
            }
        }

        console.log("Came out of while loop. Releasing stream.");
        reader.releaseLock();
        console.log("Stream released.");
        await port.close();
        console.log("Port Closed.");
        /*// Handle disconnection
        navigator.serial.addEventListener("disconnect", () => {
            clearInterval(intervalId);
            document.getElementById("connectionStatus").innerText = "Disconnected";
        });*/
    } catch (error) {
        console.error("Failed to connect to serial port:", error);
    }
}

async function disconnectSerialPort() {
    try {
        console.log("Closing port : " + port);
        isDisconnecting = true; // Set flag

        await new Promise(resolve => setTimeout(resolve, 5000)); // Wait before ca

        /*if (reader) {
            await reader.cancel();  // Ensure read operation is canceled
            //await new Promise(resolve => setTimeout(resolve, 100)); // Small delay
            reader.releaseLock();
        }*/

        /*if (port) {
            await port.close();
            clearInterval(intervalId);
            document.getElementById("connectionStatus").innerText = "Disconnected";
            document.getElementById("duration").innerText = "";
            port = null;
        }

        console.log("Serial port closed.");*/
    } catch (error) {
        console.error("Error closing the serial port:", error);
    }
}