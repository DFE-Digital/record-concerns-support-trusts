export class EditDecisionPage
{
    public withCrmEnquiry(crmNumber: string): this {
		cy.task("log", `With Crm enquiry ${crmNumber}`);

		cy.getById("crm-enquiry-number").clear().type(crmNumber);

		return this;
	}

	public withRetrospectiveRequest(isRetrospectiveRequest: boolean): this {
		cy.task("log", `With retrospective request ${isRetrospectiveRequest}`);

		if (isRetrospectiveRequest) {
			cy.getById("retrospective-approval").click();
		} else {
			cy.getById("retrospective-approval-1").click();
		}

		return this;
	}

	public withSubmissionRequired(isSubmissionRequired: Boolean): this {
		cy.task("log", `With Submission Required ${isSubmissionRequired}`);

		if (isSubmissionRequired) {
			cy.getById("submission-required").click();
		} else {
			cy.getById("submission-required-1").click();
		}

		return this;
	}

	public withSubmissionLink(submissionLink: string): this {
		cy.task("log", `With Submission link ${submissionLink}`);

		cy.getById("submission-document-link").clear().type(submissionLink);

		return this;
	}
	public withDateESFADay(dateESFADay: string): this {
		cy.task(
			"log", `With Date ESFA Day ${dateESFADay}`
		);

		cy.getById("dtr-day-request-received").clear().type(dateESFADay);

		return this;
	}

	public withDateESFAMonth(dateESFAMonth: string): this {
		cy.task(
			"log", `With Date ESFA Month ${dateESFAMonth}`
		);

		cy.getById("dtr-month-request-received").clear().type(dateESFAMonth);

		return this;
	}

	public withDateESFAYear(dateESFAYear: string): this {
		cy.task(
			"log", `With Date ESFA Year ${dateESFAYear}`
		);

		const element = cy.getById("dtr-year-request-received");
		element.clear();

		if (dateESFAYear.length > 0) {
			element.type(dateESFAYear);
		}

		return this;
	}
	public withTypeOfDecisionID(typeOfDecisionID: string): this {
		cy.task("log", `With type of decision to pick ${typeOfDecisionID}`);

		cy.getById(typeOfDecisionID).click();

		return this;
	}

	public withTotalAmountRequested(totalAmountRequested: string): this {
		cy.task("log", `With total Amount Requested ${totalAmountRequested}`);

		cy.getById("total-amount-request").clear().type(totalAmountRequested);

		return this;
	}


	public withSupportingNotes(supportingNotes: string): this {
		cy.task("log", `With Supporting Notes ${supportingNotes}`);

		cy.getById("case-decision-notes").clear().type(supportingNotes);

		return this;
	}

	public withSupportingNotesExceedingLimit(): this {
		cy.task("log", `With Supporting Notes exceeding limit`);

		cy.getById("case-decision-notes").clear().invoke("val", "x".repeat(2001));

		return this;
	}

	public saveDecision(): this {
		cy.get('#add-decision-button').click();

		return this;
	}

	public hasValidationError(message: string): this {
		cy.task("log", `Has Validation error ${message}`);

		cy.get("#decision-error-list").should(
			"contain.text",
			message
		);

		return this;
	}
}