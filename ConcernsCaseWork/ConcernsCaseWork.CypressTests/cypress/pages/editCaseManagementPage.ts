import { Logger } from "cypress/common/logger";

class EditCaseManagementPage
{
    public withIssue(value: string): this
    {
        Logger.Log(`With issue ${value}`);
        cy.getById("issue").clear().type(value);

        return this;
    }

    public withCaseHistory(value: string): this
    {
        Logger.Log(`With case history ${value}`);

        cy.getById("case-history").clear().type(value);

        return this;
    }

    public withCaseHistoryExceedingLimit(): this
    {
        Logger.Log(`With case history exceeding limit`);

        cy.getById('case-history').clear().invoke("val", "x 1".repeat(2001));

        return this;
    }

    public hasCaseHistory(value: string): this {
        Logger.Log(`Has case history ${value}`);
        cy.getById("case-history").should("has.value", value);

        return this;
    }

    public hasNoConcernType(value: string): this {
        Logger.Log(`Has no concern type for ${value}`);

        cy
        .getByTestId("concern-type-container")
        .contains(value).should("not.exist");

        return this;
    }
    
    public hasValidationError(value: string): this {
        Logger.Log(`Has validation error ${value}`);
        cy.getById("errorSummary").should("contain.text", value);

        return this;
    }

    public save(): this {
        Logger.Log("Saving case updates");
        cy.getByTestId("save-case").click();

        return this;
    }
}

export default new EditCaseManagementPage();