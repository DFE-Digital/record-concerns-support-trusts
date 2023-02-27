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
        Logger.Log(`Has Deescalation point ${value}`);

        cy.getByTestId(`next-steps`).should(
			"contain.text",
			value
		);

        return this;
    }

    public withNextSteps(value: string): this
    {
        Logger.Log(`With Deescalation point ${value}`);

        cy.getByTestId(`next-steps`).clear().type(value);

        return this;
    }

    public withExceedingTextLimit(): this {

        cy.getByTestId('next-steps').clear().invoke("val", "x".repeat(4001));

        return this;
    }

    public apply(): this
    {
        Logger.Log("Apply Next Steps");
        cy.getByTestId("apply").click();

        return this;
    }
}