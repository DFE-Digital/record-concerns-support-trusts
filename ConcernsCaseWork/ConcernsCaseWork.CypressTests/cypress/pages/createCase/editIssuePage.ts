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
        Logger.log(`Has Issue ${value}`);

        cy.getByTestId(`issue`).should(
			"contain.text",
			value
		);

        return this;
    }

    public clearIssue(): this
    {
        cy.getByTestId(`issue`).clear();

        return this;
    }

    public withIssue(value: string): this
    {
        Logger.log(`With issue ${value}`);

        cy.getByTestId(`issue`).clear().type(value);

        return this;
    }

    public withExceedingTextLimit(): this {

        cy.getByTestId('issue').clear().invoke("val", "x".repeat(2001));

        return this;
    }

    public hasCharacterCountMessage(value: string): this {
        Logger.log(`Has character count message ${value}`);
        cy.contains(value);

        return this;
    }

    public apply(): this
    {
        Logger.log("Apply Issue");
        cy.getByTestId("apply").click();

        return this;
    }
}