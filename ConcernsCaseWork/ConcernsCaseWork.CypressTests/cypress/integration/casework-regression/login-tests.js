import LoginPage from "/cypress/pages/loginPage";
import HomePage from "/cypress/pages/homePage";


describe("Users can log into the application", () => {

	afterEach(() => {
		cy.storeSessionData();
	});

	it("User is denied access to the application with invalid credentials", () => {
        cy.visit(Cypress.env('url')+"/login", { timeout: 50000 })
		LoginPage.getUserName({ timeout: 50000 }).should('be.visible');
		LoginPage.getUserName({ timeout: 50000 }).type('invalid username');
		LoginPage.getPassword({ timeout: 50000 }).should('be.visible');
		LoginPage.getPassword({ timeout: 50000 }).type('invalid password');
		LoginPage.getSubmitButton().click();
		cy.url({ timeout: 30000 }).should('eq', (Cypress.env('url')+"/login").trim());

		
	});

	it("User is shown error validation messages when no credentials are entered", () => {

		cy.reload(true);

		LoginPage.getUserName({ timeout: 50000 }).should('be.visible');
		LoginPage.getUserName({ timeout: 50000 }).type('empty string').clear();

		LoginPage.getPassword({ timeout: 50000 }).should('be.visible');
		LoginPage.getPassword({ timeout: 50000 }).type('empty string').clear();

		LoginPage.getSubmitButton({ timeout: 50000 }).click();
	
		//Tests that there is error validation displayed
		const err = '[.govuk-list.govuk-error-summary__list]'
		cy.log((err).length);

			if ((err).length > 0 ) { 
				LoginPage.getgetErrorBox().should('be.visible');
				LoginPage.getgetErrorBox().should('contain.text', 'Incorrect username or password');

			}else{

				cy.log('2nd attempt at clearing credentials');
				cy.reload();
				LoginPage.getUserName().clear();
				LoginPage.getPassword().clear();
				LoginPage.getSubmitButton().click();
				LoginPage.getgetErrorBox().should('be.visible');
				LoginPage.getgetErrorBox().should('contain.text', 'Incorrect username or password');
			}
	});

	it("User can sucessfully log into the application with valid credentials", () => {
		
		cy.reload(true);
        cy.visit(Cypress.env('url')+"/login", { timeout: 50000 })
        LoginPage.getUserName({ timeout: 50000 }).should('be.visible');
		LoginPage.getUserName({ timeout: 50000 }).type(Cypress.env('username'));
		LoginPage.getPassword({ timeout: 50000 }).type(Cypress.env('password'));

		LoginPage.getSubmitButton().click();
		HomePage.getYourCaseworkBtn({ timeout: 40000 }).should('be.visible');

	});



	after(function () {
		cy.clearLocalStorage();
		cy.clearCookies();
	});

});

