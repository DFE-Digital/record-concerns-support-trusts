import { LogTask } from '../../support/constants';

describe("User can manage cases from the case management page", () => {
    before(() => {
        cy.login();
    });

    afterEach(() => {
        cy.storeSessionData();
    });

    it("Should create a case and validate the concerns details", () => {

        cy.task(LogTask, "Searching for trust");
        cy.get('[href="/case"]').click();
        cy.get("#search").should("be.visible");

        cy.randomSelectTrust();
        cy.get("#search__option--0").click();
        cy.getById("continue").click();

        cy.selectConcernType();

        cy.selectRiskToTrust();

        cy.selectTerritory();

        cy.enterConcernDetails();

        cy.validateCaseManagPage();
		
		cy.get('[id^="accordion-default-heading"]').eq(0).should('have.attr', 'aria-expanded', 'true');
        cy.get('[id^="accordion-default-heading"]').eq(0).click().should('have.attr', 'aria-expanded', 'false');
        cy.get('[id^="accordion-default-heading"]').eq(0).click().should('have.attr', 'aria-expanded', 'true');
    });
});
