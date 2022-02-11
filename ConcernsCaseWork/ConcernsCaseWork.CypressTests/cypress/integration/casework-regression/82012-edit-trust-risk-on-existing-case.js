describe("User creates subsequent Concern to a case", () => {
	before(() => {
		cy.login();
	});

	afterEach(() => {
		cy.storeSessionData();
	});

	const searchTerm =
		"Accrington St Christopher's Church Of England High School";

	it("User clicks on Create Case and should see Search Trusts", () => {
		cy.get('[href="/case"]').click();
		cy.get("#search").should("be.visible");
	});

	it("User searches for a valid Trust and selects it", () => {
		cy.get("#search").type(searchTerm + "{enter}");
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
/*
	it("Should be able to add a concern via the Add Concern Link", () => {
		cy.contains("Add concern").click();
	});

	it("Should allow a user to select a second concern type", () => {
		cy.get(".govuk-summary-list__value").should(
			"contain.text",
			searchTerm.trim()
		);
		cy.selectConcernType();
	});
*/
	it("Should allow navigation to the risk rating page", () => {
		cy.get('span[class="govuk-visually-hidden"]').contains('risk rating').parent().click();
		cy.editRiskToTrustRedPlus();
		cy.get('[class="govuk-tag ragtag ragtag__redplus"]').should("be.visible");
	});

	it("Should navigate back to the case management page", () => {
		cy.get('*[name="caseID"]').should('be.visible');
		cy.get('[id="close-case-button"]').should('be.visible');
	});
	
	after(function () {
		cy.clearLocalStorage();
		cy.clearCookies();
	});
	
});
