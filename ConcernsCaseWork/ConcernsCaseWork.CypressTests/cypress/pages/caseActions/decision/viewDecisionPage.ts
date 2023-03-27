import { Logger } from "../../../common/logger";

export class ViewDecisionPage
{
	public hasDateOpened(value: string): this {
		Logger.Log(`Has date opened ${value}`);

		cy.getByTestId("decision-open-text").should("contain.text", value);

		return this;
	}

	public hasDateClosed(value: string): this {
		Logger.Log(`Has date closed ${value}`);

		cy.getByTestId("decision-closed-text").should("contain.text", value);

		return this;
	}

    public hasCrmEnquiry(crmNumber: string): this {
		Logger.Log(`Has CRM enquiry ${crmNumber}`);

		cy.getByTestId("crm-enquiry-text").should("contain.text", crmNumber);

		return this;
	}

	public hasRetrospectiveRequest(retrospectiveRequest: string): this {
		Logger.Log(`Has retrospective request ${retrospectiveRequest}`);

		cy.getByTestId("retrospective-request-text").should(
			"contain.text",
			retrospectiveRequest
		);

		return this;
	}

	public hasSubmissionRequired(submissionRequired: string): this {
		Logger.Log(`Has Submission Required ${submissionRequired}`);

		cy.getByTestId("submission-required-text").should(
			"contain.text",
			submissionRequired
		);

		return this;
	}

	public hasSubmissionLink(submissionLink: string): this {
		Logger.Log(`Has Submission link ${submissionLink}`);

		cy.getByTestId("submission-link-text").should(
			"contain.text",
			submissionLink
		);

		return this;
	}

	public hasDateESFAReceivedRequest(dateESFAReceivedRequest: string): this {
		Logger.Log(`Has Date ESFA Received Request ${dateESFAReceivedRequest}`);

		cy.getByTestId("date-esfa-received-text").should(
			"contain.text",
			dateESFAReceivedRequest
		);

		return this;
	}

	public hasTotalAmountRequested(totalAmountRequested: string): this {
		Logger.Log(`Has total Amount Requested ${totalAmountRequested}`);

		cy.getByTestId("amount-requested-text").should(
			"contain.text",
			totalAmountRequested
		);

		return this;
	}

	public hasTypeOfDecision(typeOfDecision: string): this {
		Logger.Log(`Has type of decision  ${typeOfDecision}`);


		cy.getByTestId("decision-type-text").should(
			"contain.text",
			typeOfDecision
		);

		return this;
	}

	public hasSupportingNotes(supportingNotes: string): this {
		Logger.Log(`Has Supporting Notes ${supportingNotes}`);

		cy.getByTestId("supporting-notes-text").should(
			"contain.text",
			supportingNotes
		);

		return this;
	}

	public hasActionEdit(): this {
		Logger.Log(`Has Edit Action`);

		cy.getByTestId("edit-decision-text").should(
			"contain.text",
			"Edit"
		);

		return this;
	}

	public hasBusinessArea(businessArea: string): this
	{
		Logger.Log(`Has business area ${businessArea}`);

		cy.getByTestId("business-areas-consulted-text").should("contain.text", businessArea);

		return this;
	}

	public hasDecisionOutcomeStatus(status: string): this
	{
		Logger.Log(`Has decision status ${status}`);

		cy.getByTestId("decision-status-text").should("contain.text", status);

		return this;
	}

	public hasMadeDate(madeDate: string): this
	{
		Logger.Log(`Has made date ${madeDate}`);

		cy.getByTestId("decision-made-date-text").should("contain.text", madeDate);

		return this;
	}

	public hasEffectiveFromDate(effectiveDate: string): this
	{
		Logger.Log(`Has effective from date ${effectiveDate}`);

		cy.getByTestId("decision-date-effective-text").should("contain.text", effectiveDate);

		return this;
	}

	public hasTotalAmountApproved(total: string): this
	{
		Logger.Log(`Has total amount approved ${total}`);

		cy.getByTestId("total-amount-approved-text").should("contain.text", total);

		return this;
	}

	public hasAuthoriser(authoriser: string): this
	{
		Logger.Log(`Has authoriser ${authoriser}`);

		cy.getByTestId("authoriser-text").should("contain.text", authoriser);

		return this;
	}

	public hasNoDecisionOutcome(): this
	{
		Logger.Log(`Has no decision outcome displayed`);

		cy.getByTestId("decision-outcome-heading").should("not.exist");
		cy.getByTestId("decision-outcome-table").should("not.exist");

		return this;
	}

	public cannotCreateAnotherDecisionOutcome(): this
	{
		Logger.Log("Checking we cannot create another decision outcome if one exists");

		cy.getByTestId("continue-record-decision-button").should("not.exist");

		return this;
	}

	public editDecision(): this {
		Logger.Log("Editing decision");

		this.getEditDecision().children('a').click();

		return this;
	}

	public canEditDecision(): this
	{
		Logger.Log("Can edit decision");

		this.getEditDecision();

		return this;
	}

	public cannotEditDecision(): this
	{
		Logger.Log("Cannot edit decision");

		this.getEditDecision().should("not.exist");

		return this;
	}

	public editDecisionOutcome(): this
	{
		Logger.Log(`Edit decision outcome`);

		this.getEditDecisionOutcome()
			.children("a")
			.should("contain.text", "Edit")
			.click();

		return this;
	}

	public canEditDecisionOutcome(): this
	{
		Logger.Log("Cannot edit a decision outcome");

		this.getEditDecisionOutcome();

		return this;
	}

	public cannotEditDecisionOutcome(): this
	{
		Logger.Log("Cannot edit a decision outcome");

		this.getEditDecisionOutcome().should("not.exist");

		return this;
	}

	public createDecisionOutcome(): this {
		Logger.Log("Creating decision outcome");

		cy.getByTestId('continue-record-decision-button').click();

		return this;
	}

	public closeDecision(): this {
		Logger.Log("Closing decision");
		this.getCloseDecision().click();

		return this;
	}

	public canCloseDecision()
	{
		Logger.Log("Can close a decision");

		this.getCloseDecision();

		return this;
	}

	public cannotCloseDecision(): this
	{
		Logger.Log("Cannot close decision");

		this.getCloseDecision().should("not.exist");

		return this;
	}

	private getEditDecision() {
		return cy.getByTestId("edit-decision-text");
	}

	private getEditDecisionOutcome() {
		return cy.getByTestId("edit-decision-outcome-text");
	}

	private getCloseDecision() {
		return cy.getByTestId('close-decision-button');
	}
}