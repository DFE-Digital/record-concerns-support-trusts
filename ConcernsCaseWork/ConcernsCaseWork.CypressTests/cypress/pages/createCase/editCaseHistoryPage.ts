import { Logger } from "../../common/logger";


export default class EditCaseHistoryPage {

    public hasValidationError(message: string): this {
		cy.task("log", `Has Validation error ${message}`);

		cy.getById("errorSummary").should(
			"contain.text",
			message
		);

		return this;
	}

    public hasCaseHistory(value: string): this
    {
        Logger.log(`Has Case history ${value}`);

        cy.getByTestId(`case-history`).should(
			"contain.text",
			value
		);

        return this;
    }

    public withCaseHistory(value: string): this
    {
        Logger.log(`With Case history ${value}`);

        if (value.length == 0)
        {
            cy.getByTestId(`case-history`).clear({ force: true });
            return this;
        }

        cy.getByTestId(`case-history`).clear({ force: true }).type(value);

        return this;
    }

    public withExceedingTextLimit(): this {

        cy.getByTestId('case-history').clear().invoke("val", "x".repeat(4301));

        return this;
    }

    public apply(): this
    {
        Logger.log("Apply Case History");
        cy.getByTestId("save-case").click();

        return this;
    }
}