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

    public deselectAllTeamMembers(): this {
        Logger.Log("Deselecting all team members");

        cy.getById('#select-colleagues').click();
        
            
         

        return this;
    }
    
    public applyingChanges(): this {
        Logger.Log("Checking if no cases are visible");
    
        cy.get('[data-testid="select-colleagues"]').click();
    
        cy.get('[data-testid="save"]').click();
    
        return this;
    }
    
    


    public hasNoCases(): this {
        Logger.Log("Has no team cases");

        cy.getByTestId("no-cases").should("contain.text", "There are no cases available.");

        return this;
    }

    // it("Should appear in the team casework section", () =>
    // {
    //     cy.visit("/TeamCasework");
    //     homePage.selectColleagues();
    //     const email = Cypress.env(EnvUsername);
    //     const name = email.split("@")[0];
    //     teamCaseworkPage
    //         .deselectAllTeamMembers()
    //         .selectTeamMember(email)
    //         .save();
    //     Logger.Log("Ensure that the case for the user is displayed");
    //     caseworkTable
    //         .getRowByCaseId(caseId)
    //         .then(row =>
    //         {
    //             row.hasCaseId(caseId)
    //                 .hasConcern("Financial compliance")
    //                 .hasRiskToTrust("Amber")
    //                 .hasRiskToTrust("Green")
    //                 .hasManagedBy("SFSO", "Midlands and West - West Midlands")
    //                 .hasTrust("Westfield Academy")
    //                 .hasLastUpdatedDate(toDisplayDate(now))
    //                 .hasOwner(name);
    //         });
    //     Logger.Log("Checking accessibility on team casework");
    //     cy.excuteAccessibilityTests();
    //     homePage.selectColleagues();
    //     teamCaseworkPage
    //         .deselectAllTeamMembers()
    //         .save();
    //     teamCaseworkPage.hasNoCases();
    //     Logger.Log("Checking accessibility on team casework with no cases");
    //     cy.excuteAccessibilityTests();
    // });









  
}

const teamCaseworkPage = new TeamCaseworkPage();

export default teamCaseworkPage;