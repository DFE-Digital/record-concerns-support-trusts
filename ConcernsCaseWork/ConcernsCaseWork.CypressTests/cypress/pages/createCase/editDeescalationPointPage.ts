import { Logger } from "../../common/logger";


export default class EditDeEscalationPointPage {

    public hasValidationError(message: string): this {
		cy.task("log", `Has Validation error ${message}`);

		cy.getById("errorSummary").should(
			"contain.text",
			message
		);

		return this;
	}

    public hasDeescalationPoint(value: string): this
    {
        Logger.log(`Has Deescalation point ${value}`);

        cy.getByTestId(`de-escalation-point`).should(
			"contain.text",
			value
		);

        return this;
    }

    public withDeescalationPoint(value: string): this
    {
        Logger.log(`With Deescalation point ${value}`);

        if (value.length == 0)
        {
            cy.getByTestId(`de-escalation-point`).clear({ force: true });
            return this;
        }

        cy.getByTestId(`de-escalation-point`).clear().type(value);

        return this;
    }

    public withExceedingTextLimit(): this {

        cy.getByTestId('de-escalation-point').clear().invoke("val", "x".repeat(1001));

        return this;
    }

    public apply(): this
    {
        Logger.log("Apply De escalation point");
        cy.getByTestId("apply").click();

        return this;
    }
}