// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

import GOVUKFrontend from "govuk-frontend/govuk/all";
GOVUKFrontend.initAll();
import MOJFrontend from "@ministryofjustice/frontend/moj/all";
MOJFrontend.initAll();

// We have our own version of the character counter copied from govuk-frontend
// The original cannot count characters for c# due to issues with new lines
// See the js file for more information
import CharacterCount from "./characterCounter";

let characterCounts = document.querySelectorAll('[data-module="govuk-concerns-character-count"]');

characterCounts.forEach((characterCount) => {
	new CharacterCount(characterCount, {}).init();
});

setScrollableErrorElements();
autoResizeTextAreaOnLoad();

function setScrollableErrorElements() {

	var elements = $('.scrollable-error');

	elements.each(function(){
		var id = $(this).data('scroll-to');
		var element = document.getElementById(id);

		// Skip to the next item as we can't find the id specified
		if (!element) return true;

		$(this).on('click', function () {
			onScrollInvoked(element);
		});
			
		$(this).on('keydown', function (event) {
			if (event.key == "Enter")
				onScrollInvoked(element);
		});
	});
}

function onScrollInvoked(element) {
	element.scrollIntoView({ behavior: 'smooth' });
}

// Write your JavaScript code.
window.showGlobalError = function() {
	$("#moj-banner-error").removeClass("govuk-!-display-none");
};
window.hideGlobalError = function() {
	$("#moj-banner-error").addClass("govuk-!-display-none");
};
window.showLoader = function() {
	$(".ccms-loader").removeClass("govuk-!-display-none");
};
window.hideLoader = function() {
	$(".ccms-loader").addClass("govuk-!-display-none");
};

// Form validator
window.formValidator = function(form) {
	return new MOJFrontend.FormValidator(form);
}

window.addRatingValidator = function (validator) {
	validator.addValidator('rating', [{
		method: function(field) {
			return field.value.trim().length > 0;
		},
		message: 'Select risk rating'
	}]);
}
window.addDirectionOfTravelValidator = function(validator) {
	validator.addValidator('direction-of-travel', [{
		method: function(field) {
			return field.value.trim().length > 0;
		},
		message: 'Select direction of travel'
	}]);
}
window.disableOnSubmit = function (buttonToDisable) {
	buttonToDisable.onclick = function () {
		setTimeout(() => {
			buttonToDisable.disabled = true;
		}, 0);

		// Renable the button a long period of time without action
		setTimeout(() => {
			buttonToDisable.disabled = false;
		}, 30000);
	}
}

window.disableOnSubmitWithJsValidationCheck = function(validator, buttonToDisable) {
	buttonToDisable.onclick = function () {
		setTimeout(() => {
			var isValid = validator.validate();
			if (isValid) {
				buttonToDisable.disabled = true;
			}
		}, 0);

		// Renable the button a long period of time without action
		setTimeout(() => {
			buttonToDisable.disabled = false;
		}, 30000);
	}
}

window.formatCurrency = function (element) {
	let currencyFormatter = new Intl.NumberFormat("en-GB", {
		style: "currency",
		currency: "GBP"
	});

	const onlyNumbersRegex = /[^\d\.]/g;

	const amount = element.val().replace(onlyNumbersRegex, "");

	// Replace the pound sign, it needs to be encoded for webpack to understand it
	const formattedCurrency = currencyFormatter.format(amount).replace(/\u00A3/g, "");
	element.val(formattedCurrency);
}

function autoResizeTextAreaOnLoad() {
	let multipleFields = document.querySelectorAll('.concern-auto-resize');
	for (let i = 0; i < multipleFields.length; i++) {
		// Force height when textarea contains data
		$(multipleFields[i]).height(multipleFields[i].scrollHeight);
	}
}

window.getFieldLengthWithNewLines = function(field) {
	// JS and C# treat new lines differently
	// JS treats a new line as one character \n
	// C# treats a new line has two characters \r\n
	// Since we use two different languages, we need to make sure they are counting correctly
	// This code switches javascript to count like C# (\r and \n)

	let numberOfNewLines = (field.match(/\n/g) || []).length;

	// Add the \r for each \n
	return field.length + numberOfNewLines;
}