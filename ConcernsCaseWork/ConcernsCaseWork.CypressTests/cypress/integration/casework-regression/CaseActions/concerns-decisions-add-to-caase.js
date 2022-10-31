import AddToCasePage from "/cypress/pages/caseActions/addToCasePage";
import CaseManagementPage from "/cypress/pages/caseMangementPage";
import utils from "/cypress/support/utils"
import ntiAddPage from "/cypress/pages/caseActions/ntiAddPage";
import CaseActionsBasePage from "/cypress/pages/caseActions/caseActionsBasePage";

describe("User can add case actions to an existing case", () => {
	before(() => {
		cy.login();
	});

	afterEach(() => {
		cy.storeSessionData();
	});

	let stText = "null";
	let condText = "null";
	let reasText = "null";
	let returnedDate = "null";
	let notesText = "null";

	
	it("Checking that Concerns decision is visible then adding concerns decision case action to a case,  Validation of wrong date when entered", function () {
		cy.checkForExistingCase();
		cy.reload();
		CaseManagementPage.getAddToCaseBtn().click();
		AddToCasePage.addToCase('Decision');
		AddToCasePage.getCaseActionRadio('Decision').siblings().should('contain.text', AddToCasePage.actionOptions[11]);
		AddToCasePage.getAddToCaseBtn().click();
		cy.log(utils.checkForGovErrorSummaryList());

		if (utils.checkForGovErrorSummaryList() > 0) {
			cy.log("Case Action already exists");
			cy.visit(Cypress.env('url'), { timeout: 30000 });
			cy.checkForExistingCase(true);
			CaseManagementPage.getAddToCaseBtn().click();
			AddToCasePage.addToCase('Decision');
			AddToCasePage.getCaseActionRadio('Decision').siblings().should('contain.text', AddToCasePage.actionOptions[11]);
			AddToCasePage.getAddToCaseBtn().click();
		} else {
			cy.log("No Case Action exists");
			cy.log(utils.checkForGovErrorSummaryList());
		}

		AddToCasePage.getDayDateField().click().type("23");
		AddToCasePage.getMonthDateField().click().type("25");
		AddToCasePage.getYearDateField().click().type("2022");
		AddToCasePage.getDecisionButton().click();

		cy.get('#decision-error-list').should(
			"contain.text",
			"23-25-2022 is an invalid date"
		);

	});


	after(function () {
		cy.clearLocalStorage();
		cy.clearCookies();
	});

});
