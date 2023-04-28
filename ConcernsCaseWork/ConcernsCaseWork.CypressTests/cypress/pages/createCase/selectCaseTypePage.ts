import { Logger } from "../../common/logger";

export class SelectCaseTypePage {

    public withTrustName(value: string): this {
        Logger.Log(`With trustName ${value}`);
        cy.getById(`search`).clear().type(value);

        return this;
    }

    public withNonConcernCaseType(value: string): this {
        Logger.Log(`With NonConcernType ${value}`);

        cy.getByTestId(value).click();

        return this;
    }

    public addConcern(): this {
        Logger.Log("Click continue button for non concern case");
        cy.getById("continue").click();

        return this;
    }

    public withTerritory(value: string): this {
        Logger.Log(`With Territory ${value}`);

        cy.getByTestId(value).click();

        return this;
    }

    public selectOption(): this {
        Logger.Log("Click first result");
        cy.get("#search__option--0").click();

        return this;
    }

    public hasTrustSummaryDetails(value: string): this {
        Logger.Log(`Has Trust summary details ${value}`);

        cy.getById("errorSummary").should(
            "contain.text",
            value
        );
        return this;
    }

    public confirmOption(): this {
        Logger.Log("Click continue button");
        cy.getById("continue").click();

        return this;
    }



}

