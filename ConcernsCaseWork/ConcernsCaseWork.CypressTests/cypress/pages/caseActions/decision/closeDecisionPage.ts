export class CloseDecisionPage
{
    public withFinaliseSupportingNotes(finaliseSupportingNotes: string): this {
		cy.task("log", `With Supporting Notes - add notes ${finaliseSupportingNotes}`);

		cy.getById("SupportingNotes").clear().type(finaliseSupportingNotes);

		return this;
	}
	public saveDecision(): this {
		cy.get('#add-decision-button').click();

		return this;
	}

	public withSupportingNotesExceedingLimit(): this {
		cy.task("log", `With Supporting Notes exceeding limit`);

		cy.getById("SupportingNotes").clear().invoke("val", "x".repeat(2001));

		return this;
	}


	public closeDecision(): this {
		cy.getByTestId('close-decision-button').click();

		return this;
	}

	public hasValidationError(message: string): this {
		cy.task("log", `Has Validation error ${message}`);

		cy.get("#errorSummary").should(
			"contain.text",
			message
		);

		return this;
	}
}