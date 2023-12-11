import { Logger } from "../../common/logger";


export default class EditNextStepsPage {

    public hasValidationError(message: string): this {
		cy.task("log", `Has Validation error ${message}`);

		cy.getById("errorSummary").should(
			"contain.text",
			message
		);

		return this;
	}

    public hasNextSteps(value: string): this
    {
        Logger.log(`Has Deescalation point ${value}`);

        cy.getByTestId(`next-steps`).should(
			"contain.text",
			value
		);

        return this;
    }

    public withNextSteps(value: string): this
    {
        Logger.log(`With Deescalation point ${value}`);

        if (value.length == 0)
        {
            cy.getByTestId(`next-steps`).clear({ force: true });
            return this;
        }

        cy.getByTestId(`next-steps`).clear({ force: true }).type(value);

        return this;
    }

    public withExceedingTextLimit(): this {

        cy.getByTestId('next-steps').clear().invoke("val", "x".repeat(4001));

        return this;
    }

    public apply(): this
    {
        Logger.log("Apply Next Steps");
        cy.getByTestId("apply").click();

        return this;
    }
}