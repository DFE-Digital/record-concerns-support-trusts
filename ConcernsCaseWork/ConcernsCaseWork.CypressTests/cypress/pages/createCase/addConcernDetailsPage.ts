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

    public withExceedingTextLimit(field: string, limit: number): this {
        Logger.Log(`With ${field} exceeding limit`);

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

    public clickCreateCaseButton(): this
    {
        Logger.Log("Creating case");
        cy.getByTestId("create-case-button").click();

        return this;
    }

}