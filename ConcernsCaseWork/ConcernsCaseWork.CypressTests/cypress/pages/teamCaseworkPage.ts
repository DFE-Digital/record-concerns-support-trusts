import { Logger } from "cypress/common/logger";
import { toTitleCase } from "cypress/support/stringUtils";

class TeamCaseworkPage {

    public selectTeamMember(name: string): this {
        Logger.Log(`Selecting team member ${name}`);
        this.enterTeamMember(name);
        cy.get("#select-colleagues-input__option--0").click();
        cy.getById("selected-colleagues").find(`[data-testid='row-${name.toLowerCase()}']`);

        return this;
    }

    public hasTeamMember(email, name: string): this {

        const nameSelector = name.toLowerCase();

        cy.getByTestId(`user-name-${nameSelector}`).should("contain.text", toTitleCase(name));
        cy.getByTestId(`user-email-${nameSelector}`).contains(email, { matchCase: false });

        return this;
    }

    public save(): this {
        Logger.Log("Saving changes");
        cy.get('[data-testid="save"]').click();

        return this;
    }

    public enterTeamMember(name: string): this {
        cy.get('#select-colleagues-input').type(name);

        return this;
    }

    public removeAddedColleagues(): this {
        Logger.Log("Removing added colleagues");
        cy.get('.user-remove').each(e => cy.wrap(e).click());
        return this;
    }

    public hasNoResultsFound(): this {
        Logger.Log("Has no results found");
        cy.get(".autocomplete__option").should("contain.text", "No results found");

        return this;
    }

    public hasNoCases(): this {
        Logger.Log("Has no team cases");
        cy.getByTestId("no-cases").should("contain.text", "There are no cases available.");

        return this;
    }

    public hasNoTeamMember(name: string): this {  
        Logger.Log("Team member is not displayed");
        cy.get('#selected-colleagues-container').contains(name).should('not.exist');

        return this;
    }


}

const teamCaseworkPage = new TeamCaseworkPage();

export default teamCaseworkPage;