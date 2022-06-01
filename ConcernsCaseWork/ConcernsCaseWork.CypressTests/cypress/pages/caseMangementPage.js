class CaseManagementPage {

    //locators
    getHeadingText() {
        return cy.get('h1[class="govuk-heading-l"]');
    }

    getSubHeadingText() {
        return cy.get('[class="govuk-caption-m"]');
    }

    getTrustHeadingText() {
        return cy.get('h2[class="govuk-heading-m"]');
    }

    getConcernEditBtn() {
        return cy.get('.govuk-table-case-details__cell_no_border [href*="edit_rating"]');
    }

    getAddToCaseBtn() {
        return cy.get('[class="govuk-button"][role="button"]').contains('Add to case');
    }

    getCaseDetailsTab() {
        return cy.get('[id="tab_case-details"]');
    }

    getTrustOverviewTab() {
        return cy.get('[id="tab_trust-overview"]');
    }

    




    

    //methods

    closeAllOpenConcerns() {
        
        const elem = '.govuk-table-case-details__cell_no_border [href*="edit_rating"]';
    if (Cypress.$(elem).length > 0) { //Cypress.$ needed to handle element missing exception
    
        //cy.get('.govuk-table-case-details__cell_no_border [href*="edit_rating"]').its('length').then(($elLen) => {
        this.getConcernEditBtn().its('length').then(($elLen) => {
            cy.log($elLen)
    
        while ($elLen > 0) {
                //cy.get('.govuk-table-case-details__cell_no_border [href*="edit_rating"]').eq($elLen-1).click();
                this.getConcernEditBtn().eq($elLen-1).click();
                cy.get('[href*="closure"]').click();
                cy.get('.govuk-button-group [href*="edit_rating/closure"]:nth-of-type(1)').click();
                $elLen = $elLen-1
                cy.log($elLen+" more open concerns")				
                }
            });
    }else {
        cy.log('All concerns closed')
        }
    
    }



}

    
    export default new CaseManagementPage();