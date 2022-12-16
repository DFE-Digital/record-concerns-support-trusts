import { LogTask } from "../../support/constants";
import  editTrustPage from "../../pages/editTerritoryPage";
import { Logger } from "../../common/logger";

describe("The correct items are visible on the details page", () => {
	beforeEach(() => {
		cy.login();
	});

	it("Should validate the case and Territory details", () => {
		
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


		Logger.Log("Editing Territory and validation")
		cy.wait(1000);
		cy.get('#issue').clear().type("ABC");
		cy.getByTestId("create-case-button").click();
		
		editTrustPage.hasTerritory("Midlands and West - South West");
		editTrustPage.editTerritory();
		editTrustPage.selectTerritoryNAUNorthEast();
		editTrustPage.save();
		editTrustPage.hasTerritory("North and UTC - North East");
	});
});
