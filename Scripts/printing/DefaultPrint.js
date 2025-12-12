function printBase64PDF(base64String) {
    var byteCharacters = atob(base64String);
    var byteNumbers = new Array(byteCharacters.length);
    for (var i = 0; i < byteCharacters.length; i++) {
        byteNumbers[i] = byteCharacters.charCodeAt(i);
    }
    var byteArray = new Uint8Array(byteNumbers);
    var blob = new Blob([byteArray], { type: 'application/pdf' });
    var blobUrl = URL.createObjectURL(blob);

    var iframe = document.createElement('iframe');
    iframe.style.display = 'none';
    document.body.appendChild(iframe);
    iframe.src = blobUrl;
    iframe.onload = function () {
        iframe.contentWindow.print();
    };
}