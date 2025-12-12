function index(dataItem, gridName = "grid") {
	//alert(gridName);
	if (undefined === gridName)
		gridName = "grid";
	var data = $('#' + gridName).data("kendoGrid").dataSource.data();
	return data.indexOf(dataItem);
}

function error_handler(e) {
	if (e.errors) {
		var message = "";//"Errors:<br>";
		var counter = 0;
		$.each(e.errors, function (key, value) {
			if ('errors' in value) {
				$.each(value.errors, function () {
					message += (++counter) + ". " + this + "<br>";
				});
			}
		});
		bootbox.alert({ title: "Errors", message: message });
	}
}

function ShowError(message) {
	bootbox.alert({
		title: "Error",
		message: message,
		centerVertical: true, // Center vertically (requires Bootstrap 4 or higher)
	});
};