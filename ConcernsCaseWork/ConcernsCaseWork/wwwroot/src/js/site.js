// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

import { initAll } from 'govuk-frontend';
initAll();

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

window.formValidatorConcernType = function(form) {
	const validator = formValidator(form);
	validator.addValidator('type', [{
		method: function(field) {
			return field.value.trim().length > 0;
		},
		message: 'Select concern type'
	}, {
		method: function() {
			let typeChecked = $('input[name="type"]:checked');
			if (typeChecked !== undefined) {
				let typeAriaControl = typeChecked.attr('aria-controls');
				if (typeAriaControl !== undefined) {
					let concernAriaControlElem = $("#" + typeAriaControl + "");
					let subTypeChildren = concernAriaControlElem.find("input[name='subType']");
					if (subTypeChildren.length > 0) {
						let subTypeChecked = concernAriaControlElem.find("input[name='subType']:checked");
						if (subTypeChecked.length === 0) {
							return false;
						}
					}
				}
			}

			return true;
		},
		message: 'Select concern sub type'
	}]);
	
	return validator;
}

