function onGridError(e) {
    console.log(JSON.stringify(e.errors))
    if (e.errors) {
        var message = "Errors:\n\n";
        $.each(e.errors, function (key, value) {
            //console.log(JSON.stringify(value.Value.Errors))
            if (value.Value.Errors && Array.isArray(value.Value.Errors)) {
                $.each(value.Value.Errors, function () {
                    message += this.ErrorMessage + "\n"; // Extract the actual error message
                });
            }
        });
        //alert(message); // Display error message
        bootbox.alert({
            message: message,
            centerVertical: true
        });
    }
}