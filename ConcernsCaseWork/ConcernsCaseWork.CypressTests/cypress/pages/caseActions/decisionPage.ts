export class DecisionPage {
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

	public hasCrmEnquiry(crmNumber: string): this {
		cy.task("log", `Has CRM enquiry ${crmNumber}`);

		cy.getByTestId("crm-enquiry-text").should("contain.text", crmNumber);

		return this;
	}

	public hasRetrospectiveRequest(retrospectiveRequest: string): this {
		cy.task("log", `Has retrospective request ${retrospectiveRequest}`);

		cy.getByTestId("retrospective-request-text").should(
			"contain.text",
			retrospectiveRequest
		);

		return this;
	}
	public hasSubmissionRequired(submissionRequired: string): this {
		cy.task("log", `Has Submission Required ${submissionRequired}`);

		cy.getByTestId("submission-required-text").should(
			"contain.text",
			submissionRequired
		);

		return this;
	}

	public hasSubmissionLink(submissionLink: string): this {
		cy.task("log", `Has Submission link ${submissionLink}`);

		cy.getByTestId("submission-link-text").should(
			"contain.text",
			submissionLink
		);

		return this;
	}
	public hasDateESFAReceivedRequest(dateESFAReceivedRequest: string): this {
		cy.task("log", `Has Date ESFA Received Request ${dateESFAReceivedRequest}`);

		cy.getByTestId("date-esfa-received-text").should(
			"contain.text",
			dateESFAReceivedRequest
		);

		return this;
	}
	public hasTotalAmountRequested(totalAmountRequested: string): this {
		cy.task("log", `Has total Amount Requested ${totalAmountRequested}`);

		cy.getByTestId("amount-requested-text").should(
			"contain.text",
			totalAmountRequested
		);

		return this;
	}
	public hasTypeOfDecision(typeOfDecision: string): this {
		cy.task("log", `Has type of decision  ${typeOfDecision}`);


		cy.getByTestId("decision-type-text").should(
			"contain.text",
			typeOfDecision
		);

		return this;
	}
	public hasSupportingNotes(supportingNotes: string): this {
		cy.task("log", `Has Supporting Notes ${supportingNotes}`);

		cy.getByTestId("supporting-notes-text").should(
			"contain.text",
			supportingNotes
		);

		return this;
	}
	public hasActionEdit(): this {
		cy.task("log", `Has Edit Action`);

		cy.getByTestId("edit-decision-text").should(
			"contain.text",
			"Edit"
		);

		return this;
	}
	public editDecision(): this {
		cy.task("log", `Edit Decision`);

		cy.getByTestId("edit-decision-text").children('a').click();

		return this;
	}

	public createDecisionOutcome(): this{
		cy.getByTestId('continue-record-decision-button').click();

		return this;
	}
}
