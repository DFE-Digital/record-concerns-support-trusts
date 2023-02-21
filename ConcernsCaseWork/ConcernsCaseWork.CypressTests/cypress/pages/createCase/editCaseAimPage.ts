import { Logger } from "../../common/logger";


export default class EditCaseAimPage {

    public hasValidationError(message: string): this {
		cy.task("log", `Has Validation error ${message}`);

		cy.getById("errorSummary").should(
			"contain.text",
			message
		);

		return this;
	}

    public hasCaseAim(value: string): this
    {
        Logger.Log(`Has Case Aim ${value}`);

        cy.getByTestId(`case-aim`).should(
			"contain.text",
			value
		);

        return this;
    }

    public withCaseAim(value: string): this
    {
        Logger.Log(`With Case aim ${value}`);

        cy.getByTestId(`case-aim`).clear().type(value);

        return this;
    }

    public withExceedingTextLimit(): this {

        cy.getByTestId('case-aim').clear().invoke("val", "x".repeat(1001));

        return this;
    }

    public apply(): this
    {
        Logger.Log("Apply Case aim");
        cy.getByTestId("apply").click();

        return this;
    }
}