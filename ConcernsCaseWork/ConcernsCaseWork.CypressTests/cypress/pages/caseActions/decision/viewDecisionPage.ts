export class ViewDecisionPage
{
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