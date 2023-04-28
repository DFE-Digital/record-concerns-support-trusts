import { Logger } from "../../common/logger";

export class SelectCaseTypePage {

    public withCaseType(value: string): this {
        Logger.Log(`With NonConcernType ${value}`);

        cy.getByTestId(value).click();

        return this;
    }


    public hasValidationError(value: string): this {
        Logger.Log(`Has validation error ${value}`);

        cy.getById("errorSummary").should(
            "contain.text",
            value
        );
        return this;
    }

    public continue(): this {
        Logger.Log("Click continue button");
        cy.getById("continue").click();

        return this;
    }



}

