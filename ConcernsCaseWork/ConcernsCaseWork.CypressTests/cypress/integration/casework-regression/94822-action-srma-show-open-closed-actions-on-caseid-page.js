describe("User can see open and closed open SRMA line on the caser page", () => {
	before(() => {
		cy.login();
	});

	afterEach(() => {
		cy.storeSessionData();
	});

	let $status = "";
	
	it("User enters the case page with No SRMA present sees no SRMA table", () => {

		cy.checkForExistingCase(true);

		cy.get('a[href*="/action/srma/"]').should('not.exist');
		cy.get('[class="govuk-table__header govuk-table__cell__cases"]').contains('Open actions').should('not.exist');
		cy.get('[class^="govuk-table__header__right"]').contains('Date Opened').should('not.exist'); 

		cy.get('[class="govuk-table__header govuk-table__cell__cases"]').contains('Closed actions').should('not.exist');
		cy.get('[class^="govuk-table__header__right"]').contains('Date Closed').should('not.exist'); 
	});

	
	it("User enters the case page with an SRMA active sees the Open SRMA table", () => {

		cy.get('[class="govuk-button"][role="button"]').click();
		cy.addActionItemToCase('Srma', 'School Resource Management Adviser (SRMA)');
		cy.get('button[data-prevent-double-click*="true"]').click();

		//User sets SRMA status 
		cy.get('[id*="status"]').eq(0).click();
		cy.get('label.govuk-label.govuk-radios__label').eq(0);

		cy.get('[id="dtr-day"]').type(Math.floor(Math.random() * 21) + 10);
		cy.get('[id="dtr-month"]').type(Math.floor(Math.random() *3) + 10);
		cy.get('[id="dtr-year"]').type("2022");
		cy.get('[id="add-srma-button"]').click();

		cy.get('a[href*="/action/srma/"]').should('exist');
		cy.get('[class="govuk-table__header govuk-table__cell__cases"]').contains('Open actions').should('exist');
		cy.get('[class="govuk-table__header govuk-table__cell__cases govuk-table__header__right"]').contains('Date Opened').should('exist'); 

		cy.get('[class="govuk-table__header govuk-table__cell__cases"]').contains('Closed actions').should('not.exist');
		cy.get('[class="govuk-table__header govuk-table__cell__cases govuk-table__header__right"]').contains('Date Closed').should('not.exist'); 

	});


	it("User enters the case page with only Closed SRMA present", () => {

		cy.closeSRMA();

		cy.get('a[href*="/action/srma/"]').should('exist');
		cy.get('[class="govuk-table__header govuk-table__cell__cases"]').contains('Open actions').should('not.exist');
		cy.get('[class^="govuk-table__header__right"]').contains('Date Opened').should('not.exist'); 

		cy.get('[class="govuk-table__header govuk-table__cell__cases"]').contains('Closed actions').should('exist');
		cy.get('th.govuk-table__cell__cases.govuk-table__header.govuk-table__header__right').contains('Date Closed').should('exist');
	});


	it("User enters the case page with both Closed and Open SRMAs present", () => {

		cy.get('[class="govuk-button"][role="button"]').click();
		cy.addActionItemToCase('Srma', 'School Resource Management Adviser (SRMA)');
		cy.get('button[data-prevent-double-click*="true"]').click();

		//User sets SRMA status 
		cy.get('[id*="status"]').eq(0).click();
		cy.get('label.govuk-label.govuk-radios__label').eq(0);

		cy.get('[id="dtr-day"]').type(Math.floor(Math.random() * 21) + 10);
		cy.get('[id="dtr-month"]').type(Math.floor(Math.random() *3) + 10);
		cy.get('[id="dtr-year"]').type("2022");
		cy.get('[id="add-srma-button"]').click();

		cy.get('a[href*="/action/srma/"]').should('exist');
		cy.get('[class="govuk-table__header govuk-table__cell__cases"]').contains('Open actions').should('exist');
		cy.get('[class*="govuk-table__cell__cases govuk-table__header__right"]').contains('Date Opened').should('exist'); 

		cy.get('[class="govuk-table__header govuk-table__cell__cases"]').contains('Closed actions').should('exist');
		cy.get('th.govuk-table__cell__cases.govuk-table__header.govuk-table__header__right').contains('Date Closed').should('exist');
	});


	it("User enters the closed SRMA cannot make edits", () => {

		cy.get('a[href*="/closed"][href*="/srma"]').click();
		cy.get('[class="govuk-table__row"]').should('not.contain.text', "Edit");
		cy.get('[class="govuk-link"]').should('not.exist');

	});
	

	after(function () {
		cy.clearLocalStorage();
		cy.clearCookies();
	});
 
});