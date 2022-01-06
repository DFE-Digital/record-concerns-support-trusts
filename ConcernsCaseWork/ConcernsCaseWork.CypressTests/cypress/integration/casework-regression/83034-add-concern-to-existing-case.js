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
		selectConcernType();
	});

	it("Should allow a user to select the risk to the trust", () => {
		selectRiskToTrust();
	});

	it("Should allow the user to enter Concern details", () => {
		enterConcernDetails();
	});

	it("Should be able to add a concern via the Add Concern Link", () => {
		cy.contains("Add concern").click();
	});

	it("Should allow a user to select a second concern type", () => {
		selectConcernType();
	});

	it("Expected number of concern types should be displayed", () => {
		isCorrectNumberOfStatuses(2, "Red plus");
	});

	function enterConcernDetails() {
		let date = new Date();
		cy.get("#issue").type("Data entered at " + date);
		cy.get("#current-status").type("Data entered at " + date);
		cy.get("#case-aim").type("Data entered at " + date);
		cy.get("#de-escalation-point").type("Data entered at " + date);
		cy.get("#next-steps").type("Data entered at " + date);
		cy.get("#case-details-form  button").click();
	}

	function selectRiskToTrust() {
		cy.debug();
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
	}

	//TODO: make this more dynamic - current usability issue raised
	//under 83452
	function selectConcernType() {
		cy.get(".govuk-summary-list__value").should(
			"contain.text",
			searchTerm.trim()
		);
		cy.get(".govuk-radios__item [value=Financial]").click();
		cy.get("[id=sub-type-3]").click();
		cy.get("[id=rating-3]").click();
		cy.get(".govuk-button").click();
	}

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
