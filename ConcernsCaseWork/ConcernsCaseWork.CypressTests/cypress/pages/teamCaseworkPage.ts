import { Logger } from "cypress/common/logger";

class TeamCaseworkPage {
    public selectTeamMember(name: string): this {
        Logger.Log(`Selecting team member ${name}`);
        cy.get('#select-colleagues-input').type(name);
        cy.get("#select-colleagues-input__option--0").click();
        cy.getById("selected-colleagues")
            .find(`[data-testid='row-${name}']`);
        cy.get('[data-testid="save"]').click();

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

    public hasNoCases(): this {
        Logger.Log("Has no team cases");
        cy.getByTestId("no-cases").should("contain.text", "There are no cases available.");

        return this;
    }


}

const teamCaseworkPage = new TeamCaseworkPage();

export default teamCaseworkPage;