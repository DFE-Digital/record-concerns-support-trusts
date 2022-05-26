class EditConcernPage {

    //locators
    getHeadingText() {
        return cy.get('HEADING ELEMENT LOCATOR HERE');
    }

    getConcernEditBtn() {
        return cy.get('.govuk-table-case-details__cell_no_border [href*="edit_rating"]')
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

    
    export default new EditConcernPage();