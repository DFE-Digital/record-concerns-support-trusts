import { Logger } from "../../../common/logger";

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

	public cannotCloseDecision(): this
	{
		Logger.Log("Checking we cannot close a decision without an outcome");

		cy.getByTestId("close-decision-button").should("not.exist");

		return this;
	}

	public editDecision(): this {
		cy.task("log", `Edit Decision`);

		cy.getByTestId("edit-decision-text").children('a').click();

		return this;
	}

	public editDecisionOutcome(): this
	{
		Logger.Log(`Edit decision outcome`);

		cy.getByTestId("edit-decision-outcome-text")
			.children("a")
			.should("contain.text", "Edit")
			.click();

		return this;
	}

	public createDecisionOutcome(): this{
		cy.getByTestId('continue-record-decision-button').click();

		return this;
	}

	public cannotEditDecision(): this
	{
		cy.task("log", `Cannot edit decision`);

		cy.getByTestId("edit-decision-text").should("not.exist");

		return this;
	}

	
	public cannotEditDecisionOutcome(): this
	{
		cy.task("log", `Cannot edit decision outcome`);

		cy.getByTestId("edit-decision-outcome-text").should("not.exist");

		return this;
	}

	public closeDecision(): this {
		cy.getByTestId('close-decision-button').click();

		return this;
	}
}