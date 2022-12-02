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

  

    public hasSupportingNotes(supportingNotes: string): this {
		cy.task("log", `Has Supporting Notes ${supportingNotes}`);

		cy.getByTestId("supporting-notes-text").should(
			"contain.text",
			supportingNotes
		);

		return this;
	}
	
}