import { LogTask } from "../../support/constants";

describe("User edits the trust risk on existing case", () => {
	beforeEach(() => {
		cy.login();
		cy.basicCreateCase();
	});

	it("Should allow the user to change an existing case", () => {
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
