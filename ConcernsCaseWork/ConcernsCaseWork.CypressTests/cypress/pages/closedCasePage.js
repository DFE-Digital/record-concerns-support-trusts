class ClosedCasePage {

    //locators
    getHeadingText() {
        return cy.get('h1[class="govuk-heading-l"]', { timeout: 30000 });
    }

    getClosedCasesTable() {
        return cy.get('.govuk-table__body tr', { timeout: 30000 });
    }

    getClosedCasesLink() {
        return cy.get('[href*=closed]', { timeout: 30000 });
    }

    getClosedCase(caseid) {
        return cy.get('a[href="/case/'+caseid+'/closed"]');
       }

  
    
    //methods

    clickFirstActiveCase() {
        this.getfirstActiveCase().click();
    }

    clickClosedCase(caseid) {
        this.getClosedCase(caseid).parent().click();
    }

    



}

    
    export default new ClosedCasePage();