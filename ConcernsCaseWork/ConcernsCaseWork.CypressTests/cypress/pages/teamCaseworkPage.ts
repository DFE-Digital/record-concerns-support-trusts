import { Logger } from "cypress/common/logger";

class TeamCaseworkPage {
    then(arg0: () => void) {
        throw new Error("Method not implemented.");
    }
    public selectTeamMember(name: string): this {
        Logger.Log(`Selecting team member ${name}`);
        cy.get('#select-colleagues-input').type(name);
        cy.get("#select-colleagues-input__option--0").click();
        cy.getById("selected-colleagues")
            .find(`[data-testid='row-${name.toLowerCase()}']`);
        cy.get('[data-testid="save"]').click();

        return this;
    }

    public selectTeamMemberForSearchFieldTest(name: string): this {
        Logger.Log(`Selecting team member ${name}`);
        cy.get('#select-colleagues-input').type(name);
        cy.get("#select-colleagues-input__option--0").click();
        cy.getById("selected-colleagues")
            .find(`[data-testid='row-${name.toLowerCase()}']`);
    

        return this;
    }

    public savingChanges(): this {
        Logger.Log("Saving changes");
        cy.get('[data-testid="save"]').click();

        return this;
    }

    public removeAddedColleagues(): this {
        Logger.Log("Removing added colleagues");
        cy.get('[data-testid="select-colleagues"]').click();
        cy.get('.user-remove').first().click();
        return this;
    }

    removeSearchColleaguesTest(): this { 
        Logger.Log("Removing added colleagues");
        cy.get('.user-remove').first().click();
        return this;
    }

    hasNoResultsFound(): this {
        Logger.Log("Has no results found");
        cy.get(".autocomplete__option").should("contain.text", "No results found");

        return this;
    }

    public hasNoCases(): this {
        Logger.Log("Has no team cases");
        cy.getByTestId("no-cases").should("contain.text", "There are no cases available.");

        return this;
    }

    public teamMemberIsNotDisplayed(name: string): this {  
        Logger.Log("Team member is not displayed");
        cy.get('#selected-colleagues-container').contains(name).should('not.exist');

        return this;
    }


}

const teamCaseworkPage = new TeamCaseworkPage();

export default teamCaseworkPage;