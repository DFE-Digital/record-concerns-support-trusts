class Utils {

    constructor() {
      //  let arrDate = ["day1", "month1", "year1","day2", "month2", "year2", ];
    }

    //locators
    getGovErrorSummaryList() {
        return     cy.get('.govuk-list.govuk-error-summary__list');
    }

    getGovErrorContainer() {
        return     cy.get('[id="errorSummary"]');
    }

    //Methods

    /*Takes a string arg of the error test to be validated
    */
    validateGovErorrList(textToVerify) {
        this.getGovErrorSummaryList().should('contain.text', textToVerify);
	}

    //Checks is the error summary list exists, and returns it's leng as a integer
    //
    checkForGovErrorSummaryList() {

       // let $elem = Cypress.$('[id="errorSummary"]');
       let $elem = Cypress.$('[class="govuk-list govuk-error-summary__list"]')
        cy.log(($elem).length)
        
        return ($elem.length);
    }     

    getFormattedDate() {

        let date = new Date();

        return date.toLocaleDateString("es-CL");
    }  
}
    
    export default new Utils();