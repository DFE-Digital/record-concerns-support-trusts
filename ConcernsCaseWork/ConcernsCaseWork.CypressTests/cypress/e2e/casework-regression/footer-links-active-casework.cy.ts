
describe('Footer links direct to the correct pages from Active Casework page', () => {
    beforeEach(() => {
        cy.login();
    });

    it('Should open Accessibility link and verify the title', () => {

        cy.getByTestId("accessibility-link").click();
        cy.get("h1").should("contain.text", "Accessibility statement for Record concerns and support for trusts");

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