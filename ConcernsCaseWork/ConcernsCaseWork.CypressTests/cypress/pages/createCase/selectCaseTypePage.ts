import { Logger } from "../../common/logger";

class SelectCaseTypePage {

    public withCaseType(value: string): this {
        Logger.log(`With NonConcernType ${value}`);

        cy.getByTestId(value).click();

        return this;
    }


    public hasValidationError(value: string): this {
        Logger.log(`Has validation error ${value}`);

        cy.getById("errorSummary").should(
            "contain.text",
            value
        );
        return this;
    }

    public continue(): this {
        Logger.log("Click continue button");
        cy.getById("continue").click();

        return this;
    }
}

var selectCaseTypePage = new SelectCaseTypePage();

export default selectCaseTypePage;

