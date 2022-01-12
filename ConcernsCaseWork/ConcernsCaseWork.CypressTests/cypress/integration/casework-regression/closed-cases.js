describe('User can view and navigate Closed cases', () => {
    before(() => {
		cy.login();
	});

	afterEach(() => {
		cy.storeSessionData();
	});

    it('User clicks on Create Case and should see Search Trusts', () => {
        cy.get('[href="/case/closed"]').click()
        cy.get('.govuk-table__body tr').should('be.visible')
    });

    it('User can view a closed case', () => {
        cy.get('[href*=closed]').first().should('be.visible')
        cy.get('[href*=closed]').first().click()
        cy.get('[name=caseID]').should('be.visible')
    });

    it('User clicks Back  and should be taken back to the Closed Cases', () => {
        cy.get('#back-link-event').click()
        cy.get('.govuk-table__body tr').should('be.visible')
    });
    
    it('User clicks Back  and should be taken back to the Active Casework screen', () => {
        cy.get('#back-link-event').click()
        cy.get('[href="/case"').should('be.visible')
        cy.get('[href="/trust"').should('be.visible')
        cy.get('[href="/case/closed"').should('be.visible')
    });
    

    after(function () {
        cy.clearLocalStorage();
        cy.clearCookies();
    });
});