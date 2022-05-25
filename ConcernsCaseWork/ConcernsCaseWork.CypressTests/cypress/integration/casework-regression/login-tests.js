import LoginPage from "/cypress/pages/loginPage";
import HomePage from "/cypress/pages/homePage";


describe("Users can log into the application", () => {

	afterEach(() => {
		cy.storeSessionData();
	});

	const lstring =
		'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwx';


	it("User can sucessfully log into the application with valid credentials", () => {
        cy.visit(Cypress.env('url')+"/login", { timeout: 30000 })
        LoginPage.getUserName({ timeout: 30000 }).should('be.visible');
		LoginPage.getUserName({ timeout: 30000 }).type('invalid username');
		LoginPage.getPassword().type(Cypress.env('password'));

		LoginPage.getSubmitButton().click();
		HomePage.getHeadingText().should('be.visible');

	});

	it("User is denied access to the application with invalid credentials", () => {
        cy.visit(Cypress.env('url')+"/login", { timeout: 30000 })
		LoginPage.getUserName({ timeout: 30000 }).should('be.visible');
		LoginPage.getUserName({ timeout: 30000 }).type('invalid username');
		LoginPage.getPassword().type('invalid password');

		LoginPage.getSubmitButton().click();
		cy.url().should('not.contain.text', Cypress.env('url')+"/login");


	});

	it("User is shown error validation messages when no credentials are entered", () => {

		LoginPage.getUserName({ timeout: 30000 }).should('be.visible');
		LoginPage.getUserName({ timeout: 30000 }).type('');
		LoginPage.getPassword().type('');

		LoginPage.getSubmitButton().click();
	
		//Tests that there is error validation displayed
		const err = '[class="govuk-list govuk-error-summary__list"]';   
		cy.log((err).length);

			if ((err).length > 0 ) { 
				LoginPage.getgetErrorBox().should('be.visible');
				LoginPage.getgetErrorBox().should('contain.text', 'Please enter a valid username');
				LoginPage.getgetErrorBox().should('contain.text', 'Please enter a valid password');
			}else{
				//cy.get('[class="govuk-tag ragtag ragtag__grey"]').eq(2).should('not.be.visible');
				cy.log('2nd attempt at clearing credentials');
				cy.reload();
				LoginPage.getUserName().clear();
				LoginPage.getPassword().clear();
				LoginPage.getSubmitButton().click();
				LoginPage.getgetErrorBox().should('be.visible');
				LoginPage.getgetErrorBox().should('contain.text', 'Please enter a valid username');
				LoginPage.getgetErrorBox().should('contain.text', 'Please enter a valid password');

			}
			cy.reload();
	});



	after(function () {
		cy.clearLocalStorage();
		cy.clearCookies();
	});

});

