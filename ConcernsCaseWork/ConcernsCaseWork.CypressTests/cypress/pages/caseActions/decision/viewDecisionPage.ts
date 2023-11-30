import { Logger } from "../../../common/logger";

export class ViewDecisionPage
{
	public hasDateOpened(value: string): this {
		Logger.log(`Has date opened ${value}`);

		cy.getByTestId("decision-open-text").should("contain.text", value);

		return this;
	}

	public hasDateClosed(value: string): this {
		Logger.log(`Has date closed ${value}`);

		cy.getByTestId("decision-closed-text").should("contain.text", value);

		return this;
	}

    public hasCrmEnquiry(crmNumber: string): this {
		cy.task("log", `Has CRM enquiry ${crmNumber}`);

		cy.getByTestId("crm-enquiry-text").should("contain.text", crmNumber);

		return this;
	}

	public hasCrmCase(hasCrmCase: string): this {
		cy.task("log", `Has CRM enquiry ${hasCrmCase}`);

		cy.getByTestId("has-crm-case").should("contain.text", hasCrmCase);

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

	public hasNoRetrospectiveRequestField(): this {
		Logger.log("Has no retrospective request");
		cy.getByTestId("row-retrospective-request").should("not.exist");

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

	public hasNoDateESFAReceivedRequestField(): this {
		Logger.log("Has no date ESFA received");
		cy.getByTestId("row-esfa-date-requested").should("not.exist");

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

	public hasNoTotalAmountRequestedField(): this {
		Logger.log("Has no total amount requested");
		cy.getByTestId("row-total-amount-requested").should("not.exist");

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

	public hasBusinessArea(businessArea: string): this
	{
		Logger.log(`Has business area ${businessArea}`);

		cy.getByTestId("business-areas-consulted-text").should("contain.text", businessArea);

		return this;
	}

	public hasDecisionOutcomeStatus(status: string): this
	{
		Logger.log(`Has decision status ${status}`);

		cy.getByTestId("decision-status-text").should("contain.text", status);

		return this;
	}

	public hasMadeDate(madeDate: string): this
	{
		Logger.log(`Has made date ${madeDate}`);

		cy.getByTestId("decision-made-date-text").should("contain.text", madeDate);

		return this;
	}

	public hasEffectiveFromDate(effectiveDate: string): this
	{
		Logger.log(`Has effective from date ${effectiveDate}`);

		cy.getByTestId("decision-date-effective-text").should("contain.text", effectiveDate);

		return this;
	}

	public hasTotalAmountApproved(total: string): this
	{
		Logger.log(`Has total amount approved ${total}`);

		cy.getByTestId("total-amount-approved-text").should("contain.text", total);

		return this;
	}

	public hasNoTotalAmountApprovedField(): this {
		Logger.log("Has no total amount approved");
		cy.getByTestId("row-total-amount-approved").should("not.exist");

		return this;
	}

	public hasAuthoriser(authoriser: string): this
	{
		Logger.log(`Has authoriser ${authoriser}`);

		cy.getByTestId("authoriser-text").should("contain.text", authoriser);

		return this;
	}

	public hasNoDecisionOutcome(): this
	{
		Logger.log(`Has no decision outcome displayed`);

		cy.getByTestId("decision-outcome-heading").should("not.exist");
		cy.getByTestId("decision-outcome-table").should("not.exist");

		return this;
	}

	public cannotCreateAnotherDecisionOutcome(): this
	{
		Logger.log("Checking we cannot create another decision outcome if one exists");

		cy.getByTestId("continue-record-decision-button").should("not.exist");

		return this;
	}

	public editDecision(): this {
		Logger.log("Editing decision");

		this.getEditDecision().children('a').click();

		return this;
	}

	public canEditDecision(): this
	{
		Logger.log("Can edit decision");

		this.getEditDecision();

		return this;
	}

	public cannotEditDecision(): this
	{
		Logger.log("Cannot edit decision");

		this.getEditDecision().should("not.exist");

		return this;
	}

	public editDecisionOutcome(): this
	{
		Logger.log(`Edit decision outcome`);

		this.getEditDecisionOutcome()
			.children("a")
			.should("contain.text", "Edit")
			.click();

		return this;
	}

	public canEditDecisionOutcome(): this
	{
		Logger.log("Cannot edit a decision outcome");

		this.getEditDecisionOutcome();

		return this;
	}

	public cannotEditDecisionOutcome(): this
	{
		Logger.log("Cannot edit a decision outcome");

		this.getEditDecisionOutcome().should("not.exist");

		return this;
	}

	public createDecisionOutcome(): this {
		Logger.log("Creating decision outcome");

		cy.getByTestId('continue-record-decision-button').click();

		return this;
	}

	public closeDecision(): this {
		Logger.log("Closing decision");
		this.getCloseDecision().click();

		return this;
	}

	public canCloseDecision()
	{
		Logger.log("Can close a decision");

		this.getCloseDecision();

		return this;
	}

	public cannotCloseDecision(): this
	{
		Logger.log("Cannot close decision");

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