import { Logger } from "cypress/common/logger";

class TeamCaseworkPage {
    public selectTeamMember(name: string): this {
        Logger.Log(`Selecting team member ${name}`);
        cy.get(`input[type="checkbox"]`).filter((index, el: any) =>
        {
            return el.value.trim().toLowerCase() === name.toLowerCase();
        }).first().check();
        return this;
    }

    public deselectAllTeamMembers(): this {
        Logger.Log("Deselecting all team members");

        cy.get('input[type="checkbox"]').each(($checkbox) => {
            if ($checkbox.is(':checked')) {
              cy.wrap($checkbox).uncheck();
            }
          });

        return this;
    }

    public hasNoCases(): this {
        Logger.Log("Has no team cases");

        cy.getByTestId("no-cases").should("contain.text", "There are no cases available.");

        return this;
    }

    public save(): this {
        Logger.Log("Saving changes");
        cy.getByTestId('save').click();

        return this;
    }
}

const teamCaseworkPage = new TeamCaseworkPage();

export default teamCaseworkPage;