describe('Concern validation tests', () => {
	before(() => {
		cy.login();
	});

	it('Error messages should be displayed when user omits data', () => {
		cy.get('[href="/case"]').click();
		cy.get('#search').should('be.visible');

		cy.randomSelectTrust();
		cy.get('#search__option--0').click();
		cy.getById("continue").click();

		cy.getByTestId('add-concern-button').click();
		cy.get('.govuk-list.govuk-error-summary__list li:nth-of-type(1)')
			.should('contain.text', 'Select concern type');
		cy.get('.govuk-list.govuk-error-summary__list li:nth-of-type(2)')
			.should('contain.text', 'Select risk rating');
	});

	after(function () {
		cy.clearLocalStorage();
		cy.clearCookies();
	});
});
