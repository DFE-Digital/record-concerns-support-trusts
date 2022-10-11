class HomePage {

    //locators
    getHeadingText() {
        return cy.get('[class="govuk-heading-l"]', { timeout: 30000 });
    }

    getfirstActiveCase() {
        return cy.get('.govuk-link[href^="case"]').eq(0);
    }

    getTeamCaseworkBtn() {
        return cy.get('[id="tab_team-casework"]');
    }

    getYourCaseworkBtn() {
        return cy.get('[id="tab_your-casework"]');
    }
 
    getTeamCaseworkHeading() {
        return cy.get('[class="govuk-table__caption govuk-table__caption--m"]');
    }
    

    //methods

    clickFirstActiveCase() {
        this.getfirstActiveCase().click();
    }


}

    
    export default new HomePage();