import { Logger } from "../../common/logger";

class SelectCaseDivisionPage {

    public withCaseDivision(value: string): this {
        Logger.log(`With NonConcernType ${value}`);

        cy.getByTestId(value).click();

        return this;
    }

    public hasBeenDisabled(value: string): this {
        Logger.log(`Has validation error ${value}`);

        cy.getByTestId(value).should("be.disabled");
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

var selectCaseDivisionPage = new SelectCaseDivisionPage();

export default selectCaseDivisionPage;

