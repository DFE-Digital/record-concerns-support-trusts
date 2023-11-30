import { Logger } from "../../common/logger";


export default class AddDetailsPage {


    public hasRating(value: string): this
    {
        Logger.log(`Has Rating ${value}`);

        cy.getByTestId(value).should("be.checked")

        return this;
    }


    public withRiskToTrust(value: string): this
    {
        Logger.log(`With risk to trust ${value}`);

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

    public nextStep(): this
    {
        Logger.log("Click next step button");
        cy.getByTestId("next-step-button").click();

        return this;
    }

    public apply(): this
    {
        Logger.log("Click apply button")
        cy.getByTestId("apply").click();

        return this;
    }

}