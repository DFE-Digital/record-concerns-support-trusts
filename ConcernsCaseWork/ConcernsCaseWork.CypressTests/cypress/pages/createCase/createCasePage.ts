import { Logger } from "../../common/logger";

export class CreateCasePage
{
    public createCase(): this
    {
        Logger.log("Creating case");
        cy.getByTestId("create-case-button").click();

        return this;
    }

    public withTrustName(value: string): this
    {
        Logger.log(`With trustName ${value}`);
        cy.getById(`search`).clear().type(value);

        return this;
    }

    public withTerritory(value: string): this
    {
        Logger.log(`With Territory ${value}`);

        cy.getByTestId(value).click();

        return this;
    }

    public selectOption(): this
    {
        Logger.log("Click first result");
        cy.get("#search__option--0").click();

        return this;
    }

    public confirmOption(): this
    {
        Logger.log("Click continue button");
        cy.getById("continue").click();

        return this;
    }

    public clickNextStepButton(): this
    {
        Logger.log("Click next step button");
        cy.getByTestId("next-step-button").click();

        return this;
    }

    public hasValidationError(message: string): this {
		cy.task("log", `Has Validation error ${message}`);

		cy.getById("errorSummary").should(
			"contain.text",
			message
		);

		return this;
	}

    public shouldNotHaveVisibleLoader(): this {
		cy.task("log", `Should not have visible loader}`);

		cy.get('.ccms-loader').should('not.be.visible');

		return this;
	}

    public hasTooManyResultsWarning(message: string): this {
		cy.task("log", `Has too many results warning ${message}`);

		cy.getById("tooManyResultsWarning").should(
			"contain.text",
			message
		);

		return this;
	}
}