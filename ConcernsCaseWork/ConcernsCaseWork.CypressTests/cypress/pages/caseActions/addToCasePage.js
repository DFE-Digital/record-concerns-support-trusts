class AddToCasePage {

    constructor() {
        this.actionOptions = ["DfE support", "Financial forecast", "Financial plan",
                            "Financial returns", "Financial support", "Forced termination",
                            "NTI: Notice to improve", "Recovery plan", "School Resource Management Adviser (SRMA)", "NTI: Under consideration", "NTI: Warning letter", "Decision"];

    }


    //locators
    getHeadingText() {
        return     cy.get('h1[class="govuk-heading-l"]');
    }

    getSubHeadingText() {
        return     cy.get('h2[class="govuk-heading-m"]');
    }

    getCancelBtn() {
        return     cy.get('[id="cancel-link"]', { timeout: 30000 });
    }

    getAddToCaseBtn() {
        return     cy.get('[data-prevent-double-click="true"]', { timeout: 30000 }).contains('Add to case');
    }

    //current status

    //Option accepts the following args: DfESupport | FinancialForecast | FinancialPlan | FinancialReturns |
    //FinancialSupport| ForcedTermination | NtiUnderConsideration| RecoveryPlan | Srma | Tff |
    getCaseActionRadio(option) {
        return     cy.get('[value="'+option+'"]');
    }    


    //Methods

    addToCase(option) {

            this.getHeadingText().should('contain.text', 'Add to case');
            
            this.getCaseActionRadio(option).click();
        }
        
    }
    
    export default new AddToCasePage();