import AddToCasePage from "/cypress/pages/caseActions/addToCasePage";
import CaseManagementPage from "/cypress/pages/caseMangementPage";
import utils from "/cypress/support/utils"

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

	it("User on the Case Actions Add to Case page is warned when proceeding without selecting an action", () => {

		CaseManagementPage.getAddToCaseBtn().click();
		utils.getGovErrorSummaryList().should('not.exist');
		AddToCasePage.getAddToCaseBtn().click();
		utils.getGovErrorSummaryList().should('exist');
		utils.getGovErrorSummaryList().should('contain', 'Please select an action');
		cy.reload();
	});


	it("User can Cancel and is returned to the Case ID Page", () => {

		AddToCasePage.getCancelBtn().scrollIntoView();
		AddToCasePage.getCancelBtn().click();

		CaseManagementPage.getHeadingText().then(($heading) =>{
			expect($heading).to.be.visible
			expect($heading.text()).to.match(/(Case ID)/i);
		});
	});

	it("User has option to add Case Action to a case", () => {
		cy.reload();
		CaseManagementPage.getAddToCaseBtn().click();
		AddToCasePage.addToCase('Srma')
		AddToCasePage.getCaseActionRadio('Srma').siblings().should('contain.text', AddToCasePage.actionOptions[8]);
	});


	after(function () {
		cy.clearLocalStorage();
		cy.clearCookies();
	});
 
});
