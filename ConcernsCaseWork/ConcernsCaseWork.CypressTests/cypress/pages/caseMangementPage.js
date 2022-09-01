class CaseManagementPage {

    //locators
    getHeadingText() {
        return cy.get('h1[class="govuk-heading-l"]');
    }

    getHeadingInnerText() {
        return cy.get('span.govuk-caption-m');
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

    getConcernTable() {
        return cy.get('[class^="govuk-table-case-details"]');
    }

    getConcernTableAddConcernBtn() {
        return cy.contains("Add concern");
    }

    getAddToCaseBtn() {
        return cy.get('[role="button"]').contains('Add to case');
    }
    

    getCaseDetailsTab() {
        return cy.get('[id="tab_case-details"]');
    }

    getTrustOverviewTab() {
        return cy.get('[id="tab_trust-overview"]');
    }

<<<<<<< Updated upstream
    

=======
    getOpenActionsTable() {
        return cy.get('[id="open-case-actions"]');
    }

    getClosedActionsTable() {
        return cy.get('[id="close-case-actions"]');
    }

    getCloseCaseBtn() {
        return cy.get('#close-case-button');
    }

    getLiveSRMALink() {
        return cy.get('a[href*="/action/srma/"]');
    }    

    getOpenActionLink(action) {
        return cy.get('a[href*="/action/'+action+'/"]');
    } 

    geBackToCaseworkBtn() {
        return cy.get('[class="buttons-topOfPage"]');
    } 

    
   
>>>>>>> Stashed changes



<<<<<<< Updated upstream
    

    //methods

    closeAllOpenConcerns() {
        
        const elem = '.govuk-table-case-details__cell_no_border [href*="edit_rating"]';
    if (Cypress.$(elem).length > 0) { //Cypress.$ needed to handle element missing exception
    
        //cy.get('.govuk-table-case-details__cell_no_border [href*="edit_rating"]').its('length').then(($elLen) => {
        this.getConcernEditBtn().its('length').then(($elLen) => {
            cy.log($elLen)
    
        while ($elLen > 0) {
                //cy.get('.govuk-table-case-details__cell_no_border [href*="edit_rating"]').eq($elLen-1).click();
=======
    const $elem = Cypress.$('.govuk-table-case-details__cell_no_border [href*="edit_rating"]');
    cy.log("About to close all lopen concerns");
    cy.log("$elem.length ="+($elem).length)
  
    if (($elem).length > 0 ) { //Cypress.$ needed to handle element missing exception

        this.getConcernEditBtn().its('length').then(($elLen) => {
            cy.log("Method $elLen " +$elLen)
        while ($elLen > 0) {
>>>>>>> Stashed changes
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

<<<<<<< Updated upstream
=======
    checkForOpenActions() {
        const $elem = Cypress.$('[id="open-case-actions"]');
        cy.log(($elem).length)
        return ($elem.length);
    }

    getCaseIDText() {

        
        //return this.getHeadingText().invoke('text');

        this.getHeadingText().invoke('text').then((text) => {
            var splitText = text.split('\n')[2]
            console.log("splitText "+splitText)

            return splitText.trim();
        });
        
/*
        this.getHeadingText().invoke('text').then((text)=>{ 
            let fullText = text;
            let pattern = /[0-9]+/g;
            let number = fullText.match(pattern);
            console.log(number);

            return number;
        });
*/
      
	}
>>>>>>> Stashed changes


}

    
    export default new CaseManagementPage();