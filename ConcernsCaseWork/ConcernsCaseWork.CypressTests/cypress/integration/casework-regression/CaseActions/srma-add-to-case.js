import AddToCasePage from "/cypress/pages/caseActions/addToCasePage";
import CaseManagementPage from "/cypress/pages/caseMangementPage";
import utils from "/cypress/support/utils"
import srmaAddPage from "/cypress/pages/caseActions/srmaAddPage";
import srmaEditPage from "/cypress/pages/caseActions/srmaEditPage";


describe("User can add case actions to an existing case", () => {
	before(() => {
		cy.login();
	});

	afterEach(() => {
		cy.storeSessionData();
	});

	let term = "";

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

		cy.wait(500);
			const err = '.govuk-list.govuk-error-summary__list';
			cy.log(err.length);
		});

	it("User can click to add Financial Plan Case Action to a case", function () {
		
		AddToCasePage.getAddToCaseBtn().click();
		cy.log(utils.checkForGovErrorSummaryList() );

			if (err.length > 0) { //Cypress.$ needed to handle element missing exception
				
				cy.log("SRMA exists")

					cy.visit(Cypress.env('url'), { timeout: 30000 });
					cy.checkForExistingCase(true);
					cy.get('[class="govuk-button"][role="button"]').click();

					AddToCasePage.getHeadingText().should('contain.text', 'Add to case');
				}else{
					cy.log("No SRMA exists")	
					AddToCasePage.getHeadingText().should('contain.text', 'Add to case');
				}

		srmaAddPage.getAddCaseActionBtn().click();
		srmaEditPage.getSrmaTableRow().should(($row) => {
			expect($row.eq(6).text().trim()).to.contain(term.trim()).and.to.match(/Notes/i);
		});
	});
	
	after(function () {
		cy.clearLocalStorage();
		cy.clearCookies();
	});
 
});
