import { Logger } from "../../common/logger";


export default class EditIssuePage {

    public hasValidationError(message: string): this {
		cy.task("log", `Has Validation error ${message}`);

		cy.getById("errorSummary").should(
			"contain.text",
			message
		);

		return this;
	}

    public hasIssue(value: string): this
    {
        Logger.Log(`Has Issue ${value}`);

        cy.getByTestId(`issue`).should(
			"contain.text",
			value
		);

        return this;
    }

    public withIssue(value: string): this
    {
        Logger.Log(`With issue ${value}`);

        cy.getByTestId(`issue`).clear().type(value);

        return this;
    }

    public withExceedingTextLimit(): this {

        cy.getByTestId('issue').clear().invoke("val", "x".repeat(2001));

        return this;
    }

    public apply(): this
    {
        Logger.Log("Apply Issue");
        cy.getByTestId("apply").click();

        return this;
    }
}