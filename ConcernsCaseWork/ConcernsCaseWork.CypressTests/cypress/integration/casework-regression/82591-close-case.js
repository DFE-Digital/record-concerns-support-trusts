describe("User closes a case", () => {
	before(() => {
		cy.login();
	});

	afterEach(() => {
		cy.storeSessionData();
	});

	const searchTerm = "Bridgnorth Endowed School"
		//"Accrington St Christopher's Church Of England High School";

	it("User creates a case", () => {
		cy.get('[href="/case"]').click();
		cy.get("#search").should("be.visible");
		cy.get("#search").type(searchTerm + "{enter}");
		cy.get("#search__option--0").click();
		cy.selectConcernType();
		cy.selectRiskToTrust();
		cy.enterConcernDetails();
	});

	//Opens the first active case in the list
	it("Opens an active case", () => {
		cy.get('.buttons-topOfPage [href="/"]').click();
		//Stores the ID of the case...
		cy.get('#your-casework tr:nth-child(1)  td:nth-child(1)  a').then(($el) => {
			cy.wrap($el.text()).as("closedCaseId");
		});
		cy.get('#your-casework tr:nth-child(1) td:nth-child(1) a').click();
	});

	it("User can close an open concern", function () {
		cy.get('.govuk-table__row:nth-of-type(1) [href*="edit_rating"]').click();
		cy.get('[href*="closure"]').click();
		cy.get(
			'.govuk-button-group [href*="edit_rating/closure"]:nth-of-type(1)'
		).click();
	});

	it("User can close an open case", function () {
		cy.get(
			'[href*="/case/' + this.closedCaseId + '/management/closure"]'
		).click();
		cy.get("#case-outcomes").type("SAMPLE CLOSURE TEXT");
		cy.get(".govuk-button").click();
	});

	it("Case should be marked as closed and removed from active cases", function () {
		cy.get("#main-content tr:nth-child(1) td:nth-child(1) a").then(($el) => {
			cy.wrap($el.text()).as("caseIdAfter");
			//Checks the the Case ID is no longer listed as Active
			expect(this.closedCaseId).to.not.equal($el.text());
		});
		cy.get('[href*="/case/closed"]').click();
		//Checks the case ID is listed as closed
		cy.get("#main-content tr:nth-child(1) td:nth-child(1)").contains(
			this.closedCaseId
		);
	});

	it("Case should be visible under other cases", function () {
		cy.visit(Cypress.env('url')+'/trust')
		cy.get("#search").type(searchTerm + "{enter}");
		cy.get("#search__option--0").click();
		cy.get('.govuk-table:nth-of-type(2) tr').contains(this.closedCaseId)
	});

	after(function () {
		cy.clearLocalStorage();
		cy.clearCookies();
	});
});
