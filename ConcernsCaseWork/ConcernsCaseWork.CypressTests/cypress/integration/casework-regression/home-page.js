describe("Home page tests", () => {
	before(() => {
		cy.setupAuthHeaders('chris.dexter',['concerns-casework.caseworker','concerns-casework.teamleader']);
		cy.login();
	});

	afterEach(() => {
		cy.storeSessionData();
	});

	it("Create, Find and Closed Case buttons should be displayed", () => {

		cy.get('.govuk-button[href="/case/closed"]').should("be.visible");
		cy.get('.govuk-button[href="/trust"]').should("be.visible");
		cy.get('.govuk-button[href="/case"]').should("be.visible");
	});

   it('User clicks on Create Case and should see Search Trusts', () => {

	   cy.get('[href="/case"]').click()
	   cy.get('#search').should('be.visible')
   });

   it('User clicks Back and should be taken back to the Active Casework screen', () => {
    cy.get('#back-link-event').click()
	cy.get('[href="/case"').should('be.visible')
	cy.get('[href="/trust"').should('be.visible')
	cy.get('[href="/case/closed"').should('be.visible')
   });

   it('User clicks on Find Trust and should see Search Trusts', () => {
	cy.get('[href="/trust"]').click()
	cy.get('#search').should('be.visible')
	});

	it('User clicks Back and should be taken back to the Active Casework screen', () => {
		cy.get('#back-link-event').click()
		cy.get('[href="/case"').should('be.visible')
		cy.get('[href="/trust"').should('be.visible')
		cy.get('[href="/case/closed"').should('be.visible')
	});
});