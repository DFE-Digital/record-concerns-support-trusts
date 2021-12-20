describe("User interactions via Find Trust route", () => {
	before(() => {
		cy.login();
	});

	afterEach(() => {
		cy.storeSessionData();
	});

    const searchTerm =
    "Accrington St Christopher's Church Of England High School";

	it("User clicks on Create Case and should see Search Trusts", () => {
		cy.get('[href="/trust"]').click();
		cy.get("#search").should("be.visible");
	});

	it("User searches for a valid Trust and selects it", () => {
		cy.get("#search").type(searchTerm + "{enter}");
		cy.get("#search__option--0").click();
	});

	it("Should display the Concern details of the specified Trust", () => {
		cy.get(
			".govuk-table__cell:nth-child(1) > a"
		).should('contain.text','Accrington')
		cy.get('[href="/trust"').should("be.visible").click();  
	});

    it('Should display an error when a user types fewer than 3 letters in the search bar', () => {
        cy.get("#search").type(searchTerm .substring(0,1)+'{enter}');
        cy.get('.govuk-list.govuk-error-summary__list a').should('contain.text', 'Enter search criteria higher than three characters')
    });

    after(function () {
        cy.clearLocalStorage();
        cy.clearCookies();
    }); 
});