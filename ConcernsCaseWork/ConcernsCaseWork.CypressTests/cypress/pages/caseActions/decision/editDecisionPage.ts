import { Logger } from "../../../common/logger";

export class EditDecisionPage
{
    public withCrmEnquiry(crmNumber: string): this {
		Logger.Log(`With Crm enquiry ${crmNumber}`);

		cy.getById("crm-enquiry-number").clear().type(crmNumber);

		return this;
	}

	public hasCrmEnquiry(crmNumber: string): this {
		Logger.Log(`Has Crm enquiry ${crmNumber}`);

		cy.getById("crm-enquiry-number").should("have.value", crmNumber);

		return this;
	}

	public withRetrospectiveRequest(isRetrospectiveRequest: string): this {
		Logger.Log(`With retrospective request ${isRetrospectiveRequest}`);

		cy.getById(`retrospective-approval-${isRetrospectiveRequest}`).click();

		return this;
	}

	public hasRetrospectiveRequest(isRetrospectiveRequest: string): this {
		Logger.Log(`Has retrospective request ${isRetrospectiveRequest}`);

		cy.getById(`retrospective-approval-${isRetrospectiveRequest}`).should("be.checked");

		return this;
	}

	public withSubmissionRequired(isSubmissionRequired: string): this {
		Logger.Log(`With Submission Required ${isSubmissionRequired}`);

		cy.getById(`submission-required-${isSubmissionRequired}`).click();

		return this;
	}

	public hasSubmissionRequired(isSubmissionRequired: string): this {
		Logger.Log(`Has Submission Required ${isSubmissionRequired}`);

		cy.getById(`submission-required-${isSubmissionRequired}`).should("be.checked");

		return this;
	}

	public withSubmissionLink(submissionLink: string): this {
		Logger.Log(`With Submission link ${submissionLink}`);

		cy.getById("submission-document-link").clear().type(submissionLink);

		return this;
	}

	public hasSubmissionLink(submissionLink: string): this {
		Logger.Log(`Has Submission link ${submissionLink}`);

		cy.getById("submission-document-link").should("have.value", submissionLink);

		return this;
	}

	public withDateESFADay(dateESFADay: string): this {
		Logger.Log(`With Date ESFA Day ${dateESFADay}`);

		cy.getById("dtr-day-request-received").clear().type(dateESFADay);

		return this;
	}

	public hasDateESFADay(dateESFADay: string): this {
		Logger.Log(`Has Date ESFA Day ${dateESFADay}`);

		cy.getById("dtr-day-request-received").should("have.value", dateESFADay);

		return this;
	}

	public withDateESFAMonth(dateESFAMonth: string): this {
		Logger.Log(`With Date ESFA Month ${dateESFAMonth}`);

		cy.getById("dtr-month-request-received").clear().type(dateESFAMonth);

		return this;
	}

	public hasDateESFAMonth(dateESFAMonth: string): this {
		Logger.Log(`Has Date ESFA Month ${dateESFAMonth}`);

		cy.getById("dtr-month-request-received").should("have.value", dateESFAMonth);

		return this;
	}

	public withDateESFAYear(dateESFAYear: string): this {
		Logger.Log(`With Date ESFA Year ${dateESFAYear}`);

		const element = cy.getById("dtr-year-request-received");
		element.clear();

		if (dateESFAYear.length > 0) {
			element.type(dateESFAYear);
		}

		return this;
	}

	public hasDateESFAYear(dateESFAYear: string): this {
		Logger.Log(`Has Date ESFA Year ${dateESFAYear}`);

		cy.getById("dtr-year-request-received").should("have.value", dateESFAYear);

		return this;
	}

	public withTypeOfDecision(typeOfDecision: string): this {
		Logger.Log(`With type of decision to pick ${typeOfDecision}`);

		cy.getById(typeOfDecision).click();

		return this;
	}

	public hasTypeOfDecision(typeOfDecision: string): this {
		Logger.Log(`With type of decision to pick ${typeOfDecision}`);

		cy.getById(typeOfDecision).should("be.checked");

		return this;
	}

	public withDrawdownFacilityAgreed(type: string, value: string): this {
		Logger.Log(`With ${type} drawdown facility agreed ${value}`);

		cy.getByTestId(`${type}-${value}`).click();

		return this;
	}

	public hasDrawdownFacilityAgreed(type: string, value: string): this {
		Logger.Log(`Has ${type} drawdown facility agreed ${value}`);

		cy.getByTestId(`${type}-${value}`).should("be.checked");

		return this;
	}

	public withFrameworkCategory(type: string, value): this {
		Logger.Log(`With ${type} framework category ${value}`);

		cy.getByTestId(`${type}-${value}`).click();

		return this;
	}

	public hasFrameworkCategory(type: string, value): this {
		Logger.Log(`Has ${type} framework category ${value}`);

		cy.getByTestId(`${type}-${value}`).should("be.checked");

		return this;
	}

	public hasNoVisibleSubQuestions(type: string): this {
		Logger.Log(`${type} sub questions are not visible`);

		cy.getByTestId(`${type}-subquestion-container`).should("not.be.visible");

		return this;
	}

	public withTotalAmountRequested(totalAmountRequested: string): this {
		Logger.Log(`With total Amount Requested ${totalAmountRequested}`);

		cy.getById("total-amount-request").clear().type(totalAmountRequested);

		return this;
	}

	public hasTotalAmountRequested(totalAmountRequested: string): this {
		Logger.Log(`Has total Amount Requested ${totalAmountRequested}`);

		cy.getById("total-amount-request").should("have.value", totalAmountRequested);

		return this;
	}

	public withSupportingNotes(supportingNotes: string): this {
		Logger.Log(`With Supporting Notes ${supportingNotes}`);

		cy.getById("case-decision-notes").clear().type(supportingNotes);

		return this;
	}

	public hasSupportingNotes(supportingNotes: string): this {
		Logger.Log(`Has Supporting Notes ${supportingNotes}`);

		cy.getById("case-decision-notes").should("have.value", supportingNotes);

		return this;
	}

	public withSupportingNotesExceedingLimit(): this {
		cy.task("log", `With Supporting Notes exceeding limit`);

		cy.getById("case-decision-notes").clear().invoke("val", "x 1".repeat(1001));

		return this;
	}

	public save(): this {

		Logger.Log("Saving decision");

		cy.getById('add-decision-button').click();

		return this;
	}

	public cancel(): this {
		Logger.Log("Cancelling edit decision");

		cy.getById("cancel-link-event").click();

		return this;
	}

	public hasValidationError(message: string): this {
		cy.task("log", `Has Validation error ${message}`);

		cy.getById("errorSummary").should(
			"contain.text",
			message
		);

		return this;
	}
}