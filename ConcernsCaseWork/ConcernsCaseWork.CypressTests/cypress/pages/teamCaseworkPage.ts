import { Logger } from "cypress/common/logger";

// class TeamCaseworkPage {
//     public selectTeamMember(name: string): this {
//         Logger.Log(`Selecting team member ${name}`);
//         cy.get(`input[type="checkbox"]`).filter((index, el: any) =>
//         {
//             return el.value.trim().toLowerCase() === name.toLowerCase();
//         }).first().check();
//         return this;
//     }

//     public deselectAllTeamMembers(): this {
//         Logger.Log("Deselecting all team members");

//         cy.get('input[type="checkbox"]').each(($checkbox) => {
//             if ($checkbox.is(':checked')) {
//               cy.wrap($checkbox).uncheck();
//             }
//           });

//         return this;
//     }
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

    public removeAllTeamMembers(): this {
        Logger.Log("Removing all previous team members");
    
        // Get the parent element and then find its children with the class 'user-remove'
        cy.get('.govuk-table tbody tr.user-colleagues-row')
            .find('.user-remove')
            .then(($removeLinks) => {
                if ($removeLinks.length > 0) {
                    cy.get('.user-remove').each(($removeLink) => {
                        cy.wrap($removeLink).click();
                    });
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