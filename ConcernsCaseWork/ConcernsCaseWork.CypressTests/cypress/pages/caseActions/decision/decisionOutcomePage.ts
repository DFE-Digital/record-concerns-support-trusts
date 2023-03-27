import { Logger } from "../../../common/logger";

export class DecisionOutcomePage {
	
	public withDecisionOutcomeStatus(decisionOutcomeID: string): this {
		Logger.Log(`With decision outcome status ${decisionOutcomeID}`);

		cy.getByTestId(decisionOutcomeID).click();

		return this;
	}
	
	public withTotalAmountApproved(totalAmountApproved: string): this {
		Logger.Log(`With total Amount Approved ${totalAmountApproved}`);

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

	public withDecisionAuthouriser(authoriserID: string): this {
		Logger.Log(`With decision authouriser to pick ${authoriserID}`);

		cy.getByTestId(authoriserID).click();

		return this;
	}

	public withBusinessArea(businessAreaID: string): this {
		Logger.Log(`With decision business area consulted to pick ${businessAreaID}`);

		cy.getByTestId(businessAreaID).click();

		return this;
	}


	public saveDecisionOutcome(): this {
		cy.get('#add-decision-outcome-button').click();

		return this;
	}
	
	public hasValidationError(message: string): this {
		Logger.Log(`Has Validation error ${message}`);

		cy.get("#decision-error-list").should(
			"contain.text",
			message
		);

		return this;
	}

	public hasDecisionOutcomeStatus(decisionOutcomeID: string): this {
		Logger.Log(`Has decision outcome status ${decisionOutcomeID}`);

		cy.getByTestId(decisionOutcomeID).should('have.value', decisionOutcomeID);

		return this;
	}

	public hasTotalAmountApproved(totalAmountApproved: string): this {
		Logger.Log(`Total amount approved ${totalAmountApproved}`);

		cy.getByTestId("total-amount-approved").should(
			"contain.value",
			totalAmountApproved
		);

		return this;
	}

	public hasDecisionMadeDay(decisionMadeDay: string): this {
		Logger.Log(`Decision Made Date ${decisionMadeDay}`);

		cy.getByTestId("dtr-day-decision-made").should(
			"contain.value",
			decisionMadeDay
		);

		return this;
	}

	public hasDecisionMadeMonth(decisionMadeMonth: string): this {
		Logger.Log(`Decision Made Date ${decisionMadeMonth}`);

		cy.getByTestId("dtr-month-decision-made").should(
			"contain.value",
			decisionMadeMonth
		);

		return this;
	}

	public hasDecisionMadeYear(decisionMadeYear: string): this {
		Logger.Log(`Decision Made Date ${decisionMadeYear}`);

		cy.getByTestId("dtr-year-decision-made").should(
			"contain.value",
			decisionMadeYear
		);

		return this;
	}

	public hasDateEffectiveFromDay(dateEffectiveFromDay: string): this {
		Logger.Log(`Date Effective From Day ${dateEffectiveFromDay}`);

		cy.getByTestId("dtr-day-take-effect").should(
			"contain.value",
			dateEffectiveFromDay
		);

		return this;
	}

	public hasDateEffectiveFromMonth(dateEffectiveFromMonth: string): this {
		Logger.Log(`Date Effective From Month ${dateEffectiveFromMonth}`);

		cy.getByTestId("dtr-month-take-effect").should(
			"contain.value",
			dateEffectiveFromMonth
		);

		return this;
	}

	public hasDateEffectiveFromYear(dateEffectiveFromYear: string): this {
		Logger.Log(`Date Effective From Year ${dateEffectiveFromYear}`);

		cy.getByTestId("dtr-year-take-effect").should(
			"contain.value",
			dateEffectiveFromYear
		);

		return this;
	}

	public hasDecisionAuthouriser(authoriserID: string): this {
		Logger.Log(`Has authoriser ${authoriserID}`);

		cy.getByTestId(authoriserID).should('have.value', authoriserID);

		return this;
	}

	public hasBusinessArea(businessAreaID: string): this {
		Logger.Log(`Has Business Area ${businessAreaID}`);

		cy.getByTestId(businessAreaID).should('have.value', businessAreaID);

		return this;
	}
}
