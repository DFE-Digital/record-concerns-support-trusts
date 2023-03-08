
describe('Footer links direct to the correct pages from Active Casework page', () => {
    beforeEach(() => {
        cy.login();
    });

    it('Should open Accessibility link and verify the title', () => {
        cy.wait(300);
        cy.fixture('footer-links-fixture').then((footerLinkData) => {
            cy.get('.govuk-footer__link[href="' + footerLinkData.accessibilityLink + '"]', { timeout: 50000 }).click()
            cy.get('.govuk-grid-column-two-thirds-from-desktop h1').contains(footerLinkData.accessibilityTestText)
        })
        cy.get('#back-link-event').click()
    });
    it('Should open Cookies link and verify the title', () => {

        cy.wait(300);
        cy.fixture('footer-links-fixture').then((footerLinkData) => {
            cy.get('.govuk-footer__link[href="' + footerLinkData.cookiesLink + '"]', { timeout: 50000 }).click()
            cy.get('.govuk-grid-column-two-thirds-from-desktop h1').contains(footerLinkData.cookiesTestText)
        })
        cy.get('#back-link-event').click()
    });
    it('Should open Privacy Policy link and verify the title', () => {
        cy.wait(300);
        cy.fixture('footer-links-fixture').then((footerLinkData) => {
            cy.get('.govuk-footer__link[href="' + footerLinkData.privacyPolicyLink + '"]', { timeout: 50000 }).click()
            cy.log(footerLinkData.privacyPolicyText)
            cy.get('.govuk-grid-column-two-thirds-from-desktop h2').contains(footerLinkData.privacyPolicyText)
        })
        cy.get('#back-link-event').click()
    });

    after(function () {
        cy.clearLocalStorage();
        cy.clearCookies()
    });
});