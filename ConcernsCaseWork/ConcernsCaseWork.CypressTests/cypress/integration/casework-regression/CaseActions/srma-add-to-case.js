import AddToCasePage from "/cypress/pages/caseActions/addToCasePage";
import CaseManagementPage from "/cypress/pages/caseMangementPage";
import srmaAddPage from "/cypress/pages/caseActions/srmaAddPage";

describe("User can add case actions to an existing case", () => {
	before(() => {
		cy.login();
	});

	afterEach(() => {
		cy.storeSessionData();
	});

	it("User enters the case page", () => {
		cy.checkForExistingCase();
	});

	it("User has option to add SRMA Case Action to a case", () => {
		cy.reload();
		CaseManagementPage.getAddToCaseBtn().click();
		AddToCasePage.addToCase('Srma')
		AddToCasePage.getCaseActionRadio('Srma').siblings().should('contain.text', AddToCasePage.actionOptions[8]);
	});


	it("User is taken to the correct Case Action page", function () {
		cy.get('button[data-prevent-double-click*="true"]').click();

		cy.wait(500).then(() => {
			const err = Cypress.$('.govuk-list.govuk-error-summary__list');
			cy.log(err.length);
		});

		srmaAddPage.getHeadingText().should('contain.text', "School Resource Management Adviser (SRMA)")
	});

	it("User is shown validation and cannot proceed without selecting a valid status", () => {

		srmaAddPage.setDateOffered();
		srmaAddPage.getAddCaseActionBtn().click();
		cy.get('[class="govuk-list govuk-error-summary__list"]').should('contain.text', 'Select status');
		cy.reload();
	});

	it("User is shown validation and cannot proceed without selecting a valid date", () => {

		srmaAddPage.setStatusSelect(0);
		cy.get('[id="dtr-day"]').type("0");
		cy.get('[id="dtr-month"]').type("0");
		cy.get('[id="dtr-year"]').type("0");
		srmaAddPage.getAddCaseActionBtn().click();
		cy.get('[class="govuk-list govuk-error-summary__list"]').should('contain.text', 'Enter the date SRMA was offered to the trust');
		cy.reload();
	});

	after(function () {
		cy.clearLocalStorage();
		cy.clearCookies();
	});
 
});
