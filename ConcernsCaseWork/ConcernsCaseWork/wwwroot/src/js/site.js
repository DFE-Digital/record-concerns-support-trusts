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
		message: 'Issue must be 2000 characters or less'
	}]);
}
window.addMeansOfReferralValidator = function (validator) {
	validator.addValidator('means-of-referral-id', [{
		method: function(field) {
			return field.value.trim().length > 0;
		},
		message: 'Select means of referral'
	}]);
}
window.addRatingValidator = function (validator) {
	validator.addValidator('rating', [{
		method: function(field) {
			return field.value.trim().length > 0;
		},
		message: 'Select risk rating'
	}]);
}
window.addConcernValidator = function(validator) {
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
				// Isn't ideal to check sub-type with type string 
				if (typeAriaControl !== undefined && typeChecked.val() !== "Force majeure") {
					let concernAriaControlElem = $("#" + typeAriaControl + "");
					let subTypeChildren = concernAriaControlElem.find("input[name='sub-type']");
					if (subTypeChildren.length > 0) {
						let subTypeChecked = concernAriaControlElem.find("input[name='sub-type']:checked");
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
			return field.value.trim().length <= 4000;
		},
		message: 'Current status must be 4000 characters or less'
	}]);
}
window.addNextStepsValidator = function(validator) {
	validator.addValidator('next-steps', [{
		method: function(field) {
			return field.value.trim().length <= 4000;
		},
		message: 'Next steps must be 4000 characters or less'
	}]);
}
window.addDeEscalationPointValidator = function(validator) {
	validator.addValidator('de-escalation-point', [{
		method: function(field) {
			return field.value.trim().length <= 1000;
		},
		message: 'De-escalation point must be 1000 characters or less'
	}]);
}
window.addCaseAimValidator = function(validator) {
	validator.addValidator('case-aim', [{
		method: function(field) {
			return field.value.trim().length <= 1000;
		},
		message: 'Case aim must be 1000 characters or less'
	}]);
}
window.addCaseHistoryValidator = function (validator) {
	validator.addValidator('case-history', [{
		method: function (field) {
			return field.value.trim().length <= 4300;
		},
		message: 'Case history must be 4300 characters or less'
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
window.addCaseActionValidator = function (validator) {
	validator.addValidator('action', [{
		method: function (field) {
			return field.value.trim().length > 0;
		},
		message: 'Please select an action to add.'
	}]);
}
window.addRolesValidator = function(validator) {
	validator.addValidator('role', [{
		method: function() {
			let checkboxRoles = $('input[name="role"]:checked');
			return checkboxRoles.length > 0;
		},
		message: 'Select role(s)'
	}]);
}
window.addUsersValidator = function(validator) {
	validator.addValidator('user', [{
		method: function() {
			let checkboxRoles = $('input[name="user"]:checked');
			return checkboxRoles.length > 0;
		},
		message: 'Select user(s)'
	}]);
}
window.addSRMANotesValidator = function (validator) {
	validator.addValidator('srma-notes', [{
		method: function (field) {
			return field.value.trim().length <= 2000;
		},
		message: 'Notes must be 2000 characters or less'
	}]);
}
window.addFinancialPlanNotesValidator = function (validator) {
	validator.addValidator('financial-plan-notes', [{
		method: function (field) {
			return field.value.trim().length <= 2000;
		},
		message: 'Notes must be 2000 characters or less'
	}]);
}

window.addNTINotesValidator = function (validator) {
	validator.addValidator('nti-notes', [{
		method: function (field) {
			return field.value.trim().length <= 2000;
		},
		message: 'Notes must be 2000 characters or less'
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