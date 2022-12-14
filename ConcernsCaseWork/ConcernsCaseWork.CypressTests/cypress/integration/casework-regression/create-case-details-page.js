import { LogTask } from "../../support/constants";
import  editTrustPage from "../../pages/editTrustPage";
import { Logger } from "../../common/logger";

describe("The correct items are visible on the details page", () => {
	before(() => {
		cy.login();
	});

	afterEach(() => {
		cy.storeSessionData();
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
		cy.get('#issue').clear().type("ABC");
		cy.get('button[data-prevent-double-click^="true"]').click();
		
		editTrustPage.validateTerritoryOldSelection();
		editTrustPage.getEditTerritory();
		editTrustPage.getTerritoryNAUNorthEastOption();
		editTrustPage.getTerritoryApplyBtn();
		editTrustPage.validateTerritoryNewSelection();

		
		

	})

	

});
