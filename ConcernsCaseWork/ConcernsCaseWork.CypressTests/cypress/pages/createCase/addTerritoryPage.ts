import { Logger } from "../../common/logger";


export default class AddTerritoryPage {


    public withTerritory(value: string): this
    {
        Logger.Log(`With Territory ${value}`);

        cy.getByTestId(value).click();

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

    public clickNextStepButton(): this
    {
        Logger.Log("Click next step button");
        cy.getByTestId("next-step-button").click();

        return this;
    }

}