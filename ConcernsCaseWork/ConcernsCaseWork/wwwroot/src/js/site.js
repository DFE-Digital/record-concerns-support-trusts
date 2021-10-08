// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

import GOVUKFrontend from "govuk-frontend/govuk/all";
GOVUKFrontend.initAll();
import MOJFrontend from "@ministryofjustice/frontend/moj/all";
MOJFrontend.initAll();

import { Closure, Issue, CurrentStatus, CaseAim, DeEscalationPoint, NextSteps } from './textbox';
Closure(200);
Issue(2000);
CurrentStatus(4000);
CaseAim(1000);
DeEscalationPoint(1000);
NextSteps(4000);

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
window.addIssueValidator = function(validator) {
	validator.addValidator('issue', [{
		method: function(field) {
			return field.value.trim().length > 0;
		},
		message: 'Issue is required'
	},
	{
		method: function(field) {
			return field.value.trim().length <= 2000;
		},
		message: 'You have exceeded one or more character limits'
	}]);
}
window.addRagRatingValidator = function(validator) {
	validator.addValidator('ragRating', [{
		method: function(field) {
			return field.value.trim().length > 0;
		},
		message: 'Select risk rating'
	}]);
}
window.addConcernTypeValidator = function(validator) {
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
}
window.addCurrentStatusValidator = function(validator) {
	validator.addValidator('current-status', [{
		method: function(field) {
			console.log("Current status validator");
			return field.value.trim().length <= 4000;
		},
		message: 'You have exceeded one or more character limits'
	}]);
}
window.addNextStepsValidator = function(validator) {
	validator.addValidator('next-steps', [{
		method: function(field) {
			return field.value.trim().length <= 4000;
		},
		message: 'You have exceeded one or more character limits'
	}]);
}
window.addDeEscalationPointValidator = function(validator) {
	validator.addValidator('de-escalation-point', [{
		method: function(field) {
			return field.value.trim().length <= 1000;
		},
		message: 'You have exceeded one or more character limits'
	}]);
}
window.addCaseAimValidator = function(validator) {
	validator.addValidator('case-aim', [{
		method: function(field) {
			return field.value.trim().length <= 1000;
		},
		message: 'You have exceeded one or more character limits'
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

// Auto resizer textBox
window.autoResizer = function() {
	let multipleFields = document.querySelectorAll('.concern-auto-resize');
	for(let i = 0; i < multipleFields.length; i++) {
		multipleFields[i].addEventListener('input', autoResizeHeight);
		
		// Force height when textarea contains data
		$(multipleFields[i]).height(multipleFields[i].scrollHeight);
	}
	
	// auto resize multiple textarea
	function autoResizeHeight() {
		this.style.height="auto";
		this.style.height= this.scrollHeight + "px";

		if ($("#" + this.id + "").val().length > 0) {
			this.style.borderColor="green";
		} else {
			this.style.borderColor="black";
		}
	}
}