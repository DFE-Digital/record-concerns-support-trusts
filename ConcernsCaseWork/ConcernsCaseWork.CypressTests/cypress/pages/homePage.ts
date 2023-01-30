import { Logger } from "cypress/common/logger";

class HomePage {

    //locators
    getHeadingText() {
        return cy.get('[class="govuk-heading-l"]', { timeout: 30000 });
    }

    getfirstActiveCase() {
        return cy.get('.govuk-link[href^="case"]').eq(0);
    }

    getTeamCaseworkBtn() {
        return cy.getByTestId("team-casework-tab");
    }

    getYourCaseworkBtn() {
        return cy.get('[id="tab_your-casework"]');
    }
 
    getTeamCaseworkHeading() {
        return cy.get('[class="govuk-table__caption govuk-table__caption--m"]');
    }

    getClosedCasesBtn() {
        return cy.get('[href="/case/closed"]');
    }

    getClosedCasesTable() {
        return cy.get('.govuk-table__body tr');
    }
    
    //methods
    clickFirstActiveCase() {
        this.getfirstActiveCase().click();
    }

    public createCase(): this
    {
        Logger.Log("Creating case");
        cy.getByTestId("create-case").click();

        return this;
    }
}

export default new HomePage();