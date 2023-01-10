import { LogTask } from "../../support/constants";

describe("User edits the trust risk on existing case", () => {
	before(() => {
		cy.login();
	});

	afterEach(() => {
		cy.storeSessionData();
	});

	it("Should allow the user to change an existing case", () => {
		cy.get('[href="/case"]').click();
		cy.get("#search").should("be.visible");

		cy.randomSelectTrust();
		cy.get("#search__option--0").click();

		cy.selectConcernType();
		cy.selectRiskToTrust();
		cy.selectTerritory();
		cy.enterConcernDetails();

		cy.task(LogTask, "Navigating to the risk rating page");
		cy.get('span[class="govuk-visually-hidden"]').contains('risk to trust').parent().click();
		cy.editRiskToTrust("apply", "Amber");
		cy.get('[class="govuk-table__cell govuk-label-wrapper"]').children()
		.should('contain.text', 'Amber')
		cy.get('[class="govuk-table__cell govuk-label-wrapper"]').should("be.visible");

		cy.task(LogTask, "Navigating back to case management");
		cy.get('*[name="caseID"]').should('be.visible');
		cy.get('[id="close-case-button"]').should('be.visible');

		cy.log("Cancelled, navigating to the management page")
		cy.get('span[class="govuk-visually-hidden"]').contains('risk to trust').parent().click();
		cy.editRiskToTrust("cancel");
		cy.get('*[name="caseID"]').should('be.visible');

	});
});
