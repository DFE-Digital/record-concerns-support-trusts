import { Logger } from "../../common/logger";

export class CreateCasePage
{
    public createCase(): this
    {
        Logger.Log("Creating case");
        cy.getByTestId("create-case-button").click();

        return this;
    }

    public withTrustName(value: string): this
    {
        Logger.Log(`With trustName ${value}`);
        cy.getById(`search`).clear().type(value);

        return this;
    }

    public withTerritory(value: string): this
    {
        Logger.Log(`With Territory ${value}`);

        cy.getByTestId(value).click();

        return this;
    }

    public selectOption(): this
    {
        Logger.Log("Click first result");
        cy.get("#search__option--0").click();

        return this;
    }

    public confirmOption(): this
    {
        Logger.Log("Click continue button");
        cy.getById("continue").click();

        return this;
    }

    public clickNextStepButton(): this
    {
        Logger.Log("Click next step button");
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
}