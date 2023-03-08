import CreateCaseConcernsPage from "/cypress/pages/createCase/createCaseConcernsPage";
import CaseMangementPage from "/cypress/pages/caseMangementPage";
import { LogTask } from "../../support/constants";

describe("User creates and adds subsequent concern to a case", () => {
	before(() => {
		cy.login();
	});

	afterEach(() => {
		cy.storeSessionData();
	});

	const searchTerm =
		"Accrington St Christopher's Church Of England High School";

	it("Should add a subsequent concern to a case", () => {
		
		cy.task(LogTask, "Searching for a trust");
		cy.get('[href="/case"]').click();
		cy.get("#search").should("be.visible");

		cy.get("#search").type(searchTerm);
		cy.get("#search__option--0").click();
		cy.getById("continue").click();

		cy.selectConcernType();
		cy.selectRiskToTrust();
		cy.selectTerritory();
		cy.enterConcernDetails();

		cy.task(LogTask, "Adding a concern via the Add Concern Link")
		CaseMangementPage.getConcernTableAddConcernBtn().click();

		cy.task(LogTask, "Selecting a second concern type");
		cy.get(".govuk-summary-list__value").then(($el) =>{
			expect($el.text()).to.match(/([A-Z])\w+/i)
		});

		CreateCaseConcernsPage.selectConcernType();
	});
});
