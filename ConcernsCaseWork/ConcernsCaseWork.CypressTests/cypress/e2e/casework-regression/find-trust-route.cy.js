const { Logger } = require("../../common/logger");

describe("User interactions via Find Trust route", () => {
	beforeEach(() => {
		cy.login();
	});

    const searchTerm =
    "Accrington St Christopher's Church Of England High School";

	it("Should display details on the selected trust", () =>
	{
		Logger.Log("User clicks on Create Case and should see Search Trusts");
		cy.get('[href="/trust"]').click();
		cy.get("#search").should("be.visible");

		Logger.Log("User searches for a valid Trust and selects it");
		cy.get("#search").type(searchTerm);
		cy.get("#search__option--0").click();
		cy.getById("continue").click();

		Logger.Log("Should display the Concern details of the specified Trust");
		cy.get(
			".govuk-table__cell:nth-child(1) > a"
		).should('contain.text','Accrington')
		cy.get('[href="/trust"').should("be.visible").click();

		Logger.Log("It should display an error when the user does not enter a trust");
		cy.get("#search").type(searchTerm .substring(0,1)+'{enter}');
        cy.getById("errorSummary").should("contain.text", 'Select a trust');
	});
});