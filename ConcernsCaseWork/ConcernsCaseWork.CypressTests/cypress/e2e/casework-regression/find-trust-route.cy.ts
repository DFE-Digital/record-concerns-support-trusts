import { Logger } from "../../common/logger";

describe("User interactions via Find Trust route", () => {
	beforeEach(() => {
		cy.login();
	});

	const searchTerm = "Accrington St Christopher's Church Of England High School";

	it("Should display details on the selected trust", () => {
		Logger.Log("User clicks on Create Case and should see Search Trusts");
		cy.get('[href="/trust"]').click();
		cy.get("#search").should("be.visible");
	
		Logger.Log("It should display an error when the user does not enter a trust");
		cy.get("#search").type(searchTerm.substring(0, 1) + "{enter}");
		cy.getById("errorSummary").should("contain.text", "Select a trust");
	
		Logger.Log("Getting too many search results should result in a warning");
		const warningMessage = "There are a large number of search results. Try a more specific search term.";
		cy.get("#search").type("sch");
		cy.getById("tooManyResultsWarning").should(
			"contain.text",
			warningMessage
		);
	
		Logger.Log("Search for a valid trust should find multiple results");
		cy.get("#search").clear().type("Manchester");
		cy.get("#search__listbox").children().should("have.length.gt", 1);
	
		Logger.Log("Selecting the trust should populate it in the search");
		cy.get("#search").clear().type("Accrington St Christopher's Church Of England High School");
		cy.get("#search__option--0").click();
		cy.get("#search").should(
			"have.value",
			"Accrington St Christopher's Church Of England High School, 10059220, 07728029 (Accrington)"
		);
	
		Logger.Log("Clicking continue should populate the trust page");
		cy.getById("continue").click();
		cy.url().should("include", "/trust");
	
		Logger.Log("Trust page should have title of the academy");
		cy.get(".govuk-heading-l").should(
			"contain.text",
			"Accrington St Christopher's Church Of England High School"
		);
	});
});
	
