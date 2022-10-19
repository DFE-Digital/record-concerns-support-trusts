class TeamCaseworkPage {

    constructor() {
        this.actionOptions = ["DfE support", "Financial forecast", "Financial plan",
                            "Financial returns", "Financial support", "Forced termination",
                            "NTI: Notice to improve", "Recovery plan", "School Resource Management Adviser (SRMA)", "NTI: Under consideration", "NTI: Warning letter"];

    }

    //locators

    getSelectColleagesBtn() {
        return cy.get('[href="/team/selectcolleagues"]');
    }

    getfirstActiveCase() {
        return cy.get('.govuk-link[href^="case"]').eq(0);
    }

    getHeadingText() {
        return cy.get('[class="govuk-table__caption govuk-table__caption--m"]');
    }

    getCheckbox() {
        return cy.get('[type="checkbox"]');
    }

    

    //methods

    clickFirstActiveCase() {
        this.getfirstActiveCase().click();
    }

    getCheckboxCount() {
        let $elem = Cypress.$('[type="checkbox"]');
        cy.log(($elem).length)

        return ($elem.length);
    }



    }
    
    export default new TeamCaseworkPage();