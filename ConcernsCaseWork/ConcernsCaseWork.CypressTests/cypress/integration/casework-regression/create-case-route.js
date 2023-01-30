const { Logger } = require("../../common/logger");

describe("User interactions via Create Case route", () => {
	beforeEach(() => {
		cy.login();
	});

	const searchTerm =
		"Accrington St Christopher's Church Of England High School";
		let term = ""

	it("User searches for a valid Trust and selects it", () => {
		cy.get('[href="/case"]').click();
		cy.get("#search").should("be.visible");
		cy.get("#search").type(searchTerm);
		cy.get("#search__option--0").invoke('text').then(text => {
			term = text;
			cy.log(term.substring(0,15))
		cy.get("#search__option--0").click();
		cy.getById("continue").click();
		});

		Logger.Log("Should display the Concern details of the specified Trust");
		cy.get(".govuk-summary-list .govuk-summary-list__row:nth-of-type(1) .govuk-summary-list__value").should("contain.text", term.substring(9,10));
		cy.get('[href="/case/concern/cancel"').should("be.visible").click();
	});

    it('Should display an error if no trust is selected', () => {
		cy.get('[href="/case"]').click();
		cy.get("#search").should("be.visible");
		cy.get("#search").type(searchTerm .substring(0,1)+'{enter}');
        cy.get('.govuk-list.govuk-error-summary__list a').should('contain.text', 'A trust is required')
    });
});