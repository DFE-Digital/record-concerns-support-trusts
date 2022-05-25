class LoginPage {

    //locators
    getUserName() {
        return cy.get("#username");   
    }
    
    getPassword() {
        return 	cy.get("#password");
    }

    getSubmitButton() {
        return cy.get('button[type="submit"]');
    }

    getgetErrorBox() {
        return cy.get('[class="govuk-list govuk-error-summary__list"]');
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
    
    export default new LoginPage();