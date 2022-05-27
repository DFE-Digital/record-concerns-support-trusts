class AddToCasePage {

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

    //Option accepts the following args:
    // DfE support, Financial forecast, Financial plan, Financial returns,
    // Financial support| Forced termination| Notice To Improve (NTI)| Recovery plan, School Resource Management Adviser (SRMA) |
    getCaseActionRadio(option) {
        return     cy.get('[value="'+option+'"]');
    }    


    //Methods
    login() {

        cy.visit(Cypress.env('url')+"/login", { timeout: 30000 })
        this.getUserName({ timeout: 30000 }).should('be.visible');
        this.getUserName({ timeout: 30000 }).type(Cypress.env('username'));
        this.getPassword().type(Cypress.env('password')+"{enter}");
        //cy.get('[id=your-casework]', { timeout: 30000 }).should('be.visible')  //verified at the test level and calls homePage
        cy.saveLocalStorage();

    }


    }
    
    export default new AddToCasePage();