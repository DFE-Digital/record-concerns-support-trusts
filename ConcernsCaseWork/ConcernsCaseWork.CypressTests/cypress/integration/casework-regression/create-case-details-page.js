import { LogTask } from "../../support/constants";

describe("The correct items are visible on the details page", () => {
	before(() => {
		cy.login();
	});

	afterEach(() => {
		cy.storeSessionData();
	});

	it("Should validate the case details", () => {
		cy.get('[href="/case"]').click();
		cy.get("#search").should("be.visible");

		cy.randomSelectTrust();
		cy.get("#search__option--0").click();

		cy.task(LogTask, "Concern type (Financial: Deficit)");
		cy.get(".govuk-summary-list__value").then(($el) =>{
			expect($el.text()).to.match(/([A-Z])\w+/i)
		});
		cy.selectConcernType();

		cy.selectRiskToTrust();
		cy.validateCreateTerritory();
		cy.selectTerritory();
		cy.get(".govuk-summary-list__value").then(($el) =>{
			expect($el.text()).to.match(/([A-Z])\w+/i)
		});
		cy.validateCreateCaseDetailsComponent();

		cy.validateCreateCaseInitialDetails();
	})
});
