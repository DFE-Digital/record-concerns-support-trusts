describe("User can manage cases from the case management page", () => {
	before(() => {
		cy.login();
	});

	afterEach(() => {
		cy.storeSessionData();
	});

	let term = ""

	it("User clicks on Create Case and should see Search Trusts", () => {
		cy.get('[href="/case"]').click();
		cy.get("#search").should("be.visible");
	});

	it("User searches for a valid Trust and selects it", () => {
		cy.randomSelectTrust();
		cy.get("#search__option--0").click();
	});

	it("Should allow a user to select a concern type", () => {
		cy.selectConcernType();
	});

	it("Should allow a user to select the risk to the trust", () => {
		cy.selectRiskToTrust();
	});

	it("Should allow the user to enter Concern details", () => {
		cy.enterConcernDetails();
	});

	it("Should verify items are visibile on the case management page", () => {
		cy.validateCaseManagPage();

		cy.get('[id^="accordion-default-heading"]').eq(0).should('have.attr', 'aria-expanded', 'true');
		cy.get('[id^="accordion-default-heading"]').eq(0).click().should('have.attr', 'aria-expanded', 'false');
		cy.get('[id^="accordion-default-heading"]').eq(0).click().should('have.attr', 'aria-expanded', 'true');
	});
		
});
