import { Logger } from "cypress/common/logger";
import { toTitleCase } from "cypress/support/stringUtils";

class TeamCaseworkPage {

    public selectTeamMember(name: string): this {
        Logger.log(`Selecting team member ${name}`);
        this.enterTeamMember(name);
        cy.getById("select-colleagues-input__option--0").click();
        cy.getByTestId(`row-${name.toLowerCase()}`);

        return this;
    }

    public hasTeamMember(email, name: string): this {

        const nameSelector = name.toLowerCase();

        cy.getByTestId(`user-name-${nameSelector}`).should("contain.text", toTitleCase(name));
        cy.getByTestId(`user-email-${nameSelector}`).contains(email, { matchCase: false });

        return this;
    }

    public save(): this {
        Logger.log("Saving changes");
        cy.getByTestId("save").click();

        return this;
    }

    public enterTeamMember(name: string): this {
        cy.getById("select-colleagues-input").type(name);

        return this;
    }

    public removeAddedColleagues(): this {
        Logger.log("Removing added colleagues");
        cy.get('.user-remove').each(e => cy.wrap(e).click());
        return this;
    }

    public hasNoResultsFound(): this {
        Logger.log("Has no results found");
        cy.get(".autocomplete__option").should("contain.text", "No results found");

        return this;
    }

    public hasNoCases(): this {
        Logger.log("Has no team cases");
        cy.getByTestId("no-cases").should("contain.text", "There are no cases available.");

        return this;
    }

    public hasNoTeamMember(name: string): this {  
        Logger.log("Team member is not displayed");
        cy.getById("selected-colleagues-container").contains(name).should('not.exist');

        return this;
    }


}

const teamCaseworkPage = new TeamCaseworkPage();

export default teamCaseworkPage;