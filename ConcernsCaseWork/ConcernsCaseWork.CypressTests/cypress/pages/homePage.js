class HomePage {

    //locators
    getHeadingText() {
        return cy.get('[id=your-casework]', { timeout: 30000 });
    }

    getfirstActiveCase() {
        return cy.get('.govuk-link[href^="case"]').eq(0);
    }

    //methods

    clickFirstActiveCase() {
        this.getfirstActiveCase().click();
    }

}

    
    export default new HomePage();