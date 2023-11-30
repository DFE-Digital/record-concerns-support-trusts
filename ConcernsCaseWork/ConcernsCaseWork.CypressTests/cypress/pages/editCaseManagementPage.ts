import { Logger } from "cypress/common/logger";

class EditCaseManagementPage
{
    public withCaseOwner(value: string): this
    {
        Logger.log(`With case owner ${value}`);

        cy.getById("case-owner-input").clear().type(value);

        return this;
    }

    public hasNoCaseOwnerResults(): this {
        Logger.log("Has case owner option");
        cy.get(".autocomplete__option").should("contain.text", "No results found");

        return this;
    }

    public selectCaseOwnerOption(): this
    {
        Logger.log("Selecting first case owner option");
        cy.get(".autocomplete__option").first().click();

        return this;
    }
    
    public clearCaseOwner(): this
    {
        Logger.log(`Clearing the case owner`);

        cy.getById("case-owner-input").clear();

        return this;
    }

    public withIssue(value: string): this
    {
        Logger.log(`With issue ${value}`);
        cy.getById("issue").clear().type(value);

        return this;
    }

    public withCaseHistory(value: string): this
    {
        Logger.log(`With case history ${value}`);

        cy.getById("case-history").clear().type(value);

        return this;
    }

    public withCaseHistoryExceedingLimit(): this
    {
        Logger.log(`With case history exceeding limit`);

        cy.getById('case-history').clear().invoke("val", "x 1".repeat(2001));

        return this;
    }

    public hasCaseOwner(value: string): this {
        Logger.log(`Has case owner ${value}`);

        // Can be improved later
        // Currently its driven by the casing of the email when the user logs in
        // We can't control this, so safer to ignore case for now
        cy.getById("case-owner-input")
            .then((element: any) =>
            {
                expect(element.val().toLowerCase()).eq(value.toLowerCase());
            });


        return this;
    }

    public hasCaseHistory(value: string): this {
        Logger.log(`Has case history ${value}`);
        cy.getById("case-history").should("has.value", value);

        return this;
    }

    public hasNoConcernType(value: string): this {
        Logger.log(`Has no concern type for ${value}`);

        cy
        .getByTestId("concern-type-container")
        .contains(value).should("not.exist");

        return this;
    }
    
    public hasValidationError(value: string): this {
        Logger.log(`Has validation error ${value}`);
        cy.getById("errorSummary").should("contain.text", value);

        return this;
    }

    public save(): this {
        Logger.log("Saving case updates");
        cy.getByTestId("save-case").click();

        return this;
    }
}

export default new EditCaseManagementPage();