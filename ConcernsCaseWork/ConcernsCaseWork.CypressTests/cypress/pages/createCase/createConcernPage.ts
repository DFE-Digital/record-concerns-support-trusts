import { Logger } from "../../common/logger";


export default class CreateConcernPage {

   

    public withConcernType(value: string): this
    {
        Logger.Log(`With concernType ${value}`);

        cy.getByTestId(value).click();

        return this;
    }

   
    public withSubConcernType(value: string): this
    {
        Logger.Log(`With subConcernType ${value}`);

        cy.getByTestId(value).click();

        return this;
    }

    public withRating(value: string): this
    {
        Logger.Log(`With Rating ${value}`);

        cy.getByTestId(value).click();

        return this;
    }

    public withMeansOfRefferal(value: string): this
    {
        Logger.Log(`With Means of refferal ${value}`);

        cy.getByTestId(value).click();

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

    public hasValidationError(message: string): this {
		cy.task("log", `Has Validation error ${message}`);

		cy.getById("errorSummary").should(
			"contain.text",
			message
		);

		return this;
	}

    public addConcern(): this
    {
        Logger.Log("Click add concern button");
        cy.getByTestId("add-concern-button").click();

        return this;
    }

    public addAnotherConcern(): this
    {
        Logger.Log("Adding another concern");

        cy.getByTestId("add-concern-button").click();

        return this;
    }

    public nextStep(): this
    {
        Logger.Log("Click next step button");
        cy.getByTestId("next-step-button").click();

        return this;
    }

    public cancel(): this
    {
        Logger.Log("Click cancel button");
        cy.getByTestId("cancel-button").click();

        return this;
    }

}