(function (global, factory) {
	typeof exports === 'object' && typeof module !== 'undefined' ? factory(exports) :
		typeof define === 'function' && define.amd ? define('TextBox', ['exports'], factory) :
			(factory((global.TextBox = {})));
}(this, (function (exports) {
	'use strict';

	// Hack error when trying to submit multiple time
	// Replace css class govuk-error-message with govuk-error-message--alt
	// Form validator function MOJFrontend.FormValidator.prototype.removeInlineError
	// fieldContainer.find('.govuk-error-message').remove();
	// Removes dom element with characters too many
	
	function Closure(maxWordCount) {
		
		function replaceErrorMessage() {
			$("#case-outcomes-info").addClass("govuk-hint govuk-error-message--alt").removeClass("govuk-error-message");
		}
		function removeErrorMessage() {
			$("#case-outcomes-info").removeClass("govuk-error-message--alt");
		}

		$("#case-outcomes").keyup(function() {
			if ($(this).val().length > maxWordCount) {
				replaceErrorMessage();
			} else {
				removeErrorMessage();
			}
		});
	}

	function Issue(maxWordCount) {

		function replaceErrorMessage() {
			$("#issue-info").addClass("govuk-hint govuk-error-message--alt").removeClass("govuk-error-message");
		}
		function removeErrorMessage() {
			$("#issue-info").removeClass("govuk-error-message--alt");
		}

		$("#issue").keyup(function() {
			if ($(this).val().length > maxWordCount) {
				replaceErrorMessage();
			} else {
				removeErrorMessage();
			}
		});
	}

	function CurrentStatus(maxWordCount) {

		function replaceErrorMessage() {
			$("#current-status-info").addClass("govuk-hint govuk-error-message--alt").removeClass("govuk-error-message");
		}
		function removeErrorMessage() {
			$("#current-status-info").removeClass("govuk-error-message--alt");
		}

		$("#current-status").keyup(function() {
			if ($(this).val().length > maxWordCount) {
				replaceErrorMessage();
			} else {
				removeErrorMessage();
			}
		});
	}

	function CaseAim(maxWordCount) {

		function replaceErrorMessage() {
			$("#case-aim-info").addClass("govuk-hint govuk-error-message--alt").removeClass("govuk-error-message");
		}
		function removeErrorMessage() {
			$("#case-aim-info").removeClass("govuk-error-message--alt");
		}

		$("#case-aim").keyup(function() {
			if ($(this).val().length > maxWordCount) {
				replaceErrorMessage();
			} else {
				removeErrorMessage();
			}
		});
	}

	function DeEscalationPoint(maxWordCount) {

		function replaceErrorMessage() {
			$("#de-escalation-point-info").addClass("govuk-hint govuk-error-message--alt").removeClass("govuk-error-message");
		}
		function removeErrorMessage() {
			$("#de-escalation-point-info").removeClass("govuk-error-message--alt");
		}

		$("#de-escalation-point").keyup(function() {
			if ($(this).val().length > maxWordCount) {
				replaceErrorMessage();
			} else {
				removeErrorMessage();
			}
		});
	}

	function NextSteps(maxWordCount) {

		function replaceErrorMessage() {
			$("#next-steps-info").addClass("govuk-hint govuk-error-message--alt").removeClass("govuk-error-message");
		}
		function removeErrorMessage() {
			$("#next-steps-info").removeClass("govuk-error-message--alt");
		}

		$("#next-steps").keyup(function() {
			if ($(this).val().length > maxWordCount) {
				replaceErrorMessage();
			} else {
				removeErrorMessage();
			}
		});
	}
	
	exports.Closure = Closure;
	exports.Issue = Issue;
	exports.CurrentStatus = CurrentStatus;
	exports.CaseAim = CaseAim;
	exports.DeEscalationPoint = DeEscalationPoint;
	exports.NextSteps = NextSteps;
	
})));