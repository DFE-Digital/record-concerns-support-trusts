import { Logger } from "../../common/logger";


export default class EditCurrentStatusPage {

    public hasValidationError(message: string): this {
		cy.task("log", `Has Validation error ${message}`);

		cy.getById("errorSummary").should(
			"contain.text",
			message
		);

		return this;
	}

    public hasCurrentStatus(value: string): this
    {
        Logger.log(`Has Current status ${value}`);

        cy.getByTestId(`current-status`).should(
			"contain.text",
			value
		);

        return this;
    }

    public withCurrentStatus(value: string): this
    {
        Logger.log(`With Current status ${value}`);

        if (value.length == 0)
        {
            cy.getByTestId(`current-status`).clear({ force: true });
            return this;
        }

        cy.getByTestId(`current-status`).clear({ force: true }).type(value);

        return this;
    }

    public withExceedingTextLimit(): this {

        cy.getByTestId('current-status').clear().invoke("val", "x".repeat(4001));

        return this;
    }

    public apply(): this
    {
        Logger.log("Apply Current Status");
        cy.getByTestId("apply").click();

        return this;
    }
}