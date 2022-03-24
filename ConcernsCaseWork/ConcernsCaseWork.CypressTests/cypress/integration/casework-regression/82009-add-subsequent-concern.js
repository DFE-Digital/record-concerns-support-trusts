describe("User adds subsequent Concern to a case", () => {
	before(() => {
		cy.login();
	});

	afterEach(() => {
		cy.storeSessionData();
	});

	const searchTerm =
		"Accrington St Christopher's Church Of England High School";

	let term = "";

	it("User clicks on Create Case and should see Search Trusts", () => {
		cy.get('[href="/case"]').click();
		cy.get("#search").should("be.visible");
	});

	it("User searches for a valid Trust and selects it", () => {
		cy.randomSelectTrust();
		cy.get("#search__option--0").invoke('text').then(text => {
			term = text;
			cy.log(term.substring(0,15));
			});

		cy.get("#search__option--0").click();
		});

	//TODO: make this more dynamic
	it("Should allow a user to select a concern type (Financial: Deficit)", () => {
		cy.get(".govuk-summary-list .govuk-summary-list__row:nth-of-type(1) .govuk-summary-list__value")
			.should("contain.text", term.substring(9,10));
		cy.selectConcernType();
	});

	it("Should allow the user to add a subsequent concern type", () => {
		cy.get(".govuk-summary-list .govuk-summary-list__row:nth-of-type(1) .govuk-summary-list__value")
			.should("contain.text", term.substring(9,10));

		cy.get(".govuk-button.govuk-button--secondary").click();
		cy.get(".govuk-radios__item [value=Compliance]").click();
		cy.get("[id=sub-type-1]").click();
		cy.get("[id=rating-2]").click();
		cy.get(".govuk-button").click();
	});

	it("Should allow a user to select the risk to the trust", () => {
		cy.selectRiskToTrust();
	});

	it("Should allow the user to enter Concern details", () => {
		cy.enterConcernDetails();
	});

	it("Should display a Case Created entry", () => {
		cy.get('.govuk-table__row .govuk-table-case-details__cell_no_border .govuk-table__row')
			.should('contain.text', 'Financial');

		cy.get('.govuk-table__row .govuk-table-case-details__cell_no_border .govuk-table__row')
			.should('contain.text', 'Financial');
	});

	after(function () {
		cy.clearLocalStorage();
		cy.clearCookies();
	});
});
