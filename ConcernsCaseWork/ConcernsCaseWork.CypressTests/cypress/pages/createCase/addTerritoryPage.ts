import { Logger } from "../../common/logger";


export default class AddTerritoryPage {

    public hasTerritory(value: string): this
    {
        Logger.Log(`Has territory ${value}`);

        cy.getByTestId(value).should("be.checked")

        return this;
    }

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

    public hasRiskToTrust(value: string): this
    {
        Logger.Log(`Has Risk to trust ${value}`);

        cy.getByTestId(`risk-to-trust`).should(
			"contain.text",
			value
		);
        return this;
    }

    public nextStep(): this
    {
        Logger.Log("Click next step button");
        cy.getByTestId("next-step-button").click();

        return this;
    }

    public apply(): this
    {
        Logger.Log("Click apply button");
        cy.getByTestId("apply_territory_btn").click();

        return this;
    }

}