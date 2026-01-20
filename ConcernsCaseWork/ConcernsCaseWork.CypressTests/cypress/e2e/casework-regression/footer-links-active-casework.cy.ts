
describe('Footer links direct to the correct pages from Active Casework page', () => {
    beforeEach(() => {
        cy.login();
    });

    it('Should validate Accessibility link', () => {

		cy.contains('a', 'Accessibility statement')
			.should('be.visible')
			.should('have.attr', 'href', 'https://accessibility-statements.education.gov.uk/s/30')
			.should('have.attr', 'target', '_blank');


        cy.excuteAccessibilityTests();
    });

    it('Should open Cookies link and verify the title', () => {
        cy.getByTestId("cookies-link").click();
        cy.get("h1").should("contain.text", "Cookies on Record concerns and support for trusts");

        cy.excuteAccessibilityTests();
    });

    it('Should open Privacy Policy link and verify the title', () => {

        cy.getByTestId("privacy-policy-link").click();
        cy.get("h1").should("contain.text", "Privacy policy");

        cy.excuteAccessibilityTests();
    });
});