
describe('Footer links direct to the correct pages from Create Case page', () => {
    before(() => {
		cy.login();
        cy.visit(Cypress.env('url')+"/case");
	});

	afterEach(() => {
		cy.storeSessionData();
	});

    it('Should open Accessibility link ', () => {
        cy.fixture('footer-links-fixture').then((footerLinkData) =>{
        cy.get('.govuk-footer__link[href="'+footerLinkData.accessibilityLink+'"]').click()
        cy.get('.govuk-grid-column-two-thirds-from-desktop h1').contains(footerLinkData.accessibilityTestText)
        })
        cy.get('#back-link-event').click()
    });

    it('Should open Cookies link', () => {
        cy.fixture('footer-links-fixture').then((footerLinkData) =>{
        cy.get('.govuk-footer__link[href="'+footerLinkData.cookiesLink+'"]').click()
        cy.get('.govuk-grid-column-two-thirds-from-desktop h1').contains(footerLinkData.cookiesTestText)
        })
        cy.get('#back-link-event').click()
    });

    it('Should open Privacy Policy link', () => {
        cy.fixture('footer-links-fixture').then((footerLinkData) =>{
        cy.get('.govuk-footer__link[href="'+footerLinkData.privacyPolicyLink+'"]').click()
        cy.get('.govuk-grid-column-two-thirds-from-desktop h2').contains(footerLinkData.privacyPolicyText)
        })
        cy.get('#back-link-event').click()
    });

    it('Should open Admin link and clicking the "Back To Casework" button should take the user back to the dashboard ', () => {
        cy.fixture('footer-links-fixture').then((footerLinkData) =>{
        cy.get('.govuk-footer__link[href="'+footerLinkData.adminLink+'"]').click()
        cy.get('.govuk-heading-l').contains(footerLinkData.adminText)
        })
        cy.get('.govuk-button--secondary[href="/"]').click()
        cy.get('.govuk-button[href="/case"]').should('be.visible')
    });

    after(function () {
        cy.clearLocalStorage();
        cy.clearCookies()
    });
});