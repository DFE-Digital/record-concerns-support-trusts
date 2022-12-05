export class CloseDecisionPage
{
 
	public withSupportingNotes(supportingNotes: string): this {
		cy.task("log", `With Supporting Notes - add notes ${supportingNotes}`);

		cy.getById("case-decision-notes").clear().type(supportingNotes);

		return this;
	}

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

	public continueToCloseDecision(): this {
		cy.get('#close-decision-button').click();

		return this;
	}

	public closeDecision(): this {
		cy.getByTestId('close-decision-button').click();

		return this;
	}

	
	public continueRecordDecisionOutcome(): this {
		cy.getByTestId('continue-record-decision-button').click();

		return this;
	}
	
	public saveAndContinueOutcome(): this {
		cy.getByTestId('add-decision-outcome-button').click();



		return this;
	}
	
		public selectDecisionOutcome(): this {
		cy.getByTestId('Approved').click();

		return this;
	}
	
	public selectCreatedDecision(): this {
		cy.get("#open-case-actions td")
		.should("contain.text", "Decision: No Decision Types")
		.eq(-3)
		.children("a")
		.click();

		return this;
	}
	
	


	public selectClosedActionDecision(): this {
		cy.get("#close-case-actions td")
	.should("contain.text", "Decision: No Decision Types")
	.eq(-4)
	.children("a")
	.click();

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

	public hasValidationErrorForNotes(message: string): this {
		cy.task("log", `Has Validation error ${message}`);

		cy.get("#errorSummary").should(
			"contain.text",
			message
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
	
}