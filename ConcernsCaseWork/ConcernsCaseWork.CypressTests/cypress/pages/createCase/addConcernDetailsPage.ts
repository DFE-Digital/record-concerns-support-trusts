import { Logger } from "../../common/logger";


export default class AddConcernDetailsPage {


    public hasValidationError(message: string): this {
		cy.task("log", `Has Validation error ${message}`);

		cy.getById("errorSummary").should(
			"contain.text",
			message
		);

		return this;
	}

    public hasTrustSummaryDetails(value: string): this
    {
        Logger.log(`Has Trust summary details ${value}`);

        cy.getByTestId("trust-summary").should(
			"contain.text",
			value
		);
        return this;
    }

    public withIssueExceedingLimit(): this {
        Logger.log(`With exceeding issue limit`);

        this.withExceedingTextLimit("issue", 2001);

        return this;
    }

    public withCurrentStatusExceedingLimit(): this {
        Logger.log(`With exceeding current status limit`);

        this.withExceedingTextLimit("current-status", 4001);

        return this;
    }

    public withCaseAimExceedingLimit(): this {
        Logger.log(`With exceeding case aim limit`);

        this.withExceedingTextLimit("case-aim", 1001);

        return this;
    }

    public withDeEscalationPointExceedingLimit(): this {
        Logger.log(`With exceeding deescalation point limit`);

        this.withExceedingTextLimit("de-escalation-point", 1001);

        return this;
    }

    public withNextStepsExceedingLimit(): this {
        Logger.log(`With exceeding next steps limit`);

        this.withExceedingTextLimit("next-steps", 4001);

        return this;
    }

    public withCaseHistoryExceedingLimit(): this {
        Logger.log(`With exceeding case history limit`);

        this.withExceedingTextLimit("case-history", 4300);

        return this;
    }

    public withExceedingTextLimit(field: string, limit: number): this {

        cy.getByTestId(field).clear().invoke("val", "x 1".repeat(limit));

        return this;
    }

    public withIssue(value: string): this {
        Logger.log(`With issue ${value}`);

        cy.getByTestId(`issue`).clear({force: true}).type(value);

        return this;
    }

    public withCurrentStatus(value: string): this {
        Logger.log(`With current status ${value}`);

        cy.getByTestId(`current-status`).clear({force: true}).type(value);

        return this;
    }

    public withCaseAim(value: string): this {
        Logger.log(`With case aim ${value}`);

        cy.getByTestId(`case-aim`).clear({force: true}).type(value);

        return this;
    }

    public withDeEscalationPoint(value: string): this {
        Logger.log(`With deEscalation point ${value}`);

        cy.getByTestId(`de-escalation-point`).clear({force: true}).type(value);

        return this;
    }

    public withNextSteps(value: string): this {
        Logger.log(`With next steps ${value}`);

        cy.getByTestId(`next-steps`).clear({force: true}).type(value);

        return this;
    }

    public withCaseHistory(value: string): this {
        Logger.log(`With case history ${value}`);
        cy.getByTestId(`case-history`).click({ force: true }).clear().type(value);

        return this;
    }

    getAddConcernBtn() {
        cy.get('[data-testid="add-concern-button"]').click();
    }

    public createCase(): this
    {
        Logger.log("Creating case");
        cy.getByTestId("create-case-button").click();

        return this;
    }

    public nextStep(): this
    {
        Logger.log("Click next step button");
        cy.getByTestId("next-step-button").click();

        return this;
    }

}