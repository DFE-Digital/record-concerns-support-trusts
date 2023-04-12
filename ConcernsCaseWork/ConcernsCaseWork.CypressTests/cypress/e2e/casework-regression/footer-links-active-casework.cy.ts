
describe('Footer links direct to the correct pages from Active Casework page', () => {
    beforeEach(() => {
        cy.login();
    });

    it('Should open Accessibility link and verify the title', () => {
        cy.fixture('footer-links-fixture').then((footerLinkData) => {
            cy.get('.govuk-footer__link[href="' + footerLinkData.accessibilityLink + '"]').click()
            cy.get('.govuk-grid-column-two-thirds-from-desktop h1').contains(footerLinkData.accessibilityTestText)
        });
    });

    it('Should open Cookies link and verify the title', () => {
        cy.fixture('footer-links-fixture').then((footerLinkData) => {
            cy.get('.govuk-footer__link[href="' + footerLinkData.cookiesLink + '"]').click()
            cy.get('.govuk-grid-column-two-thirds-from-desktop h1').contains(footerLinkData.cookiesTestText)
        });
    });

    it('Should open Privacy Policy link and verify the title', () => {
        cy.fixture('footer-links-fixture').then((footerLinkData) => {
            cy.get('.govuk-footer__link[href="' + footerLinkData.privacyPolicyLink + '"]').click()
            cy.get('.govuk-grid-column-two-thirds-from-desktop h2').contains(footerLinkData.privacyPolicyText)
        });
    });
});