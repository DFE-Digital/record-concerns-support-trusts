class Utils {

    constructor() {
        //this.something = 
        let arrDate = ["day1", "month1", "year1","day2", "month2", "year2", ];
    }


    //locators
    getGovErrorSummaryList() {
        return     cy.get('[class="govuk-list govuk-error-summary__list"]');
    }


    //Methods
    
    /*Takes a string arg of the error test to be validated
    */
    validateGovErorrList(textToVerify) {
        this.getGovErrorSummaryList().should('contain.text', textToVerify);
	}

        
}
    
    export default new Utils();