import LoginPage from "/cypress/pages/loginPage";
import HomePage from "/cypress/pages/homePage";


describe("Users can log into the application", () => {

	afterEach(() => {
		cy.storeSessionData();
	});

	const lstring =
		'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwx';

	it("User is denied access to the application with invalid credentials", () => {
        cy.visit(Cypress.env('url')+"/login", { timeout: 30000 })
		LoginPage.getUserName({ timeout: 30000 }).should('be.visible');
		LoginPage.getUserName({ timeout: 30000 }).type('invalid username');

		LoginPage.getPassword({ timeout: 30000 }).should('be.visible');
		LoginPage.getPassword({ timeout: 30000 }).type('invalid password');

		LoginPage.getSubmitButton().click();
		//cy.url().should('have.text', (Cypress.env('url')+"/login").trim() );
		//cy.url().should('match', /(https:\/\/amsd-casework-)(london.cloudapps.digital\/login)/i);

		//cy.url().should('contain.text', (Cypress.env('url')+"/login").trim());

		cy.url({ timeout: 30000 }).should('eq', (Cypress.env('url')+"/login").trim());
		 //(Cypress.env('url')+"/login").trim());
		
	});

	it("User is shown error validation messages when no credentials are entered", () => {

		cy.reload(true);

		LoginPage.getUserName({ timeout: 30000 }).should('be.visible');
		LoginPage.getUserName({ timeout: 30000 }).type('empty string').clear();

		LoginPage.getPassword({ timeout: 30000 }).should('be.visible');
		LoginPage.getPassword({ timeout: 30000 }).type('empty string').clear();

		LoginPage.getSubmitButton({ timeout: 30000 }).click();
	
		//Tests that there is error validation displayed
		//const err = '[id="error-summary-title"]'; 
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
			//cy.reload();
	});

	it("User can sucessfully log into the application with valid credentials", () => {
		
		cy.reload(true);
        cy.visit(Cypress.env('url')+"/login", { timeout: 30000 })
        LoginPage.getUserName({ timeout: 30000 }).should('be.visible');
		LoginPage.getUserName({ timeout: 30000 }).type(Cypress.env('username'));
		LoginPage.getPassword({ timeout: 30000 }).type(Cypress.env('password'));

		LoginPage.getSubmitButton().click();
		HomePage.getHeadingText({ timeout: 40000 }).should('be.visible');

	});



	after(function () {
		cy.clearLocalStorage();
		cy.clearCookies();
	});

});

