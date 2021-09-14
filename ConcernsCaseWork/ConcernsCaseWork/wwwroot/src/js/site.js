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

