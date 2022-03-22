describe("User creates and adds subsequent concern to a case", () => {
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

	it("Should be able to add a concern via the Add Concern Link", () => {
		cy.contains("Add concern").click();
	});

	it("Should allow a user to select a second concern type", () => {
		cy.get(".govuk-summary-list__value").then(($el) =>{
			expect($el.text()).to.match(/(school|england|academy)/i)
		});

		cy.selectConcernType();
	});

	it("Expected number of concern types should be displayed", () => {
		isCorrectNumberOfStatuses(2, "Red plus");
	});

	function isCorrectNumberOfStatuses(expectedNumberOfRagStatus, ragStatus) {
		switch (ragStatus) {
			case "Red plus":
				cy.get(
					".govuk-table__body .govuk-table__row .govuk-ragtag-wrapper .ragtag__redplus"
				).should("have.length", expectedNumberOfRagStatus);
				break;
			case "Amber":
				cy.get(
					".govuk-table__body .govuk-table__row .govuk-ragtag-wrapper .ragtag__amber"
				).should("have.length", expectedNumberOfRagStatus);
				break;
			case "Green":
				cy.get(
					".govuk-table__body .govuk-table__row .govuk-ragtag-wrapper .ragtag__green"
				).should("have.length", expectedNumberOfRagStatus);
				break;
			case "Red":
				cy.get(
					".govuk-table__body .govuk-table__row .govuk-ragtag-wrapper .ragtag__red"
				).should("have.length", expectedNumberOfRagStatus);
				break;
			default:
				cy.log("Could not do the thing");
		}
	}

	after(function () {
		cy.clearLocalStorage();
		cy.clearCookies();
	});
});
