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

    public hasRiskToTrust(index:number, value: string): this
    {
        Logger.Log(`Has Risk to trust ${value}`);

        cy.getByTestId(`risk-to-trust-${index}`).should(
			"contain.text",
			value
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