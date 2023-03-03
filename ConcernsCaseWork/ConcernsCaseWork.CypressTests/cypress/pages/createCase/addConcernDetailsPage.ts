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
        Logger.Log(`Has Trust summary details ${value}`);

        cy.getByTestId("trust-summary").should(
			"contain.text",
			value
		);
        return this;
    }


    public hasConcernType(value: string): this
    {
        Logger.Log(`Has Concern Type ${value}`);

        cy.getByTestId("concern-type").should(
			"contain.text",
			value
		);
        return this;
    }

    public hasRiskToTrust(value: string): this
    {
        Logger.Log(`Has Risk to trust ${value}`);

        cy.getByTestId(`risk-to-trust`).should(
			"contain.text",
			value
		);
        return this;
    }

    public hasTerritory(value: string): this
    {
        Logger.Log(`Has Territory ${value}`);

        cy.getByTestId(`territory`).should(
			"contain.text",
			value
		);
        return this;
    }


    public withIssueExceedingLimit(): this {
        Logger.Log(`With exceeding issue limit`);

        this.withExceedingTextLimit("issue", 2001);

        return this;
    }

    public withCurrentStatusExceedingLimit(): this {
        Logger.Log(`With exceeding current status limit`);

        this.withExceedingTextLimit("current-status", 4001);

        return this;
    }

    public withCaseAimExceedingLimit(): this {
        Logger.Log(`With exceeding case aim limit`);

        this.withExceedingTextLimit("case-aim", 1001);

        return this;
    }

    public withDeEscalationPointExceedingLimit(): this {
        Logger.Log(`With exceeding deescalation point limit`);

        this.withExceedingTextLimit("de-escalation-point", 1001);

        return this;
    }

    public withNextStepsExceedingLimit(): this {
        Logger.Log(`With exceeding next steps limit`);

        this.withExceedingTextLimit("next-steps", 4001);

        return this;
    }

    public withCaseHistoryExceedingLimit(): this {
        Logger.Log(`With exceeding case history limit`);

        this.withExceedingTextLimit("case-history", 4300);

        return this;
    }

    public withExceedingTextLimit(field: string, limit: number): this {

        cy.getByTestId(field).clear().invoke("val", "x 1".repeat(limit));

        return this;
    }

    public withIssue(value: string): this {
        Logger.Log(`With issue ${value}`);

        cy.getByTestId(`issue`).clear().type(value);

        return this;
    }

    public withCurrentStatus(value: string): this {
        Logger.Log(`With current status ${value}`);

        cy.getByTestId(`current-status`).clear().type(value);

        return this;
    }

    public withCaseAim(value: string): this {
        Logger.Log(`With case aim ${value}`);

        cy.getByTestId(`case-aim`).clear().type(value);

        return this;
    }

    public withDeEscalationPoint(value: string): this {
        Logger.Log(`With deEscalation point ${value}`);

        cy.getByTestId(`de-escalation-point`).clear().type(value);

        return this;
    }

    public withNextSteps(value: string): this {
        Logger.Log(`With next steps ${value}`);

        cy.getByTestId(`next-steps`).clear().type(value);

        return this;
    }

    public withCaseHistory(value: string): this {
        Logger.Log(`With case history ${value}`);

        cy.getByTestId(`case-history`).clear().type(value);

        return this;
    }

    public createCase(): this
    {
        Logger.Log("Creating case");
        cy.getByTestId("create-case-button").click();

        return this;
    }

}