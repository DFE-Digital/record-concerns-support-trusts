import { Logger } from "../../../common/logger";

export class CloseDecisionPage
{
    public withFinaliseSupportingNotes(finaliseSupportingNotes: string): this {
		Logger.log(`With Supporting Notes ${finaliseSupportingNotes}`);

		cy.getById("SupportingNotes").clear().type(finaliseSupportingNotes);

		return this;
	}

	public hasFinaliseSupportingNotes(finaliseSupportingNotes: string): this {
		Logger.log(`With Supporting Notes ${finaliseSupportingNotes}`);

		cy.getById("SupportingNotes").should("have.value", finaliseSupportingNotes);

		return this;
	}

	public closeDecision(): this {
		Logger.log("Confirm closing the decision");

		cy.get('#add-decision-button').click();

		return this;
	}

	public withSupportingNotesExceedingLimit(): this {
		Logger.log("With supporting notes exceeding the limit");

		cy.getById("SupportingNotes").clear().invoke("val", "x".repeat(2001));

		return this;
	}

	public hasValidationError(message: string): this {
		Logger.log(`Has Validation error ${message}`);

		cy.get("#errorSummary").should(
			"contain.text",
			message
		);

		return this;
	}
}