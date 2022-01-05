describe("User add subsequent Concern to a case", () => {
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

	//TODO: make this more dynamic - current usability issue raised
	//under 83452
	it("Should allow a user to select a concern type", () => {
		cy.get(".govuk-summary-list__value").should(
			"contain.text",
			searchTerm.trim()
		);
		cy.get(".govuk-radios__item [value=Financial]").click();
		cy.get("[id=sub-type-3]").click();
		cy.get("[id=rating-3]").click();
		cy.get(".govuk-button").click();
	});

	it("Should allow the user to add a subsequent concern type", () => {
		cy.get(".govuk-summary-list__value").should(
			"contain.text",
			searchTerm.trim()
		);
		cy.get(".govuk-button.govuk-button--secondary").click();
		cy.get(".govuk-radios__item [value=Compliance]").click();
		cy.get("[id=sub-type-1]").click();
		cy.get("[id=rating-2]").click();
		cy.get(".govuk-button").click();
	});

	it("Should allow a user to select the risk to the trust", () => {
		cy.get('[href="/case/rating"').click();
		cy.get(".ragtag").should("be.visible");
		//Randomly select a RAG status
		cy.get(".govuk-radios .ragtag:nth-of-type(1)")
			.its("length")
			.then((ragtagElements) => {
				let num = Math.floor(Math.random() * ragtagElements);
				cy.get(".govuk-radios .ragtag:nth-of-type(1)").eq(num).click();
			});
		cy.get("#case-rating-form > div.govuk-button-group > button").click();
	});

	it("Should allow the user to enter Concern details", () => {
		let date = new Date();
		cy.get("#issue").type("Data entered at " + date);
		cy.get("#current-status").type("Data entered at " + date);
		cy.get("#case-aim").type("Data entered at " + date);
		cy.get("#de-escalation-point").type("Data entered at " + date);
		cy.get("#next-steps").type("Data entered at " + date);
		cy.get("#case-details-form  button").click();
	});

	it("Should display a Case Created entry", () => {
		cy.get('.govuk-table__row .govuk-table-case-details__cell_no_border .govuk-table__row:nth-of-type(1) td').should('contain.text', 'Financial: Deficit')
		cy.get('.govuk-table__row .govuk-table-case-details__cell_no_border .govuk-table__row:nth-of-type(2) td').should('contain.text', 'Compliance: Financial reporting')
		cy.get('.moj-timeline__description p').contains("Case created");
	});

	after(function () {
		cy.clearLocalStorage();
		cy.clearCookies();
	});
});
