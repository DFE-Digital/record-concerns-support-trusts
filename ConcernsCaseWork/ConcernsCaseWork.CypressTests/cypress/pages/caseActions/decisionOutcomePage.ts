export class DecisionOutcomePage {
	
	public withDecisionOutcomeID(decisionOutcomeID: string): this {
		cy.task("log", `With decision outcome to pick ${decisionOutcomeID}`);

		cy.getByTestId(decisionOutcomeID).click();

		return this;
	}
	
	public withTotalAmountApproved(totalAmountApproved: string): this {
		cy.task("log", `With total Amount Approved ${totalAmountApproved}`);

		cy.getByTestId("total-amount-approved").clear().type(totalAmountApproved);

		return this;
	}

	public withDateDecisionMadeDay(dateDecisionMadeDay: string): this {
		cy.task(
			"log", `With Decision Made Day ${dateDecisionMadeDay}`
		);

		cy.getByTestId("dtr-day-decision-made").clear().type(dateDecisionMadeDay);

		return this;
	}

	public withDateDecisionMadeMonth(dateDecisionMadeMonth: string): this {
		cy.task(
			"log", `With Decision Made Month ${dateDecisionMadeMonth}`
		);

		cy.getByTestId("dtr-month-decision-made").clear().type(dateDecisionMadeMonth);

		return this;
	}

	public withDateDecisionMadeYear(dateDecisionMadeYear: string): this {
		cy.task(
			"log", `With Decision Made Year ${dateDecisionMadeYear}`
		);

		const element = cy.getByTestId("dtr-year-decision-made");
		element.clear();

		if (dateDecisionMadeYear.length > 0) {
			element.type(dateDecisionMadeYear);
		}

		return this;
	}

	public withDecisionTakeEffectDay(dateDecisionTakeEffectDay: string): this {
		cy.task(
			"log", `With Decision Take Effect Day ${dateDecisionTakeEffectDay}`
		);

		cy.getByTestId("dtr-day-take-effect").clear().type(dateDecisionTakeEffectDay);

		return this;
	}

	public withDecisionTakeEffectMonth(dateDecisionTakeEffectMonth: string): this {
		cy.task(
			"log", `With Decision Take Effect Month ${dateDecisionTakeEffectMonth}`
		);

		cy.getByTestId("dtr-month-take-effect").clear().type(dateDecisionTakeEffectMonth);

		return this;
	}

	public withDecisionTakeEffectYear(dateDecisionTakeEffectYear: string): this {
		cy.task(
			"log", `With Decision Take Effect Year ${dateDecisionTakeEffectYear}`
		);

		const element = cy.getByTestId("dtr-year-take-effect");
		element.clear();

		if (dateDecisionTakeEffectYear.length > 0) {
			element.type(dateDecisionTakeEffectYear);
		}

		return this;
	}

	public withDecisionAuthouriserID(authoriserID: string): this {
		cy.task("log", `With decision authouriser to pick ${authoriserID}`);

		cy.getByTestId(authoriserID).click();

		return this;
	}

	public withBusinessAreaID(businessAreaID: string): this {
		cy.task("log", `With decision business area consulted to pick ${businessAreaID}`);

		cy.getByTestId(businessAreaID).click();

		return this;
	}


	public saveDecisionOutcome(): this {
		cy.get('#add-decision-outcome-button').click();

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
}
