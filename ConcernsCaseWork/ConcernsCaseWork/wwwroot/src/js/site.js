// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
let showGlobalError = function(){
	$("#moj-banner-error").removeClass("govuk-!-display-none");
};
let hideGlobalError = function(){
	$("#moj-banner-error").addClass("govuk-!-display-none");
};
let showLoader = function(){
	$(".ccms-loader").removeClass("govuk-!-display-none");
};
let hideLoader = function(){
	$(".ccms-loader").addClass("govuk-!-display-none");
};

