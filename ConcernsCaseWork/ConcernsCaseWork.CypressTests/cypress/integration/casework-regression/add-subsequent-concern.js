import CreateCaseConcernsPage from "/cypress/pages/createCase/createCaseConcernsPage";
import CaseMangementPage from "/cypress/pages/caseMangementPage";
import { LogTask } from "../../support/constants";

describe("User adds subsequent Concern to a case", () => {
	before(() => {
		cy.login();
	});

	afterEach(() => {
		cy.storeSessionData();
	});

	let term = "";

	it("Should add the subsequent concern to the case", () => {
		cy.get('[href="/case"]').click();
		cy.get("#search").should("be.visible");

		cy.randomSelectTrust();
		cy.get("#search__option--0").invoke('text').then(text => {
			term = text;
		});
		cy.get("#search__option--0").click();

		cy.get(".govuk-summary-list .govuk-summary-list__row:nth-of-type(1) .govuk-summary-list__value");
		CreateCaseConcernsPage.selectConcernType();

		cy.task(LogTask, "Adding a subsequent concern type")
		cy.get(".govuk-summary-list .govuk-summary-list__row:nth-of-type(1) .govuk-summary-list__value").should("be.visible");
		CreateCaseConcernsPage.getAddAnotherConcernBtn().click();
		CreateCaseConcernsPage.selectConcernType();
		CreateCaseConcernsPage.getNextStepBtn().click();

		cy.selectRiskToTrust();
		cy.selectTerritory();

		cy.enterConcernDetails();

		CaseMangementPage.getConcernTable().should('contain.text', 'Financial');
	});
});
