import { Logger } from "../../../common/logger";

export class CloseDecisionPage
{
    public withFinaliseSupportingNotes(finaliseSupportingNotes: string): this {
		Logger.Log(`With Supporting Notes - add notes ${finaliseSupportingNotes}`);

		cy.getById("SupportingNotes").clear().type(finaliseSupportingNotes);

		return this;
	}
	public closeDecision(): this {
		Logger.Log("Confirm closing the decision");

		cy.get('#add-decision-button').click();

		return this;
	}

	public withSupportingNotesExceedingLimit(): this {
		Logger.Log("With supporting notes exceeding the limit");

		cy.getById("SupportingNotes").clear().invoke("val", "x".repeat(2001));

		return this;
	}

	public hasValidationError(message: string): this {
		Logger.Log(`Has Validation error ${message}`);

		cy.get("#errorSummary").should(
			"contain.text",
			message
		);

		return this;
	}
}