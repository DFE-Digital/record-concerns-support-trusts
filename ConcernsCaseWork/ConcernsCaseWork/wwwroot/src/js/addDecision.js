function formatCurrency(element) {

	let currencyFormatter = new Intl.NumberFormat("en-GB", {
		style: "currency",
		currency: "GBP"
	});

	const onlyNumbersRegex = /[^\d\.]/g;

	const amount = element.val().replace(onlyNumbersRegex, "");

	const formattedCurrency = currencyFormatter.format(amount).replace("£", "");
	element.val(formattedCurrency);
}

$(function () {

	$totalAmountRequestedObj = $('#total-amount-request');

	$(document).ready(function () {
		formatCurrency($totalAmountRequestedObj);
	});

	$totalAmountRequestedObj.focusout(function () {
		formatCurrency($totalAmountRequestedObj);
	});

	autoResizer();
});