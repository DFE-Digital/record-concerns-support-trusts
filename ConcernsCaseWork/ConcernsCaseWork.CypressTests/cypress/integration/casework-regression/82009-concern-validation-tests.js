describe('Error messages should be displayed when user omits data', () => {
	before(() => {
		cy.login();
	});

	afterEach(() => {
		cy.storeSessionData();
	});

	it('User clicks on Create Case and should see Search Trusts', () => {
		cy.get('[href="/case"]').click();
		cy.get('#search').should('be.visible');
	});

	it('User searches for a valid Trust and selects it', () => {
		cy.randomSelectTrust();
		cy.get('#search__option--0').click();
	});

	it('Should throw an error when a Concern Type is omitted', () => {
		cy.get('.govuk-button').click();
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
