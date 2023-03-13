import { LogTask } from "../../support/constants";

describe("User can manage cases from the case management page", () => {
	beforeEach(() => {
		cy.login();
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
	});
});
