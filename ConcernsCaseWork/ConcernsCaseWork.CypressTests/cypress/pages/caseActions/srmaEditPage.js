import caseActionsBase from "/cypress/pages/caseActions/caseActionsBasePage";

class SRMAEditPage {


    constructor() {
        this.arrDate =[];
    }


    //locators

    getHeadingText() {
        return     cy.get('h1[class="govuk-heading-l"]');
    }

    getSubHeadingText() {
        return     cy.get('h2[class="govuk-heading-m"]');
    }

    //SRMA TABLE
    getSrmaTable() {
        return    cy.get('[class="govuk-table__cell"]', { timeout: 30000 });
    }

    getSrmaTableRow() {
        return caseActionsBase.getTableRow();
    }

    getSrmaTableRowEmpty() {
        return    cy.get('tr.govuk-table__row', { timeout: 30000 }).contains('Empty');
    }

    getNotesInfo() {
        return    cy.get('[id="srma-notes-info"]', { timeout: 30000 });
    }

    getTableAddEditLink() {
        return    cy.get('[class="govuk-link"]', { timeout: 30000 });
    }

    //Option accepts the following args: DfESupport | FinancialForecast | FinancialPlan | FinancialReturns |
    //FinancialSupport| ForcedTermination | Nti| RecoveryPlan | Srma | Tff |
    getCaseActionRadio(option) {
        return     cy.get('[value="'+option+'"]');
    }   
    
    getNotesBox() {
        return    cy.get('[id="srma-notes"]', { timeout: 30000 });
    }
    
    getNotesInfo() {
        return    cy.get('[id="srma-notes-info"]', { timeout: 30000 });
    }


    //Methods

    checkForTableEntry() {
        let $elem = Cypress.$('.govuk-tag.ragtag.ragtag__grey');
        cy.log(($elem).length)
        
        return ($elem.length);
    }

        
}

    export default new SRMAEditPage();

