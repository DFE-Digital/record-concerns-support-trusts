describe("User edits the trust risk on existing case", () => {
	before(() => {
		cy.login();
	});

	afterEach(() => {
		cy.storeSessionData();
	});

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

	it("Should allow navigation to the risk rating page", () => {
		cy.get('span[class="govuk-visually-hidden"]').contains('risk rating').parent().click();
		cy.editRiskToTrust("apply", "Amber");
		cy.get('[class="govuk-table__cell govuk-label-wrapper"]').children()
		.should('contain.text', 'Amber')
		cy.get('[class="govuk-table__cell govuk-label-wrapper"]').should("be.visible");
	});

	it("Should navigate back to the case management page", () => {
		cy.get('*[name="caseID"]').should('be.visible');
		cy.get('[id="close-case-button"]').should('be.visible');
	});

	it("Should navigate to the management page on cancel", () => {
		cy.get('span[class="govuk-visually-hidden"]').contains('risk rating').parent().click();
		cy.editRiskToTrust("cancel");
		cy.get('*[name="caseID"]').should('be.visible');
	});
	
	after(function () {
		cy.clearLocalStorage();
		cy.clearCookies();
	});
	
});
