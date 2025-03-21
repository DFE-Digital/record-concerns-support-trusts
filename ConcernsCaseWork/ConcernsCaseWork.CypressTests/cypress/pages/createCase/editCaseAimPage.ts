import { Logger } from "../../common/logger";


export default class EditCaseAimPage {

    public hasValidationError(message: string): this {
        Logger.log(`Has Validation error ${message}`);

        cy.getById("errorSummary").should(
            "contain.text",
            message
        );

        return this;
    }

    public hasCaseAim(value: string): this
    {
        Logger.log(`Has Case Aim ${value}`);

        cy.getByTestId(`case-aim`).should(
            "contain.text",
            value
        );

        return this;
    }

    public withCaseAim(value: string): this
    {
        Logger.log(`With Case aim ${value}`);

        if (value.length == 0)
        {
            cy.getByTestId(`case-aim`).clear({ force: true });
            return this;
        }

        cy.getByTestId(`case-aim`).clear({ force: true });
        cy.getByTestId(`case-aim`).type(value);

        return this;
    }

    public withExceedingTextLimit(): this {

        cy.getByTestId('case-aim').clear();
        cy.getByTestId(`case-aim`).invoke("val", "x".repeat(1001));

        return this;
    }

    public apply(): this
    {
        Logger.log("Apply Case aim");
        cy.getByTestId("apply").click();

        return this;
    }
}
